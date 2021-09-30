namespace YaBot.PriorityApp
{
    using System;
    using System.Threading;
    using Newtonsoft.Json;
    using Telegram.Bot;
    using YaBot.Core;
    using YaBot.Core.Database;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;
    using YaBot.PriorityApp.Database;
    using YaBot.PriorityApp.Tree;
    using File = System.IO.File;
    using Project = YaBot.PriorityApp.Database.Project;

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

            var objectives = new Crudl<Context,Objective>(context, _ => _.Objectives);
            var service = new Service(
                new Crudl<Context, Project>(context, _ => _.Projects),
                objectives,
                projectId => Tree.Project.Create(
                    projectId, 
                    objectives,
                    () => new BalancedTree<int>(
                        new Measure(1024))));
            
            var bot = new Bot(
                createReceiver: () => service.Process,
                begin: DateTime.UtcNow,
                Console.WriteLine);

            var handler = new Handler(
                new InputFactory(new FormattedText("**").Serialize).CreateAsync,
                bot.Receive,
                Log);

            return new App(
                new TelegramBotClient(credentials.Telegram),
                new CancellationTokenSource(),
                (client, cancellation) => TelegramBotClientPollingExtensions.ReceiveAsync(
                    client, 
                    handler, 
                    cancellationToken: cancellation),
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