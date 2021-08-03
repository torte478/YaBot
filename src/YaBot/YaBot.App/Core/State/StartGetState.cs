namespace YaBot.App.Core.State
{
    using Extensions;

    public sealed class StartGetState : IState
    {
        private readonly IState next;

        public StartGetState(IState next)
        {
            this.next = next;
        }

        public bool IsInput(Input input)
        {
            return input.Message.Text?.Contains("get") ?? false; // TODO : hardcode
        }

        public (Output, IState) Process(Input input)
        {
            return ("Введите номер места".ToOutput(), next);
        }

        public IState Reset()
        {
            return this;
        }
    }
}