namespace YaBot.Core
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Telegram.Bot;
    using Telegram.Bot.Extensions.Polling;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.InputFiles;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;

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

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update?.Message is not {ForwardFrom: null})
                return;

            log($"=> {update.Message?.Text ?? "?"}");

            var output = toInputAsync(botClient, update, cancellationToken).Result
                ._(receive);

            await SendAsync(botClient, update.Message.Chat, output, cancellationToken);
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
                await using var stream = new MemoryStream(output.Image);
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
                    entities: output.MessageEntities,
                    cancellationToken: cancellation)
                .ConfigureAwait(false);
            }
            
        }
    }
}