namespace YaBot.Tests
{
    using App.Core;
    using App.Core.State;
    using App.Extensions;
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
            var changeState = A.Fake<Input>();
            var reset = A.Fake<Input>();

            A.CallTo(() => stoppers.Match(reset.Message)).Returns(true);
            A.CallTo(() => start.Process(A<Input>._)).Returns(("start".ToAnswer(), next));
            A.CallTo(() => next.Process(A<Input>._)).Returns(("next".ToAnswer(), next));

            var states = new States(start, stoppers, _ => { });

            states.Process(changeState);
            states.Process(reset);
            
            Assert.That(states.Process(A.Fake<Input>()).Text, Is.EqualTo("start"));
        }
    }
}