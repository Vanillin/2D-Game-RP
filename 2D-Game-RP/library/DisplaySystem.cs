using Game_STALKER_Exclusion_Zone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TwoD_Game_RP.library
{
    internal class DBDisplay
    {
        int height;
        int wight;
        int maxdepth;
        Image[,,] images;
        int[,] depths;
        public DBDisplay(int Height, int Wight, int Depth)
        {
            this.height = Height;
            this.wight = Wight;
            this.maxdepth = Depth;
            images = new Image[Height, Wight, Depth];
            depths = new int[Height, Wight];
        }
        public DBDisplay()
        {
            this.height = 10;
            this.wight = 10;
            this.maxdepth = 10;
            images = new Image[10, 10, 5];
            depths = new int[10, 10];
        }
        private void Resize(int height, int wight, int depth)
        {
            int copyh = this.height;
            int copyw = this.wight;
            int copyd = this.maxdepth;
            if (this.height < height) this.height = height;
            if (this.wight < wight) this.wight = wight;
            if (this.maxdepth < depth) this.maxdepth = depth;
            Image[,,] newImages = new Image[this.height, this.wight, this.maxdepth];
            depths = new int[this.height, this.wight];

            for (int i = 0; i < copyh; i++)
            {
                for (int j = 0; j < copyw; j++)
                {
                    for (int k = 0; k < copyd; k++)
                    {
                        newImages[i,j,k] = images[i,j,k];
                    }
                }
            }
            images = newImages;
        }
        public void Update(ListLocationCell[,] cell, int h, int w)
        {
            int depth = 0;
            foreach (var pict in cell[h, w])
            {
                if (depth + 1 > this.maxdepth) Resize(this.height, this.wight, cell[h, w].Count);

                if (images[h, w, depth] == null)
                {
                    images[h, w, depth] = new Image()
                    {
                        Source = new BitmapImage(new Uri(pict.Picture(), UriKind.Relative)),
                    };
                    if (pict.Rotate90 || pict.Rotate180 || pict.Rotate270)
                        if (pict.Rotate90 || pict.Rotate180)
                            if (pict.Rotate90) images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 90);
                            else images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 180);
                        else images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 270);
                    else images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 0);
                }
                else if (( (KeyValuePair<string, int>) images[h, w, depth].Tag ).Key != pict.Picture())
                {
                    images[h, w, depth].Source = new BitmapImage(new Uri(pict.Picture(), UriKind.Relative));

                    if (pict.Rotate90 || pict.Rotate180 || pict.Rotate270)
                        if (pict.Rotate90 || pict.Rotate180)
                            if (pict.Rotate90) images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 90);
                            else images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 180);
                        else images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 270);
                    else images[h, w, depth].Tag = new KeyValuePair<string, int>(pict.Picture(), 0);
                }
                depth++;
                if (depth > cell[h, w].Count) break;
            }
            depths[h, w] = depth;
        }
        public void Update(ListLocationCell[,] cell)
        {
            int listhei = cell.GetLength(0);
            int listwig = cell.GetLength(1);
            if (listhei > height || listwig > wight) Resize(listhei, listwig, maxdepth);
            for (int i = 0; i < listhei; i++)
            {
                for (int j = 0; j < listwig; j++)
                {
                    Update(cell, i, j);
                }
            }
        }
        public void Display(Canvas canvas, double size, List<UIElement> SystemObj)
        {
            canvas.Children.Clear();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < wight; j++)
                {
                    for (int k = 0; k < depths[i, j]; k++)
                    {
                        images[i,j,k].Width = size;
                        images[i,j,k].Height = size;
                        Canvas.SetLeft(images[i, j, k], size + size * j);
                        Canvas.SetTop(images[i, j, k], size + size * i);
                        images[i, j, k].RenderTransform = new RotateTransform(  ((KeyValuePair<string, int>)images[i,j,k].Tag).Value  , size/2, size/2);
                        canvas.Children.Add(images[i, j, k]);
                    }
                }
            }
            if (SystemObj != null)
                foreach (var obj in SystemObj)
                    canvas.Children.Add(obj);
        }
        public void DisplayToPoints(List<Point> displayPoints, Canvas canvas, double size, List<UIElement> SystemObj)
        {
            canvas.Children.Clear();
            foreach (var v in displayPoints)
            {
                int i = (int)v.X;
                int j = (int)v.Y;
                for (int k = 0; k < depths[i, j]; k++)
                {
                    images[i, j, k].Width = size;
                    images[i, j, k].Height = size;
                    Canvas.SetLeft(images[i, j, k], size + size * j);
                    Canvas.SetTop(images[i, j, k], size + size * i);
                    images[i, j, k].RenderTransform = new RotateTransform(((KeyValuePair<string, int>)images[i, j, k].Tag).Value, size / 2, size / 2);
                    canvas.Children.Add(images[i, j, k]);
                }
            }
            if (SystemObj != null)
                foreach (var obj in SystemObj)
                    canvas.Children.Add(obj);
        }
        public void DisplayToPointsWithBorder(List<Point> displayPoints, Point LeftUpCorner, Canvas canvas, double size, List<UIElement> SystemObj)
        {
            canvas.Children.Clear();
            foreach (var v in displayPoints)
            {
                int i = (int)v.X;
                int j = (int)v.Y;
                for (int k = 0; k < depths[i, j]; k++)
                {
                    images[i, j, k].Width = size;
                    images[i, j, k].Height = size;
                    Canvas.SetLeft(images[i, j, k], size + size * (j - LeftUpCorner.Y));
                    Canvas.SetTop(images[i, j, k], size + size * (i - LeftUpCorner.X));
                    images[i, j, k].RenderTransform = new RotateTransform(((KeyValuePair<string, int>)images[i, j, k].Tag).Value, size / 2, size / 2);
                    canvas.Children.Add(images[i, j, k]);
                }
            }
            if (SystemObj != null)
                foreach (var obj in SystemObj)
                    canvas.Children.Add(obj);
        }
    }
}
