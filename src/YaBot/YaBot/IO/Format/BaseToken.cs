namespace YaBot.IO.Format
{
    using System.Collections.Generic;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public abstract class BaseToken : IToken
    {
        private readonly string border;

        protected BaseToken(MessageEntityType type, string border)
        {
            this.border = border;
            Type = type;
        }

        public MessageEntityType Type { get; }

        public string Serialize(MessageEntity entity, string text)
        {
            return $"{border}{InnerSerialize(entity, text)}{border}";
        }

        public IEnumerable<(string, MessageEntity)> Deserialize(string text)
        {
            var tokens = text.Split(border);

            for (var i = 0; i < tokens.Length; ++i)
            {
                yield return i % 2 == 1
                    ? InnerDeserialize(tokens[i])
                    : (tokens[i], null);
            }
        }

        protected virtual string InnerSerialize(MessageEntity entity, string text)
        {
            return text;
        }

        protected virtual (string, MessageEntity) InnerDeserialize(string text)
        {
            return (text, new MessageEntity { Type = Type });
        }
    }
}