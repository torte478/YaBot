#nullable enable
namespace YaBot.App.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using Telegram.Bot;
    using Telegram.Bot.Extensions.Polling;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public sealed class Handler : IUpdateHandler
    {
        private readonly Func<ITelegramBotClient, Update, CancellationToken, Task> receive;
        private readonly Action<string> log;

        public UpdateType[]? AllowedUpdates { get; }

        public Handler(Func<ITelegramBotClient, Update, CancellationToken, Task> receive, Action<string> log)
        {
            this.receive = receive;
            this.log = log;
        }

        public Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            return update
#if DEBUG
                ._(_ => log($"=> {_.Message.Text}"))
#endif
                ._(_ => receive(botClient, _, cancellationToken));
        }
        
        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            return exception
                ._(_ => _.ToString()._(log))
                ._(_ => Task.CompletedTask);
        }
    }
}