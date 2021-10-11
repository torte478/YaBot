namespace YaBot.Core
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using YaBot.Core.Extensions;

    public sealed class JsonConfig
    {
        private readonly JsonDocument document;

        public static JsonConfig Create(string path)
        {
            return path
                ._(File.ReadAllText)
                ._(JsonDocument.Parse, new JsonDocumentOptions())
                ._(_ => new JsonConfig(_));
        }

        private JsonConfig(JsonDocument document)
        {
            this.document = document;
        }

        public JsonElement this[string key]
        {
            get
            {
                var element = document
                    .RootElement
                    .EnumerateObject()
                    .FirstOrDefault(_ => _.Name == key)
                    .Value;

                if (element.Equals(default(JsonElement)))
                    throw new Exception($"key '{key}' not found");

                return element;
            }
        }
    }
}