using System.Configuration;

namespace TwoD_Game_RP
{
    internal interface ISomePicture : IPicture
    {
        void ChangeIndexPicture(int index);
    }
    public class AlivePicture : ISomePicture
    {
        public int Rotate { get; set; }

        int _indexPicture;
        string _pictureLive;
        string _pictureDead;

        public AlivePicture(string pictureLive, string pictureDead)
        {
            _pictureLive = pictureLive;
            _pictureDead = pictureDead;
            _indexPicture = 0;
        }

        public void NextPicture()
        { }
        public string Picture()
        {
            if (_indexPicture == 0)
                return _pictureLive;
            else
                return _pictureDead;
        }
        public void ReloadPictures()
        { }

        public void ChangeIndexPicture(int index)
        {
            switch (index)
            {
                case 0: _indexPicture = 0; break;
                case 1: _indexPicture = 1; break;
                default: throw new CustomException("Index is not correct");
            }
        }
    }
    public class DoorPicture : ISomePicture
    {
        public int Rotate { get; set; }

        int _indexPicture;
        string _pictureOpen;
        string _pictureClose;

        public DoorPicture(string pictureOpen, string pictureClose)
        {
            _pictureOpen = pictureOpen;
            _pictureClose = pictureClose;
            _indexPicture = 0;
        }

        public void NextPicture()
        { }
        public string Picture()
        {
            if (_indexPicture == 0)
                return _pictureOpen;
            else
                return _pictureClose;
        }
        public void ReloadPictures()
        { }

        public void ChangeIndexPicture(int index)
        {
            switch (index)
            {
                case 0: _indexPicture = 0; break;
                case 1: _indexPicture = 1; break;
                default: throw new CustomException("Index is not correct");
            }
        }
    }
    public class MapPicture : ISomePicture
    {
        public int Rotate { get; set; }

        int _indexPicture;
        string _picture;
        string _pictureDark;

        public MapPicture(string picture)
        {
            _picture = picture;
            _pictureDark = picture.Substring(0, picture.Length - 4) + "_sD_.png";
            _indexPicture = 0;
        }

        public void NextPicture()
        { }
        public string Picture()
        {
            if (_indexPicture == 0)
                return _picture;
            else
                return _pictureDark;
        }
        public void ReloadPictures()
        { }

        public void ChangeIndexPicture(int index)
        {
            switch (index)
            {
                case 0: _indexPicture = 0; break;
                case 1: _indexPicture = 1; break;
                default: throw new CustomException("Index is not correct");
            }
        }
    }
}
