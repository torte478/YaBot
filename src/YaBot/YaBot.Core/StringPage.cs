namespace YaBot.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using YaBot.Core.Extensions;

    public sealed class StringPage<T>
    {
        private readonly Func<IEnumerable<T>, int, Pagination<T>> getPage;
        private readonly string format;

        public StringPage(Func<IEnumerable<T>, int, Pagination<T>> getPage, string format)
        {
            this.getPage = getPage;
            this.format = format;
        }

        public Pagination<string> Paginate(IEnumerable<T> items, int index)
        {
            var pagination = items._(getPage, index);

            return new Pagination<string>
            {
                Start = pagination.Start,
                Finish = pagination.Finish,
                Paginated = pagination.Paginated,
                Total = pagination.Total,
                Items = pagination.Items
                    .Select(_ => (_.Item1, string.Format(format, _.Item1, _.Item2)))
            };
        }
    }
}