namespace YaBot.App.Core.Outputs
{
    using YaBot.Core;

    public interface IOutputFactory<in T>
    {
        IOutput ToEmpty();
        IOutput Create(T input);
    }

    public interface IOutputFactory<in T1, in T2> : IOutputFactory<T1>
    {
        IOutput Create(T2 input);
    }

    public interface IOutputFactory<in T1, in T2, in T3> : IOutputFactory<T1, T2>
    {
        IOutput Create(T3 input);
    }
}