namespace YaBot.PriorityApp.Tree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using YaBot.Core;
    using YaBot.PriorityApp.Database;

    public sealed class Project : IProject, IDisposable
    {
        private readonly int project;
        private readonly IBalancedTree<int> tree;
        private readonly ICrudl<int, Objective> storage;

        private Project(int project, IBalancedTree<int> tree, ICrudl<int, Objective> storage)
        {
            this.project = project;
            this.tree = tree;
            this.storage = storage;
            
            tree.Rebuilded += UpdateStored;
        }

        public static IProject Create(int project, ICrudl<int, Objective> storage, Func<IBalancedTree<int>> createTree)
        {
            var tree = createTree();
            foreach (var item in storage.All().Where(_ => _.Project == project))
                tree.Add(item.Id, item.Value);
        
            return new Project(project, tree, storage);
        }
        
        public void Dispose()
        {
            tree.Rebuilded -= UpdateStored;
        }

        public IEnumerable<(int id, string text)> ToPriorityList()
        {
            return storage.All()
                .Where(_ => _.Project == project)
                .OrderByDescending(_ => _.Value)
                .ToArray()
                .Select(_ => (_.Id, _.Text));
        }

        public int? FindRoot()
        {
            var (exists, root) = tree.FindRoot();
            return exists ? root : null;
        }

        public (bool added, int next) TryAdd(int target, bool greater, string text)
        {
            var (can, next) = tree.CanAdd(target, greater);
            if (!can) return (false, next);
            
            var id = storage.Create(new Objective
            {
                Project = project,
                Text = text
            });
            
            var value = tree.Add(target, greater, id);
            
            var item = storage.Read(id);
            item.Value = value;
            storage.Update(item);

            return (true, next);
        }

        public bool Remove(int id)
        {
            var deleted = storage.Delete(id);

            if (deleted)
                tree.Remove(id);
            
            return deleted;
        }

        public bool Update(int id, string text)
        {
            return storage.Update(new Objective
            {
                Id = id,
                Text = text
            });
        }

        private void UpdateStored(Dictionary<int, int> update)
        {
            var projectId = storage
                .All()
                .First(_ => _.Id == update.Keys.First())
                .Project;
            
            var entities = storage
                .All()
                .Where(_ => _.Project == projectId)
                .ToArray();
            
            foreach (var entity in entities)
            {
                entity.Value = update.ContainsKey(entity.Id)
                    ? update[entity.Id]
                    : int.MaxValue;
                
                storage.Update(entity);
            }
        }
    }
}