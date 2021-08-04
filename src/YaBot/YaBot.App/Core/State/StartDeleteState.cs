namespace YaBot.App.Core.State
{
    using Extensions;

    public sealed class StartDeleteState : IState
    {
        private readonly IState next;
        private readonly IWords keys;
        private readonly IWords result;

        public StartDeleteState(IWords keys, IWords result, IState next)
        {
            this.next = next;
            this.keys = keys;
            this.result = result;
        }

        public bool IsInput(Input input)
        {
            return keys.Match(input.Message);
        }

        public (Output, IState) Process(Input input)
        {
            return (result.ToRandom().ToOutput(), next);
        }

        public IState Reset()
        {
            return this;
        }
    }
}