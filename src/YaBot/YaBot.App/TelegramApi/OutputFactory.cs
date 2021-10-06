namespace YaBot.App.TelegramApi
{
    using System;
    using Core;
    using Core.Database;
    using Core.Outputs;
    using Extensions;
    using Telegram.Bot.Types;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;

    public sealed class OutputFactory : IOutputFactory<string, IWords, Place>, IOutputFactory<Place>
    {
        private readonly Func<string, (string, MessageEntity[])> deserialize;

        public OutputFactory(Func<string, (string, MessageEntity[])> deserialize)
        {
            this.deserialize = deserialize;
        }

        public IOutput ToEmpty()
        {
            return new Output();
        }

        public IOutput Create(string input)
        {
            var (text, entities) = input._(deserialize);
            return new Output
            {
                Text = text,
                MessageEntities = entities
            };
        }

        public IOutput Create(IWords input)
        {
            var (text, entities) = input.ToRandom()._(deserialize);
            return new Output
            {
                Text = text,
                MessageEntities = entities
            };
        }

        public IOutput Create(Place input)
        {
            var (text, entities) = input.Name._(deserialize);
            return new Output
            {
                Text = text,
                MessageEntities = entities,
                Image = input.Image
            };
        }
    }
}