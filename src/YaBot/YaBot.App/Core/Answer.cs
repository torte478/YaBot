namespace YaBot.App.Core
{
    using Telegram.Bot.Types.InputFiles;

    public sealed class Answer // TOOD : IDisposable
    {
        public string Text { get; set; }
        public InputOnlineFile Image { get; set; }
    }
}