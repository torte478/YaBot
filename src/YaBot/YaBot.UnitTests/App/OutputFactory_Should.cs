namespace YaBot.Tests.App
{
    using System.Linq;
    using FakeItEasy;
    using NUnit.Framework;
    using Telegram.Bot.Types;
    using YaBot.App.Core;
    using YaBot.App.Core.Database;

    [TestFixture]
    internal sealed class OutputFactory_Should
    {
        [Test]
        public void InitMessageEntities_WhenCreatesFromPlace()
        {
            var entity = new MessageEntity();
            var factory = new OutputFactory(_ => (_, new[] { entity }));

            var actual = factory.Create(A.Fake<Place>());

            Assert.That(actual.MessageEntities.First(), Is.EqualTo(entity));
        }

        [Test]
        public void ReturnDeserializedText_WhenCreatesFromPlace()
        {
            var entity = new MessageEntity();
            var factory = new OutputFactory(_ => ("EXPECTED", new[] { entity }));

            var actual = factory.Create(A.Fake<Place>());

            Assert.That(actual.Text, Is.EqualTo("EXPECTED"));
        }
    }
}