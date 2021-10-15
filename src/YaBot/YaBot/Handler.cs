namespace YaBot
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
    using YaBot.Extensions;
    using YaBot.IO;

    public sealed class Handler : IUpdateHandler
    {
        private readonly Func<ITelegramBotClient, Update, CancellationToken, Task<IInput>> toInputAsync;
        private readonly Func<IInput, IOutput> receive;
        private readonly Action<string> log;
        private readonly Func<Update, string> serialize;

#pragma warning disable 8632
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public UpdateType[]? AllowedUpdates { get; }
#pragma warning restore 8632

        public Handler(
            Func<ITelegramBotClient, Update, CancellationToken, Task<IInput>> toInputAsync, 
            Func<IInput, IOutput> receive, 
            Func<Update, string> serialize,
            Action<string> log)
        {
            this.receive = receive;
            this.log = log;
            this.serialize = serialize;
            this.toInputAsync = toInputAsync;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update?.Message is not {ForwardFrom: null})
                    return;

#if DEBUG
                log($"{update.Message?.Text ?? "?"}");
#endif
                var input = await toInputAsync(client, update, cancellationToken)
                    .ConfigureAwait(false);

                var output = receive(input);

                await SendAsync(client, update.Message.Chat, output, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log(ex.ToString());
                log(update._(serialize));
                throw;
            }
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
                    captionEntities: output.MessageEntities,
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