namespace YaBot.Core
{
    using System.Collections.Generic;

    public sealed class Pagination<T>
    {
        public IEnumerable<T> Items { get; init; }
        public bool Paginated { get; init;}
        public int Start { get; init;}
        public int Finish { get; init;}
        public int Total { get; init;}
    }
}