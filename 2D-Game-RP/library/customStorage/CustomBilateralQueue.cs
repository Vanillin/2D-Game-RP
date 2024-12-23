using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TwoD_Game_RP
{
    internal class CustomBilateralQueue<T> : IEnumerable<T>
    {
        private int _count;
        private int _capacity;
        private int _head;
        private int _tail;
        private T[] _memory;

        public int Capacity => _capacity;
        public int Count => _count;

        public CustomBilateralQueue()
        {
            _capacity = 4;
            _count = 0;
            _memory = new T[_capacity];
            _tail = -1;
            _head = 0;
        }

        public CustomBilateralQueue(int length)
        {
            _capacity = length;
            _count = 0;
            _memory = new T[_capacity];
            _tail = -1;
            _head = 0;
        }

        public CustomBilateralQueue(IEnumerable<T> elements)
        {
            _capacity = elements.Count();
            _count = 0;
            _tail = -1;
            _memory = new T[_capacity];
            foreach (T element in elements)
            {
                EnqueueInBack(element);
            }

            _head = 0;
        }

        public void Clear()
        {
            _tail = -1;
            _head = 0;
            _count = 0;
        }

        private void Resize()
        {
            T[] memory = _memory;
            _memory = new T[_capacity * 2];
            int num = 0;
            while (_head != _tail)
            {
                _memory[num] = memory[_head];
                _head++;
                _head %= _capacity;
                num++;
            }

            _memory[num] = memory[_tail];
            _head = 0;
            _tail = num;
            _capacity *= 2;
        }

        public T Peek()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException();
            }

            return _memory[_head];
        }

        public T Dequeue()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException();
            }

            _count--;
            T result = _memory[_head];
            _head++;
            _head %= _capacity;
            return result;
        }

        public void RemoveUp()
        {
            if (_count == 0)
            {
                return;
            }
            _count--;
            _head++;
            _head %= _capacity;
        }

        public void EnqueueInBack(T element)
        {
            _count++;
            if (_count > _capacity)
            {
                Resize();
            }

            _tail++;
            _tail %= _capacity;
            _memory[_tail] = element;
        }

        public void EnqueueInFront(T element)
        {
            _count++;
            if (_count > _capacity)
            {
                Resize();
            }

            _head--;
            if (_head < 0)
            {
                _head += _capacity;
            }

            _memory[_head] = element;
        }

        public bool Contains(T element)
        {
            if (_count != 0)
            {
                int num;
                for (num = _head; num != _tail; num %= _capacity)
                {
                    if (_memory[num].Equals(element))
                    {
                        return true;
                    }

                    num++;
                }

                if (_memory[_tail].Equals(element))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int index2 = _head;
            if (_count != 0)
            {
                while (index2 != _tail)
                {
                    yield return _memory[index2];
                    index2++;
                    index2 %= _capacity;
                }

                yield return _memory[_tail];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
