namespace YaBot.App.Core.State
{
    using System;
    using System.Text;
    using Outputs;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;

    public sealed class States
    {
        private readonly string version;
        private readonly IWords status;
        private readonly IState start;
        private readonly IWords stoppers;
        private readonly IWords auf;
        private readonly IOutputFactory<string, IWords> outputs;
        private readonly Action<string> log;

        private IState current;
        private IState Current
        {
            get => current;
            set
            {
                if (current != value)
                    log($"{current.Name} => {value.Name}");
                current = value;
            }
        }

        // TODO : wrong name
        private DateTime StartTime { get; }

        public States(
            string version,
            IState start, 
            IWords stoppers, 
            IWords auf, 
            IWords status,
            IOutputFactory<string, IWords> outputs, 
            Action<string> log)
        {
            this.version = version;
            this.start = start;
            this.stoppers = stoppers;
            this.auf = auf;
            this.status = status;
            this.outputs = outputs;
            this.log = log;

            current = start;
            StartTime = DateTime.Now;
        }

        public IOutput Process(IInput input)
        {
            if (status.Match(input.Text))
                return GetStatus()._(outputs.Create);
            
            var reset = Current != start && stoppers.Match(input.Text); 
            if (reset)
            {
                Current.Reset();
                Current = start;
                return auf._(outputs.Create);
            }
            
            var (answer, next) =  Current.Process(input);

            next ??= start;
            
            Current = next;
            
            return answer;
        }

        private string GetStatus()
        {
            return new StringBuilder()
                .AppendLine($"Version: {version}")
                .AppendLine($"Started at {StartTime}")
                .AppendLine($"Uptime: {GetFormattedUptime()}")
                .AppendLine($"State: {Current}")
                .ToString();
        }

        private string GetFormattedUptime()
        {
            var total = DateTime.Now.Subtract(StartTime);

            return new StringBuilder()
                .Append(total.Days > 0 ? $"{total.Days} d " : string.Empty)
                .Append(total.Hours > 0 ? $"{total.Hours} h " : string.Empty)
                .Append($"{total.Minutes} m")
                .ToString();
        }
    }
}