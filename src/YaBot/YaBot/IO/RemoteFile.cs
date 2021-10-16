namespace YaBot.IO
{
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public static class RemoteFile
    {
        public static async Task<byte[]> Load(ITelegramBotClient client, Update update, CancellationToken cancellation)
        {
            var photo = update!.Message!.Photo!
                .OrderByDescending(_ => _.Width * _.Height)
                .First();

            await using var stream = new MemoryStream();
            await client.GetInfoAndDownloadFileAsync(photo.FileId, stream, cancellation)
                .ConfigureAwait(false);

            return stream.ToArray();
        }
    }
}