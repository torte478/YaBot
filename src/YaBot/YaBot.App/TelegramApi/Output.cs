namespace YaBot.App.TelegramApi
{
    using System.IO;
    using Core;
    using Core.Database;
    using Extensions;

    public sealed class Output : IOutput
    {
        public byte[] Image { get; init; }
        public string Text { get; init; }
        
        public bool IsImage => Image != null;
        public bool IsText => string.IsNullOrEmpty(Text).Not();
        
        private Output() {}

        public static IOutput Create(IWords words)
        {
            return new Output
            {
                Text = words.ToRandom()
            };
        }
        
        public static IOutput Create(string text)
        {
            return new Output
            {
                Text = text
            };
        }

        public static IOutput Create(Place place)
        {
            return new Output
            {
                Text = place.Name,
                Image = place.Image
            };
        }
    }
}