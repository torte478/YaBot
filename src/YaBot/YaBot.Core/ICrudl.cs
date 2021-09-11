namespace YaBot.Core
{
    using System.Collections.Generic;
    using YaBot.Core.Database;

    public interface ICrudl<TKey, TValue> where TValue : IHasId<TKey>
    {
        TKey Create(TValue value);
        TValue Read(TKey key);
        bool Update(TValue value);
        bool Delete(TKey key);
        IEnumerable<TValue> Enumerate();
    }
}