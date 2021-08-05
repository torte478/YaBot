namespace YaBot.App.Core.State
{
    public interface IState
    {
        bool IsInput(IInput input);
        (IOutput, IState) Process(IInput input);
        IState Reset();
    }
}