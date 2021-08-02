namespace YaBot.App.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Telegram.Bot.Types;

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

        public bool Match(Message message)
        {
            return message.Text != null 
                   && words.Any(message.Text.Contains);
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