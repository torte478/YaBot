namespace YaBot.Tests.Fake
{
    using System;
    using System.Linq;
    using FakeItEasy;
    using Telegram.Bot.Types;
    using YaBot.App.Core;
    using YaBot.App.Core.Database;
    using YaBot.IO;

    internal sealed class FakeOutputFactory : IOutputFactory<Place>, IOutputFactory<string, IWords, Place>
    {
        public IOutput ToEmpty()
        {
            return A.Fake<IOutput>();
        }

        public IOutput Create(string input)
        {
            var output = Create();
            A.CallTo(() => output.Text).Returns(input);
            return output;
        }

        public IOutput Create(Place input)
        {
            var output = Create();
            A.CallTo(() => output.Text).Returns(input.Name);
            return output;
        }

        public IOutput Create(IWords input)
        {
            var output = Create();
            A.CallTo(() => output.Text).Returns(input?.FirstOrDefault());
            return output;
        }

        private static IOutput Create()
        {
            var output = A.Fake<IOutput>();
            A.CallTo(() => output.MessageEntities).Returns(Array.Empty<MessageEntity>());
            A.CallTo(() => output.IsText).Returns(true);
            return output;
        }
    }
}