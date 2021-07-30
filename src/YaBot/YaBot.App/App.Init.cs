namespace YaBot.App
{
    using System;
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

            var findPlaceState = new FindPlace();
            
            var startState = new Start(
                config.Names._(Words.Create),
                config.States["Start"].Words._(Words.Create),
                findPlaceState);

            var states = new States(
                startState,
                config.StopWords._(Words.Create),
                Log);
            
            var bot = new Bot(
                parse: new Text(Words.Create).Parse,
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