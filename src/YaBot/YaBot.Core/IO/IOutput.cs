namespace YaBot.Core.IO
{
    using Telegram.Bot.Types;

    public interface IOutput
    {
        byte[] Image { get; }
        string Text { get; }
        MessageEntity[] MessageEntities { get; }
        bool IsImage { get; }
        bool IsText { get; }
    }
}