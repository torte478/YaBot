namespace YaBot.Tests
{
    using System.Linq;
    using FakeItEasy;
    using NUnit.Framework;
    using Telegram.Bot.Types;
    using YaBot.App.Core.Database;
    using YaBot.App.TelegramApi;

    [TestFixture]
    internal sealed class OutputFactory_Should
    {
        [Test]
        public void InitMessageEntities_WhenCreateFromPlace()
        {
            var entity = new MessageEntity();
            var factory = new OutputFactory(_ => (_, new[] { entity }));

            var actual = factory.Create(A.Fake<Place>());

            Assert.That(actual.MessageEntities.First(), Is.EqualTo(entity));
        }
    }
}