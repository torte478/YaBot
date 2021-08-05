namespace YaBot.App.TelegramApi
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Core;
    using Extensions;
    using Telegram.Bot;
    using Telegram.Bot.Extensions.Polling;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.InputFiles;

    public sealed class Handler : IUpdateHandler
    {
        private readonly Func<ITelegramBotClient, Update, CancellationToken, Task<IInput>> toInputAsync;
        private readonly Func<IInput, IOutput> receive;
        private readonly Action<string> log;

#pragma warning disable 8632
        public UpdateType[]? AllowedUpdates { get; }
#pragma warning restore 8632

        public Handler(
            Func<ITelegramBotClient, Update, CancellationToken, Task<IInput>> toInputAsync, 
            Func<IInput, IOutput> receive, 
            Action<string> log)
        {
            this.receive = receive;
            this.log = log;
            this.toInputAsync = toInputAsync;
        }

        public Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update?.Message == null)
                return Task.CompletedTask;

            log($"=> {update.Message?.Text ?? "?"}");

            var output = toInputAsync(botClient, update, cancellationToken).Result
                ._(receive);

            return SendAsync(botClient, update.Message.Chat, output, cancellationToken);
        }

        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            return exception
                ._(_ => _.ToString()._(log))
                ._(_ => Task.CompletedTask);
        }

        private static async Task SendAsync(ITelegramBotClient client, Chat chat, IOutput output, CancellationToken cancellation)
        {
            if (output == null)
                return;
            
            if (output.IsImage)
            {
                using var stream = new MemoryStream(output.Image);
                await client.SendPhotoAsync(
                    chat, 
                    new InputOnlineFile(stream),
                    output.Text,
                    cancellationToken: cancellation)
                    .ConfigureAwait(false);
            }
            
            if (output.IsText)
            {
                await client.SendTextMessageAsync(
                    chat, 
                    output.Text,
                    cancellationToken: cancellation)
                .ConfigureAwait(false);
            }
            
        }
    }
}