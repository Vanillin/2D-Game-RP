using System.Collections.Generic;

namespace TwoD_Game_RP
{
    internal interface IPictureMapList : IPictureList
    {
        bool IsBlock { get; }
        bool IsWatch { get; }
        bool IsWasView { get; set; }
        void ChangeHavingDark(bool isHaveDark);
        (double sizeh, double sizew)[] GetSizes();
    }
    internal class MemoryPictureLocation : IPictureMapList
    {
        class NodeMemoryMap
        {
            public IPicture PictureCell;
            public NodeMemoryMap Next;
            public Layer Index;
            public NodeMemoryMap(IPicture pictureCell, Layer index)
            {
                PictureCell = pictureCell;
                Next = null;
                Index = index;
            }
        }
        /// <summary>
        /// Разделение по слоям: -1 - ужатая земля, 0 - земля, 1 - люди, 2 - аномалии ящики, 3 - деревья, 4 - системные знаки
        /// </summary>
        enum Layer
        {
            Earth,
            Wall,
            Skelet,
            BoxAnomaly,
            Sky,
            System
        }

        int _count;
        NodeMemoryMap _head;
        bool _isBlock;
        bool _isWatch;
        bool _isHaveDark;
        List<IPicture> _pictureCellSkelet;
        public bool IsBlock => _isBlock;
        public bool IsWatch => _isWatch;
        public bool IsWasView { get; set; }
        public int Count => _count;
        public MemoryPictureLocation()
        {
            _count = 0;
            _head = null;
            _isBlock = false;
            _isWatch = true;
            IsWasView = false;
            _isHaveDark = false;
            _pictureCellSkelet = new List<IPicture>();
        }

        public void Clear()
        {
            _count = 0;
            _head = null;
            _isBlock = false;
            _isWatch = true;
            IsWasView = false;
            _isHaveDark = false;
            _pictureCellSkelet = new List<IPicture>();
        }
        public void ChangeHavingDark(bool isHaveDark)
        {
            if (isHaveDark == _isHaveDark) return;
            if (isHaveDark)
            {
                _isHaveDark = true;

                foreach (var v in _pictureCellSkelet)
                {
                    SystemRemoveCell(v);
                }
                AddCell(DarkenPicCell.Taking(), -1);

                var start = _head;
                while (start.Next != null)
                {
                    var next = start.Next;
                    if (next.Index != Layer.Earth && next.Index != Layer.System && next.Index != Layer.Skelet)
                    {
                        //Its over DANDER !!! Check its in adding pictures
                        (next.PictureCell as ISomePicture).ChangeIndexPicture(1);
                    }
                    start = next;
                }
            }
            else
            {
                _isHaveDark = false;
                RemoveCell(DarkenPicCell.Taking());

                var start = _head;
                while (start.Next != null)
                {
                    var next = start.Next;
                    if (next.Index != Layer.Earth && next.Index != Layer.System && next.Index != Layer.Skelet)
                    {
                        //Its over DANDER !!! Check its in adding pictures
                        (next.PictureCell as ISomePicture).ChangeIndexPicture(0);
                    }
                    start = next;
                }

                foreach (var v in _pictureCellSkelet)
                {
                    SystemAddCell(v, 1);
                }
            }
        }
        private void AnalyzeOnWatch(IPicture picture)
        {
            if (!_isWatch) return;
            SubstringSearch ss = SubstringSearch.Creating();
            foreach (var v in Information.NotWatch)
            {
                if (ss.CheckSubstring(picture.Picture(), v))
                {
                    _isWatch = false;
                    return;
                }
            }
        }
        private void AnalyzeOnBlock(IPicture picture)
        {
            if (_isBlock) return;
            SubstringSearch ss = SubstringSearch.Creating();
            foreach (var v in Information.Blocks)
            {
                if (ss.CheckSubstring(picture.Picture(), v))
                {
                    _isBlock = true;
                    return;
                }
            }
        }
        private Layer TranslateIntoLayer(int index)
        {
            Layer layer;
            switch (index)
            {
                case -1: layer = Layer.Earth; break;
                case 0: layer = Layer.Wall; break;
                case 1: layer = Layer.Skelet; break;
                case 2: layer = Layer.BoxAnomaly; break;
                case 3: layer = Layer.Sky; break;
                case 4: layer = Layer.System; break;
                default: throw new CustomException("IndexLayer is not correct");
            }
            return layer;
        }
        private void SystemAddCell(IPicture pictureCell, int indexlayer)
        {
            Layer layer = TranslateIntoLayer(indexlayer);
            if (indexlayer == 0 ||/* indexlayer == 2 ||*/ indexlayer == 3)
            {
                if (!(pictureCell is ISomePicture))
                    throw new CustomException("PictureCell is not ISomePicture");
            }

            _count++;
            var addcell = new NodeMemoryMap(pictureCell, layer);

            AnalyzeOnBlock(pictureCell);
            AnalyzeOnWatch(pictureCell);
            if (_head == null)
            {
                _head = addcell;
                return;
            }
            NodeMemoryMap current = _head;
            NodeMemoryMap last = null;
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
        public void AddCell(IPicture pictureCell, int indexlayer)
        {
            if (TranslateIntoLayer(indexlayer) == Layer.Skelet)
            {
                if (_isHaveDark)
                {
                    _pictureCellSkelet.Add(pictureCell);
                }
                else
                {
                    SystemAddCell(pictureCell, indexlayer);
                    _pictureCellSkelet.Add(pictureCell);
                }
            }
            else
            {
                SystemAddCell(pictureCell, indexlayer);
            }
        }
        private void SystemRemoveCell(IPicture pictureCell)
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
        public void RemoveCell(IPicture pictureCell)
        {
            SystemRemoveCell(pictureCell);
            _pictureCellSkelet.Remove(pictureCell);
        }
        IEnumerator<IPicture> IPictureList.GetEnumerator()
        {
            var current = _head;
            while (current != null)
            {
                yield return current.PictureCell;
                current = current.Next;
            }
        }
        public (double sizeh, double sizew)[] GetSizes()
        {
            (double sizeh, double sizew)[] sizes = new (double sizeh, double sizew)[_count];
            if (_count == 0) return sizes;
            var current = _head;
            int i = 0;
            while (current != null)
            {
                switch (current.Index)
                {
                    case Layer.Earth: sizes[i] = (1, 1); break;
                    default: sizes[i] = (2, 1); break;
                }
                current = current.Next;
                i++;
            }
            return sizes;
        }
    }
}
