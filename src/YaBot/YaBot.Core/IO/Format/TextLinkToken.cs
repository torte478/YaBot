namespace YaBot.Core.IO.Format
{
    using System.Collections.Generic;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public sealed class TextLinkToken : IToken
    {
        private readonly string border;
        private readonly string delimiter;

        public TextLinkToken(string border, string delimiter)
        {
            this.border = border;
            this.delimiter = delimiter;
        }

        public string Serialize(MessageEntity entity, string text)
        {
            return $"{border}{text}{delimiter}{entity.Url}{border}";
        }

        public  IEnumerable<(string, MessageEntity)> Deserialize(string text)
        {
            var tokens = text.Split(border);

            for (var i = 0; i < tokens.Length; ++i)
            {
                if (i % 2 == 1)
                {
                    var link = tokens[i].Split(delimiter);

                    var entity = new MessageEntity
                    {
                        Type = MessageEntityType.TextLink,
                        Url = link[1]
                    };

                    yield return (link[0], entity);
                }
                else
                {
                    yield return (tokens[i], null);
                }
            }
        }
    }
}