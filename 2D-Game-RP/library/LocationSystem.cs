using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TwoD_Game_RP
{
    /// <summary>
    /// Разделение по слоям: -1 - ужатая земля, 0 - земля, 1 - люди, 2 - аномалии ящики, 3 - деревья, 4 - системные знаки
    /// </summary>
    public class Location
    {
        private int _maxDepth;
        private LocationCell[,] _cells;
        private Graf _grafLocToMove;
        private Graf _grafLocToWatch;
        private Graf _grafLocAll;
        private List<SystemSkelet> _lives;
        private DBDisplay _display;
        private CustomDictionary<GamePoint, string> _transitGamePoint;
        private CustomDictionary<string, List<GamePoint>> _spawnGamePoint;
        public CustomSortedEnum<GamePoint> IsBusy { get; set; }
        public int Height { get; }
        public int Width { get; }
        public int MaxDepth => _maxDepth;
        public string Name { get; private set; }
        public string SystemName { get; private set; }
        public int CreateLenghtPathVisible(GamePoint start, GamePoint finish)
        {
            if (_grafLocToWatch == null) { throw new CustomException("Граф не создан"); }
            return _grafLocToWatch.SearchWidth(start, finish).Count;
        }
        public List<GamePoint> CreatePath_OutOfPoint(GamePoint start, GamePoint finish, CustomSortedEnum<GamePoint> deletedpoints)
        {
            if (_grafLocToMove == null) { throw new CustomException("Граф не создан"); }
            return _grafLocToMove.SearchWidth_OutOfPoint(start, finish, deletedpoints);
        }
        public CustomSortedEnum<GamePoint> GetWatchCirlce(GamePoint start, double count, int minX, int minY, int maxX, int maxY)
        {
            return _grafLocToWatch.SearchCircle(start, count, minX, minY, maxX, maxY);
        }
        public CustomSortedEnum<GamePoint> GetWatchCircle_WithAngleOutOfPoint(GamePoint start, double count, int minX, int minY, int maxX, int maxY, CustomSortedEnum<GamePoint> deletedpoints)
        {
            return _grafLocToWatch.SearchCircle_WithAngleOutOfPoint(start, count, minX, minY, maxX, maxY, deletedpoints);
        }

        public Location(string name, string systemName, int height, int width, double compressH, double compressW, List<(string, GamePoint)> transitPoint)
        {
            _maxDepth = 0;
            _cells = new LocationCell[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    _cells[i, j] = new LocationCell();
                }
            }
            _grafLocToMove = null;
            _grafLocToWatch = null;
            _lives = new List<SystemSkelet>();
            _display = new DBDisplay(height, width, 6, compressH, compressW);

            _transitGamePoint = new CustomDictionary<GamePoint, string>();
            _spawnGamePoint = new CustomDictionary<string, List<GamePoint>>();
            foreach (var pair in transitPoint)
            {
                if (!_spawnGamePoint.ContainsKey(pair.Item1))
                    _spawnGamePoint.Add(pair.Item1,new List<GamePoint>());
                _spawnGamePoint[pair.Item1].Add(pair.Item2);
                _transitGamePoint.Add(pair.Item2, pair.Item1);
            }

            IsBusy = new CustomSortedEnum<GamePoint>();
            Height = height;
            Width = width;
            Name = name;
            SystemName = systemName;
        }
        public List<SystemSkelet> GetLives()
        {
            return _lives;
        }
        public SystemSkelet FindLives(int heightInd, int weightInd)
        {
            return _lives.Find(x => x.Cord.CompareTo((heightInd, weightInd)) == 0);
        }
        public bool IsCellBusy(int heightInd, int weightInd)
        {
            return IsBusy.Contains(new GamePoint(heightInd, weightInd));
        }
        public GamePoint GetGamePointTransitSpawn(string nameLoc)
        {
            if (_spawnGamePoint.ContainsKey(nameLoc))
            {
                int i = new Random().Next(0, _spawnGamePoint[nameLoc].Count);
                return _spawnGamePoint[nameLoc][i];
            }
            throw new CustomException($"Transit in Location {nameLoc} not find");
        }
        public string GetSystemNameLocTransitCell(int heinghtInd, int weightInd)
        {
            if (_transitGamePoint.ContainsKey(new GamePoint(heinghtInd, weightInd)))
            {
                return _transitGamePoint[new GamePoint(heinghtInd, weightInd)];
            }
            return null;
        }
        public bool IsDescriptionsCell(int heightInd, int wightInd, out string description)
        {
            description = null;
            foreach (var v in _cells[heightInd, wightInd])
            {
                bool Ok = Information.GetDescriptionToPicture(v.Item1.Picture(), out string currentdescription);
                if (Ok) description = currentdescription;
            }
            if (description == null)
                return false;
            else
                return true;
        }
        public bool GetIsBlockCell(int heightInd, int weightInd)
        {
            if (_cells[heightInd, weightInd] == null) return true;
            return _cells[heightInd, weightInd].IsBlock;
        }
        public bool GetIsWatchCell(int heightInd, int weightInd)
        {
            return _cells[heightInd, weightInd].IsWatch;
        }

        public void CreateDarkPictCell(string path)
        {
            if (DarkenPicCell.Taking() == null)
            {
                DarkenPicCell.Creating(path);
            }
        }
        //public void CreateShootPictCell(string path)
        //{
        //    if (ShootPicCell.Taking() == null)
        //    {
        //        ShootPicCell.Creating(path);
        //    }
        //}
        public void CreateGrafMove()
        {
            _grafLocToMove = new Graf();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (_cells[i, j].IsBlock)
                    {
                        continue;
                    }

                    var cur = new Vertex(new GamePoint(i, j));
                    _grafLocToMove.Vertexes.Add(cur);
                    if (i - 1 >= 0 && !_cells[i - 1, j].IsBlock)
                    {
                        var up = _grafLocToMove.Find(new GamePoint(i - 1, j));
                        up.Near.Add(cur);
                        cur.Near.Add(up);
                    }
                    if (j - 1 >= 0 && !_cells[i, j - 1].IsBlock)
                    {
                        var left = _grafLocToMove.Find(new GamePoint(i, j - 1));
                        left.Near.Add(cur);
                        cur.Near.Add(left);
                    }
                }
            }
        }
        public void CreateGrafAll()
        {
            _grafLocAll = new Graf();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var cur = new Vertex(new GamePoint(i, j));
                    _grafLocAll.Vertexes.Add(cur);
                    if (i - 1 >= 0)
                    {
                        var up = _grafLocAll.Find(new GamePoint(i - 1, j));
                        up.Near.Add(cur);
                        cur.Near.Add(up);
                    }
                    if (j - 1 >= 0)
                    {
                        var left = _grafLocAll.Find(new GamePoint(i, j - 1));
                        left.Near.Add(cur);
                        cur.Near.Add(left);
                    }
                }
            }
        }
        public void CreateGrafWatch()
        {
            _grafLocToWatch = new Graf();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (!_cells[i, j].IsWatch)
                    {
                        continue;
                    }

                    var cur = new Vertex(new GamePoint(i, j));
                    _grafLocToWatch.Vertexes.Add(cur);
                    if (i - 1 >= 0 && _cells[i - 1, j].IsWatch)
                    {
                        var up = _grafLocToWatch.Find(new GamePoint(i - 1, j));
                        up.Near.Add(cur);
                        cur.Near.Add(up);
                    }
                    if (j - 1 >= 0 && _cells[i, j - 1].IsWatch)
                    {
                        var left = _grafLocToWatch.Find(new GamePoint(i, j - 1));
                        left.Near.Add(cur);
                        cur.Near.Add(left);
                    }
                }
            }
        }

        public void UpdateDisplay()
        {
            _display.Update(_cells);
        }
        public void Display(Canvas canvas, double size, List<UIElement> systemObj)
        {
            foreach (var v in _cells)
            {
                v.ChangeHavingDark(false);
            }
            _display.Update(_cells);
            _display.Display(canvas, size, systemObj);
        }
        //public void DisplayPoints(SortedEnum<GamePoint> displayPoints, Canvas canvas, double size, List<UIElement> systemObj)
        //{
        //    _display.DisplayPoints(displayPoints, canvas, size, systemObj);
        //}
        //public void DisplayPointsForCorner(SortedEnum<GamePoint> displayPoints, GamePoint LeftUpCorner, Canvas canvas, double size, List<UIElement> systemObj)
        //{
        //    _display.DisplayPointsForCorner(displayPoints, LeftUpCorner, canvas, size, systemObj);
        //}
        public void DisplayPointsForCornerAndDark(CustomSortedEnum<GamePoint> displayPointsView, CustomSortedEnum<GamePoint> displayPointsAll, GamePoint LeftUpCorner, Canvas canvas, double size, List<UIElement> systemObj)
        {
            foreach (var v in displayPointsAll)
            {
                if (displayPointsView.Contains(v))
                {
                    _cells[(int)v.X, (int)v.Y].ChangeHavingDark(false);
                    _cells[(int)v.X, (int)v.Y].IsWasView = true;
                    _display.Update(_cells, (int)v.X, (int)v.Y);
                }
                else if (_cells[(int)v.X, (int)v.Y].IsWasView)
                {
                    displayPointsView.Add(v);
                    _cells[(int)v.X, (int)v.Y].ChangeHavingDark(true);
                    _display.Update(_cells, (int)v.X, (int)v.Y);
                }
            }
            _display.DisplayPointsForCorner(displayPointsView, LeftUpCorner, canvas, size, systemObj);
        }

        public void AddLocationCellsLayer(IPictureCell[,] cells, int index)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (cells[i, j] == null) continue;
                    _cells[i, j].AddCell(cells[i, j], index);
                }
            }
        }
        public void AddFourthLayerWithCell(IPictureCell cell, int heightInd, int weightInd)
        {
            AddCell(cell, 4, heightInd, weightInd);
        }
        public void AddSecondLayerWithCell(SystemSkelet skelet)
        {
            AddCell(skelet, 2, (int)skelet.Cord.X, (int)skelet.Cord.Y);
            if (skelet is Skelet)
                IsBusy.Add(new GamePoint(skelet.Cord.X, skelet.Cord.Y));
            _lives.Add(skelet);
        }
        public void AddFirstLayerWithCell(SystemSkelet skelet)
        {
            AddCell(skelet, 1, (int)skelet.Cord.X, (int)skelet.Cord.Y);
            if (skelet is Skelet)
                IsBusy.Add(new GamePoint(skelet.Cord.X, skelet.Cord.Y));
            _lives.Add(skelet);
        }
        public void RemoveFirstLayerWithCell(SystemSkelet skelet)
        {
            RemoveFirstLayerWithCell(skelet, (int)skelet.Cord.X, (int)skelet.Cord.Y);
            if (skelet is Skelet)
                IsBusy.Remove(new GamePoint(skelet.Cord.X, skelet.Cord.Y));
            _lives.Remove(skelet);
        }
        public void MoveFirstLayerWithCell(SystemSkelet skelet, GamePoint newPoint)
        {
            if (skelet is Skelet)
            {
                RemoveFirstLayerWithCell(skelet, (int)skelet.Cord.X, (int)skelet.Cord.Y);
                IsBusy.Remove(new GamePoint(skelet.Cord.X, skelet.Cord.Y));
                skelet.Cord = newPoint;
                AddCell(skelet, 1, (int)skelet.Cord.X, (int)skelet.Cord.Y);
                IsBusy.Add(new GamePoint(skelet.Cord.X, skelet.Cord.Y));
            }
            else
            {
                RemoveFirstLayerWithCell(skelet, (int)skelet.Cord.X, (int)skelet.Cord.Y);
                skelet.Cord = newPoint;
                AddCell(skelet, 1, (int)skelet.Cord.X, (int)skelet.Cord.Y);
            }
        }
        //public IEnumerable<IPictureCell> GetEnumerableCell(int heightInd, int weightInd)
        //{
        //    return _cells[heightInd, weightInd];
        //}
        private void AddCell(IPictureCell cell, int index, int heightInd, int weightInd)
        {
            if (index == 1)
                _cells[heightInd, weightInd].AddFirstLayerWithCell(cell);
            else
                _cells[heightInd, weightInd].AddCell(cell, index);
            _display.Update(_cells, heightInd, weightInd);
        }
        private void RemoveFirstLayerWithCell(IPictureCell cell, int heightInd, int weightInd)
        {
            _cells[heightInd, weightInd].RemoveFirstLayerWithCell(cell);
            _display.Update(_cells, heightInd, weightInd);
        }
        private void RemoveCell(IPictureCell cell, int heightInd, int weightInd)
        {
            _cells[heightInd, weightInd].RemoveCell(cell);
            _display.Update(_cells, heightInd, weightInd);
        }
    }
}
