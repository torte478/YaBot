namespace YaBot.Tests.App
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FakeItEasy;
    using NUnit.Framework;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using YaBot.IO;

    [TestFixture]
    internal sealed class InputFactory_Should
    {
        [Test]
        public async Task SerializeText_WhenMessageContainsImage()
        {
            var factory = new InputFactory(
                (_, _) => "EXPECTED",
                (_, _, _) => Task.Run(() => new byte[0]));

            var actual = await factory.CreateAsync(
                A.Fake<ITelegramBotClient>(),
                new Update
                {
                    Message = new Message
                    {
                        Chat = new Chat(),
                        Photo = new[] {new PhotoSize()}
                    }
                },
                CancellationToken.None)
                .ConfigureAwait(false);

            Assert.That(actual.Text, Is.EqualTo("EXPECTED"));
        }

        [Test]
        public async Task FormatText_WhenMessageContainsCaptionEntities()
        {
            var factory = new InputFactory(
                (_, entities) => entities?.Any() ?? false ? "EXPECTED" : string.Empty,
                (_, _, _) => Task.Run(() => new byte[0]));

            var actual = await factory.CreateAsync(
                    A.Fake<ITelegramBotClient>(),
                    new Update
                    {
                        Message = new Message
                        {
                            Chat = new Chat(),
                            Photo = new[] {new PhotoSize()},
                            CaptionEntities = new[] { new MessageEntity() }
                        }
                    },
                    CancellationToken.None)
                .ConfigureAwait(false);

            Assert.That(actual.Text, Is.EqualTo("EXPECTED"));
        }
    }
}