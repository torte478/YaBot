namespace YaBot.App.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    public sealed class Words : IWords
    {
        private readonly ImmutableArray<string> words;

        public static IWords Create(IEnumerable<string> words)
        {
            return new Words(words.ToImmutableArray());
        }
        
        public Words(ImmutableArray<string> words)
        {
            this.words = words;
        }

        public bool Match(string text)
        {
            return text != null 
                   && words.Any(text.Contains);
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