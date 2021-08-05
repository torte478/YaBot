namespace YaBot.App.Core.State
{
    using System;
    using System.Collections.Generic;
    using Database;
    using Extensions;

    public sealed class GetRandomPlaceState : IState
    {
        private readonly Random random = new();
        
        private readonly IWords keys;
        private readonly Func<IEnumerable<Place>> getPlaces;
        private readonly Func<Place, IOutput> toOutput;

        private readonly List<Place> places = new();
        private int index = 0;

        public GetRandomPlaceState(IWords keys, Func<IEnumerable<Place>> getPlaces, Func<Place, IOutput> toOutput)
        {
            this.keys = keys;
            this.getPlaces = getPlaces;
            this.toOutput = toOutput;
        }

        public bool IsInput(IInput input)
        {
            return keys.Match(input.Text);
        }

        public (IOutput, IState) Process(IInput input)
        {
            if (index >= places.Count)
                ReloadPlaces();

            var place = places[index++];

            return (place._(toOutput), this);
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
            
            // TODO : write better algorithm
            
            var input = getPlaces() 
                ._(_ => new LinkedList<Place>(_));

            while (input.Count > 0)
            {
                var current = input.First;
                for (var i = random.Next(input.Count); i > 0; --i)
                    current = current.Next;
                
                places.Add(current.Value);
                input.Remove(current);
            }
        }
    }
}