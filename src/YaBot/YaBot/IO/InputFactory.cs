namespace YaBot.IO
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using YaBot.Extensions;

    public sealed class InputFactory
    {
        private readonly Func<Message, string> serialize;
        private readonly Func<ITelegramBotClient, Update, CancellationToken, Task<byte[]>> loadImageAsync;

        public InputFactory(
            Func<Message, string> serialize,
            Func<ITelegramBotClient, Update, CancellationToken, Task<byte[]>> loadImageAsync)
        {
            this.serialize = serialize;
            this.loadImageAsync = loadImageAsync;
        }

        public async Task<IInput> CreateAsync(ITelegramBotClient client, Update update, CancellationToken cancellation)
        {
            var input = new Input
            {
                Date = update!.Message!.Date,
                Chat = update.Message.Chat.Id,
                Text = update.Message._(serialize)
            };

            if (update.Message.Photo != null)
            {
                input.Image = await loadImageAsync(client, update, cancellation);
                input.Text = update.Message.Caption;
            }

            return input;
        }
    }
}