namespace YaBot.App
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Threading;
    using Configs;
    using Core;
    using Core.Database;
    using Core.State;
    using Extensions;
    using Newtonsoft.Json;
    using Telegram.Bot;
    using Telegram.Bot.Types.InputFiles;
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

            var path = Path.Combine(Directory.GetCurrentDirectory(), "data");
            var rows = Path
                .Combine(path, "db.json")
                ._(File.ReadAllText)
                ._(JsonConvert.DeserializeObject<DataRow[]>)
                .ToImmutableArray();
            
            var findPlaceState = new GetRandomPlaceState(path, rows);

            var startState = new StartState(
                config.Names._(Words.Create),
                new IState[]
                {
                    new StartCreatePlaceState(
                        config.States["StartCreatePlace"].Words._(Words.Create),
                        new FinishCreatePlaceState(context) 
                    ),
                    findPlaceState
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

        private static ImmutableArray<Output> InitAnswers()
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "data");

            var rows = Path.Combine(directory, "db.json")
                ._(File.ReadAllText)
                ._(JsonConvert.DeserializeObject<DataRow[]>);

            var result = new List<Output>();
            foreach (var row in rows)
            {
                var stream = File.OpenRead(Path.Combine(directory, row.Image));
                var answer = new Output
                {
                    Text = row.Name,
                    Image = new InputOnlineFile(stream)
                };
                result.Add(answer);
            }

            return result.ToImmutableArray();
        }
    }
}