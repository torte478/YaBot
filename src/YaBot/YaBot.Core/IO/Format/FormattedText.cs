namespace YaBot.Core.IO.Format
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using YaBot.Core.Extensions;

    public sealed class FormattedText
    {
        private readonly IImmutableDictionary<MessageEntityType, IToken> formats;

        public FormattedText(IImmutableDictionary<MessageEntityType, IToken> formats)
        {
            this.formats = formats;
        }

        public string Serialize(Message message)
        {
            //TODO : to cool swtich
            if (message?.Text?._(string.IsNullOrEmpty) ?? true)
                return null;

            if (message.Entities == null)
                return message.Text;

            var entities = message.Entities
                .Where(_ => _.Type._(formats.ContainsKey))
                .OrderBy(_ => _.Offset)
                ._(_ => new Queue<MessageEntity>(_));

            var result = new StringBuilder();
            for (var i = 0; i < message.Text.Length; ++i)
            {
                if (entities.TryPeek(out var entity) && entity.Offset == i)
                {
                    var end = entity.Offset + entity.Length - 1;

                    message.Text
                        .Substring(entity.Offset, entity.Length)
                        ._(_ => formats[entity.Type].Serialize(entity, _))
                        ._(result.Append);

                    entities.Dequeue();
                    i = end;
                }
                else
                {
                    result.Append(message.Text[i]);
                }
            }

            return result.ToString();
        }

        private string Serialize(string text, MessageEntity entity)
        {
            return formats.TryGetValue(entity.Type, out var format)
                ? format.Serialize(entity, text)
                : text;
        }

        public (string, MessageEntity[]) Deserialize(string text)
        {
            (string token, MessageEntity entity) seed = (token: text, null);
            var tokens =  formats.Values
                .Aggregate(
                    new[] { seed }.AsEnumerable(),
                    Deserialize);

            var result = new StringBuilder();
            var entities = new List<MessageEntity>();
            foreach (var (token, entity) in tokens)
            {
                if (entity != null)
                {
                    entity.Offset = result.Length;
                    entity.Length = token.Length;
                    entities.Add(entity);
                }

                result.Append(token);
            }

            return (result.ToString(), entities.ToArray());
        }

        private IEnumerable<(string, MessageEntity)> Deserialize(
            IEnumerable<(string token, MessageEntity entity)> acc,
            IToken format)
        {
            return acc
                .SelectMany(_ => _.entity != null
                    ? new[] { _ }
                    : format.Deserialize(_.token));
        }
    }
}