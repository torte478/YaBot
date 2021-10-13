namespace YaBot.App.Core.State.Impl
{
    using YaBot.App.Extensions;
    using YaBot.IO;

    public sealed class AufState : BaseState
    {
        private readonly IWords response;

        public AufState(IWords keys, IOutputFactory<string> output, IWords response)
            : base(keys, output)
        {
            this.response = response;
        }

        public override string Name => "Auf";

        public override bool IsInput(IInput input)
        {
            return Keys.Match(input.Text, true);
        }

        protected override string InnerProcess(IInput input)
        {
            return response.ToRandom();
        }
    }
}