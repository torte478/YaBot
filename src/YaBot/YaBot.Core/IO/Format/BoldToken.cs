namespace YaBot.Core.IO.Format
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Text;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public sealed class BoldToken : IToken
    {
        private readonly string border;

        public BoldToken(string border)
        {
            this.border = border;
        }

        public string Serialize(MessageEntity entity, string text)
        {
            return $"{border}{text}{border}";
        }

        public  IEnumerable<(string, MessageEntity)> Deserialize(string text)
        {
            var tokens = text.Split(border);

            for (var i = 0; i < tokens.Length; ++i)
            {
                MessageEntity entity = null;

                if (i % 2 == 1)
                {
                    entity = new MessageEntity
                    {
                        Type = MessageEntityType.Bold
                    };
                }

                yield return (tokens[i], entity);
            }
        }
    }
}