namespace YaBot.App.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using YaBot.Core.Extensions;

    public sealed class Words : IWords
    {
        private readonly ImmutableHashSet<string> keys;

        public static IWords Create(IEnumerable<string> words)
        {
            return new Words(words.ToImmutableHashSet());
        }
        
        private Words(ImmutableHashSet<string> keys)
        {
            this.keys = keys;
        }

        public bool Match(string text)
        {
            return text != null
                   && text._(Parse).Any(keys.Contains);
        }
        
        public IEnumerator<string> GetEnumerator()
        {
            return keys.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<string> Parse(string text)
        {
            var punctuation = text
                .Where(char.IsPunctuation)
                .Distinct()
                .ToArray();

            return text
                .ToLower()
                .Split()
                .Select(_ => _.Trim(punctuation));
        }
    }
}