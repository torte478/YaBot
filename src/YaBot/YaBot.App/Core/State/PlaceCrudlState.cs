namespace YaBot.App.Core.State
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using Database;
    using Extensions;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.InputFiles;

    public sealed class PlaceCrudlState : IState
    {
        private readonly Keys keys;
        private readonly ICrudl<int, Place> places;
        
        private State state;
        
        public PlaceCrudlState(Keys keys, ICrudl<int, Place> places)
        {
            this.keys = keys;
            this.places = places;

            state = State.Start;
        }

        public bool IsInput(Input input)
        {
            return state != State.Start
                   || Match(input.Message) != State.Unknown;
        }

        public IState Reset()
        {
            state = State.Start;
            return this;
        }

        public (Output, IState) Process(Input input)
        {
            return state switch
            {
                State.Start => StartOperation(input),
                State.Create => RunCreate(input),
                State.Read => RunRead(input),
                State.Delete => RunDelete(input),
                _ => (keys.Error.ToError("Неизвестное состояние").ToOutput(), null)
            };
        }

        private (Output, IState) StartOperation(Input input)
        {
            return Match(input.Message) switch
            {
                State.Create => StartOperation(keys.Create, State.Create),
                State.Read => StartOperation(keys.Read, State.Read),
                State.Delete => StartOperation(keys.Delete, State.Delete),
                State.List => RunList(),
                _ => (keys.Error.ToError("Не удалось выйти из начального состояния").ToOutput(), null) 
            };
        }

        private (Output, IState) RunDelete(Input input)
        {
            //TODO : validation

            if (int.TryParse(input.Message.Text, out var index).Not())
                return (keys.Error.ToError("Неправильный формат. Введите только индекс").ToOutput(), this);

            var place = places.Enumerate().ToList()[index];
            places.Delete(place.Id);

            Reset();
            return (keys.Delete.Success.ToRandomOutput(), null);
        }

        private (Output, IState) RunRead(Input input)
        {
            //TODO : validation

            if (int.TryParse(input.Message.Text, out var index).Not())
                return (keys.Error.ToError("Неправильный формат. Введите только индекс").ToOutput(), this);

            var place = places.Enumerate().ToList()[index];

            Reset();

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

        private (Output, IState) RunCreate(Input input)
        {
            // TODO : null validators

            input
                ._(GetPlace)
                ._(places.Create);

            Reset();

            return (keys.Create.Success.ToRandomOutput(), null); // (_, TODO)       
        }

        private (Output, IState) StartOperation(StateKeys key, State next)
        {
            state = next;
            return (key.Start.ToRandomOutput(), this);
        }

        private (Output, IState) RunList()
        {
            var list = places
                .Enumerate()
                .Select(_ => _.Name)
                .ToList()
                .Select((x, i) => $"{i}. {x}")
                .Aggregate(
                    new StringBuilder().AppendLine(keys.List.Success.ToRandom()),
                    (acc, x) => acc.AppendLine(x))
                .ToString()
                .ToOutput();
            
            // TODO : pagination

            return (list, null); // TODO : null
        }

        private static Place GetPlace(Input input)
        {
            if (input.Message.Photo == null)
                return new Place { Name = input.Message.Text };
            
            var photo = input.Message.Photo
                .OrderByDescending(_ => _.Width * _.Height)
                .First();
                
            using var stream = new MemoryStream();
            input.Client.GetInfoAndDownloadFileAsync(photo.FileId, stream).Wait(); // TODO : to async

            return new Place
            {
                Name = input.Message.Caption,
                Image = stream.ToArray()
            };
        }

        private State Match(Message message)
        {
            if (keys.Create?.Keys.Match(message) ?? false)
                return State.Create;

            if (keys.Read?.Keys.Match(message) ?? false)
                return State.Read;
                
            if (keys.Delete?.Keys.Match(message) ?? false)
                return State.Delete;
                
            if (keys.List?.Keys.Match(message) ?? false)
                return State.List;

            return State.Unknown;
        }
        
        private enum State
        {
            Unknown,
            Start,

            Create,
            Read,
            Delete,
            List
        }

        public sealed class Keys
        {
            public StateKeys Create { get; set; }
            public StateKeys Read { get; set; }
            public StateKeys Delete { get; set; }
            public StateKeys List { get; set; }
            
            public IWords Error { get; set; }
        }

        public sealed class StateKeys
        {
            public IWords Keys { get; set; }
            public IWords Start { get; set; }
            public IWords Success { get; set; }
        }
    }
}