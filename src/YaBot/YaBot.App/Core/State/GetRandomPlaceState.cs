﻿namespace YaBot.App.Core.State
{
    using System.Collections.Immutable;
    using System.IO;
    using Configs;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.InputFiles;
    using File = System.IO.File;

    internal sealed class GetRandomPlaceState : IState
    {
        private readonly string path;
        private readonly ImmutableArray<DataRow> rows;

        private int index = 0;

        public GetRandomPlaceState(string path, ImmutableArray<DataRow> rows)
        {
            this.rows = rows;
            this.path = path;
        }

        public bool IsInput(Input input)
        {
            return false; // TODO
        }

        public (Output, IState) Process(Input input)
        {
            var row = rows[(++index) % rows.Length];
            var stream = File.OpenRead(Path.Combine(path, row.Image));
            var answer = new Output
            {
                Text = row.Name,
                Image = new InputOnlineFile(stream)
            };
            return (answer, this);
        }

        public IState Reset()
        {
            return this;
        }

        public override string ToString()
        {
            return "FindPlace";
        }
    }
}