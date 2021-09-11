namespace YaBot.Core
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public sealed class Input : IInput
    {
        public DateTime Date { get; private init; }
        public long Chat { get; private init;}
        
        public string Text { get; private set;}
        public byte[] Image { get; private set;}
        
        public bool IsImage => Image != null;

        private Input() {}
        
        public static async Task<IInput> CreateAsync(ITelegramBotClient client, Update update, CancellationToken cancellation)
        {
            var input = new Input
            {
                Date = update.Message.Date,
                Chat = update.Message.Chat.Id,
            };

            if (update.Message.Text != null)
            {
                input.Text = update.Message.Text;
            }

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