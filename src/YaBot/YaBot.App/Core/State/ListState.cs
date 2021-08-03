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

        public ListState(Func<IEnumerable<Place>> getPlaces)
        {
            this.getPlaces = getPlaces;
        }

        public bool IsInput(Input input)
        {
            return input.Message.Text?.Contains("list") ?? false; // TODO : hardcode
        }

        public (Output, IState) Process(Input input)
        {
            var list = getPlaces()
                .Select(_ => _.Name)
                .ToList()
                .Select((x, i) => $"{i}. {x}")
                .Aggregate(
                    new StringBuilder().AppendLine("Список мест:"),
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