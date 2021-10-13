namespace YaBot.App.Core.State
{
    using YaBot.IO;

    public interface IState
    {
        string Name { get; }
        
        bool IsInput(IInput input);
        (IOutput, IState) Process(IInput input);
        IState Reset();
    }
}