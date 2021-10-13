namespace YaBot.App.Core.State
{
    using YaBot.IO;

    public abstract class BaseState : IState
    {
        protected BaseState(IWords keys, IOutputFactory<string> output)
        {
            Keys = keys;
            Output = output;
        }

        public abstract string Name { get; }

        protected IWords Keys { get; }

        protected IOutputFactory<string> Output { get; }

        public virtual bool IsInput(IInput input)
        {
            return Keys.Match(input.Text);
        }

        public (IOutput, IState) Process(IInput input)
        {
            var result = InnerProcess(input);
            return (Output.Create(result), null);
        }

        public IState Reset()
        {
            return this;
        }

        protected abstract string InnerProcess(IInput input);
    }
}