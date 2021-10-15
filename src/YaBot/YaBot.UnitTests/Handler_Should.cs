namespace YaBot.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FakeItEasy;
    using NUnit.Framework;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using YaBot;
    using YaBot.IO;
    using YaBot.Tests.Fake;
    using static System.Threading.Tasks.Task;

    [TestFixture]
    internal sealed class Handler_Should
    {
        private Handler handler;
        private FakeReceiver receiver;

        [SetUp]
        public void SetUp()
        {
            receiver = new FakeReceiver();

            handler = Create(
                (_, _, _) => Run(A.Fake<IInput>),
                receiver.Receive,
                _ => { }
            );
        }

        [Test]
        public async Task AllowUpdate_WhenTypeIsNotText()
        {
            var update = new Update { Message = new Message() };

            await handler.HandleUpdateAsync(null, update, CancellationToken.None);

            Assert.That(receiver.Called, Is.True);
        }

        [Test]
        public void CheckThatMessageIsNotNull_WhenLog()
        {
            Assert.DoesNotThrowAsync(() =>
                handler.HandleUpdateAsync(null, new Update(), CancellationToken.None));
        }

        [Test]
        public async Task IgnoreMessage_WhenItIsNull()
        {
            await handler.HandleUpdateAsync(null, new Update(), CancellationToken.None);

            Assert.That(receiver.Called, Is.False);
        }

        [Test]
        public async Task IgnoreMessage_WhenItWasForwarded()
        {
            var update = new Update { Message = new Message { ForwardFrom = A.Fake<User>() } };

            await handler.HandleUpdateAsync(null, update, CancellationToken.None);

            Assert.That(receiver.Called, Is.False);
        }

        [Test]
        public void CatchExceptions_FromHandleUpdate()
        {
            var actual = string.Empty;
            Action<string> log = _ =>
            {
                actual += _;
            };

            var handler = Create(
                (_, _, _) => Run(() =>
                {
                    throw new Exception("EXPECTED");
#pragma warning disable 162
                    return A.Fake<IInput>();
#pragma warning restore 162
                }),
                _ => null,
                log);

            Assert.ThrowsAsync<Exception>(() =>
                handler.HandleUpdateAsync(null, new Update {Message = new Message()}, CancellationToken.None));

            Assert.That(actual.Contains("EXPECTED"), Is.True);
        }

        [Test]
        public async Task DoNotWriteArrow_WhenLogInputMessage()
        {
            var actual = string.Empty;
            Action<string> log = _ =>
            {
                actual = _;
            };

            var instance = Create(
                (_, _, _) => Run(A.Fake<IInput>),
                _ => null,
                log);

            await instance.HandleUpdateAsync(null, new Update {Message = new Message()}, CancellationToken.None);

            Assert.That(actual.Contains("=>"), Is.False);
        }

        [Test]
        public async Task SendCaptionEntities_WhenOutputHasImage()
        {
            var output = A.Fake<IOutput>();
            var expected = new MessageEntity();
            A.CallTo(() => output.IsImage).Returns(true);
            A.CallTo(() => output.Image).Returns(new byte[0]);
            A.CallTo(() => output.MessageEntities).Returns(new[] { expected });

            var handler = Create(
                (_, _, _) => Run(A.Fake<IInput>),
                _ => output,
                _ => { }
            );

            var client = new FakeClient();
            await handler.HandleUpdateAsync(
                    client,
                    new Update
                    {
                        Message = new Message
                        {
                            Chat = new Chat()
                        }

                    }, CancellationToken.None)
                .ConfigureAwait(false);

            var actual = client.LastPhotoRequest?.CaptionEntities?.First();
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static Handler Create(
            Func<ITelegramBotClient, Update, CancellationToken, Task<IInput>> toInputAsync,
            Func<IInput, IOutput> receive,
            Action<string> log)
        {
            return new Handler(toInputAsync, receive, _ => string.Empty, log);
        }

        private class FakeReceiver
        {
            public bool Called { get; private set; }

            public IOutput Receive(IInput input)
            {
                Called = true;
                return null;
            }
        }
    }
}