namespace YaBot.App.Core.State.Impl
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using YaBot.App.Core.Database;
    using YaBot.App.Extensions;
    using YaBot;
    using YaBot.Extensions;
    using YaBot.IO;

    public sealed class PlaceCrudlState : IState
    {
        private const int Undefined = -1;
        
        private readonly Keys keys;
        private readonly ICrudl<int, Place> places;
        private readonly IOutputFactory<string, IWords, Place> outputs;
        private readonly Func<IQueryable<string>, int, Pagination<string>> paginate;
        private readonly Func<string, string> getTitle;

        private readonly ImmutableArray<(IWords, State)> matches;

        private State current;

        private int page;
        
        public string Name => "PlaceCrudl";

        public PlaceCrudlState(
            Keys keys, 
            ICrudl<int, Place> places, 
            IOutputFactory<string, IWords, Place> outputs,
            Func<IQueryable<string>, int, Pagination<string>> paginate,
            Func<string, string> getTitle)
        {
            this.keys = keys;
            this.places = places;
            this.outputs = outputs;
            this.paginate = paginate;
            this.getTitle = getTitle;

            matches = FillMatches();

            current = State.Start;
        }

        public bool IsInput(IInput input)
        {
            return current != State.Start
                   || Match(input.Text) != State.Unknown;
        }

        public IState Reset()
        {
            page = 0;
            current = State.Start;
            return this;
        }

        public (IOutput, IState) Process(IInput input)
        {
            return current switch
            {
                State.Start => ProcessInnerState(input),
                State.Create => RunCreate(input),
                State.Read => RunRead(input),
                State.Delete => RunDelete(input),
                _ => (keys.Error.ToError("Неизвестное состояние")._(outputs.Create), null)
            };
        }

        private (IOutput, IState) ProcessInnerState(IInput input)
        {
            return Match(input.Text) switch
            {
                State.Create => StartOperation(keys.Create, State.Create),
                State.Read => StartOperation(keys.Read, State.Read),
                State.Delete => StartOperation(keys.Delete, State.Delete),

                State.List => ShowPage(page),
                State.Previous => ShowPage(page - 1),
                State.Next => ShowPage(page + 1),

                _ => (keys.Error.ToError("Не удалось выйти из начального состояния")._(outputs.Create), null) 
            };
        }

        private (IOutput, IState) RunDelete(IInput input)
        {
            var (index, error) = TryParseIndex(input.Text);
            if (index == Undefined)
                return (keys.Error.ToError(error)._(outputs.Create), this);

            var place = places.All().ToList()[index];
            places.Delete(place.Id);

            Reset();
            return (keys.Delete.Success._(outputs.Create), null);
        }

        private (int, string) TryParseIndex(string text)
        {
            if (text == null)
                return (Undefined, "Необходимо ввести текстовое сообщение");

            var list = places.All().ToList();
            var max = list.Count - 1;

            var parsed = int.TryParse(text, out var index);
            --index;

            if (parsed.Not()
                || index < 0
                || index > max)
                return (Undefined, $"Неправильный формат. Нужно ввести число в диапазоне 1 - {max + 1}");

            return (index, null);
        }

        private (IOutput, IState) RunRead(IInput input)
        {
            var (index, error) = TryParseIndex(input.Text);
            if (index == Undefined)
                return (keys.Error.ToError(error)._(outputs.Create), this);

            var place = places.All().ToList()[index];

            Reset();

            return (outputs.Create(place), null);
        }

        private (IOutput, IState) RunCreate(IInput input)
        {
            if (input.Text._(string.IsNullOrEmpty))
                return (keys.Error.ToError("Нужно ввести название")._(outputs.Create), this);

            input
                ._(GetPlace)
                ._(places.Create);

            Reset();

            return (keys.Create.Success._(outputs.Create), null);       
        }

        private (IOutput, IState) StartOperation(StateKeys key, State next)
        {
            current = next;
            return (key.Start._(outputs.Create), this);
        }

        private (IOutput, IState) ShowPage(int index)
        {
            var pagination = places
                .All()
                .Select(_ => getTitle(_.Name))
                ._(paginate, index);

            page = pagination.Index;

            var interval = pagination.Paginated
                ? $": {pagination.Start + 1}..{pagination.Finish + 1} из {pagination.Total}"
                : string.Empty;

            var header = new StringBuilder()
                .Append(keys.List.Success.ToRandom())
                .AppendLine(interval)
                .AppendLine();

            var result = pagination.Items
                .Select(_ => $"{_.Item1 + 1}: {_.Item2}")
                .Aggregate(
                    header,
                    (acc, x) => acc.AppendLine(x))
                .ToString()
                ._(outputs.Create);

            return (result, null);
        }

        private static Place GetPlace(IInput input)
        {
            var place = new Place { Name = input.Text };
            
            if (input.IsImage)
                place.Image = input.Image;
            
            return place;
        }

        private ImmutableArray<(IWords, State)> FillMatches()
        {
            return new[]
            {
                (keys.Create?.Keys, State.Create),
                (keys.Read?.Keys, State.Read),
                (keys.Delete?.Keys, State.Delete),
                (keys.List?.Keys, State.List),
                (keys.List?.Next, State.Next),
                (keys.List?.Previous, State.Previous),
            }
                .Where(x => x.Item1 != null)
                .ToImmutableArray();
        }

        private State Match(string message)
        {
            foreach (var (key, state) in matches)
            {
                if (key.Match(message))
                    return state;
            }

            return State.Unknown;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine(base.ToString())
                .AppendLine(current.ToString())
                .ToString();
        }

        private enum State
        {
            Unknown,
            Start,

            Create,
            Read,
            Delete,
            List,
            Next,
            Previous
        }

        public sealed class Keys
        {
            public StateKeys Create { get; init; }
            public StateKeys Read { get; init; }
            public StateKeys Delete { get; init; }
            public StateKeys List { get; init; }
            public IWords Error { get; init; }
        }

        public sealed class StateKeys
        {
            public IWords Keys { get; init; }
            public IWords Start { get; init; }
            public IWords Success { get; init; }
            public IWords Next { get; init; }
            public IWords Previous { get; init; }
        }
    }
}