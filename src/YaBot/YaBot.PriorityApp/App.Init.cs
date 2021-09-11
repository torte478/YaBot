namespace YaBot.PriorityApp
{
    using System;
    using System.Threading;
    using Newtonsoft.Json;
    using Telegram.Bot;
    using YaBot.Core;
    using YaBot.Core.Extensions;
    using YaBot.PriorityApp.Database;
    using File = System.IO.File;

    internal partial class App
    {
        private const string CredentialsPath = "credentials.json";

        public static App Init()
        {
            var credentials = CredentialsPath
                ._(File.ReadAllText)
                ._(JsonConvert.DeserializeObject<Credentials>)
                ?? 
                throw new Exception("Credentials is null");

            var context = new Context(credentials.Database);
            
            var places = new Crudl<Context, Project>(context, _ => _.Projects);

            var bot = new Bot(createReceiver: 
                () => _ => null, // TODO
                begin: DateTime.UtcNow); 

            var handler = new Handler(Input.CreateAsync, bot.Receive, Log);

            return new App(
                new TelegramBotClient(credentials.Telegram),
                new CancellationTokenSource(),
                (client, cancellation) => TelegramBotClientPollingExtensions.ReceiveAsync(client, handler, cancellationToken: cancellation),
                context,
                Log);
        }

        private static void Log(string message)
        {
            #if DEBUG

            if (string.IsNullOrEmpty(message))
                return;
            
            $"{DateTime.Now.ToLongTimeString()} {message}"
                ._(Console.WriteLine);
            
            #endif
        }
    }
}