using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TwoD_Game_RP
{
    internal interface IDisplayCanvas
    {
        void UpdateCell(IPictureList pictures, int indexh, int indexw, (double sizeh, double sizew)[] sizes);
        void Update(IPictureList[,] cell, (double sizeh, double sizew)[,][] sizes);
        void Display(Canvas canvas, double size);
        void AdditionDisplayCell(Canvas canvas, double size, int indexh, int indexw, int shiftH, int shiftW);
        void AdditionDisplayUIElement(Canvas canvas, List<UIElement> systemObj);
    }
    internal class MemoryImage : IDisplayCanvas
    {
        double _compressH;
        double _compressW;
        int _height;
        int _wight;
        int _maxdepth;
        (Image image, double sizeh, double sizew)[,,] _images;
        int[,] _depths;
        public MemoryImage(int Height, int Wight, int Depth, double compressH, double compressW)
        {
            _height = Height;
            _wight = Wight;
            _maxdepth = Depth;
            _compressH = compressH;
            _compressW = compressW;
            _images = new (Image, double, double)[_height, _wight, _maxdepth];
            _depths = new int[_height, _wight];
        }
        public MemoryImage(double compressH, double compressW)
        {
            _height = 10;
            _wight = 10;
            _maxdepth = 5;
            _compressH = compressH;
            _compressW = compressW;
            _images = new (Image, double, double)[_height, _wight, _maxdepth];
            _depths = new int[_height, _wight];
        }
        private void Resize(int height, int wight, int depth)
        {
            int copyh = _height;
            int copyw = _wight;
            int copyd = _maxdepth;
            if (_height < height) _height = height;
            if (_wight < wight) _wight = wight;
            if (_maxdepth < depth) _maxdepth = depth;
            (Image, double, double)[,,] newImages = new (Image, double, double)[_height, _wight, _maxdepth];
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
        public void UpdateCell(IPictureList pictures, int indexh, int indexw, (double sizeh, double sizew)[] sizes)
        {
            int depth = 0;
            foreach (var pair in pictures)
            {
                var pict = pair;
                if (depth + 1 > _maxdepth) Resize(_height, _wight, pictures.Count);

                if (_images[indexh, indexw, depth].image == null)
                {
                    _images[indexh, indexw, depth] = (new Image()
                    {
                        Source = new BitmapImage(new Uri(pict.Picture(), UriKind.Relative)),
                        Tag = new KeyValuePair<string, int>(pict.Picture(), pict.Rotate)
                    }, sizes[depth].sizeh, sizes[depth].sizew);
                }
                else if (((KeyValuePair<string, int>)_images[indexh, indexw, depth].image.Tag).Key != pict.Picture())
                {
                    _images[indexh, indexw, depth].image.Source = new BitmapImage(new Uri(pict.Picture(), UriKind.Relative));
                    _images[indexh, indexw, depth].image.Tag = new KeyValuePair<string, int>(pict.Picture(), pict.Rotate);
                    _images[indexh, indexw, depth].sizeh = sizes[depth].sizeh;
                    _images[indexh, indexw, depth].sizew = sizes[depth].sizew;
                }
                depth++;
                if (depth >= pictures.Count) break;
            }
            _depths[indexh, indexw] = depth;
        }
        public void Update(IPictureList[,] cell, (double sizeh, double sizew)[,][] sizes)
        {
            int listhei = cell.GetLength(0);
            int listwig = cell.GetLength(1);
            if (listhei > _height || listwig > _wight) Resize(listhei, listwig, _maxdepth);
            for (int i = 0; i < listhei; i++)
            {
                for (int j = 0; j < listwig; j++)
                {
                    UpdateCell(cell[i, j], i, j, sizes[i, j]);
                }
            }
        }
        private void DisplayCell(Canvas canvas, double size, int i, int j, int k, int positionH, int positionW)
        {
            int shiftSizeImage = 1;
            if (_images[i, j, k].sizeh == 2)
                positionH -= 1;
            _images[i, j, k].image.Width = size * _compressW * _images[i, j, k].sizew + shiftSizeImage;
            _images[i, j, k].image.Height = size * _compressH * _images[i, j, k].sizeh + shiftSizeImage;
            _images[i, j, k].image.Stretch = Stretch.Fill;
            Canvas.SetLeft(_images[i, j, k].image, size * _compressW * positionW);
            Canvas.SetTop(_images[i, j, k].image, size * _compressH * positionH);
            _images[i, j, k].image.RenderTransform = new RotateTransform(((KeyValuePair<string, int>)_images[i, j, k].image.Tag).Value, size * _images[i, j, k].sizew / 2, size * _images[i, j, k].sizeh / 2);
            canvas.Children.Add(_images[i, j, k].image);
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
                        DisplayCell(canvas, size, i, j, k, 0, 0);
                    }
                }
            }
        }
        public void AdditionDisplayCell(Canvas canvas, double size, int indexh, int indexw, int shiftH, int shiftW)
        {
            for (int k = 0; k < _depths[indexh, indexw]; k++)
            {
                DisplayCell(canvas, size, indexh, indexw, k, indexh - shiftH, indexw - shiftW);
            }
        }
        public void AdditionDisplayUIElement(Canvas canvas, List<UIElement> systemObj)
        {
            if (systemObj != null)
                foreach (var obj in systemObj)
                    canvas.Children.Add(obj);
        }
    }
}
