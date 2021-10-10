namespace YaBot.Core.IO
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Telegram.Bot.Types;

    public sealed class JsonUpdate
    {
        private readonly Converter converter = new();

        private readonly Update origin;

        public JsonUpdate(Update origin)
        {
            this.origin = origin;
        }

        public override string ToString()
        {
            return origin?.Message?.Chat == null
                ? string.Empty
                : JsonConvert.SerializeObject(origin);
        }

        private sealed class Converter : DateTimeConverterBase
        {
            private readonly IsoDateTimeConverter origin = new IsoDateTimeConverter();

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var time = (DateTime)value;
                if (time.Year < 1970)
                    writer.WriteRawValue("NULL");
                else
                    origin.WriteJson(writer, value, serializer);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return origin.ReadJson(reader, objectType, existingValue, serializer);
            }
        }
    }
}