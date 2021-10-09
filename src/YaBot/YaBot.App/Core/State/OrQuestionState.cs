namespace YaBot.App.Core.State
{
    using System;
    using System.Text.RegularExpressions;
    using YaBot.App.Core.Outputs;
    using YaBot.App.Extensions;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;

    public sealed class OrQuestionState : IState
    {
        private readonly Regex regex = new Regex(@"\s(\w+)\sили\s(\w+)?");

        private readonly IWords success;
        private readonly IOutputFactory<string> output;
        private readonly Func<int> getRandom;

        public OrQuestionState(IWords success, IOutputFactory<string> output, Func<int> getRandom)
        {
            this.success = success;
            this.output = output;
            this.getRandom = getRandom;
        }

        public string Name => "Question";

        public bool IsInput(IInput input)
        {
            if (input.Text == null)
                return false;

            var match = regex.Match(input.Text);
            return match.Success && match.Groups.Count == 3;
        }

        public (IOutput, IState) Process(IInput input)
        {
            var match = regex.Match(input.Text);
            var answer = match.Groups[getRandom() + 1].Value;

            var result = $"{success.ToRandom()} {answer}"._(output.Create);

            return (result, null);
        }

        public IState Reset()
        {
            return this;
        }
    }
}