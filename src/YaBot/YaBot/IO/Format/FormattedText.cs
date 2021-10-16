namespace YaBot.IO.Format
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using Telegram.Bot.Types;
    using YaBot.Extensions;

    public sealed class FormattedText
    {
        private readonly ImmutableArray<IToken> formats;

        public FormattedText(ImmutableArray<IToken> formats)
        {
            this.formats = formats;
        }

        public string Serialize(string text, MessageEntity[] entities)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            if (entities == null)
                return text;

            return InnerSerialize(text, entities);
        }

        public (string, MessageEntity[]) Deserialize(string text)
        {
            (string token, MessageEntity entity) seed = (token: text, null);
            var tokens =  formats
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

        private string InnerSerialize(string text, MessageEntity[] messageEntities)
        {
            var tokens = messageEntities
                !.Where(_ => formats.Any(f => f.Type == _.Type))
                .OrderBy(_ => _.Offset)
                ._(_ => new Queue<MessageEntity>(_));

            var result = new StringBuilder();
            for (var i = 0; i < text!.Length; ++i)
            {
                if (tokens.TryPeek(out var entity) && entity.Offset == i)
                {
                    var end = entity.Offset + entity.Length - 1;
                    var format = formats.Single(_ => _.Type == entity.Type);

                    text
                        .Substring(entity.Offset, entity.Length)
                        ._(_ => format.Serialize(entity, _))
                        ._(result.Append);

                    tokens.Dequeue();
                    i = end;
                }
                else
                {
                    result.Append(text[i]);
                }
            }

            return result.ToString();
        }

        private static IEnumerable<(string, MessageEntity)> Deserialize(
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