namespace YaBot.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using App.Core;
    using App.Core.Outputs;
    using App.TelegramApi;
    using FakeItEasy;
    using NUnit.Framework;
    using Telegram.Bot.Types;
    using YaBot.Core;

    [TestFixture]
    internal sealed class Handler_Should
    {
        [Test]
        public async Task AllowUpdate_WhenTypeIsNotText()
        {
            var called = false;
            var handler = Create(_ => { called = true; return null; });
            var update = new Update { Message = new Message() };

            await handler.HandleUpdateAsync(null, update, new CancellationToken());

            Assert.That(called, Is.True);
        }

        [Test]
        public void CheckThatMessageIsNotNull_WhenLog()
        {
            var handler = Create(null);

            Assert.DoesNotThrowAsync(() => 
                handler.HandleUpdateAsync(null, new Update(), new CancellationToken()));
        }

        [Test]
        public async Task IgnoreMessage_WhenItIsNull()
        {
            var called = false;
            var handler = Create(_ => { called = true; return null; });

            await handler.HandleUpdateAsync(null, new Update(), new CancellationToken());

            Assert.That(called, Is.False);
        }

        private static Handler Create(Func<IInput, IOutput> receive)
        {
            return new Handler(
                (_, _, _) => Task.Run(A.Fake<IInput>),
                receive,
                _ => { }
            );
        }
    }
}