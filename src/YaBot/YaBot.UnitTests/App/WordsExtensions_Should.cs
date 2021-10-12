namespace YaBot.Tests.App
{
    using System;
    using NUnit.Framework;
    using YaBot.App.Core;
    using YaBot.App.Extensions;
    using YaBot.Extensions;

    [TestFixture]
    internal sealed class WordsExtensions_Should
    {
        [Test]
        public void ReturnEmptyString_WhenWordsIsEmpty()
        {
            var words = Array.Empty<string>()._(Words.Create);

            var actual = words.ToRandom();
            
            Assert.That(actual, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ReturnEmptyString_WhenWordsIsNull()
        {
            IWords words = null;

            var actual = words.ToRandom();
            
            Assert.That(actual, Is.EqualTo(string.Empty));
        }
    }
}