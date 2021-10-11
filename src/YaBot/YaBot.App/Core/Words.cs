namespace YaBot.App.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text.RegularExpressions;
    using YaBot.Core.Extensions;

    public sealed class Words : IWords
    {
        private static readonly Regex Regex = new Regex(@"(\w+)");

        private readonly ImmutableHashSet<string> keys;

        public static IWords Create(IEnumerable<string> words)
        {
            return new Words(words.ToImmutableHashSet());
        }
        
        private Words(ImmutableHashSet<string> keys)
        {
            this.keys = keys;
        }

        public bool Match(string text, bool substring)
        {
            if (text == null)
                return false;

            var lower = text.ToLower();
            return substring
                ? keys.Any(lower.Contains)
                : lower._(Regex.Split).Any(keys.Contains);
        }
        
        public IEnumerator<string> GetEnumerator()
        {
            return keys.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}