namespace YaBot.App.Core.State.Impl
{
    using YaBot.App.Extensions;
    using YaBot.Extensions;
    using YaBot.IO;

    public sealed class WhoQuestionState : IState
    {
        private readonly IWords success;
        private readonly IWords answers;
        private readonly IOutputFactory<string> output;

        public WhoQuestionState(IWords success, IWords answers, IOutputFactory<string> output)
        {
            this.success = success;
            this.answers = answers;
            this.output = output;
        }

        public string Name => "Question";

        public bool IsInput(IInput input)
        {
            if (input.Text == null)
                return false;
            return input.Text.EndsWith('?') && input.Text.ToLower().Contains("кто");
        }

        public (IOutput, IState) Process(IInput input)
        {
            var result = $"{success.ToRandom()} {answers.ToRandom()}"._(output.Create);
            return (result, null);
        }

        public IState Reset()
        {
            return this;
        }
    }
}