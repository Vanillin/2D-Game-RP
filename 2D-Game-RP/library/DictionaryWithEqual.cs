using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace TwoD_Game_RP
{
    internal class NodeDict<Tkey, TValue>
    {
        public Tkey Key { get; set; }
        public TValue Value { get; set; }
        public NodeDict<Tkey, TValue> Parent { get; set; }
        public NodeDict<Tkey, TValue> LeftChildren { get; set; }
        public NodeDict<Tkey, TValue> RightChildren { get; set; }
        public NodeDict(Tkey key, TValue value)
        {
            Key = key;
            Value = value;
            Parent = null;
            LeftChildren = null;
            RightChildren = null;
        }
        public IEnumerable<KeyValuePair<Tkey, TValue>> GetEnumerable()
        {
            if (LeftChildren == null && RightChildren == null)
                return Enumerable.Repeat(new KeyValuePair<Tkey, TValue>(Key, Value), 1);
            else if (LeftChildren == null && RightChildren != null)
                return Enumerable.Repeat(new KeyValuePair<Tkey, TValue>(Key, Value), 1).Union(RightChildren.GetEnumerable());
            else if (LeftChildren != null && RightChildren == null)
                return LeftChildren.GetEnumerable().Append(new KeyValuePair<Tkey, TValue>(Key, Value));
            else
                return LeftChildren.GetEnumerable().Append(new KeyValuePair<Tkey, TValue>(Key, Value)).Union(RightChildren.GetEnumerable());
        }
    }

    public class DictionaryWithEqual<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey>
    {
        NodeDict<TKey, TValue> root;
        int count;
        public TValue this[TKey key]
        {
            get
            {
                var current = Find(key);
                if (current != null)
                    return current.Value;
                else
                    throw new KeyNotFoundException();
            }
            set
            {
                var current = Find(key);
                if (current != null)
                    current.Value = value;
                else
                    throw new KeyNotFoundException();
            }
        }
        public int Count => count;
        public DictionaryWithEqual()
        {
            root = null;
            count = 0;
        }
        private NodeDict<TKey, TValue> Find(TKey key)
        {
            if (root == null) return null;

            var current = root;
            while (current != null)
            {
                if (current.Key.CompareTo(key) > 0)
                    current = current.LeftChildren;
                else if (current.Key.CompareTo(key) < 0)
                    current = current.RightChildren;
                else return current;
            }
            return null;
        }
        public bool ContainsKey(TKey key)
        {
            return Find(key) != null;
        }
        public void Add(TKey key, TValue value)
        {
            var newNode = new NodeDict<TKey, TValue>(key, value);
            if (root == null)
            {
                root = newNode;
                count++;
                return;
            }
            var current = root;
            while (current != null)
            {
                if (current.Key.CompareTo(key) > 0)
                {
                    if (current.LeftChildren == null)
                    {
                        current.LeftChildren = newNode;
                        newNode.Parent = current;
                        count++;
                        return;
                    }
                    current = current.LeftChildren;
                }
                else if (current.Key.CompareTo(key) < 0)
                {
                    if (current.RightChildren == null)
                    {
                        current.RightChildren = newNode;
                        newNode.Parent = current;
                        count++;
                        return;
                    }
                    current = current.RightChildren;
                }
                else throw new ArgumentException();
            }
        }
        public void Remove(TKey key)
        {
            var current = Find(key);
            if (current == null)
                throw new KeyNotFoundException();

            count--;
            if (current.LeftChildren == null && current.RightChildren == null) //нет потомков
            {
                NodeDict<TKey, TValue> parent = current.Parent;
                if (parent == null)
                    root = null;
                else
                    if (parent.LeftChildren == current)
                    parent.LeftChildren = null;
                else
                    parent.RightChildren = null;

            }
            else if (current.RightChildren != null && current.LeftChildren != null) //оба потомка
            {
                NodeDict<TKey, TValue> VecB = current.RightChildren;
                while (VecB.LeftChildren != null)
                    VecB = VecB.LeftChildren;

                TKey memoryKey = VecB.Key;
                TValue memoryValue = VecB.Value;

                Remove(memoryKey);

                current.Key = memoryKey;
                current.Value = memoryValue;
            }
            else if (current.LeftChildren != null) //левый потомок
            {
                NodeDict<TKey, TValue> left = current.LeftChildren;
                NodeDict<TKey, TValue> parent = current.Parent;
                if (parent == null)
                    root = left;
                else
                    if (parent.LeftChildren == current)
                    parent.LeftChildren = left;
                else
                    parent.RightChildren = left;
                left.Parent = parent;

            }
            else //правый потомок
            {
                NodeDict<TKey, TValue> right = current.RightChildren;
                NodeDict<TKey, TValue> parent = current.Parent;
                if (parent == null)
                    root = right;
                else
                    if (parent.LeftChildren == current)
                    parent.LeftChildren = right;
                else
                    parent.RightChildren = right;
                right.Parent = parent;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (root == null)
                return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            return root.GetEnumerable().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
