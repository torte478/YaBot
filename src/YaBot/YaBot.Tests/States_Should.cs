namespace YaBot.Tests
{
    using App.Core;
    using App.Core.State;
    using App.Extensions;
    using App.TelegramApi;
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
            var changeState = A.Fake<IInput>();
            var reset = A.Fake<IInput>();

            A.CallTo(() => stoppers.Match(reset.Text)).Returns(true);
            A.CallTo(() => start.Process(A<IInput>._)).Returns(("start"._(Output.Create), next));
            A.CallTo(() => next.Process(A<IInput>._)).Returns(("next"._(Output.Create), next));

            var states = new States(
                start, 
                stoppers, 
                A.Fake<IWords>(),
                Output.Create,
                _ => { });

            states.Process(changeState);
            states.Process(reset);
            
            Assert.That(states.Process(A.Fake<IInput>()).Text, Is.EqualTo("start"));
        }
    }
}