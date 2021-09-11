namespace YaBot.PriorityApp.Tree
{
    using System.Collections.Generic;

    public interface IProject
    {
        IEnumerable<(int id, string text)> ToPriorityList();
        int? FindRoot();
        (bool added, int next) TryAdd(int target, bool greater, string text);
        bool Remove(int id);
        bool Update(int id, string text);
    }
}