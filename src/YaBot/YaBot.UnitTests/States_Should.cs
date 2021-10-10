namespace YaBot.Tests
{
    using System;
    using System.Collections.Immutable;
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
            states.StateChanged += _ => { changed = _; };

            states.Process(A.Fake<IInput>());
            states.Process(reset);

            Assert.That(changed, Is.EqualTo("start"));
        }
    }
}