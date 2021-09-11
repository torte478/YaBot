namespace YaBot.App.Core.State
{
    using System;
    using System.Collections.Generic;
    using Database;
    using Extensions;
    using Outputs;
    using YaBot.Core;
    using YaBot.Core.Extensions;

    public sealed class GetRandomPlaceState : IState
    {
        private readonly Random random = new();
        
        private readonly IWords keys;
        private readonly IWords next;
        private readonly Func<IEnumerable<Place>> getPlaces;
        private readonly IOutputFactory<Place> outputs;

        private readonly List<Place> places = new();
        private int index;
        
        public string Name => "GetRandomPlace";

        public GetRandomPlaceState(
            IWords keys, 
            IWords next, 
            Func<IEnumerable<Place>> getPlaces, 
            IOutputFactory<Place> outputs)
        {
            this.keys = keys;
            this.next = next;
            this.getPlaces = getPlaces;
            this.outputs = outputs;
        }

        public bool IsInput(IInput input)
        {
            return keys.Match(input.Text);
        }

        public (IOutput, IState) Process(IInput input)
        {
            var process = input.Text
                ._(_ => keys.Match(_) || next.Match(_));
            if (!process)
                return (outputs.ToEmpty(), this);
            
            if (index >= places.Count)
                ReloadPlaces();

            var place = places[index++];

            return (place._(outputs.Create), this);
        }

        public IState Reset()
        {
            index = 0;
            places.Clear();
            return this;
        }

        private void ReloadPlaces()
        {
            Reset();
            
            places.AddRange(getPlaces());

            for (var i = places.Count - 1; i > 0; --i)
            {
                var j = random.Next(i + 1);
                
                var temp = places[j];
                places[j] = places[i];
                places[i] = temp;
            }
        }
    }
}