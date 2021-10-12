namespace YaBot.Tests.App
{
    using NUnit.Framework;
    using YaBot.App.Core;

    [TestFixture]
    internal sealed class Words_Should
    {
        [Test]
        public void ConvertStringToLowerCase_BeforeParse()
        {
            var words = Create("test");

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
            var words = Create("first", "second");
            var input = $"first{symbol}second";

            Assert.That(words.Match(input), Is.True);
        }

        [Test]
        [TestCase(" .,!&^#%TeSt123-or-Not.,!?\"  ", true)]
        [TestCase(" .,!&^#%TeSt_123or-Not.,!?\" ", false)]
        [TestCase(" .,!&^#%TeS123-or-Not.,!?\"  ", false)]
        public void IgnoreNotDigitsOrLetter_WhenParse(string input, bool expected)
        {
            var words = Create("test123");

            var actual = words.Match(input);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private static IWords Create(params string[] keys)
        {
            return Words.Create(keys);
        }
    }
}