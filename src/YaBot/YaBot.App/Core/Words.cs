namespace YaBot.App.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    internal sealed class Words : IWords
    {
        private readonly ImmutableArray<string> words;

        public Words(ImmutableArray<string> words)
        {
            this.words = words;
        }

        public bool Match(IWords other)
        {
            return words.Any(other.Contains);
        }
        
        public IEnumerator<string> GetEnumerator()
        {
            return words.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}