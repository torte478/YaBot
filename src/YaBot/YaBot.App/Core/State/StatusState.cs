namespace YaBot.App.Core.State
{
    using System;
    using System.Text;
    using YaBot.App.Core.Outputs;
    using YaBot.Core.IO;

    public sealed class StatusState : BaseState
    {
        private readonly string version;

        private readonly DateTime start;

        private string state;

        public StatusState(IWords keys, IOutputFactory<string> output, string version) : base(keys, output)
        {
            this.version = version;

            start = DateTime.Now;
            state = "Start"; //TODO : actual
        }

        public override string Name => "Status";

        public void Update(string state)
        {
            this.state = state;
        }

        protected override string InnerProcess(IInput input)
        {
            return new StringBuilder()
                .AppendLine($"Version: {version}")
                .AppendLine($"Started at {start}")
                .AppendLine($"Uptime: {GetFormattedUptime()}")
                .AppendLine($"State: {state}")
                .ToString();
        }

        private string GetFormattedUptime()
        {
            var total = DateTime.Now.Subtract(start);

            return new StringBuilder()
                .Append(total.Days > 0 ? $"{total.Days} d " : string.Empty)
                .Append(total.Hours > 0 ? $"{total.Hours} h " : string.Empty)
                .Append($"{total.Minutes} m")
                .ToString();
        }
    }
}