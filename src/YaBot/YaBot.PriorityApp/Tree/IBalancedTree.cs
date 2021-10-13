namespace YaBot.PriorityApp.Tree
{
    using System;
    using System.Collections.Generic;

    public interface IBalancedTree<T>
    {
        event Action<Dictionary<T, int>> Rebuilded;

        void Add(T item, int value);
        (bool can, T next) CanAdd(T target, bool greater);
        int Add(T target, bool greater, T item);

        (bool exists, T root) FindRoot();

        
        bool Remove(T item);

    }
}