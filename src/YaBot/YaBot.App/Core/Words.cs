namespace YaBot.App.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
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

        public bool Match(string text, bool substring)
        {
            if (text == null)
                return false;

            var lower = text.ToLower();
            return substring
                ? keys.Any(lower.Contains)
                : lower._(Parse).Any(keys.Contains);
        }
        
        public IEnumerator<string> GetEnumerator()
        {
            return keys.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static IEnumerable<string> Parse(string text)
        {
            // TODO : refactor
            var result = new StringBuilder();
            foreach (var symbol in text.Concat(new[] { '.' }))
            {
                if (char.IsLetterOrDigit(symbol))
                {
                    result.Append(symbol);
                }
                else
                {
                    if (result.Length > 0)
                        yield return result.ToString();

                    result.Clear();
                }
            }
        }
    }
}