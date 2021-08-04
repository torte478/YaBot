namespace YaBot.App.Core.State
{
    using System;
    using System.IO;
    using System.Linq;
    using Castle.Core.Internal;
    using Database;
    using Extensions;

    internal sealed class FinishCreatePlaceState : IState
    {
        private readonly Action<Place> savePlace;
        
        public FinishCreatePlaceState(Action<Place> savePlace)
        {
            this.savePlace = savePlace;
        }

        public bool IsInput(Input input)
        {
            // TODO : is text with image
            return !input.Message.Text.IsNullOrEmpty()
                || input.Message.Photo != null;
        }

        public (Output, IState) Process(Input input)
        {
            // TODO : null validators
            
            input
                ._(GetPlace)
                ._(savePlace);

            return ("Новое место сохранено".ToOutput(), null); // (TODO, TODO)
        }

        public IState Reset()
        {
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
        
    }
}