namespace YaBot.Tests.App
{
    using FakeItEasy;
    using NUnit.Framework;
    using YaBot.App.Core;
    using YaBot.App.Core.State.Impl;
    using YaBot.IO;
    using YaBot.Tests.Fake;

    [TestFixture]
    internal sealed class AufState_Should
    {
        [Test]
        public void MatchSubstring_WhenParseInput()
        {
            var keys = A.Fake<IWords>();
            A.CallTo(() => keys.Match(A<string>._, true)).Returns(true);

            var state = new AufState(keys, new FakeOutputFactory(), A.Fake<IWords>());

            var match = state.IsInput(A.Fake<IInput>());

            Assert.That(match, Is.True);
        }
    }
}