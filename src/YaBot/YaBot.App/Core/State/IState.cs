namespace YaBot.App.Core.State
{
    public interface IState
    {
        bool IsInput(Input input);
        (Output, IState) Process(Input input);
        IState Reset();
    }
}