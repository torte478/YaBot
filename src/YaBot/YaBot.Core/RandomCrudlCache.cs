namespace YaBot.Core
{
    using System;
    using System.Collections.Generic;
    using YaBot.Core.Extensions;

    public sealed class RandomCrudlCache<TKey, TValue> where TValue : IHasId<TKey>
    {
        private readonly Func<TKey, TKey, TKey> getRandom;
        private readonly ICrudl<TKey, TValue> crudl;
        private readonly Args args;

        private readonly Stack<TValue> values = new();

        public RandomCrudlCache(Func<TKey, TKey, TKey> getRandom, ICrudl<TKey, TValue> crudl, Args args)
        {
            this.getRandom = getRandom;
            this.crudl = crudl;
            this.args = args;
        }

        public TValue Next()
        {
            if (values.Count == 0)
                LoadValues();

            return values.Pop();
        }

        private void LoadValues()
        {
            var ids = new HashSet<TKey>();
            for (var i = 0; i < args.Limit; ++i)
            {
                if (values.Count >= args.Count)
                    return;

                var value = getRandom(args.Min, args.Max)
                    ._(crudl.Read);

                if (ids.Contains(value.Id).Not())
                {
                    ids.Add(value.Id);
                    values.Push(value);
                }
            }

            throw new Exception($"Limit {args.Limit} overflowed");
        }

        public sealed class Args
        {
            public TKey Min { get; init; }
            public TKey Max { get; init; }
            public int Count { get; init; }
            public int Limit { get; init; }
        }
    }
}