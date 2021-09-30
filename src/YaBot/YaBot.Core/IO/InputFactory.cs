namespace YaBot.Core.IO
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using YaBot.Core.Extensions;

    public sealed class InputFactory
    {
        private readonly Func<Message, string> serialize;

        public InputFactory(Func<Message, string> serialize)
        {
            this.serialize = serialize;
        }

        public async Task<IInput> CreateAsync(ITelegramBotClient client, Update update, CancellationToken cancellation)
        {
            var input = new Input
            {
                Date = update.Message.Date,
                Chat = update.Message.Chat.Id,
                Text = update.Message._(serialize)
            };

            if (update.Message.Photo != null)
            {
                var photo = update.Message.Photo
                    .OrderByDescending(_ => _.Width * _.Height)
                    .First();

                await using var stream = new MemoryStream();
                await client.GetInfoAndDownloadFileAsync(photo.FileId, stream, cancellation)
                    .ConfigureAwait(false);

                input.Image = stream.ToArray();
                input.Text = update.Message.Caption;
            }

            return input;
        }
    }
}