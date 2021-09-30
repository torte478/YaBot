namespace YaBot.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using YaBot.Core.Extensions;

    public sealed class FormattedText
    {
        private readonly string bold;

        public FormattedText(string bold)
        {
            this.bold = bold;
        }

        public string Serialize(Message message)
        {
            if (message?.Text?._(string.IsNullOrEmpty) ?? true)
                return null;

            var entities = message
                               .Entities?
                               .Where(_ => _.Type == MessageEntityType.Bold)
                               .OrderBy(_ => _.Offset)
                           ?? Enumerable.Empty<MessageEntity>();

            var queue = new Queue<MessageEntity>(entities);

            var result = new StringBuilder();
            for (var i = 0; i < message.Text.Length; ++i)
            {
                if (queue.TryPeek(out var entity) && i == entity.Offset)
                    result.Append(bold);

                result.Append(message.Text[i]);

                if (entity != null && i == entity.Offset + entity.Length - 1)
                {
                    result.Append(bold);
                    queue.Dequeue();
                }
            }

            return result.ToString();
        }

        public (string, MessageEntity[]) Deserialize(string text)
        {
            var result = new StringBuilder();
            var entities = new List<MessageEntity>();

            for (var i = 0; i < text.Length; ++i)
            {
                //TODO : refactor that shit
                if (i < text.Length - 2 && text[i] == bold[0] && text[i + 1] == bold[1])
                {
                    var end = text.IndexOf(bold, i + 2, StringComparison.Ordinal);
                    if (end == -1)
                        throw new Exception($"Can't deserialize string '{text}'. Token's end not found");

                    var length = end - i - 2;
                    entities.Add(new MessageEntity
                    {
                        Type = MessageEntityType.Bold,
                        Offset = result.Length,
                        Length =  length
                    });
                    result.Append(text.Substring(i + 2, length));
                    i += length + 3;
                }
                else
                {
                    result.Append(text[i]);
                }
            }

            return (result.ToString(), entities.ToArray());
        }
    }
}