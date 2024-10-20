using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{
    public class BilateralQueue<T> : IEnumerable<T>, IEnumerable
    {
        private int count;

        private int capacity;

        private int head;

        private int tail;

        private T[] Memory;

        public int Capacity => capacity;

        public int Count => count;

        public BilateralQueue()
        {
            capacity = 4;
            count = 0;
            Memory = new T[capacity];
            tail = -1;
            head = 0;
        }

        public BilateralQueue(int length)
        {
            capacity = length;
            count = 0;
            Memory = new T[capacity];
            tail = -1;
            head = 0;
        }

        public BilateralQueue(IEnumerable<T> elements)
        {
            capacity = elements.Count();
            count = 0;
            tail = -1;
            Memory = new T[capacity];
            foreach (T element in elements)
            {
                EnqueueInBack(element);
            }

            head = 0;
        }

        public void Clear()
        {
            tail = -1;
            head = 0;
            count = 0;
        }

        private void Resize()
        {
            T[] memory = Memory;
            Memory = new T[capacity * 2];
            int num = 0;
            while (head != tail)
            {
                Memory[num] = memory[head];
                head++;
                head %= capacity;
                num++;
            }

            Memory[num] = memory[tail];
            head = 0;
            tail = num;
            capacity *= 2;
        }

        public T Peek()
        {
            if (count == 0)
            {
                throw new InvalidOperationException();
            }

            return Memory[head];
        }

        public T Dequeue()
        {
            if (count == 0)
            {
                throw new InvalidOperationException();
            }

            count--;
            T result = Memory[head];
            head++;
            head %= capacity;
            return result;
        }

        public void RemoveUp()
        {
            if (count == 0)
            {
                return;
            }
            count--;
            head++;
            head %= capacity;
        }

        public void EnqueueInBack(T element)
        {
            count++;
            if (count > capacity)
            {
                Resize();
            }

            tail++;
            tail %= capacity;
            Memory[tail] = element;
        }

        public void EnqueueInFront(T element)
        {
            count++;
            if (count > capacity)
            {
                Resize();
            }

            head--;
            if (head < 0)
            {
                head += capacity;
            }

            Memory[head] = element;
        }

        public bool Contains(T element)
        {
            if (count != 0)
            {
                int num;
                for (num = head; num != tail; num %= capacity)
                {
                    if (Memory[num].Equals(element))
                    {
                        return true;
                    }

                    num++;
                }

                if (Memory[tail].Equals(element))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int index2 = head;
            if (count != 0)
            {
                while (index2 != tail)
                {
                    yield return Memory[index2];
                    index2++;
                    index2 %= capacity;
                }

                yield return Memory[tail];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            int index2 = head;
            if (count != 0)
            {
                while (index2 != tail)
                {
                    yield return Memory[index2];
                    index2++;
                    index2 %= capacity;
                }

                yield return Memory[tail];
            }
        }
    }
}
