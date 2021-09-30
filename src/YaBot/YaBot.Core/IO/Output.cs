namespace YaBot.Core.IO
{
    using Telegram.Bot.Types;
    using YaBot.Core.Extensions;

    public sealed class Output : IOutput
    {
        public byte[] Image { get; init; }
        public string Text { get; init; }
        public MessageEntity[] MessageEntities { get; init; }

        public bool IsImage => Image != null;
        public bool IsText => IsImage.Not()
                              && string.IsNullOrEmpty(Text).Not();
    }
}