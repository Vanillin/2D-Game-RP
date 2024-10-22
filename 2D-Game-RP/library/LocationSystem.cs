using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TwoD_Game_RP.library;

namespace Game_STALKER_Exclusion_Zone
{
    public class Location
    {
        private int maxDepth;
        private ListLocationCell[,] Cells;
        private  Graf grafLocToMove;
        private Graf grafLocToWatch;
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
        public List<Skelet> Lives;
        private DBDisplay display;

        public Location(string name, string systemName, int height, int width)
        {
            maxDepth = 0;
            Cells = new ListLocationCell[height, width];
            for( int i  = 0; i < height; i++ )
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
            Lives = new List<Skelet>();
            display = new DBDisplay(height, width, 6);
        }
        public void Display(Canvas canvas, double size)
        {
            display.Display(canvas, size);
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

                    var cur = new Vertex(new Point(i, j));
                    grafLocToMove.Vertexes.Add(cur);
                    if (i-1>=0 && !Cells[i-1, j].IsBlock)
                    {
                        var up = grafLocToMove.Find(new Point(i - 1, j));
                        up.Near.Add(cur);
                        cur.Near.Add(up);
                    }
                    if (j-1>=0 && !Cells[i, j-1].IsBlock)
                    {
                        var left = grafLocToMove.Find(new Point(i, j-1));
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

                    var cur = new Vertex(new Point(i, j));
                    grafLocToWatch.Vertexes.Add(cur);
                    if (i - 1 >= 0 && Cells[i - 1, j].IsWatch)
                    {
                        var up = grafLocToWatch.Find(new Point(i - 1, j));
                        up.Near.Add(cur);
                        cur.Near.Add(up);
                    }
                    if (j - 1 >= 0 && Cells[i, j - 1].IsWatch)
                    {
                        var left = grafLocToWatch.Find(new Point(i, j - 1));
                        left.Near.Add(cur);
                        cur.Near.Add(left);
                    }
                }
            }
        }
        public void AddLayerCells(IPictureCell[,] cells, int index)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (cells[i,j] == null) continue;
                    Cells[i, j].AddLocationCell(cells[i,j], index);
                }
            }
        }
        public void UpdateDisplay()
        {
            display.Update(Cells);
        }
        public void AddCell(IPictureCell cell, int index, int heightInd, int weightInd)
        {
            Cells[heightInd, weightInd].AddLocationCell(cell, index);
            display.Update(Cells, heightInd, weightInd);
        }
        public void RemoveCell(IPictureCell cell, int heightInd, int weightInd)
        {
            Cells[heightInd, weightInd].RemoveLocationCell(cell);
            display.Update(Cells, heightInd, weightInd);
        }
        public bool GetIsBlockCell(int heightInd, int weightInd)
        {
            return Cells[heightInd, weightInd].IsBlock;
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
        public bool IsBlock => isBlock;
        public bool IsWatch => isWatch;
        public int Count => count;
        public ListLocationCell(IPictureCell pictureCell, int index)
        {
            this.count = 0;
            this.head = new LocationCell(pictureCell, index);
            this.isBlock = false;
            this.isWatch = true;
        }
        public ListLocationCell()
        {
            this.count = 0;
            this.head = null;
            this.isBlock = false;
            this.isWatch = true;
        }
            
        private void AnalyzeOnWatch(IPictureCell picture)
        {
            if (!isWatch) return;
            var elements = picture.Picture().Split(new char[] { '/', '\\', '.' }, StringSplitOptions.RemoveEmptyEntries);
            string sign = elements[elements.Length - 2];
            if (Information.NotWatch.Contains(sign))
            {
                isWatch = false;
            }
        }
        private void AnalyzeOnBlock(IPictureCell picture)
        {
            if (isBlock) return;
            var elements = picture.Picture().Split(new char[] { '/', '\\', '.' }, StringSplitOptions.RemoveEmptyEntries);
            string sign = elements[elements.Length - 2];

            if (Information.Blocks.Contains(sign))
            {
                isBlock = true;
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
            count--;
            if (head.pictureCell == pictureCell)
            {
                head = head.next;
            }
            var current = head;
            while (current.next != null)
            {
                if (current.next.pictureCell == pictureCell)
                {
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

    public interface IPictureCell
    {
        string Picture();
        void NextPicture();
        void ReloadPictures();
    }    

    public class StaticPicCell : IPictureCell
    {
        string picture;
        public StaticPicCell(string picture)
        {
            this.picture = picture;
        }

        public void NextPicture()
        {        }
        public string Picture()
        {
            return picture;
        }
        public void ReloadPictures()
        {        }
    }

    public class AnimatedPicCell : IPictureCell
    {
        string[] picture;
        int index;
        public int Count => picture.Length;
        public AnimatedPicCell(string[] picture)
        {
            this.picture = picture;
            index = 0;
        }

        public void NextPicture()
        {
            index = (index + 1) % picture.Length;
        }
        public string Picture()
        {
            return picture[index];
        }
        public void ReloadPictures()
        {
            index = 0;
        }
    }

    public class RandomPicCell : IPictureCell
    {
        string picture;
        public RandomPicCell(Random rd, string[] picture)
        {
            int index = rd.Next(picture.Length);
            this.picture = picture[index];
        }
        public void NextPicture()
        {        }
        public string Picture()
        {
            return picture;
        }
        public void ReloadPictures()
        {        }
    }
}
