namespace YaBot.PriorityApp
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Telegram.Bot;
    using YaBot.Core.Extensions;
    using YaBot.PriorityApp.Database;

    internal sealed partial class App : IDisposable
    {
        private readonly ITelegramBotClient me;
        private readonly CancellationTokenSource cancellation;
        private readonly Func<ITelegramBotClient, CancellationToken, Task> run;
        private readonly Context context;
        private readonly Action<string> log;

        private App(
            ITelegramBotClient me, 
            CancellationTokenSource cancellation, 
            Func<ITelegramBotClient, CancellationToken, Task> run,
            Context context,
            Action<string> log)
        {
            this.me = me;
            this.cancellation = cancellation;
            this.run = run;
            this.context = context;
            this.log = log;
        }

        public void Run()
        {
            me
                .GetMeAsync()
                .Result
                ._(_ => $"{_.Username} started at {DateTime.Now}")
                ._(log);

            var main = me._(run, cancellation.Token);
            var exit = Task.Delay(int.MaxValue, cancellation.Token);

            Task.WaitAny(main, exit);
        }

        public void Dispose()
        {
            cancellation.Dispose();
            context.Dispose();
        }
    }
}