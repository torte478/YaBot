namespace YaBot.Core
{
    using System.Linq;

    public interface ICrudl<TKey, TValue> where TValue : IHasId<TKey>
    {
        TKey Create(TValue value);
        TValue Read(TKey key);
        bool Update(TValue value);
        bool Delete(TKey key);
        IQueryable<TValue> All();
    }
}