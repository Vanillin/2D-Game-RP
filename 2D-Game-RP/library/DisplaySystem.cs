using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TwoD_Game_RP
{
    internal class DBDisplay
    {
        double _compressH;
        double _compressW;
        int _height;
        int _wight;
        int _maxdepth;
        (Image, bool)[,,] _images; //bool Is floor?
        int[,] _depths;
        public DBDisplay(int Height, int Wight, int Depth, double compressH, double compressW)
        {
            _height = Height;
            _wight = Wight;
            _maxdepth = Depth;
            _images = new (Image, bool)[Height, Wight, Depth];
            _depths = new int[Height, Wight];
            _compressH = compressH;
            _compressW = compressW;
        }
        public DBDisplay(double compressH, double compressW)
        {
            _height = 10;
            _wight = 10;
            _maxdepth = 10;
            _images = new (Image, bool)[10, 10, 5];
            _depths = new int[10, 10];
            _compressH = compressH;
            _compressW = compressW;
        }
        private void Resize(int height, int wight, int depth)
        {
            int copyh = _height;
            int copyw = _wight;
            int copyd = _maxdepth;
            if (_height < height) _height = height;
            if (_wight < wight) _wight = wight;
            if (_maxdepth < depth) _maxdepth = depth;
            (Image, bool)[,,] newImages = new (Image, bool)[_height, _wight, _maxdepth];
            _depths = new int[_height, _wight];

            for (int i = 0; i < copyh; i++)
            {
                for (int j = 0; j < copyw; j++)
                {
                    for (int k = 0; k < copyd; k++)
                    {
                        newImages[i, j, k] = _images[i, j, k];
                    }
                }
            }
            _images = newImages;
        }
        public void Update(LocationCell[,] cell, int h, int w)
        {
            UpdateCell(cell, h, w, out bool IsFloor);
            //while (!IsFloor)
            //{
            //    h++;
            //    if (h >= _height) return;
            //    UpdateCell(cell, h, w, out IsFloor);
            //}
        }
        private void UpdateCell(LocationCell[,] cell, int h, int w, out bool IsFloor)
        {
            int depth = 0;
            IsFloor = true;
            foreach (var pair in cell[h, w])
            {
                var pict = pair.Item1;
                if (depth + 1 > _maxdepth) Resize(_height, _wight, cell[h, w].Count);

                if (_images[h, w, depth].Item1 == null)
                {
                    _images[h, w, depth] = (new Image()
                    {
                        Source = new BitmapImage(new Uri(pict.Picture(), UriKind.Relative)),
                        Tag = new KeyValuePair<string, int>(pict.Picture(), pict.Rotate)
                    },
                    pair.Item2);
                }
                else if (((KeyValuePair<string, int>)_images[h, w, depth].Item1.Tag).Key != pict.Picture())
                {
                    _images[h, w, depth].Item1.Source = new BitmapImage(new Uri(pict.Picture(), UriKind.Relative));
                    _images[h, w, depth].Item1.Tag = new KeyValuePair<string, int>(pict.Picture(), pict.Rotate);
                    _images[h, w, depth].Item2 = pair.Item2;
                }
                depth++;
                if (IsFloor && !pair.Item2) IsFloor = false;
                if (depth >= cell[h, w].Count) break;
            }
            _depths[h, w] = depth;
        }
        public void Update(LocationCell[,] cell)
        {
            int listhei = cell.GetLength(0);
            int listwig = cell.GetLength(1);
            if (listhei > _height || listwig > _wight) Resize(listhei, listwig, _maxdepth);
            for (int i = 0; i < listhei; i++)
            {
                for (int j = 0; j < listwig; j++)
                {
                    UpdateCell(cell, i, j, out bool _);
                }
            }
        }
        private void DisplayImageInventory(int i, int j, int k, double size, Canvas canvas, int shiftX, int shiftY)
        {
            _images[i, j, k].Item1.Width = size * _compressW + 1;
            _images[i, j, k].Item1.Height = size * _compressH + 1;
            _images[i, j, k].Item1.Stretch = Stretch.Fill;
            Canvas.SetLeft(_images[i, j, k].Item1, size * _compressW * (j));
            Canvas.SetTop(_images[i, j, k].Item1, (size * _compressH) * (i));
            _images[i, j, k].Item1.RenderTransform = new RotateTransform(((KeyValuePair<string, int>)_images[i, j, k].Item1.Tag).Value, size / 2, size / 2);
            canvas.Children.Add(_images[i, j, k].Item1);
        }
        private void DisplayImage(int i, int j, int k, double size, Canvas canvas, int shiftX, int shiftY)
        {
            if (_images[i, j, k].Item2) //isFloor
            {
                _images[i, j, k].Item1.Width = size * _compressW + 1;
                _images[i, j, k].Item1.Height = size * _compressH + 1;
                _images[i, j, k].Item1.Stretch = Stretch.Fill;
                Canvas.SetLeft(_images[i, j, k].Item1, size * _compressW * (j - shiftY));
                Canvas.SetTop(_images[i, j, k].Item1, (size * _compressH) * (i - shiftX + 1));
                _images[i, j, k].Item1.RenderTransform = new RotateTransform(((KeyValuePair<string, int>)_images[i, j, k].Item1.Tag).Value, size / 2, size / 2);
                canvas.Children.Add(_images[i, j, k].Item1);
            }
            else
            {
                _images[i, j, k].Item1.Width = size * _compressW + 1;
                _images[i, j, k].Item1.Height = size * _compressH * 2 + 1;
                _images[i, j, k].Item1.Stretch = Stretch.Fill;
                Canvas.SetLeft(_images[i, j, k].Item1, size * _compressW * (j - shiftY));
                Canvas.SetTop(_images[i, j, k].Item1, (size * _compressH) * (i - shiftX));
                _images[i, j, k].Item1.RenderTransform = new RotateTransform(((KeyValuePair<string, int>)_images[i, j, k].Item1.Tag).Value, size / 2, size / 2);
                canvas.Children.Add(_images[i, j, k].Item1);
            }
        }
        private void DisplaySystemObj(List<UIElement> SystemObj, Canvas canvas)
        {
            if (SystemObj != null)
                foreach (var obj in SystemObj)
                    canvas.Children.Add(obj);
        }
        public void Display(Canvas canvas, double size)
        {
            canvas.Children.Clear();
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _wight; j++)
                {
                    for (int k = 0; k < _depths[i, j]; k++)
                    {
                        DisplayImage(i, j, k, size, canvas, 0, 0);
                    }
                }
            }
        }
        public void DisplayInventory(Canvas canvas, double size)
        {
            canvas.Children.Clear();
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _wight; j++)
                {
                    for (int k = 0; k < _depths[i, j]; k++)
                    {
                        DisplayImageInventory(i, j, k, size, canvas, 0, 0);
                    }
                }
            }
        }
        public void Display(Canvas canvas, double size, List<UIElement> SystemObj)
        {
            Display(canvas, size);
            DisplaySystemObj(SystemObj, canvas);
        }
        public void DisplayPoints(CustomSortedEnum<GamePoint> displayPoints, Canvas canvas, double size)
        {
            canvas.Children.Clear();
            foreach (var v in displayPoints)
            {
                int i = (int)v.X;
                int j = (int)v.Y;
                for (int k = 0; k < _depths[i, j]; k++)
                {
                    DisplayImage(i, j, k, size, canvas, 0, 0);
                }
            }
        }
        public void DisplayPoints(CustomSortedEnum<GamePoint> displayPoints, Canvas canvas, double size, List<UIElement> SystemObj)
        {
            DisplayPoints(displayPoints, canvas, size);
            DisplaySystemObj(SystemObj, canvas);
        }
        public void DisplayPointsForCorner(CustomSortedEnum<GamePoint> displayPoints, GamePoint LeftUpCorner, Canvas canvas, double size)
        {
            canvas.Children.Clear();
            foreach (var v in displayPoints)
            {
                int i = (int)v.X;
                int j = (int)v.Y;
                for (int k = 0; k < _depths[i, j]; k++)
                {
                    DisplayImage(i, j, k, size, canvas, LeftUpCorner.X, LeftUpCorner.Y);
                }
            }
        }
        public void DisplayPointsForCorner(CustomSortedEnum<GamePoint> displayPoints, GamePoint LeftUpCorner, Canvas canvas, double size, List<UIElement> SystemObj)
        {
            DisplayPointsForCorner(displayPoints, LeftUpCorner, canvas, size);
            DisplaySystemObj(SystemObj, canvas);
        }
        public void DisplayInventory(Canvas canvas, double size, CustomDictionary<Item, List<GamePoint>> Items)
        {
            DisplayInventory(canvas, size);
            foreach (var pair in Items)
            {
                foreach (var point in pair.Value)
                {
                    Image image = new Image()
                    {
                        Source = new BitmapImage(new Uri(pair.Key.Picture.Picture(), UriKind.Relative)),
                        Width = size * pair.Key.SizeW,
                        Height = size * pair.Key.SizeH
                    };
                    Canvas.SetLeft(image, size * point.Y);
                    Canvas.SetTop(image, size * point.X);
                    canvas.Children.Add(image);
                }
            }
        }
    }
}
