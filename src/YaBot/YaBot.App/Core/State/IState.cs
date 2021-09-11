namespace YaBot.App.Core.State
{
    using Outputs;
    using YaBot.Core;
    using YaBot.Core.IO;

    public interface IState
    {
        string Name { get; }
        
        bool IsInput(IInput input);
        (IOutput, IState) Process(IInput input);
        IState Reset();
    }
}