namespace YaBot.Tests
{
    using System;
    using App.Core;
    using App.Extensions;
    using NUnit.Framework;

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