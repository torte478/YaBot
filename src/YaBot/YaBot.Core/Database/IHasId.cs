namespace YaBot.Core.Database
{
    public interface IHasId<out T>
    {
        public T Id { get; }
    }
}