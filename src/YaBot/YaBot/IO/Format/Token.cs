namespace YaBot.IO.Format
{
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public sealed class Token : BaseToken
    {
        public Token(MessageEntityType type, string border) : base(type, border)
        {
        }

        protected override string InnerSerialize(MessageEntity entity, string text)
        {
            return text;
        }

        protected override  (string, MessageEntity) InnerDeserialize(string text)
        {
            return (text, new MessageEntity { Type = Type });
        }
    }
}