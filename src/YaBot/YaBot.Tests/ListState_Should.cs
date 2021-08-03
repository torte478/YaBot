namespace YaBot.Tests
{
    using System;
    using System.Linq;
    using App.Core;
    using App.Core.Database;
    using App.Core.State;
    using FakeItEasy;
    using NUnit.Framework;

    [TestFixture]
    internal sealed class ListState_Should
    {
        [Test]
        public void SetHeaderToOwnLine_AfterListFormat()
        {
            var state = new ListState(
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