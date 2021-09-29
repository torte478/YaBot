﻿namespace YaBot.App.Core.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Database;
    using Extensions;
    using Outputs;
    using YaBot.Core;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;

    public sealed class PlaceCrudlState : IState
    {
        private const int Undefined = -1;
        
        private readonly Keys keys;
        private readonly ICrudl<int, Place> places;
        private readonly IOutputFactory<string, IWords, Place> outputs;
        private readonly Func<IEnumerable<string>, int, Pagination<string>> paginate;

        private State state;

        private int page;
        
        public string Name => "PlaceCrudl";
        
        public PlaceCrudlState(
            Keys keys, 
            ICrudl<int, Place> places, 
            IOutputFactory<string, IWords, Place> outputs,
            Func<IEnumerable<string>, int, Pagination<string>> paginate)
        {
            this.keys = keys;
            this.places = places;
            this.outputs = outputs;
            this.paginate = paginate;

            state = State.Start;
        }

        public bool IsInput(IInput input)
        {
            return state != State.Start
                   || Match(input.Text) != State.Unknown;
        }

        public IState Reset()
        {
            page = 0;
            state = State.Start;
            return this;
        }

        public (IOutput, IState) Process(IInput input)
        {
            return state switch
            {
                State.Start => StartOperation(input),
                State.Create => RunCreate(input),
                State.Read => RunRead(input),
                State.Delete => RunDelete(input),
                State.List => RunList(input),
                _ => (keys.Error.ToError("Неизвестное состояние")._(outputs.Create), null)
            };
        }

        private (IOutput, IState) StartOperation(IInput input)
        {
            return Match(input.Text) switch
            {
                State.Create => StartOperation(keys.Create, State.Create),
                State.Read => StartOperation(keys.Read, State.Read),
                State.Delete => StartOperation(keys.Delete, State.Delete),
                State.List => RunList(page),
                _ => (keys.Error.ToError("Не удалось выйти из начального состояния")._(outputs.Create), null) 
            };
        }

        private (IOutput, IState) RunDelete(IInput input)
        {
            var (index, error) = TryParseIndex(input.Text);
            if (index == Undefined)
                return (keys.Error.ToError(error)._(outputs.Create), this);

            var place = places.Enumerate().ToList()[index];
            places.Delete(place.Id);

            Reset();
            return (keys.Delete.Success._(outputs.Create), null);
        }

        private (int, string) TryParseIndex(string text)
        {
            if (text == null)
                return (Undefined, "Необходимо ввести текстовое сообщение");

            var list = places.Enumerate().ToList();
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

            var place = places.Enumerate().ToList()[index];

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
            state = next;
            return (key.Start._(outputs.Create), this);
        }

        private (IOutput, IState) RunList(IInput input)
        {
            if (keys.List.Next.Match(input.Text))
                return RunList(page + 1);

            if (keys.List.Previous.Match(input.Text))
                return RunList(page - 1);

            if (keys.List.Close.Match(input.Text))
            {
                Reset();
                return (null, null);
            }

            return (null, this);
        }

        private (IOutput, IState) RunList(int index)
        {
            var pagination = places
                .Enumerate()
                .Select(x => x.Name)
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

            state = State.List;
            return (result, this);
        }

        private static Place GetPlace(IInput input)
        {
            var place = new Place { Name = input.Text };
            
            if (input.IsImage)
                place.Image = input.Image;
            
            return place;
        }

        private State Match(string message)
        {
            //TODO : shit
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

        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine(base.ToString())
                .AppendLine(state.ToString())
                .ToString();
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
            public IWords Close { get; init; }
        }
    }
}