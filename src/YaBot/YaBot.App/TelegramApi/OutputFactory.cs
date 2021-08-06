namespace YaBot.App.TelegramApi
{
    using Core;
    using Core.Database;
    using Core.Outputs;
    using Extensions;

    public sealed class OutputFactory : IOutputFactory<string, IWords, Place>, IOutputFactory<Place>
    {
        public IOutput ToEmpty()
        {
            return new Output();
        }

        public IOutput Create(string input)
        {
            return new Output
            {
                Text = input
            };
        }

        public IOutput Create(IWords input)
        {
            return new Output
            {
                Text = input.ToRandom()
            };
        }

        public IOutput Create(Place input)
        {
            return new Output
            {
                Text = input.Name,
                Image = input.Image
            };
        }
    }
}