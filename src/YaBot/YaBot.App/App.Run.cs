﻿namespace YaBot.App
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Core;
    using Extensions;
    using Telegram.Bot;
    using Telegram.Bot.Extensions.Polling;

    internal partial class App : IDisposable
    {
        private readonly ITelegramBotClient me;
        private readonly CancellationTokenSource cancellation;
        private readonly Func<ITelegramBotClient, CancellationToken, Task> run;

        private App(
            ITelegramBotClient me, 
            CancellationTokenSource cancellation, 
            Func<ITelegramBotClient, CancellationToken, Task> run)
        {
            this.me = me;
            this.cancellation = cancellation;
            this.run = run;
        }

        public void Run()
        {
            me
                .GetMeAsync()
                .Result
                ._(_ => $"{_.Username} started at {DateTime.Now}")
                ._(Console.WriteLine);

            var task = me._(run, cancellation.Token);
            
            Console.ReadLine()
                ._(_ => Console.WriteLine("stopped"))
                ._(_ => cancellation.Cancel());
            
        }

        public void Dispose()
        {
            cancellation.Dispose();
        }
    }
}