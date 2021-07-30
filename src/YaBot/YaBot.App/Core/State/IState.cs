namespace YaBot.App.Core.State
{
    public interface IState
    {
        (string, IState) Process(IWords words);
        IState Reset();
    }
}