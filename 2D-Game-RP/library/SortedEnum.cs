using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TwoD_Game_RP
{
    public class SortedEnum<T> : IEnumerable<T> where T : IComparable<T>
    {
        NodeSortedEnum<T> root;
        int count;
        public int Count => count;
        public SortedEnum()
        {
            root = null;
            count = 0;
        }
        private NodeSortedEnum<T> Find(T value)
        {
            if (root == null) return null;

            var current = root;
            while (current != null)
            {
                if (current.Value.CompareTo(value) > 0)
                    current = current.LeftChildren;
                else if (current.Value.CompareTo(value) < 0)
                    current = current.RightChildren;
                else return current;
            }
            return null;
        }
        public bool Contains(T value)
        {
            var current = Find(value);
            if (current != null)
                return true;
            return false;
        }
        public void Add(T value)
        {
            var newNode = new NodeSortedEnum<T>(value);
            if (root == null)
            {
                root = newNode;
                count++;
                return;
            }
            var current = root;
            while (current != null)
            {
                if (current.Value.CompareTo(value) > 0)
                {
                    if (current.LeftChildren == null)
                    {
                        current.LeftChildren = newNode;
                        count++;
                        return;
                    }
                    current = current.LeftChildren;
                }
                else if (current.Value.CompareTo(value) < 0)
                {
                    if (current.RightChildren == null)
                    {
                        current.RightChildren = newNode;
                        count++;
                        return;
                    }
                    current = current.RightChildren;
                }
                else throw new ArgumentException();
            }
        }
        private void AnalyzeRemove(bool IsChildrenLeft, NodeSortedEnum<T> parent, NodeSortedEnum<T> child)
        {
            if (parent == null)
            {
                root = child;
                return;
            }
            if (IsChildrenLeft) parent.LeftChildren = child;
            else parent.RightChildren = child;
        }
        public void Remove(T value)
        {
            if (root == null)
            {
                return;
            }
            NodeSortedEnum<T> parent = null;
            bool IsChildrenLeft = true;
            var current = root;
            while (current != null)
            {
                if (current.Value.CompareTo(value) == 0)
                {
                    if (current.LeftChildren == null && current.RightChildren == null)
                    {
                        AnalyzeRemove(IsChildrenLeft, parent, null);
                    }
                    else if (current.LeftChildren == null && current.RightChildren != null)
                    {
                        AnalyzeRemove(IsChildrenLeft, parent, current.RightChildren);
                    }
                    else if (current.LeftChildren != null && current.RightChildren == null)
                    {
                        AnalyzeRemove(IsChildrenLeft, parent, current.LeftChildren);
                    }
                    else
                    {
                        var maxleftright = current.RightChildren;
                        while (maxleftright.LeftChildren != null) maxleftright = maxleftright.LeftChildren;
                        maxleftright.LeftChildren = current.LeftChildren;
                        AnalyzeRemove(IsChildrenLeft, parent, current.RightChildren);
                    }
                    count--;
                    return;
                }
                if (current.Value.CompareTo(value) < 0)
                {
                    parent = current;
                    IsChildrenLeft = false;
                    current = current.RightChildren;
                }
                else
                {
                    parent = current;
                    IsChildrenLeft = true;
                    current = current.LeftChildren;
                }
            }
        }
        public IEnumerator<T> GetEnumerator()
        {
            if (root == null)
                return Enumerable.Empty<T>().GetEnumerator();
            return root.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    internal class NodeSortedEnum<T> : IEnumerable<T>
    {
        public T Value { get; set; }
        public NodeSortedEnum<T> LeftChildren { get; set; }
        public NodeSortedEnum<T> RightChildren { get; set; }
        public NodeSortedEnum(T value)
        {
            Value = value;
            LeftChildren = null;
            RightChildren = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (LeftChildren != null)
            {
                foreach (T v in LeftChildren)
                {
                    yield return v;
                }
            }
            yield return Value;
            if (RightChildren != null)
            {
                foreach (T v in RightChildren)
                {
                    yield return v;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
