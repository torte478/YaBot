namespace YaBot.Tests
{
    using System.Linq;
    using NUnit.Framework;
    using YaBot.Core;

    [TestFixture]
    internal sealed class Page_Should
    {
        [Test]
        [TestCase(10, 0, 0, 0, 0, false)]
        [TestCase(10, 5, 0, 0, 4, false)]
        [TestCase(10, 5, 1, 0, 4, false)]
        [TestCase(10, 10, 0, 0, 9, false)]
        [TestCase(10, 11, 0, 0, 9, true)]
        [TestCase(10, 11, 1, 10, 10, true)]
        [TestCase(10, 11, 10, 10, 10, true)]
        [TestCase(2, 6, 4, 4, 5, true)]
        public void CorrectCalcFinish_AfterPaginate(int size, int count, int current, int start, int finish, bool paginated)
        {
            var page = new Page(size);

            var actual = page.Create(CreateQuery(count), current);

            Assert.That(actual.Start, Is.EqualTo(start));
            Assert.That(actual.Finish, Is.EqualTo(finish));
            Assert.That(actual.Paginated, Is.EqualTo(paginated));
        }

        [Test]
        public void ReturnCorrectIndex_ForPaginatedItems()
        {
            var page = new Page(10);

            var actual = page.Create(CreateQuery(30), 1);

            Assert.That(actual.Items.First().Item1, Is.EqualTo(10));
        }

        [Test]
        [TestCase(-1, 0)]
        [TestCase(2, 2)]
        [TestCase(100, 2)]
        public void ReturnCurrentPage_ForAnyPageNumber(int current, int expected)
        {
            var page = new Page(2);

            var actual = page.Create(CreateQuery(6), current);

            Assert.That(actual.Index, Is.EqualTo(expected));
        }

        private static IQueryable<int> CreateQuery(int size)
        {
            return Enumerable.Range(0, size).AsQueryable();
        }
    }
}