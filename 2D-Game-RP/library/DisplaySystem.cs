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
        int _height;
        int _wight;
        int _maxdepth;
        Image[,,] _images;
        int[,] _depths;
        public DBDisplay(int Height, int Wight, int Depth)
        {
            _height = Height;
            _wight = Wight;
            _maxdepth = Depth;
            _images = new Image[Height, Wight, Depth];
            _depths = new int[Height, Wight];
        }
        public DBDisplay()
        {
            _height = 10;
            _wight = 10;
            _maxdepth = 10;
            _images = new Image[10, 10, 5];
            _depths = new int[10, 10];
        }
        private void Resize(int height, int wight, int depth)
        {
            int copyh = _height;
            int copyw = _wight;
            int copyd = _maxdepth;
            if (_height < height) _height = height;
            if (_wight < wight) _wight = wight;
            if (_maxdepth < depth) _maxdepth = depth;
            Image[,,] newImages = new Image[_height, _wight, _maxdepth];
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
            int depth = 0;
            foreach (var pict in cell[h, w])
            {
                if (depth + 1 > _maxdepth) Resize(_height, _wight, cell[h, w].Count);

                if (_images[h, w, depth] == null)
                {
                    _images[h, w, depth] = new Image()
                    {
                        Source = new BitmapImage(new Uri(pict.Picture(), UriKind.Relative)),
                    };
                    if (pict.Rotate90 || pict.Rotate180 || pict.Rotate270)
                        if (pict.Rotate90 || pict.Rotate180)
                            if (pict.Rotate90) _images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 90);
                            else _images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 180);
                        else _images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 270);
                    else _images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 0);
                }
                else if (((KeyValuePair<string, int>)_images[h, w, depth].Tag).Key != pict.Picture())
                {
                    _images[h, w, depth].Source = new BitmapImage(new Uri(pict.Picture(), UriKind.Relative));

                    if (pict.Rotate90 || pict.Rotate180 || pict.Rotate270)
                        if (pict.Rotate90 || pict.Rotate180)
                            if (pict.Rotate90) _images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 90);
                            else _images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 180);
                        else _images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 270);
                    else _images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 0);
                }
                depth++;
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
                    Update(cell, i, j);
                }
            }
        }
        private void DisplayImage(int i, int j, int k, double size, Canvas canvas, int shiftX, int shiftY)
        {
            _images[i, j, k].Width = size;
            _images[i, j, k].Height = size;
            Canvas.SetLeft(_images[i, j, k], size * (j - shiftY));
            Canvas.SetTop(_images[i, j, k], size * (i - shiftX));
            _images[i, j, k].RenderTransform = new RotateTransform(((KeyValuePair<string, int>)_images[i, j, k].Tag).Value, size / 2, size / 2);
            canvas.Children.Add(_images[i, j, k]);
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
            Display(canvas, size);
            foreach (var pair in Items)
            {
                foreach (var point in pair.Value)
                {
                    Image image = new Image()
                    {
                        Source = new BitmapImage(new Uri(pair.Key.Picture.Picture(), UriKind.Relative)),
                        Width = size,
                        Height = size
                    };
                    Canvas.SetLeft(image, size * point.Y);
                    Canvas.SetTop(image, size * point.X);
                    canvas.Children.Add(image);
                }
            }
        }
    }
}
