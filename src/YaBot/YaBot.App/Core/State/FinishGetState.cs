namespace YaBot.App.Core.State
{
    using System;
    using System.IO;
    using Database;
    using Extensions;
    using Telegram.Bot.Types.InputFiles;

    public sealed class FinishGetState : IState
    {
        private readonly Func<int, Place> getPlace;

        public FinishGetState(Func<int, Place> getPlace)
        {
            this.getPlace = getPlace;
        }

        public bool IsInput(Input input)
        {
            return true;
        }

        public (Output, IState) Process(Input input)
        {
            //TODO : validation

            if (int.TryParse(input.Message.Text, out var index).Not())
                return ("Неправильный формат. Введите только индекс".ToOutput(), this);

            var place = getPlace(index);

            if (place.Image == null)
                return (place.Name.ToOutput(), null);

            var stream = new MemoryStream(place.Image);
            var answer = new Output
            {
                Text = place.Name,
                Image = new InputOnlineFile(stream) // TODO : refactor Telegram.Bot.Api using
            };
            return (answer, null);
        }

        public IState Reset()
        {
            return this;
        }
    }
}