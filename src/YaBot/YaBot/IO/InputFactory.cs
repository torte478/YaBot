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
        private readonly Func<string, MessageEntity[], string> serialize;
        private readonly Func<ITelegramBotClient, Update, CancellationToken, Task<byte[]>> loadImageAsync;

        public InputFactory(
            Func<string, MessageEntity[], string> serialize,
            Func<ITelegramBotClient, Update, CancellationToken, Task<byte[]>> loadImageAsync)
        {
            this.serialize = serialize;
            this.loadImageAsync = loadImageAsync;
        }

        public async Task<IInput> CreateAsync(ITelegramBotClient client, Update update, CancellationToken cancellation)
        {
            var document = update?.Message?.Document;
            if (document != null)
            {
                var error = document.MimeType?.Contains("image") ?? false
                    ? " Нельзя отправлять изображения без нажатой галочки сжатия"
                    : string.Empty;
                throw new Exception($"Неверный формат сообщения.{error}");
            }

            var input = new Input
            {
                Date = update!.Message!.Date,
                Chat = update.Message.Chat.Id,
            };

            if (update.Message.Photo == null)
            {
                input.Text = update.Message._(_ => serialize(_.Text, _.Entities));
            }
            else
            {
                input.Image = await loadImageAsync(client, update, cancellation);
                input.Text = update.Message._(_ => serialize(_.Caption, _.CaptionEntities));
            }

            return input;
        }
    }
}