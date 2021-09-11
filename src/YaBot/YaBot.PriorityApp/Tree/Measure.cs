namespace YaBot.PriorityApp.Tree
{
    using System.Collections.Generic;

    public sealed class Measure : IMeasure
    {
        private readonly int value;

        public Measure(int value)
        {
            this.value = value;
        }

        public int Next()
        {
            return value;
        }

        public int Next(int min, int max)
        {
            return (max + min) / 2;
        }

        public int NextMax(int max)
        {
            return max + value;
        }

        public int NextMin(int min)
        {
            return min - value;
        }

        public IEnumerable<int> Fill(int count)
        {
            var next = value;
            for (var i = 0; i < count; ++i)
            {
                yield return next;
                next += value;
            }
        }
    }
}