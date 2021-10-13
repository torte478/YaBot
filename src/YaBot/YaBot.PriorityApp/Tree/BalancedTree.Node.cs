namespace YaBot.PriorityApp.Tree
{
    using System;

    public sealed partial class BalancedTree<T>
    {
        private sealed class Node : IComparable<Node>
        {
            public T Item { get;  }
            public int Value { get; }
            
            public Node(T item, int value)
            {
                Item = item;
                Value = value;
            }

            public int CompareTo(Node other)
            {
                return Value.CompareTo(other.Value);
            }
        }
    }
}