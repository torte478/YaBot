namespace YaBot.App.Core.State
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Database;
    using Extensions;
    using Telegram.Bot.Types.InputFiles;

    public sealed class GetRandomPlaceState : IState
    {
        private readonly Random random = new();
        
        private readonly IWords keys;
        private readonly Func<IEnumerable<Place>> getPlaces;

        private readonly List<Place> places = new();
        private int index = 0;

        public GetRandomPlaceState(IWords keys, Func<IEnumerable<Place>> getPlaces)
        {
            this.keys = keys;
            this.getPlaces = getPlaces;
        }

        public bool IsInput(Input input)
        {
            return keys.Match(input.Message);
        }

        public (Output, IState) Process(Input input)
        {
            if (index >= places.Count)
                ReloadPlaces();

            var place = places[index++];

            return place._(ToAnswer);
        }

        private (Output, IState) ToAnswer(Place place)
        {
            //TODO : GetState duplicate
            
            if (place.Image == null)
                return (place.Name.ToOutput(), this);

            var stream = new MemoryStream(place.Image);
            var answer = new Output
            {
                Text = place.Name,
                Image = new InputOnlineFile(stream) // TODO : refactor Telegram.Bot.Api using
            };
            return (answer, this);
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