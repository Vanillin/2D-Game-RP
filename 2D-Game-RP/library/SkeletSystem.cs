using System.Collections.Generic;
using System.Configuration;
using System.Windows.Controls;

namespace TwoD_Game_RP
{
    public enum NPSGroup
    {
        People,
        Box,
        Door,
    }
    public enum NPSIntellect
    {
        Non //стоит вкопанный не реагирует
    }
    public abstract class Door : Skelet
    {
        public bool IsLock;
        public Door(string name, string systemName, GamePoint coord, char rotate, bool isLock) :
            base(name, "", NPSGroup.Door, NPSIntellect.Non, coord, rotate, systemName, new List<Item>(0), 0, 0, false)
        {
            IsLock = isLock;
        }
    }
    public abstract class Skelet
    {
        private NPSGroup _fraction;
        private NPSIntellect _intellect;
        private DBDisplay _inventoryDisplay;
        private CustomBilateralQueue<IAction> _globalActions;
        private CustomBilateralQueue<IAction> _actions;
        private Inventory _inventoryList;
        public string Name { get; }
        public string SecondName { get; }
        public string SystemName { get; }
        public NPSGroup Fraction => _fraction;
        public NPSIntellect Intellect => _intellect;
        public GamePoint Cord { get; set; }
        public bool IsClarity { get; }
        public List<NPSGroup> FriendFranction { get; set; }
        public IPictureCell Picture { get; set; }

        public Skelet(string name, string secondname, NPSGroup fraction, NPSIntellect intellect, GamePoint coord, char rotate,
            string systemname, List<Item> inventoryList, int inventoryHeight, int inventoryWidth, bool isClarity)
        {
            Name = name;
            SecondName = secondname;
            SystemName = systemname;
            _intellect = intellect;
            Cord = coord;
            _inventoryList = Inventory.Creating(inventoryHeight, inventoryWidth, inventoryList);
            LocationCell[,] cell = new LocationCell[inventoryHeight, inventoryWidth];
            IsClarity = isClarity;
            _fraction = fraction;

            FriendFranction = new List<NPSGroup> { NPSGroup.People };
            _inventoryDisplay = new DBDisplay();
            Picture = new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{SystemName}-map.png"));
            _globalActions = new CustomBilateralQueue<IAction>();
            _actions = new CustomBilateralQueue<IAction>();

            for (int i = 0; i < inventoryHeight; i++)
            {
                for (int j = 0; j < inventoryWidth; j++)
                {
                    cell[i, j] = new LocationCell();
                    cell[i, j].AddCell(new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"emptyitem.png")), 0);
                }
            }
            this._inventoryDisplay.Update(cell);

            switch (rotate)
            {
                case '1': Picture.Rotate90 = true; break;
                case '2': Picture.Rotate180 = true; break;
                case '3': Picture.Rotate270 = true; break;
            }
        }
        public Item InventSearchItem(int H, int W)
        {
            return _inventoryList.SearchItem(H, W);
        }
        public bool InventAdd(Item item)
        {
            return _inventoryList.Add(item);
        }
        public void InventRemove(Item item)
        {
            _inventoryList.Remove(item);
        }
        public bool InventContains(Item item)
        {
            return _inventoryList.Contains(item);
        }

        public void DisplayInventory(Canvas canvas, double size)
        {
            _inventoryDisplay.DisplayInventory(canvas, size, _inventoryList.ReferenceItem);
        }
        public void DoAction(Location location)
        {
            if (PeekGlobalAction() == null) return;
            if (PeekAction() == null) CreateActions(PeekGlobalAction().CreateActions(this, location));

            if (!PeekAction().IsCanComplete(this, location))
            {
                ClearActions();
                CreateActions(PeekGlobalAction().CreateActions(this, location));
            }
            if (!PeekAction().IsCanComplete(this, location)) return;

            PeekAction().CompleteAction(this, location);
            RemoveAction();
            while (PeekAction() != null && PeekAction().IsSystem)
            {
                PeekAction().CompleteAction(this, location);
                RemoveAction();
            }
        }
        private IAction PeekGlobalAction()
        {
            if (_globalActions.Count == 0) return null;
            return _globalActions.Peek();
        }
        private IAction PeekAction()
        {
            if (_actions.Count == 0) return null;
            return _actions.Peek();
        }
        private void RemoveAction()
        {
            _actions.RemoveUp();
        }
        private void CreateActions(IEnumerable<IAction> actions)
        {
            foreach (var v in actions)
            {
                _actions.EnqueueInBack(v);
            }
        }

        public void RemoveGlobalAction()
        {
            var v = _globalActions.Dequeue();
            if (v.IsCycle)
            {
                _globalActions.EnqueueInBack(v);
            }
        }
        public void ClearGlobalActions()
        {
            _globalActions.Clear();
        }
        public void ClearActions()
        {
            _actions.Clear();
        }
        public void EnqueueUpGlobalAction(IAction action)
        {
            _globalActions.EnqueueInFront(action);
        }
        public void EnqueueDownGlobalAction(IAction action)
        {
            _globalActions.EnqueueInBack(action);
        }
    }
}
