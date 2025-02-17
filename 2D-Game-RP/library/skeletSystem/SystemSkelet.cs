using System.Configuration;

namespace TwoD_Game_RP
{
    public abstract class SystemSkelet
    {
        private IMemoryAction _memoryAction;
        protected IPicture _picture;
        public string SystemName { get; }
        public GamePoint GPoint { get; set; }
        public bool IsClarity { get; set; }
        public int Rotate
        {
            get { return _picture.Rotate; }
            set { _picture.Rotate = value; }
        }
        internal IPicture GetPicture() => _picture;
        public string Picture() => _picture.Picture();
        public void NextPicture() => _picture.NextPicture();
        public void ReloadPictures() => _picture.ReloadPictures();
        public bool IsEmptyAction() => _memoryAction.IsEmptyAction();
        public bool DoAction(Location location) => _memoryAction.DoAction(this, location);
        public void RemoveGlobalAction() => _memoryAction.RemoveGlobalAction();
        public void EnqueueUpGlobalAction(IAction action) => _memoryAction.EnqueueDownGlobalAction(action);
        public void EnqueueDownGlobalAction(IAction action) => _memoryAction.EnqueueDownGlobalAction(action);
        public void ClearGlobalActions() => _memoryAction.ClearGlobalActions();
        public void ClearActions() => _memoryAction.ClearActions();

        internal SystemSkelet(string systemName, IPicture picture, int rotate, GamePoint point, bool isClarity, IMemoryAction memoryAction)
        {
            SystemName = systemName;
            _picture = picture;
            _picture.Rotate = rotate;
            GPoint = point;
            IsClarity = isClarity;
            _memoryAction = memoryAction;
        }
        internal SystemSkelet(string systemName, IPicture picture, GamePoint point, bool isClarity, IMemoryAction memoryAction)
        {
            SystemName = systemName;
            _picture = picture;
            GPoint = point;
            IsClarity = isClarity;
            _memoryAction = memoryAction;
        }
        internal SystemSkelet(string systemNamePicture, int rotate, GamePoint point, bool isClarity, IMemoryAction memoryAction)
        {
            SystemName = systemNamePicture;
            _picture = new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{SystemName}-map.png"));
            _picture.Rotate = rotate;
            GPoint = point;
            IsClarity = isClarity;
            _memoryAction = memoryAction;
        }
        internal SystemSkelet(string systemNamePicture, GamePoint point, bool isClarity, IMemoryAction memoryAction)
        {
            SystemName = systemNamePicture;
            _picture = new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{SystemName}-map.png"));
            GPoint = point;
            IsClarity = isClarity;
            _memoryAction = memoryAction;
        }
    }

    public class SimpleElement : SystemSkelet
    {
        public SimpleElement(string systemNamePicture, GamePoint point, bool isClarity)
            : base(systemNamePicture, point, isClarity, new MemoryAction())
        { }
        internal SimpleElement(string systemName, IPicture picture, GamePoint point, bool isClarity)
            : base(systemName, picture, point, isClarity, new MemoryAction())
        { }
    }
}
