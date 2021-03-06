namespace YaBot.Tests.App
{
    using System.Collections.Immutable;
    using FakeItEasy;
    using NUnit.Framework;
    using YaBot.App.Core.State;
    using YaBot.IO;

    [TestFixture]
    internal sealed class States_Should
    {
        [Test]
        public void ChangeStateToStart_AfterReset()
        {
            var start = A.Fake<IState>();
            A.CallTo(() => start.Name).Returns("start");
            A.CallTo(() => start.Process(A.Fake<IInput>()))
                .Returns((A.Fake<IOutput>(), A.Fake<IState>()));

            var reset = A.Fake<IInput>();
            var resetState = A.Fake<IState>();
            A.CallTo(() => resetState.IsInput(reset)).Returns(true);

            var states = new States(
                start,
                ImmutableArray<IState>.Empty,
                new[] { resetState }.ToImmutableArray(),
                ImmutableArray<IState>.Empty,
                _ => { });
            var changed = string.Empty;
            states.Changed += _ => { changed = _; };

            states.Process(A.Fake<IInput>());
            states.Process(reset);

            Assert.That(changed, Is.EqualTo("start"));
        }
    }
}