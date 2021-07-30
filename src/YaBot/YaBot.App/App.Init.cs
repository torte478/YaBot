namespace YaBot.App
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Threading;
    using Configs;
    using Core;
    using Core.State;
    using Extensions;
    using Newtonsoft.Json;
    using Telegram.Bot;
    using Telegram.Bot.Types.InputFiles;
    using File = System.IO.File;

    internal partial class App
    {
        private const string ConfigPath = "config.json";
        private const string TokenPath = "token";

        public static App Init()
        {
            var token = File.ReadAllText(TokenPath);

            static void Log(string text) => 
                $"{DateTime.Now.ToLongTimeString()} {text}"
                ._(Console.WriteLine);

            var config = Config.Load(ConfigPath);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "data");
            var rows = Path
                .Combine(path, "db.json")
                ._(File.ReadAllText)
                ._(JsonConvert.DeserializeObject<DataRow[]>)
                .ToImmutableArray();
            
            var findPlaceState = new FindPlace(path, rows);
            
            var startState = new Start(
                config.Names._(Words.Create),
                config.States["Start"].Words._(Words.Create),
                findPlaceState);

            var stoppers = config.StopWords._(Words.Create);
            
            var bot = new Bot(
                parse: new Text(Words.Create).Parse,
                createReceiver: () => new States(startState, stoppers, Log).Process); 

            var handler = new Handler(bot.ReceiveAsync, Log);

            return new App(
                new TelegramBotClient(token),
                new CancellationTokenSource(),
                (client, cancellation) => client.ReceiveAsync(handler, cancellation),
                Log);
        }

        private static ImmutableArray<Answer> InitAnswers()
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "data");

            var rows = Path.Combine(directory, "db.json")
                ._(File.ReadAllText)
                ._(JsonConvert.DeserializeObject<DataRow[]>);

            var result = new List<Answer>();
            foreach (var row in rows)
            {
                var stream = File.OpenRead(Path.Combine(directory, row.Image));
                var answer = new Answer
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