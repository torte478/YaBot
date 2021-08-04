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
                   || keys.Match(input.Message) != null;
        }

        public (Output, IState) Process(Input input)
        {
            if (state == State.Start)
            {
                var match = keys.Match(input.Message);
                if (match == keys.Create)
                {
                    state = State.Create;
                    return (keys.Create.Start.ToRandomOutput(), this);
                }
                if (match == keys.Read)
                {
                    state = State.Read;
                    return (keys.Read.Start.ToRandomOutput(), this);
                }
                if (match == keys.Delete)
                {
                    state = State.Delete;
                    return (keys.Delete.Start.ToRandomOutput(), this);
                }

                if (match == keys.List)
                {
                    var list = places
                        .ToList()
                        .Select(_ => _.Name)
                        .ToList()
                        .Select((x, i) => $"{i}. {x}")
                        .Aggregate(
                            new StringBuilder().AppendLine(keys.List.Success.ToRandom()),
                            (acc, x) => acc.AppendLine(x))
                        .ToString()
                        .ToOutput();
            
                    // TODO : pagination

                    return (list, null);
                }

                return (keys.Error.ToError("Не удалось выйти из начального состояния").ToOutput(), null);
            }

            if (state == State.Create)
            {
                // TODO : null validators
            
                input
                    ._(GetPlace)
                    ._(places.Create);

                Reset();
                
                return (keys.Create.Success.ToRandomOutput(), null); // (_, TODO)       
            }

            if (state == State.Read)
            {
                //TODO : validation

                if (int.TryParse(input.Message.Text, out var index).Not())
                    return (keys.Error.ToError("Неправильный формат. Введите только индекс").ToOutput(), this);

                var place = places.Read(index);

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

            if (state == State.Delete)
            {
                //TODO : validation

                if (int.TryParse(input.Message.Text, out var index).Not())
                    return (keys.Error.ToError("Неправильный формат. Введите только индекс").ToOutput(), this);

                places.Delete(index);

                Reset();
                return (keys.Delete.Success.ToRandomOutput(), null);
            }
            
            return (keys.Error.ToError("Неизвестное состояние").ToOutput(), null);
        }

        public IState Reset()
        {
            state = State.Start;
            return this;
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

        private enum State
        {
            Start,

            Create,
            Read,
            Delete
        }

        public sealed class Keys
        {
            public StateKeys Create { get; set; }
            public StateKeys Read { get; set; }
            public StateKeys Delete { get; set; }
            public StateKeys List { get; set; }
            
            public IWords Error { get; set; }

            public StateKeys Match(Message message)
            {
                return new[]
                    {
                        Create,
                        Read,
                        Delete,
                        List
                    }
                    .FirstOrDefault(_ => _?.Keys.Match(message) ?? false); // TODO : null
            }
        }

        public sealed class StateKeys
        {
            public IWords Keys { get; set; }
            public IWords Start { get; set; }
            public IWords Success { get; set; }
        }
    }
}