using Game_STALKER_Exclusion_Zone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
                        Tag = pict.Picture()
                    };
                }
                else if (images[h, w, depth].Tag.ToString() != pict.Picture())
                {
                    images[h, w, depth].Source = new BitmapImage(new Uri(pict.Picture(), UriKind.Relative));
                    images[h, w, depth].Tag = pict.Picture();
                }
                depth++;
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
        public void Display(Canvas canvas, double size)
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
                        canvas.Children.Add(images[i, j, k]);
                    }
                }
            }
        }
    }
}
