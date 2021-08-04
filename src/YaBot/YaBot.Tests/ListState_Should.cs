namespace YaBot.Tests
{
    using System;
    using System.Linq;
    using App.Core;
    using App.Core.Database;
    using App.Core.State;
    using App.Extensions;
    using FakeItEasy;
    using NUnit.Framework;
    using Telegram.Bot.Types;

    [TestFixture]
    internal sealed class ListState_Should
    {
        [Test]
        public void SetHeaderToOwnLine_AfterListFormat()
        {
            var keys = A.Fake<IWords>();
            A.CallTo(() => keys.Match(A<Message>._)).Returns(true);

            var state = new ListState(
                keys,
                new[] {"test"}._(Words.Create),
                () => Enumerable.Range(1, 3).Select(_ => A.Fake<Place>()));

            var count = state.Process(A.Fake<Input>())
                .Item1
                .Text
                .Split(Environment.NewLine)
                .Length;
            
            Assert.That(count, Is.EqualTo(5));
        }
    }
}