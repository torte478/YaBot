namespace YaBot.Core
{
    public interface IOutput
    {
        byte[] Image { get; }
        string Text { get; }
        bool IsImage { get; }
        bool IsText { get; }
    }
}