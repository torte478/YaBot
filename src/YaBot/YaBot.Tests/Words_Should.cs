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
    }
}