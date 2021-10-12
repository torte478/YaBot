namespace YaBot.Tests
{
    using NUnit.Framework;
    using Telegram.Bot.Types;
    using YaBot.IO;

    [TestFixture]
    internal sealed class JsonUpdate_Should
    {
        [Test]
        public void ConvertDateTimeEpoch_OnSerialize()
        {
            var update = new JsonUpdate(new Update { Message = new Message() });

            Assert.DoesNotThrow(
                () => update.ToString());
        }
    }
}