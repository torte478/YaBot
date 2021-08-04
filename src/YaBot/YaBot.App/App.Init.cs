namespace YaBot.App
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Configs;
    using Core;
    using Core.Database;
    using Core.State;
    using Extensions;
    using Newtonsoft.Json;
    using Telegram.Bot;
    using File = System.IO.File;

    internal partial class App
    {
        private const string ConfigPath = "config.json";
        private const string CredentialsPath = "credentials.json";

        public static App Init()
        {
            var credentials = CredentialsPath
                ._(File.ReadAllText)
                ._(JsonConvert.DeserializeObject<Credentials>);

            static void Log(string text) => 
                $"{DateTime.Now.ToLongTimeString()} {text}"
                ._(Console.WriteLine);

            var config = Config.Load(ConfigPath);
            
            var context = new Context(credentials.Database);
            
            var places = new Crudl<Place>(context, _ => _.Places);
            
            var startState = new StartState(
                config.Names._(Words.Create),
                new IState[]
                {
                    new StartCreatePlaceState(
                        config.States["StartCreatePlace"].Words._(Words.Create),
                        new FinishCreatePlaceState(
                            _ => places.Create(_)) 
                    ),
                    new ListState(places.ToList),
                    new StartGetState(
                        new FinishGetState(places.Read)),
                    new StartDeleteState(
                        new FinishDeleteState(
                            _ => places.Delete(_))),
                    new GetRandomPlaceState(
                        config.States["Start"].Words._(Words.Create),
                        places.ToList) // TODO : replace from init
                }
                .ToImmutableArray());

            var stoppers = config.StopWords._(Words.Create);
            
            var bot = new Bot(createReceiver: () => new States(startState, stoppers, Log).Process,
                begin: DateTime.UtcNow); 

            var handler = new Handler(bot.ReceiveAsync, Log);

            return new App(
                new TelegramBotClient(credentials.Telegram),
                new CancellationTokenSource(),
                (client, cancellation) => client.ReceiveAsync(handler, cancellation),
                context,
                Log);
        }
    }
}