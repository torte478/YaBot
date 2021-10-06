namespace YaBot.Tests
{
    using NUnit.Framework;
    using YaBot.Core.IO.Format;

    [TestFixture]
    internal sealed class Title_Should
    {
        [Test]
        public void TaskFirstLine_FromMultilineText()
        {
            var title = new Title(100, _ => _);

            var actual = title.Create("123\n456");

            Assert.That(actual, Is.EqualTo("123"));
        }

        [Test]
        public void FormatText_BeforeCreate()
        {
            var title = new Title(100, _ => "EXPECTED");

            var actual = title.Create("123");

            Assert.That(actual, Is.EqualTo("EXPECTED"));
        }
    }
}