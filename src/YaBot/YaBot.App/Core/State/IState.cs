namespace YaBot.App.Core.State
{
    using Outputs;

    public interface IState
    {
        bool IsInput(IInput input);
        (IOutput, IState) Process(IInput input);
        IState Reset();
    }
}