namespace TwoD_Game_RP
{
    public interface IPictureCell
    {
        bool Rotate90 { get; set; }
        bool Rotate180 { get; set; }
        bool Rotate270 { get; set; }
        string Picture();
        void NextPicture();
        void ReloadPictures();
    }
    internal class DarkenPicCell : IPictureCell
    {
        string _picture;
        private static DarkenPicCell _darken;
        public bool Rotate90 { get; set; }
        public bool Rotate180 { get; set; }
        public bool Rotate270 { get; set; }
        private DarkenPicCell(string picture)
        {
            _picture = picture;
            Rotate90 = false;
            Rotate180 = false;
            Rotate270 = false;
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
    public class StaticPicCell : IPictureCell
    {
        string _picture;
        public bool Rotate90 { get; set; }
        public bool Rotate180 { get; set; }
        public bool Rotate270 { get; set; }

        public StaticPicCell(string picture)
        {
            _picture = picture;
            Rotate90 = false;
            Rotate180 = false;
            Rotate270 = false;
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
