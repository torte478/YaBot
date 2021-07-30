namespace YaBot.Tests
{
    using App.Core;
    using App.Core.State;
    using FakeItEasy;
    using NUnit.Framework;

    [TestFixture]
    internal sealed class States_Should
    {
        [Test]
        public void ChangeStateToStart_AfterReset()
        {
            var start = A.Fake<IState>();
            var next = A.Fake<IState>();
            var stoppers = A.Fake<IWords>();
            var changeState = A.Fake<IWords>();
            var reset = A.Fake<IWords>();

            A.CallTo(() => stoppers.Match(reset)).Returns(true);
            A.CallTo(() => start.Process(A<IWords>._)).Returns(("start", next));
            A.CallTo(() => next.Process(A<IWords>._)).Returns(("next", next));

            var states = new States(start, stoppers, _ => { });

            states.Process(changeState);
            states.Process(reset);
            
            Assert.That(states.Process(A.Fake<IWords>()), Is.EqualTo("start"));
        }
    }
}