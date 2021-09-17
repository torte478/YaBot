namespace YaBot.Tests
{
    using NUnit.Framework;
    using YaBot.App.Core;

    [TestFixture]
    internal sealed class Words_Should
    {
        [Test]
        public void ConvertStringToLowerCase_BeforeParse()
        {
            var words = Words.Create(new[] { "test" });

            Assert.That(words.Match("TEST"), Is.True);
        }

        [Test]
        [TestCase('-')]
        [TestCase('^')]
        [TestCase('?')]
        [TestCase(';')]
        [TestCase('.')]
        [TestCase(',')]
        [TestCase('!')]
        public void IgnorePunctuation_WhenParse(char symbol)
        {
            var words = Words.Create(new[] { "first", "second" });
            var input = $"first{symbol}second";

            Assert.That(words.Match(input), Is.True);
        }
    }
}