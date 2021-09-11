namespace YaBot.App.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using YaBot.Core.Extensions;

    public sealed class Text
    {
        private readonly Func<IEnumerable<string>, IWords> toWords;

        public Text(Func<IEnumerable<string>, IWords> toWords)
        {
            this.toWords = toWords;
        }

        public IWords Parse(string text)
        {
            var punctuation = text
                .Where(char.IsPunctuation)
                .Distinct()
                .ToArray();

            return text
                .ToLower()
                .Split()
                .Select(_ => _.Trim(punctuation))
                ._(toWords);
        }
    }
}