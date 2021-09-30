namespace YaBot.Tests
{
    using System;
    using App.Core;
    using App.Core.State;
    using App.TelegramApi;
    using FakeItEasy;
    using NUnit.Framework;
    using Telegram.Bot.Types;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;

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
            var outputs = new OutputFactory(_ => (_, Array.Empty<MessageEntity>())); //TODO: dependency

            A.CallTo(() => stoppers.Match(reset.Text)).Returns(true);
            A.CallTo(() => start.Process(A<IInput>._)).Returns(("start"._(outputs.Create), next));
            A.CallTo(() => next.Process(A<IInput>._)).Returns(("next"._(outputs.Create), next));

            var states = new States(
                string.Empty,
                start, 
                stoppers, 
                A.Fake<IWords>(),
                A.Fake<IWords>(),
                outputs,
                _ => { });

            states.Process(changeState);
            states.Process(reset);
            
            Assert.That(states.Process(A.Fake<IInput>()).Text, Is.EqualTo("start"));
        }
    }
}