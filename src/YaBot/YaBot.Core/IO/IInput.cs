namespace YaBot.Core.IO
{
    using System;

    public interface IInput
    {
        DateTime Date { get; }
        long Chat { get; }
        string Text { get; }
        bool IsImage { get; }
        byte[] Image { get; }
    }
}