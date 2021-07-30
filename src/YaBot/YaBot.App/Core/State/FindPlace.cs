namespace YaBot.App.Core.State
{
    using System.Collections.Immutable;
    using System.IO;
    using Configs;
    using Telegram.Bot.Types.InputFiles;

    internal sealed class FindPlace : IState
    {
        private readonly string path;
        private readonly ImmutableArray<DataRow> rows;

        private int index = 0;
        
        public FindPlace(string path, ImmutableArray<DataRow> rows)
        {
            this.rows = rows;
            this.path = path;
        }

        public (Answer, IState) Process(IWords words)
        {
            var row = rows[(++index) % rows.Length];
            var stream = File.OpenRead(Path.Combine(path, row.Image));
            var answer = new Answer
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