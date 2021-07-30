namespace YaBot.App
{
    using System.Threading;
    using Core;
    using Telegram.Bot;

    internal partial class App
    {
        private const string Config = "config.json";
        private const string Token = "token";

        public static App Init()
        {
            var token = System.IO.File.ReadAllText(Token);
            var me = new TelegramBotClient(token);

            return new App(
                me,
                new CancellationTokenSource(),
                (client, cancellation) => client.ReceiveAsync<Handler>(cancellation));
        }
    }
}