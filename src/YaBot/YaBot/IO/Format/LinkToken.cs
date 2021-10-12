namespace YaBot.IO.Format
{
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public sealed class LinkToken : BaseToken
    {
        private readonly string delimiter;

        public LinkToken(MessageEntityType type, string border, string delimiter) : base(type, border)
        {
            this.delimiter = delimiter;
        }

        protected override string InnerSerialize(MessageEntity entity, string text)
        {
            return $"{text}{delimiter}{entity.Url}";
        }

        protected override (string, MessageEntity) InnerDeserialize(string text)
        {
            var link = text.Split(delimiter);

            var entity = new MessageEntity
            {
                Type = Type,
                Url = link[1]
            };

            return (link[0], entity);
        }
    }
}