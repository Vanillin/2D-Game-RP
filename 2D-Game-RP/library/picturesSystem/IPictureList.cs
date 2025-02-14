using System.Collections.Generic;

namespace TwoD_Game_RP
{
    internal interface IPictureList
    {
        int Count { get; }
        void AddCell(IPicture pictureCell, int indexlayer);
        void RemoveCell(IPicture pictureCell);
        void Clear();
        IEnumerator<IPicture> GetEnumerator();
    }

    internal class MemoryPictureInventory : IPictureList
    {
        class NodeMemoryPicture
        {
            public IPicture PictureCell;
            public NodeMemoryPicture Next;
            public Layer Index;
            public NodeMemoryPicture(IPicture pictureCell, Layer index)
            {
                PictureCell = pictureCell;
                Next = null;
                Index = index;
            }
        }
        enum Layer
        {
            Fon,
            Item
        }

        int _count;
        NodeMemoryPicture _head;
        public int Count => _count;
        public MemoryPictureInventory()
        {
            _count = 0;
            _head = null;
        }
        /// <summary>
        /// Param indexlayer is 0 (Fon) and 1(Item).
        /// </summary>
        /// <param name="pictureCell"></param>
        /// <param name="indexlayer"></param>
        public void AddCell(IPicture pictureCell, int indexlayer)
        {
            Layer layer;
            switch (indexlayer)
            {
                case 0: layer = Layer.Fon; break;
                case 1: layer = Layer.Item; break;
                default: throw new CustomException("IndexLayer is not correct");
            }

            _count++;
            var addcell = new NodeMemoryPicture(pictureCell, layer);

            if (_head == null)
            {
                _head = addcell;
                return;
            }
            NodeMemoryPicture current = _head;
            NodeMemoryPicture last = null;
            while (true)
            {
                if (current == null || current.Index > addcell.Index)
                {
                    if (current == _head)
                    {
                        _head = addcell;
                    }
                    else
                    {
                        last.Next = addcell;
                    }
                    addcell.Next = current;
                    return;
                }
                else
                {
                    last = current;
                    current = current.Next;
                }
            }
        }
        public void RemoveCell(IPicture pictureCell)
        {
            if (_head == null) { return; }
            if (_head.PictureCell == pictureCell)
            {
                _count--;
                _head = _head.Next;
                return;
            }
            var current = _head;
            while (current.Next != null)
            {
                if (current.Next.PictureCell == pictureCell)
                {
                    _count--;
                    current.Next = current.Next.Next;
                    return;
                }
                else
                {
                    current = current.Next;
                }
            }
        }
        public void Clear()
        {
            _head = null;
            _count = 0;
        }
        public IEnumerator<IPicture> GetEnumerator()
        {
            var current = _head;
            while (current != null)
            {
                yield return (current.PictureCell);
                current = current.Next;
            }
        }
    }
}
