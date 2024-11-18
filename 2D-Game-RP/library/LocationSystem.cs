using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TwoD_Game_RP
{
    public class Location
    {
        private int maxDepth;
        private ListLocationCell[,] Cells;
        private Graf grafLocToMove;
        private Graf grafLocToWatch;
        private Graf grafLocAll;
        private List<Skelet> lives;
        public SortedEnum<GamePoint> IsBusy;
        public int Height { get; }
        public int Width { get; }
        public int MaxDepth => maxDepth;

        public readonly string Name;
        public readonly string SystemName;
        public Graf GrafLocToMove
        {
            get
            {
                if (this.grafLocToMove == null) { throw new Exception("Граф не создан"); }
                else return this.grafLocToMove;
            }
        }
        public Graf GrafLocToWatch
        {
            get
            {
                if (this.grafLocToWatch == null) { throw new Exception("Граф не создан"); }
                else return this.grafLocToWatch;
            }
        }
        public Graf GrafLocAll
        {
            get
            {
                if (this.grafLocAll == null) { throw new Exception("Граф не создан"); }
                else return this.grafLocAll;
            }
        }
        private DBDisplay display;

        public Location(string name, string systemName, int height, int width)
        {
            maxDepth = 0;
            Cells = new ListLocationCell[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Cells[i, j] = new ListLocationCell();
                }
            }
            Height = height;
            Width = width;
            Name = name;
            SystemName = systemName;
            grafLocToMove = null;
            grafLocToWatch = null;
            lives = new List<Skelet>();
            display = new DBDisplay(height, width, 6);
            IsBusy = new SortedEnum<GamePoint>();
        }
        public List<Skelet> GetLives()
        {
            return lives;
        }
        public void AddLivesWithCell(Skelet skelet)
        {
            
            AddCell(skelet.picture, 1, (int)skelet.Cord.X, (int)skelet.Cord.Y);
            IsBusy.Add(new GamePoint(skelet.Cord.X, skelet.Cord.Y));
            lives.Add(skelet);
        }
        public void RemoveLivesWithCell(Skelet skelet)
        {
            RemoveLivesWithCell(skelet.picture, (int)skelet.Cord.X, (int)skelet.Cord.Y);
            IsBusy.Remove(new GamePoint(skelet.Cord.X, skelet.Cord.Y));
            lives.Remove(skelet);
        }
        public Skelet FindLives(int heightInd, int weightInd)
        {
            return lives.Find(x => x.Cord.CompareTo((heightInd, weightInd))==0);
        }
        public void MoveLivesWithCell(Skelet skelet, GamePoint newPoint)
        {
            RemoveLivesWithCell(skelet.picture, (int)skelet.Cord.X, (int)skelet.Cord.Y);
            IsBusy.Remove(new GamePoint(skelet.Cord.X, skelet.Cord.Y));

            skelet.Cord = newPoint;

            AddCell(skelet.picture, 1, (int)skelet.Cord.X, (int)skelet.Cord.Y);
            IsBusy.Add(new GamePoint(skelet.Cord.X, skelet.Cord.Y));
        }
        public void Display(Canvas canvas, double size, List<UIElement> systemObj)
        {
            foreach (var v in Cells)
            {
                v.ChangeHavingDark(false);
            }
            display.Update(Cells);
            display.Display(canvas, size, systemObj);
        }
        public void DisplayToPoints(SortedEnum<GamePoint> displayPoints, Canvas canvas, double size, List<UIElement> systemObj)
        {
            display.DisplayToPoints(displayPoints, canvas, size, systemObj);
        }
        public void DisplayToPointsWithBorder(SortedEnum<GamePoint> displayPoints, GamePoint LeftUpCorner, Canvas canvas, double size, List<UIElement> systemObj)
        {
            display.DisplayToPointsWithBorder(displayPoints, LeftUpCorner, canvas, size, systemObj);
        }
        public void DisplayToPointsWithBorderAndView(SortedEnum<GamePoint> displayPointsView, SortedEnum<GamePoint> displayPointsAll, GamePoint LeftUpCorner, Canvas canvas, double size, List<UIElement> systemObj)
        {
            foreach (var v in displayPointsAll)
            {
                if (displayPointsView.Contains(v))
                {
                    Cells[(int)v.X, (int)v.Y].ChangeHavingDark(false);
                    Cells[(int)v.X, (int)v.Y].IsWasView = true;
                    display.Update(Cells, (int)v.X, (int)v.Y);
                }
                else if (Cells[(int)v.X, (int)v.Y].IsWasView)
                {
                    displayPointsView.Add(v);
                    Cells[(int)v.X, (int)v.Y].ChangeHavingDark(true);
                    display.Update(Cells, (int)v.X, (int)v.Y);
                }
            }
            display.DisplayToPointsWithBorder(displayPointsView, LeftUpCorner, canvas, size, systemObj);
        }
        public void CreateGrafMove()
        {
            grafLocToMove = new Graf();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (Cells[i, j].IsBlock)
                    {
                        continue;
                    }

                    var cur = new Vertex(new GamePoint(i, j));
                    grafLocToMove.Vertexes.Add(cur);
                    if (i - 1 >= 0 && !Cells[i - 1, j].IsBlock)
                    {
                        var up = grafLocToMove.Find(new GamePoint(i - 1, j));
                        up.Near.Add(cur);
                        cur.Near.Add(up);
                    }
                    if (j - 1 >= 0 && !Cells[i, j - 1].IsBlock)
                    {
                        var left = grafLocToMove.Find(new GamePoint(i, j - 1));
                        left.Near.Add(cur);
                        cur.Near.Add(left);
                    }
                }
            }
        }
        public void CreateGrafAll()
        {
            grafLocAll = new Graf();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var cur = new Vertex(new GamePoint(i, j));
                    grafLocAll.Vertexes.Add(cur);
                    if (i - 1 >= 0)
                    {
                        var up = grafLocAll.Find(new GamePoint(i - 1, j));
                        up.Near.Add(cur);
                        cur.Near.Add(up);
                    }
                    if (j - 1 >= 0)
                    {
                        var left = grafLocAll.Find(new GamePoint(i, j - 1));
                        left.Near.Add(cur);
                        cur.Near.Add(left);
                    }
                }
            }
        }
        public void CreateGrafWatch()
        {
            grafLocToWatch = new Graf();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (!Cells[i, j].IsWatch)
                    {
                        continue;
                    }

                    var cur = new Vertex(new GamePoint(i, j));
                    grafLocToWatch.Vertexes.Add(cur);
                    if (i - 1 >= 0 && Cells[i - 1, j].IsWatch)
                    {
                        var up = grafLocToWatch.Find(new GamePoint(i - 1, j));
                        up.Near.Add(cur);
                        cur.Near.Add(up);
                    }
                    if (j - 1 >= 0 && Cells[i, j - 1].IsWatch)
                    {
                        var left = grafLocToWatch.Find(new GamePoint(i, j - 1));
                        left.Near.Add(cur);
                        cur.Near.Add(left);
                    }
                }
            }
        }
        public bool IsCellBusy(int heightInd, int weightInd)
        {
            return IsBusy.Contains(new GamePoint(heightInd, weightInd));
        }
        public void AddLayerCells(IPictureCell[,] cells, int index)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (cells[i, j] == null) continue;
                    Cells[i, j].AddLocationCell(cells[i, j], index);
                }
            }
        }
        public void UpdateDisplay()
        {
            display.Update(Cells);
        }
        public bool GetIsBlockCell(int heightInd, int weightInd)
        {
            if (Cells[heightInd, weightInd] == null) return true;
            return Cells[heightInd, weightInd].IsBlock;
        }
        public bool GetIsWatchCell(int heightInd, int weightInd)
        {
            return Cells[heightInd, weightInd].IsWatch;
        }
        private void AddCell(IPictureCell cell, int index, int heightInd, int weightInd)
        {
            if (index == 1)
                Cells[heightInd, weightInd].AddSkeletWithLocationCell(cell);
            else
                Cells[heightInd, weightInd].AddLocationCell(cell, index);
            display.Update(Cells, heightInd, weightInd);
        }
        private void RemoveLivesWithCell(IPictureCell cell, int heightInd, int weightInd)
        {
            Cells[heightInd, weightInd].RemoveSkeletWithLocationCell(cell);
            display.Update(Cells, heightInd, weightInd);
        }
        private void RemoveCell(IPictureCell cell, int heightInd, int weightInd)
        {
            Cells[heightInd, weightInd].RemoveLocationCell(cell);
            display.Update(Cells, heightInd, weightInd);
        }
        public IEnumerable<IPictureCell> GetEnumerableCell(int heightInd, int weightInd)
        {
            return Cells[heightInd, weightInd];
        }
    }

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
        1 - люди ящики
        2 - аномалии
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
                foreach(var v in PictureCellOn1)
                {
                    RemoveLocationCell(v);
                }
            }
            else
            {
                this.isHaveDark = false;
                RemoveLocationCell(DarkenPicCell.Taking());
                foreach(var v in PictureCellOn1)
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

    /*     
    public abstract class Location
    {
        string name;
        string[,] firstLayerAir;
        string[,] thirdLayerBlock;
        Subject[,] secondLayerCreature;
        List<Subject> subjects;
        Graf toMove;
        Graf toWatch;
        int rows;
        int columns;
        LocationMethods methods;
        public int Rows => rows;
        public int Columns => columns;
        public Subject[,] SecondLayerCreature => secondLayerCreature;
        public List<Subject> Subjects => subjects;
        public Graf ToMove => toMove;
        public Graf ToWatch => toWatch;
        public string Name => name;
        public string[,] FirstLayerAir => firstLayerAir;
        public string[,] ThirdLayerBlock => thirdLayerBlock;
        public Location(string name, string systemName)
        {
            this.name = name;
            this.rows = 0; this.columns = 0;
            string[,] firstLayerAir = new string[0, 0];
            string[,] thirdLayerBlock = new string[0, 0];
            new ReaderFile().ReadFileLocation(systemName, ref firstLayerAir, ref thirdLayerBlock, ref rows, ref columns);
            this.firstLayerAir = firstLayerAir;
            this.thirdLayerBlock = thirdLayerBlock;
            secondLayerCreature = new Subject[rows, columns];
            subjects = new List<Subject>();
            toMove = new Graf(this.thirdLayerBlock, true, rows, columns);
            toWatch = new Graf(this.thirdLayerBlock, false, rows, columns);
            this.methods = new LocationMethods();
        }
        public void AddSubject(Subject subject, GamePoint cord)
        {
            methods.AddSubject(subject, cord, this);
        }
        public void DeleteSubject(Subject subject, GamePoint cord)
        {
            methods.DeleteSubject(subject, cord, this);
        }
        public void GoingSubject(Subject subject, GamePoint cord)
        {
            methods.GoingSubject(subject, cord, this);
        }
    }
    public interface ILocationMethods
    {
        void AddSubject(Subject subject, GamePoint cord, Location location);
        void DeleteSubject(Subject subject, GamePoint cord, Location location);
        void GoingSubject(Subject subject, GamePoint cord, Location location);
    }
    public class LocationMethods : ILocationMethods
    {
        public void AddSubject(Subject subject, GamePoint cord, Location location)
        {
            location.Subjects.Add(subject);
            if (location.SecondLayerCreature[cord.H, cord.W] != null)
            {
                throw new Exception("Существо не может быть расположено, место занято");
            }
            subject.ChangeLocation(location, cord);
            location.SecondLayerCreature[cord.H, cord.W] = subject;
            location.ToMove.Find(cord).Busy = true;
        }
        public void DeleteSubject(Subject subject, GamePoint cord, Location location)
        {
            location.Subjects.RemoveAll(X => X.GamePoint.H == cord.H && X.GamePoint.W == cord.W);
            location.SecondLayerCreature[cord.H, cord.W] = null;
        }
        public void GoingSubject(Subject subject, GamePoint cord, Location location)
        {
            if (location.Subjects.Contains(subject) && location.SecondLayerCreature[cord.H, cord.W] == null)
            {
                location.SecondLayerCreature[subject.GamePoint.H, subject.GamePoint.W] = null;
                location.ToMove.Find(subject.GamePoint).Busy = false;
                subject.GamePoint = cord;
                location.SecondLayerCreature[cord.H, cord.W] = subject;
                location.ToMove.Find(cord).Busy = true;
            }
        }
    }
    */
}
