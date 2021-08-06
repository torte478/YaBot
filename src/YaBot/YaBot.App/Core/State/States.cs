namespace YaBot.App.Core.State
{
    using System;
    using System.Text;
    using Extensions;
    using Outputs;

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
            
            var reset = current != start && stoppers.Match(input.Text); 
            if (reset)
            {
                current.Reset();
                current = start;
                return auf._(outputs.Create);
            }
            
            var (answer, next) =  current.Process(input);

            next ??= start;
            
            if (current != next)
                log($"{current} => {next}");
            
            current = next;
            
            return answer;
        }

        private string GetStatus()
        {
            return new StringBuilder()
                .Append("Version: ")
                .AppendLine(version)
                .Append("State: ")
                .AppendLine(current.ToString())
                .ToString();
        }
    }
}