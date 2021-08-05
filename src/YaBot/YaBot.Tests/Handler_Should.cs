namespace YaBot.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using App.Core;
    using App.TelegramApi;
    using NUnit.Framework;
    using Telegram.Bot.Types;

    [TestFixture]
    internal sealed class Handler_Should
    {
        [Test]
        public void AllowUpdate_WhenTypeIsNotText()
        {
            var called = false;
            var handler = Create(_ => { called = true; return null; });
            var update = new Update { Message = new Message() };

            handler.HandleUpdate(null, update, new CancellationToken()).Wait();

            Assert.That(called, Is.True);
        }

        [Test]
        public void CheckThatMessageIsNotNull_WhenLog()
        {
            var handler = Create(null);

            Assert.DoesNotThrow(() =>
            {
                handler.HandleUpdate(null, new Update(), new CancellationToken()).Wait();
            });
        }

        [Test]
        public void IgnoreMessage_WhenItIsNull()
        {
            var called = false;
            var handler = Create(_ => { called = true; return null; });

            handler.HandleUpdate(null, new Update(), new CancellationToken()).Wait();

            Assert.That(called, Is.False);
        }

        private static Handler Create(Func<IInput, IOutput> receive)
        {
            return new Handler(
                (_, _, _) => null,
                receive,
                _ => { }
            );
        }
    }
}