namespace YaBot
{
    using System;
    using System.Linq;

    public sealed class Page
    {
        private readonly int size;

        public Page(int size)
        {
            this.size = size;
        }

        public Pagination<T> Create<T>(IQueryable<T> items, int page)
        {
            var total = items.Count();
            if (total == 0)
                return new Pagination<T> { Items = Enumerable.Empty<(int, T)>() };

            var actualPage = Math.Min(
                Math.Max(page, 0),
                total / size + Math.Sign(total % size) - 1);
            var start = size * actualPage;
            var finish = Math.Min(start + size - 1, total - 1);

            return new Pagination<T>
            {
                Items = items
                    .Skip(start)
                    .Take(size)
                    .AsEnumerable()
                    .Select((x, i) => (start + i, x)),
                Start = start,
                Finish = finish,
                Total = total,
                Paginated = !(start == 0 && finish == total - 1),
                Index = actualPage
            };
        }
    }
}