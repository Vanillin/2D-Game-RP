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
        Monster,
    }
    public enum NPSIntellect
    {
        Non, //стоит вкопанный не реагирует
        Attack
    }
    public class Door : SystemSkelet
    {
        public bool IsLock;
        public Door(string systemName, GamePoint coord, int rotate, bool isLock) :
            base(coord, rotate, systemName, false)
        {
            IsLock = isLock;
        }
    }
    public class SystemSkelet
    {
        internal CustomBilateralQueue<IAction> _globalActions;
        internal CustomBilateralQueue<IAction> _actions;
        public string SystemName { get; }
        public GamePoint Cord { get; set; }
        public bool IsClarity { get; }
        public IPictureCell Picture { get; set; }

        public SystemSkelet(GamePoint coord, int rotate, string systemname, bool isClarity)
        {
            SystemName = systemname;
            Cord = coord;
            IsClarity = isClarity;

            Picture = new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{SystemName}-map.png"));
            Picture.Rotate = rotate;
            _globalActions = new CustomBilateralQueue<IAction>();
            _actions = new CustomBilateralQueue<IAction>();
        }

        //=================================== Actions ====================================
        public virtual bool DoAction(Location location)
        {
            if (PeekGlobalAction() == null) return false;
            if (PeekAction() == null) CreateActions(PeekGlobalAction().CreateActions(this, location));

            if (!PeekAction().IsCanComplete(this, location))
            {
                ClearActions();
                CreateActions(PeekGlobalAction().CreateActions(this, location));
            }
            if (!PeekAction().IsCanComplete(this, location)) return false;

            PeekAction().CompleteAction(this, location);
            RemoveAction();
            while (PeekAction() != null && PeekAction().IsSystem)
            {
                PeekAction().CompleteAction(this, location);
                RemoveAction();
            }
            return true;
        }
        public void ClearGlobalActions() => _globalActions.Clear();
        public void ClearActions() => _actions.Clear();
        public void EnqueueUpGlobalAction(IAction action) => _globalActions.EnqueueInFront(action);
        public void EnqueueDownGlobalAction(IAction action) => _globalActions.EnqueueInBack(action);
        public void RemoveGlobalAction()
        {
            var v = _globalActions.Dequeue();
            if (v.IsCycle)
            {
                _globalActions.EnqueueInBack(v);
            }
        }
        private void RemoveAction() => _actions.RemoveUp();
        private void CreateActions(IEnumerable<IAction> actions)
        {
            foreach (var v in actions)
            {
                _actions.EnqueueInBack(v);
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
    }
    public interface IBoxSkelet
    {
        void DisplayInventory(Canvas canvas, double size);
        bool AddInBackpack(Item item);
        void RemoveInBackpack(Item item);
        bool ContainsInBackpack(Item item);
        Item SearchInBackpack(int h, int w);
    }
    public class Box : SystemSkelet, IBoxSkelet
    {
        private DBDisplay _inventoryDisplay;
        private Inventory _inventory;
        public Box(GamePoint coord, int rotate, string systemname, bool isClarity, List<Item> inventoryList, int inventoryHeight, int inventoryWidth)
            : base(coord, rotate, systemname, isClarity)
        {
            _inventory = new Inventory(inventoryHeight, inventoryWidth, inventoryList);
            _inventoryDisplay = new DBDisplay(1, 1);
        }

        //=================================== method Inventory ====================================
        public void DisplayInventory(Canvas canvas, double size) => _inventoryDisplay.DisplayInventory(canvas, size, _inventory.ReferenceItem);
        public bool AddInBackpack(Item item) => _inventory.AddInBackpack(item);
        public void RemoveInBackpack(Item item) => _inventory.RemoveInBackpack(item);
        public bool ContainsInBackpack(Item item) => _inventory.ContainsInBackpack(item);
        public Item SearchInBackpack(int h, int w) => _inventory.SearchInBackpack(h, w);
    }
    public class Skelet : SystemSkelet, IBoxSkelet
    {
        private NPSGroup _fraction;
        private NPSIntellect _intellect;
        private DBDisplay _inventoryDisplay;
        private Inventory _inventory;
        private HealthSkelet _health;
        public string Name { get; }
        public string SecondName { get; }
        public NPSGroup Fraction => _fraction;
        public NPSIntellect Intellect => _intellect;
        public List<NPSGroup> FriendFranction { get; set; }

        internal Skelet(string name, string secondname, NPSGroup fraction, NPSIntellect intellect, GamePoint coord, int rotate,
            string systemname, List<Item> inventoryList, int inventoryHeight, int inventoryWidth, bool isClarity, int health)
            : base(coord, rotate, systemname, isClarity)
        {
            Name = name;
            SecondName = secondname;
            _intellect = intellect;
            _inventory = new Inventory(inventoryHeight, inventoryWidth, inventoryList);
            LocationCell[,] cell = new LocationCell[inventoryHeight, inventoryWidth];
            _fraction = fraction;
            _health = new HealthSkelet(health, health);

            FriendFranction = GetFriendFraction(fraction);
            _inventoryDisplay = new DBDisplay(1, 1);

            for (int i = 0; i < inventoryHeight; i++)
            {
                for (int j = 0; j < inventoryWidth; j++)
                {
                    cell[i, j] = new LocationCell();
                    cell[i, j].AddCell(new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"emptyitem.png")), 0);
                }
            }
            this._inventoryDisplay.Update(cell);
        }
        private List<NPSGroup> GetFriendFraction(NPSGroup fraction)
        {
            switch (fraction)
            {
                case NPSGroup.Monster: return new List<NPSGroup>() { NPSGroup.Monster };
                default: return new List<NPSGroup>() { NPSGroup.People };
            }
        }
        //=================================== method Inventory ====================================
        public void DisplayInventory(Canvas canvas, double size) => _inventoryDisplay.DisplayInventory(canvas, size, _inventory.ReferenceItem);
        public void ChangeGunInHandAndInBack() => _inventory.ChangeGunInHandAndInBack();
        public Gun GetGunInHand() => _inventory.GetGunInHand();
        public bool GiveGunInHand(Gun gun) => _inventory.GiveGunInHand(gun);
        public bool TakeAwayGunInHand() => _inventory.TakeAwayGunInHand();
        public void DropGunInHand() => _inventory.DropGunInHand();
        public Gun GetGunInBack() => _inventory.GetGunInBack();
        public bool GiveGunInBack(Gun gun) => _inventory.GiveGunInBack(gun);
        public bool TakeAwayGunInBack() => _inventory.TakeAwayGunInBack();
        public void DropGunInBack() => _inventory.DropGunInBack();
        public bool AddInBackpack(Item item) => _inventory.AddInBackpack(item);
        public void RemoveInBackpack(Item item) => _inventory.RemoveInBackpack(item);
        public bool ContainsInBackpack(Item item) => _inventory.ContainsInBackpack(item);
        public Item SearchInBackpack(int h, int w) => _inventory.SearchInBackpack(h, w);

        //================================== method Health ==============================
        public bool IsAlive => _health.IsAlive;
        public int Health => _health.Health;
        public double HealthPercent => _health.HealthPercent;
        public void MinusHealth(int health) => _health.MinusHealth(health);
        public void PlusHealth(int health) => _health.PlusHealth(health);
    }
}
