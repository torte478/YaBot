namespace YaBot.App.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Telegram.Bot;
    using Telegram.Bot.Extensions.Polling;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class Handler : IUpdateHandler
    {
        public UpdateType[]? AllowedUpdates { get; }

        public Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message == null)
                return Task.CompletedTask;
            
            return botClient.SendTextMessageAsync(update.Message.Chat, $"Answer: {update.Message.Text}");
        }
        
        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.ToString());
            return Task.CompletedTask;
        }
    }
}