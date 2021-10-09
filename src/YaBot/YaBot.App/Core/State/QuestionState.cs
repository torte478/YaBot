namespace YaBot.App.Core.State
{
    using System;
    using YaBot.App.Core.Outputs;
    using YaBot.App.Extensions;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;

    public sealed class QuestionState : IState
    {
        private readonly IWords success;
        private readonly Func<string> getAnswer;
        private readonly IOutputFactory<string> output;

        public QuestionState(IWords success, IOutputFactory<string> output, Func<string> getAnswer)
        {
            this.success = success;
            this.getAnswer = getAnswer;
            this.output = output;
        }

        public string Name => "Question";

        public bool IsInput(IInput input)
        {
            return input.Text?.EndsWith('?') ?? false;
        }

        public (IOutput, IState) Process(IInput input)
        {
            var result = $"{success.ToRandom()} {getAnswer()}"._(output.Create);
            return (result, null);
        }

        public IState Reset()
        {
            return this;
        }
    }
}