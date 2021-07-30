namespace YaBot.App
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Threading;
    using Core;
    using Core.State;
    using Extensions;
    using Telegram.Bot;

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

            static IWords ToWords(IEnumerable<string> words) => 
                new Words(words.ToImmutableArray());

            var findPlaceState = new FindPlace();
            
            var startState = new Start(
                config.Names._(ToWords),
                config.States["Start"].Words._(ToWords),
                findPlaceState);

            var states = new States(
                startState,
                config.StopWords._(ToWords),
                Log);
            
            var bot = new Bot(
                parse: new Text(ToWords).Parse,
                process: states.Process);

            var handler = new Handler(bot.ReceiveAsync, Log);

            return new App(
                new TelegramBotClient(token),
                new CancellationTokenSource(),
                (client, cancellation) => client.ReceiveAsync(handler, cancellation),
                Log);
        }
    }
}