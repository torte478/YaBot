namespace YaBot.PriorityApp.Tree.RBT
{
    using System;
    using System.Collections.Generic;

    public sealed partial class RedBlackTree<T> : ICollection<T> where T : IComparable<T>
    {
        public void Add(T item)
        {
            this.Insert(item);
        }

        public bool Contains(T item)
        {
            RedBlackNode<T> current = this.Root;
            while (current != null)
            {
                int compareResult = current.Value.CompareTo(item);
                if (compareResult == 0)
                    break;
                else if (compareResult > 0)
                    current = current.Left;
                else
                    current = current.Right;
            }

            return current != null;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var index = arrayIndex;
            foreach (var x in this)
                array[index++] = x;
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return this.Delete(item);
        }

        public void Clear()
        {
            this.Root = null;
            this.Count = 0;
        }
    }
}
