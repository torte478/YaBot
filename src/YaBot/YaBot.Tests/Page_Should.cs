namespace YaBot.Tests
{
    using System.Linq;
    using NUnit.Framework;
    using YaBot.Core;

    [TestFixture]
    internal sealed class Page_Should
    {
        [Test]
        [TestCase(0, 0, 0, 0, false)]
        [TestCase(5, 0, 0, 4, false)]
        [TestCase(5, 1, 0, 4, false)]
        [TestCase(10, 0, 0, 9, false)]
        [TestCase(11, 0, 0, 9, true)]
        [TestCase(11, 1, 10, 10, true)]
        public void CorrectCalcFinish_AfterPaginate(int count, int current, int start, int finish, bool paginated)
        {
            var page = new Page(10);

            var actual = page.Create(Enumerable.Range(0, count), current);

            Assert.That(actual.Start, Is.EqualTo(start));
            Assert.That(actual.Finish, Is.EqualTo(finish));
            Assert.That(actual.Paginated, Is.EqualTo(paginated));
        }
    }
}