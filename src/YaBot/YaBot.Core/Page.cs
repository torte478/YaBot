namespace YaBot.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class Page
    {
        private readonly int size;

        public Page(int size)
        {
            this.size = size;
        }

        public Pagination<T> Create<T>(IEnumerable<T> items, int page)
        {
            // TODO : optimize
            var total = items.Count();
            if (total == 0)
                return new Pagination<T> { Items = Enumerable.Empty<(int, T)>() };

            var start = size * page;
            if (start >= total)
                start = size * (page - 1);

            var finish = Math.Min(start + size - 1, total - 1);

            return new Pagination<T>
            {
                Items = items
                    .Skip(start)
                    .Take(size)
                    .Select((x, i) => (start + i, x)),
                Start = start,
                Finish = finish,
                Total = total,
                Paginated = !(start == 0 && finish == total - 1)
            };
        }
    }
}