namespace YaBot.App.Core.State
{
    using System;

    internal sealed class States
    {
        private readonly IState start;
        private readonly IWords stoppers;
        private readonly Action<string> log;

        private IState current;

        public States(IState start, IWords stoppers, Action<string> log)
        {
            this.start = start;
            this.stoppers = stoppers;
            this.log = log;

            current = start;
        }

        public string Process(IWords words)
        {
            var reset = current != start && stoppers.Match(words); 
            if (reset)
            {
                current.Reset();
                return "Ауф!"; // TODO
            }
            
            var (answer, next) = current.Process(words);
            
            if (current != next)
                log($"{current} => {next}");
            
            current = next;
            
            return answer;
        }
    }
}