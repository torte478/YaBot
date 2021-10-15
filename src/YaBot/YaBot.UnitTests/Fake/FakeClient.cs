namespace YaBot.Tests.Fake
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Exceptions;
    using Telegram.Bot.Requests;
    using Telegram.Bot.Requests.Abstractions;

    internal sealed class FakeClient : ITelegramBotClient
    {
        public SendPhotoRequest LastPhotoRequest { get; private set; }

        public Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (request is SendPhotoRequest photo)
                LastPhotoRequest = photo;

            return Task.Run(() => default(TResponse));
        }

        public Task<bool> TestApiAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task DownloadFileAsync(string filePath, Stream destination,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public long? BotId { get; }
        public TimeSpan Timeout { get; set; }
        public IExceptionParser ExceptionsParser { get; set; }
        public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest;
        public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived;
    }
}