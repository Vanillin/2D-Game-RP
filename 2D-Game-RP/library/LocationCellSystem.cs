using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{

    internal class LocationCell
    {
        public IPictureCell pictureCell;
        public LocationCell next;
        public int index;
        public LocationCell(IPictureCell pictureCell, int index)
        {
            this.pictureCell = pictureCell;
            this.next = null;
            this.index = index;
        }
    }

    internal class ListLocationCell : IEnumerable<IPictureCell>
    {
        int count;
        LocationCell head;
        bool isBlock;
        bool isWatch;
        bool isHaveDark;
        List<IPictureCell> PictureCellOn1;
        public bool IsBlock => isBlock;
        public bool IsWatch => isWatch;
        public bool IsWasView { get; set; }
        public int Count => count;
        public ListLocationCell(IPictureCell pictureCell, int index)
        {
            this.count = 0;
            this.head = new LocationCell(pictureCell, index);
            this.isBlock = false;
            this.isWatch = true;
            this.IsWasView = false;
            this.isHaveDark = false;
            PictureCellOn1 = new List<IPictureCell>();
        }
        public ListLocationCell()
        {
            this.count = 0;
            this.head = null;
            this.isBlock = false;
            this.isWatch = true;
            this.IsWasView = false;
            this.isHaveDark = false;
            PictureCellOn1 = new List<IPictureCell>();
        }
        /*
        0 - земля
        1 - люди 
        2 - аномалии ящики
        3 - деревья 
        4 - системные знаки
        */
        public void AddSkeletWithLocationCell(IPictureCell pictureCell)
        {
            PictureCellOn1.Add(pictureCell);
            if (!isHaveDark)
                AddLocationCell(pictureCell, 1);
        }
        public void RemoveSkeletWithLocationCell(IPictureCell pictureCell)
        {
            PictureCellOn1.Remove(pictureCell);
            RemoveLocationCell(pictureCell);
        }
        public void ChangeHavingDark(bool IsHaveDark)
        {
            if (IsHaveDark == this.isHaveDark) return;
            if (IsHaveDark)
            {
                this.isHaveDark = true;
                AddLocationCell(DarkenPicCell.Taking(), 4);
                foreach (var v in PictureCellOn1)
                {
                    RemoveLocationCell(v);
                }
            }
            else
            {
                this.isHaveDark = false;
                RemoveLocationCell(DarkenPicCell.Taking());
                foreach (var v in PictureCellOn1)
                {
                    AddLocationCell(v, 1);
                }
            }
        }
        private void AnalyzeOnWatch(IPictureCell picture)
        {
            if (!isWatch) return;
            SubstringSearch ss = SubstringSearch.Creating();
            foreach (var v in Information.NotWatch)
            {
                if (ss.CheckSubstring(picture.Picture(), v))
                {
                    isWatch = false;
                    return;
                }
            }
        }
        private void AnalyzeOnBlock(IPictureCell picture)
        {
            if (isBlock) return;
            SubstringSearch ss = SubstringSearch.Creating();
            foreach (var v in Information.Blocks)
            {
                if (ss.CheckSubstring(picture.Picture(), v))
                {
                    isBlock = true;
                    return;
                }
            }
        }
        public void AddLocationCell(IPictureCell pictureCell, int index)
        {
            count++;
            var addcell = new LocationCell(pictureCell, index);

            AnalyzeOnBlock(pictureCell);
            AnalyzeOnWatch(pictureCell);
            if (head == null)
            {
                head = addcell;
                return;
            }
            LocationCell current = head;
            LocationCell last = null;
            while (true)
            {
                if (current == null || current.index > addcell.index)
                {
                    if (current == head)
                    {
                        head = addcell;
                    }
                    else
                    {
                        last.next = addcell;
                    }
                    addcell.next = current;
                    return;
                }
                else
                {
                    last = current;
                    current = current.next;
                }
            }
        }
        public void RemoveLocationCell(IPictureCell pictureCell)
        {
            if (head == null) { return; }
            if (head.pictureCell == pictureCell)
            {
                count--;
                head = head.next;
                return;
            }
            var current = head;
            while (current.next != null)
            {
                if (current.next.pictureCell == pictureCell)
                {
                    count--;
                    current.next = current.next.next;
                    return;
                }
                else
                {
                    current = current.next;
                }
            }
        }

        public IEnumerator<IPictureCell> GetEnumerator()
        {
            var current = head;
            while (current != null)
            {
                yield return current.pictureCell;
                current = current.next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var current = head;
            while (current != null)
            {
                yield return current.pictureCell;
                current = current.next;
            }
        }
    }
}
