namespace YaBot.Tests
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using YaBot.Core;

    [TestFixture]
    internal sealed class StringPage_Should
    {
        [Test]
        public void AddIndex_WhenFormatString()
        {
            var page = new StringPage<int>(
                (_, _) => new Pagination<int> { Items = new[] { (1, 3), (2, 4) } },
                "{0}: {1}");

            var actual = page.Paginate(Array.Empty<int>(), 1);

            Assert.That(actual.Items.First().Item2, Is.EqualTo("1: 3"));
        }
    }
}