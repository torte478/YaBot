namespace YaBot.App.Core.State
{
    using System;
    using Extensions;

    public sealed class States
    {
        private readonly IState start;
        private readonly IWords stoppers;
        private readonly IWords auf;
        private readonly Func<IWords, IOutput> toOutput;
        private readonly Action<string> log;

        private IState current;

        public States(
            IState start, 
            IWords stoppers, 
            IWords auf, 
            Func<IWords, IOutput> toOutput, 
            Action<string> log)
        {
            this.start = start;
            this.stoppers = stoppers;
            this.auf = auf;
            this.toOutput = toOutput;
            this.log = log;

            current = start;
        }

        public IOutput Process(IInput input)
        {
            var reset = current != start && stoppers.Match(input.Text); 
            if (reset)
            {
                current.Reset();
                current = start;
                return auf._(toOutput);
            }
            
            var (answer, next) =  current.Process(input);

            next ??= start;
            
            if (current != next)
                log($"{current} => {next}");
            
            current = next;
            
            return answer;
        }
    }
}