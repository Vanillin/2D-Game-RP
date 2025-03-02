using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TwoD_Game_RP
{
    public class Location
    {
        private int _maxDepth;
        private Graf _grafLocToMove;
        private Graf _grafLocToWatch;
        private Graf _grafLocAll;
        private IPictureMapList[,] _cells;
        private List<SystemSkelet> _lives;
        private IDisplayCanvas _display;
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
            _cells = new IPictureMapList[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    _cells[i, j] = new MemoryPictureLocation();
                }
            }
            _grafLocToMove = null;
            _grafLocToWatch = null;
            _lives = new List<SystemSkelet>();
            _display = new MemoryImage(false, height, width, 6, compressH, compressW);

            _transitGamePoint = new CustomDictionary<GamePoint, string>();
            _spawnGamePoint = new CustomDictionary<string, List<GamePoint>>();
            AddTransitPoint(transitPoint);

            IsBusy = new CustomSortedEnum<GamePoint>();
            Height = height;
            Width = width;
            Name = name;
            SystemName = systemName;
        }
        public void AddTransitPoint(List<(string, GamePoint)> transitPoint)
        {
            foreach (var pair in transitPoint)
            {
                if (!_spawnGamePoint.ContainsKey(pair.Item1))
                    _spawnGamePoint.Add(pair.Item1, new List<GamePoint>());
                _spawnGamePoint[pair.Item1].Add(pair.Item2);
                if (_transitGamePoint.ContainsKey(pair.Item2))
                    throw new CustomException("Transit point has been create");
                _transitGamePoint.Add(pair.Item2, pair.Item1);
            }
        }
        public List<SystemSkelet> GetLives()
        {
            return _lives;
        }
        public SystemSkelet FindLives(int heightInd, int weightInd)
        {
            return _lives.Find(x => x.GPoint.CompareTo((heightInd, weightInd)) == 0);
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
        public string GetSystemNameLocTransitCell(GamePoint point)
        {
            if (_transitGamePoint.ContainsKey(point))
            {
                return _transitGamePoint[point];
            }
            return null;
        }
        public string GetSystemNameLocTransitCell(int heinghtInd, int weightInd)
        {
            return GetSystemNameLocTransitCell(new GamePoint(heinghtInd, weightInd));
        }
        public bool IsDescriptionsCell(int heightInd, int wightInd, out string description)
        {
            description = null;
            foreach (var v in _cells[heightInd, wightInd])
            {
                bool Ok = Information.GetDescriptionToPicture(v.Picture(), out string currentdescription);
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
        public void UpdateDisplay(int indexH, int indexW)
        {
            _display.UpdateCell(_cells[indexH, indexW], indexH, indexW, _cells[indexH, indexW].GetParameters());
        }
        public void UpdateDisplay()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    _display.UpdateCell(_cells[i, j], i, j, _cells[i, j].GetParameters());
                }
            }
        }
        public void Display(Canvas canvas, double size, List<UIElement> systemObj)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    _cells[i, j].ChangeHavingDark(false);
                    _display.UpdateCell(_cells[i, j], i, j, _cells[i, j].GetParameters());
                }
            }
            _display.Display(canvas, size);
            _display.AdditionDisplayUIElement(canvas, systemObj);
        }
        public void DisplayPointsForCornerAndDark(CustomSortedEnum<GamePoint> displayPointsView, CustomSortedEnum<GamePoint> displayPointsAll, GamePoint LeftUpCorner, Canvas canvas, double size, List<UIElement> systemObj)
        {
            canvas.Children.Clear();

            foreach (var v in displayPointsAll)
            {
                if (displayPointsView.Contains(v))
                {
                    _cells[v.X, v.Y].ChangeHavingDark(false);
                    _cells[v.X, v.Y].IsWasView = true;
                }
                else if (_cells[(int)v.X, (int)v.Y].IsWasView)
                {
                    displayPointsView.Add(v);
                    _cells[v.X, v.Y].ChangeHavingDark(true);
                }
                else
                {
                    continue;
                }
                _display.UpdateCell(_cells[v.X, v.Y], v.X, v.Y, _cells[v.X, v.Y].GetParameters());
                _display.AdditionDisplayCell(canvas, size, v.X, v.Y, LeftUpCorner.X, LeftUpCorner.Y);
            }
            _display.AdditionDisplayUIElement(canvas, systemObj);
        }

        public void AddLocationCellsLayer(IPicture[,] cells, int index)
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
        public void AddFourthLayerWithCell(IPicture cell, int heightInd, int weightInd)
        {
            AddCell(cell, 4, heightInd, weightInd);
        }
        public void AddSecondLayerWithCell(SystemSkelet skelet)
        {
            AddCell(skelet.GetPicture(), 2, (int)skelet.GPoint.X, (int)skelet.GPoint.Y);
            if (skelet is AliveSkelet)
                IsBusy.Add(new GamePoint(skelet.GPoint.X, skelet.GPoint.Y));
            _lives.Add(skelet);
        }
        public void AddFirstLayerWithCell(SystemSkelet skelet)
        {
            AddCell(skelet.GetPicture(), 1, (int)skelet.GPoint.X, (int)skelet.GPoint.Y);
            if (skelet is AliveSkelet)
                IsBusy.Add(new GamePoint(skelet.GPoint.X, skelet.GPoint.Y));
            _lives.Add(skelet);
        }
        public void RemoveFirstLayerWithCell(SystemSkelet skelet)
        {
            RemoveCell(skelet.GetPicture(), (int)skelet.GPoint.X, (int)skelet.GPoint.Y);
            if (skelet is AliveSkelet)
                IsBusy.Remove(new GamePoint(skelet.GPoint.X, skelet.GPoint.Y));
            _lives.Remove(skelet);
        }
        public void MoveFirstLayerWithCell(SystemSkelet skelet, GamePoint newPoint)
        {
            if (skelet is AliveSkelet)
            {
                RemoveCell(skelet.GetPicture(), (int)skelet.GPoint.X, (int)skelet.GPoint.Y);
                IsBusy.Remove(new GamePoint(skelet.GPoint.X, skelet.GPoint.Y));
                skelet.GPoint = newPoint;
                AddCell(skelet.GetPicture(), 1, (int)skelet.GPoint.X, (int)skelet.GPoint.Y);
                IsBusy.Add(new GamePoint(skelet.GPoint.X, skelet.GPoint.Y));
            }
            else
            {
                RemoveCell(skelet.GetPicture(), (int)skelet.GPoint.X, (int)skelet.GPoint.Y);
                skelet.GPoint = newPoint;
                AddCell(skelet.GetPicture(), 1, (int)skelet.GPoint.X, (int)skelet.GPoint.Y);
            }
        }
        private void AddCell(IPicture cell, int index, int heightInd, int weightInd)
        {
            _cells[heightInd, weightInd].AddCell(cell, index);
            _display.UpdateCell(_cells[heightInd, weightInd], heightInd, weightInd, _cells[heightInd, weightInd].GetParameters());
        }
        private void RemoveCell(IPicture cell, int heightInd, int weightInd)
        {
            _cells[heightInd, weightInd].RemoveCell(cell);
            _display.UpdateCell(_cells[heightInd, weightInd], heightInd, weightInd, _cells[heightInd, weightInd].GetParameters());
        }
    }
}
