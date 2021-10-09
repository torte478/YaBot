namespace YaBot.App.Core.State
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using Outputs;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;

    public sealed class States
    {
        // TODO : refactor
        private readonly string version;
        private readonly IWords status;
        private readonly IState start;
        private readonly ImmutableArray<IState> liners;
        private readonly IWords stoppers;
        private readonly IWords reset;
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
            ImmutableArray<IState> liners,
            IWords stoppers, 
            IWords reset,
            IWords status,
            IOutputFactory<string, IWords> outputs, 
            Action<string> log)
        {
            this.version = version;
            this.start = start;
            this.stoppers = stoppers;
            this.reset = reset;
            this.status = status;
            this.outputs = outputs;
            this.log = log;
            this.liners = liners;

            current = start;
            StartTime = DateTime.Now;
        }

        public IOutput Process(IInput input)
        {
            // TODO : status => IState
            if (status.Match(input.Text))
                return GetStatus()._(outputs.Create);
            
            var stop = Current != start && stoppers.Match(input.Text); 
            if (stop)
            {
                Current.Reset();
                Current = start;
                return reset._(outputs.Create);
            }

            var liner = liners.FirstOrDefault(_ => _.IsInput(input));

            if (liner != null)
                return liner.Process(input).Item1;
            
            var (answer, next) = Current.Process(input);

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