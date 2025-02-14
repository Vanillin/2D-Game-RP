namespace TwoD_Game_RP
{
    public abstract class DoorSkelet : SystemSkelet
    {
        private bool _isLock;
        private ISomePicture _doorPicture;
        public bool IsLock => _isLock;
        public void ChangeIndexPicture(int index) => _doorPicture.ChangeIndexPicture(index);
        public bool Open()
        {
            if (_isLock) return false;
            else
            {
                IsClarity = true;
                ChangeIndexPicture(0);
                return true;
            }
        }
        public void Close()
        {
            IsClarity = false;
            ChangeIndexPicture(1);
        }
        public void Lock()
        {
            Close();
            _isLock = true;
        }
        public void UnLock()
        {
            _isLock = false;
        }

        internal DoorSkelet(string systemName, ISomePicture doorPicture, GamePoint point, bool isClarity, IMemoryAction memoryAction)
            : base(systemName, doorPicture, point, isClarity, memoryAction)
        {
            _doorPicture = doorPicture;
        }

        //public DoorSkelet(string systemNamePicture, GamePoint point, bool isClarity, IMemoryAction memoryAction)
        //    : base(systemNamePicture,
        //          new AlivePicture(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{systemNamePicture}-map.png"),
        //          System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{systemNamePicture}-map-dead.png")),
        //    point, isClarity, memoryAction)
        //{
        //    _doorPicture = new AlivePicture(
        //        System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{systemNamePicture}-map.png"),
        //        System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{systemNamePicture}-map-dead.png"));
        //}
    }
}
