namespace YaBot.App.Core.State
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Extensions;
    using Outputs;
    using YaBot.Core;
    using YaBot.Core.Extensions;

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
                .Append("Version: ")
                .AppendLine(version)
                .Append("State: ")
                .AppendLine(Current.ToString())
                .ToString();
        }
    }
}