namespace YaBot.App
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Extensions;
    using Newtonsoft.Json;

    internal sealed class Config
    {
        public static Config Load(string path)
        {
            try
            {
                return path
                    ._(File.ReadAllText)
                    ._(JsonConvert.DeserializeObject<Config>);
            }
            catch (Exception e)
            {
                throw new Exception("Config format error");
            }
        }
        
        public string[] Names { get; set; }
        public string[] StopWords { get; set; }
        public Dictionary<string, State> States { get; set; }

        internal sealed class State
        {
            public bool WithName { get; set; }
            public string[] Words { get; set; }
        }
    }
}