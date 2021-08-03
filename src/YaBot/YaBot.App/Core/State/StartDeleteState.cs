namespace YaBot.App.Core.State
{
    using Extensions;

    public sealed class StartDeleteState : IState
    {
        private readonly IState next;

        public StartDeleteState(IState next)
        {
            this.next = next;
        }

        public bool IsInput(Input input)
        {
            return input.Message.Text?.Contains("delete") ?? false; // TODO : hardcode
        }

        public (Output, IState) Process(Input input)
        {
            return ("Введите номер места".ToOutput(), next); // TODO : hardcode
        }

        public IState Reset()
        {
            return this;
        }
    }
}