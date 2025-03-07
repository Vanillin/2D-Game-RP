﻿namespace TwoD_Game_RP
{
    public interface IPicture
    {
        int Rotate { get; set; }
        string Picture();
        void NextPicture();
        void ReloadPictures();
    }
    internal class DarkenPicCell : IPicture
    {
        string _picture;
        private static DarkenPicCell _darken;
        public int Rotate { get; set; }

        private DarkenPicCell(string picture)
        {
            _picture = picture;
        }
        public static DarkenPicCell Creating(string picture)
        {
            if (_darken == null)
            {
                _darken = new DarkenPicCell(picture);
            }
            return _darken;
        }
        public static DarkenPicCell Taking()
        {
            return _darken;
        }

        public void NextPicture()
        { }
        public string Picture()
        {
            return _picture;
        }
        public void ReloadPictures()
        { }
    }
    public class StaticPicCell : IPicture
    {
        string _picture;
        public int Rotate { get; set; }

        public StaticPicCell(string picture)
        {
            _picture = picture;
        }

        public void NextPicture()
        { }
        public string Picture()
        {
            return _picture;
        }
        public void ReloadPictures()
        { }
    }

    //public class AnimatedPicCell : IPictureCell
    //{
    //    string[] picture;
    //    int index;
    //    public int Count => picture.Length;
    //    public AnimatedPicCell(string[] picture)
    //    {
    //        this.picture = picture;
    //        index = 0;
    //    }

    //    public void NextPicture()
    //    {
    //        index = (index + 1) % picture.Length;
    //    }
    //    public string Picture()
    //    {
    //        return picture[index];
    //    }
    //    public void ReloadPictures()
    //    {
    //        index = 0;
    //    }
    //}

    //public class RandomPicCell : IPictureCell
    //{
    //    string picture;
    //    public RandomPicCell(Random rd, string[] picture)
    //    {
    //        int index = rd.Next(picture.Length);
    //        this.picture = picture[index];
    //    }
    //    public void NextPicture()
    //    {        }
    //    public string Picture()
    //    {
    //        return picture;
    //    }
    //    public void ReloadPictures()
    //    {        }
    //}
}
