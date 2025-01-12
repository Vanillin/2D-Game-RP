using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace TwoD_Game_RP
{
    /// <summary>
    /// Разделение по слоям: -1 - ужатая земля, 0 - земля, 1 - люди, 2 - аномалии ящики, 3 - деревья, 4 - системные знаки
    /// </summary>
    internal class NodePictCell
    {
        public IPictureCell PictureCell;
        public NodePictCell Next;
        public int Index;
        public NodePictCell(IPictureCell pictureCell, int index)
        {
            PictureCell = pictureCell;
            Next = null;
            Index = index;
        }
    }
    /// <summary>
    /// Разделение по слоям: -1 - ужатая земля, 0 - земля, 1 - люди, 2 - аномалии ящики, 3 - деревья, 4 - системные знаки
    /// </summary>
    internal class LocationCell
    {
        int _count;
        NodePictCell _head;
        bool _isBlock;
        bool _isWatch;
        bool _isHaveDark;
        List<IPictureCell> _pictureCellOn1;
        public bool IsBlock => _isBlock;
        public bool IsWatch => _isWatch;
        public bool IsWasView { get; set; }
        public int Count => _count;
        private LocationCell(IPictureCell pictureCell, int index)
        {
            _count = 0;
            _head = new NodePictCell(pictureCell, index);
            _isBlock = false;
            _isWatch = true;
            IsWasView = false;
            _isHaveDark = false;
            _pictureCellOn1 = new List<IPictureCell>();
        }
        public LocationCell()
        {
            _count = 0;
            _head = null;
            _isBlock = false;
            _isWatch = true;
            IsWasView = false;
            _isHaveDark = false;
            _pictureCellOn1 = new List<IPictureCell>();
        }
        public void AddFirstLayerWithCell(IPictureCell pictureCell)
        {
            _pictureCellOn1.Add(pictureCell);
            if (!_isHaveDark)
                AddCell(pictureCell, 1);
        }
        public void RemoveFirstLayerWithCell(IPictureCell pictureCell)
        {
            _pictureCellOn1.Remove(pictureCell);
            RemoveCell(pictureCell);
        }
        public void ChangeHavingDark(bool isHaveDark)
        {
            if (isHaveDark == _isHaveDark) return;
            if (isHaveDark)
            {
                _isHaveDark = true;
                AddCell(DarkenPicCell.Taking(), 4);
                foreach (var v in _pictureCellOn1)
                {
                    RemoveCell(v);
                }
            }
            else
            {
                _isHaveDark = false;
                RemoveCell(DarkenPicCell.Taking());
                foreach (var v in _pictureCellOn1)
                {
                    AddCell(v, 1);
                }
            }
        }
        private void AnalyzeOnWatch(IPictureCell picture)
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
        private void AnalyzeOnBlock(IPictureCell picture)
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
        public void AddCell(IPictureCell pictureCell, int index)
        {
            _count++;
            var addcell = new NodePictCell(pictureCell, index);

            AnalyzeOnBlock(pictureCell);
            AnalyzeOnWatch(pictureCell);
            if (_head == null)
            {
                _head = addcell;
                return;
            }
            NodePictCell current = _head;
            NodePictCell last = null;
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
        public void RemoveCell(IPictureCell pictureCell)
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
        public enum Floor
        {
            Yes,
            No
        }
        /// <summary>
        /// Return pair IPictureCell picture and bool IsFloor
        /// </summary>
        /// <returns></returns>
        public IEnumerator<(IPictureCell, bool)> GetEnumerator()
        {
            var current = _head;
            while (current != null)
            {
                if(current.Index == -1)
                    yield return (current.PictureCell, true);
                else
                    yield return (current.PictureCell, false);

                current = current.Next;
            }
        }
    }
}
