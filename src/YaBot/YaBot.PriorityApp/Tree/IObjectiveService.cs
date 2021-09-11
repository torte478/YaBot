namespace YaBot.PriorityApp.Tree
{
    using System.Collections.Generic;

    public interface IObjectiveService
    {
        IEnumerable<(int id, string text)> ToPriorityList(int project);
        int? FindRoot(int project);
        (bool added, int next) TryAdd(int project, int target, bool greater, string text);
        bool Remove(int project, int id);
        bool Update(int project, int id, string text);
    }
}