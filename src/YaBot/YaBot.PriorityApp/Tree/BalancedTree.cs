namespace YaBot.PriorityApp.Tree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using YaBot.Extensions;
    using YaBot.PriorityApp.Tree.RBT;

    public sealed partial class BalancedTree<T> : IBalancedTree<T>
    {
        private readonly RedBlackTree<Node> tree = new();
        private readonly IMeasure measure;
        
        public event Action<Dictionary<T, int>> Rebuilded;

        public BalancedTree(IMeasure measure)
        {
            this.measure = measure;
        }

        public void Add(T item, int value)
        {
            tree.Add(new Node(item, value));
        }
        
        public (bool can, T next) CanAdd(T target, bool greater)
        {
            if (tree.Count == 0)
                return (true, default);

            var node = ToNode(target);
            var child = greater ? node.Right : node.Left;

            return child == null
                ? (true, default)
                : (false, child.Value.Item);
        }
        
        public int Add(T target, bool greater, T item)
        {
            if (tree.Count == 0)
            {
                var result = measure.Next();
                tree.Add(new Node(item, result));
                return result;
            }

            int value;
            while (!TryAdd(target, greater, item, out value))
            {
                Rebuild();
            }
            return value;
        }

        public (bool exists, T root) FindRoot()
        {
            return tree.Root != null
                ? (true, tree.Root.Value.Item)
                : (false, default);
        }

        public bool Remove(T item)
        {
            var node = tree.FirstOrDefault(_ => _.Item.Equals(item));
            
            var exists = node != null;
            if (exists)
                tree.Remove(node);
            
            return exists;
        }

        private void Rebuild()
        {
            var updated = tree
                .InOrderIterator
                .Select(_ => _.Item)
                .Zip<T, int>(measure.Fill(tree.Count))
                .ToDictionary(x => x.First, x => x.Second);

            tree.Clear();
            foreach (var (item, value) in updated)
            {
                tree.Add(new Node(item, value));
            }

            Rebuilded.Raise(updated);
        }

        private RedBlackNode<Node> ToNode(T item)
        {
            var value = tree.First(_ => _.Item.Equals(item)).Value;

            var current = tree.Root;
            while (true)
            {
                var compare = value.CompareTo(current.Value.Value);
                if (compare == 0)
                    return current;

                current = compare < 0
                    ? current.Left
                    : current.Right;
            }
        }

        private int FindNextValue(RedBlackNode<Node> node)
        {
            var next = false;
            foreach (var current in tree.InOrderIterator)
            {
                if (next)
                    return current.Value;

                next = current.CompareTo(node.Value) == 0;
            }

            return measure.NextMax(tree.Max().Value);
        }

        private int FindPrevious(RedBlackNode<Node> node)
        {
            var previous = measure.NextMin(tree.Min().Value);

            foreach (var current in tree.InOrderIterator)
            {
                if (current.CompareTo(node.Value) == 0)
                    break;

                previous = current.Value;
            }

            return previous;
        }
        
        private bool TryAdd(T target, bool greater, T item, out int value)
        {
            var node = ToNode(target);
            value = greater
                ? measure.Next(node.Value.Value, FindNextValue(node))
                : measure.Next(FindPrevious(node), node.Value.Value);

            var added = new Node(item, value);
            if (tree.Contains(added)) 
                return false;
            
            tree.Add(added);
            return true;
        }
    }
}