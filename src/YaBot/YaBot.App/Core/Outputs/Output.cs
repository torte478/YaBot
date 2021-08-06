namespace YaBot.App.Core.Outputs
{
    using Extensions;

    public sealed class Output : IOutput
    {
        public byte[] Image { get; init; }
        public string Text { get; init; }
        
        public bool IsImage => Image != null;
        public bool IsText => IsImage.Not()
                              && string.IsNullOrEmpty(Text).Not();
    }
}