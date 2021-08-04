namespace YaBot.App.Core.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Database;
    using Extensions;

    public sealed class ListState : IState
    {
        private readonly Func<IEnumerable<Place>> getPlaces;
        private readonly IWords keys;
        private readonly IWords success;

        public ListState(IWords keys, IWords success, Func<IEnumerable<Place>> getPlaces)
        {
            this.getPlaces = getPlaces;
            this.keys = keys;
            this.success = success;
        }

        public bool IsInput(Input input)
        {
            return keys.Match(input.Message);
        }

        public (Output, IState) Process(Input input)
        {
            var list = getPlaces()
                .Select(_ => _.Name)
                .ToList()
                .Select((x, i) => $"{i}. {x}")
                .Aggregate(
                    new StringBuilder().AppendLine(success.ToRandom()),
                    (acc, x) => acc.AppendLine(x))
                .ToString()
                .ToOutput();
            
            // TODO : pagination

            return (list, null);
        }

        public IState Reset()
        {
            return this;
        }
    }
}