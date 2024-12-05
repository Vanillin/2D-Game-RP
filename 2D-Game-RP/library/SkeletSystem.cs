using System.Collections.Generic;
using System.Configuration;
using System.Windows.Controls;

namespace TwoD_Game_RP
{
    public enum NPSGroup
    {
        //Stalker,
        //Naemnik,
        //Mutant,
        People,
        Box,
        Door,
    }
    public enum NPSIntellect
    {
        Non, //стоит вкопанный не реагирует
        StandPassive, // стоит, при приближении уходит с линии огня
        StandAgressive, // стоит, при приближении подходит
        RandomPassive, // ещё и радномно
        RandomAgressive, //
    }
    public abstract class Skelet
    {
        private NPSGroup fraction;
        private NPSIntellect intellect;
        private DBDisplay InventoryDisplay;
        private BilateralQueue<IAction> GlobalActions;
        private BilateralQueue<IAction> Actions;
        public string Name { get; }
        public string SecondName { get; }
        public string SystemName { get; }
        public NPSGroup Fraction => fraction;
        public NPSIntellect Intellect => intellect;
        public GamePoint Cord { get; set; }
        public bool IsClarity { get; }
        public Inventory InventoryList { get; set; }
        public List<NPSGroup> FriendFranction { get; set; }
        public IPictureCell picture { get; set; }

        //public string FractionString()
        //{
        //    if (Fraction is NPSGroup.Stalker) return "Сталкер";
        //    else if (Fraction is NPSGroup.Naemnik) return "Наёмник";
        //    else if (Fraction is NPSGroup.Mutant) return "Мутант";
        //    else return "";
        //}
        //public Gun Gun;
        //public Cloth Cloth;
        //private int Health;
        //public int MaxHealth;
        //public int HealthInf() { return Health; }
        //public bool See;
        //public bool IsAlive;
        //public SortedEnum<GamePoint> OblSee = new SortedEnum<GamePoint>();
        //public SortedEnum<GamePoint> OblAttack = new SortedEnum<GamePoint>();
        //public Skelet LastSeeEnemy;
        //public GamePoint LastGoingPoint = new GamePoint(-1, -1);
        //public int Money;


        public Skelet(string name, string secondname, NPSGroup fraction, NPSIntellect intellect, GamePoint coord, char rotate,
            string systemname, List<Item> inventoryList, int inventoryHeight, int inventoryWidth, bool isClarity)
        {
            this.Name = name;
            this.SecondName = secondname;
            this.SystemName = systemname;
            this.intellect = intellect;
            this.Cord = coord;
            this.InventoryList = Inventory.Creating(inventoryHeight, inventoryWidth, inventoryList);
            ListLocationCell[,] cell = new ListLocationCell[inventoryHeight, inventoryWidth];
            this.IsClarity = isClarity;
            this.fraction = fraction;

            this.FriendFranction = new List<NPSGroup> { NPSGroup.People };
            this.InventoryDisplay = new DBDisplay();
            picture = new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{SystemName}-map.png"));
            GlobalActions = new BilateralQueue<IAction>();
            Actions = new BilateralQueue<IAction>();

            for (int i = 0; i < inventoryHeight; i++)
            {
                for (int j = 0; j < inventoryWidth; j++)
                {
                    cell[i, j] = new ListLocationCell();
                    cell[i, j].AddLocationCell(new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"emptyitem.png")), 0);
                }
            }
            this.InventoryDisplay.Update(cell);

            switch (rotate)
            {
                case '1': picture.Rotate90 = true; break;
                case '2': picture.Rotate180 = true; break;
                case '3': picture.Rotate270 = true; break;
            }
        }
        public void DisplayInventory(Canvas canvas, double size)
        {
            InventoryDisplay.DisplayInventory(canvas, size, InventoryList.ReferenceItem);
        }
        public IAction PeekGlobalAction()
        {
            if (GlobalActions.Count == 0) return null;
            return GlobalActions.Peek();
        }
        public void RemoveGlobalAction()
        {
            var v = GlobalActions.Dequeue();
            if (v.IsCycle)
            {
                GlobalActions.EnqueueInBack(v);
            }
        }
        public void ClearGlobalActions()
        {
            GlobalActions.Clear();
        }
        public void ClearActions()
        {
            Actions.Clear();
        }
        public void EnqueueUpGlobalAction(IAction action)
        {
            GlobalActions.EnqueueInFront(action);
        }
        public void EnqueueDownGlobalAction(IAction action)
        {
            GlobalActions.EnqueueInBack(action);
        }
        public IAction PeekAction()
        {
            if (Actions.Count == 0) return null;
            return Actions.Peek();
        }
        public void RemoveAction()
        {
            Actions.RemoveUp();
        }
        public void CreateActions(IEnumerable<IAction> actions)
        {
            foreach (var v in actions)
            {
                Actions.EnqueueInBack(v);
            }
        }

        //public Skelet(string name, string secondname, NPSGroup fraction, Gun gun, NPSIntellect intellect, GamePoint coord, char rotate,
        //    Cloth cloth, string systemname, int money, List<Item> inventoryList, List<NPSGroup> friendFranction, int maxHealth, bool isClarity)
        //{
        //    this.Name = name;
        //    this.SecondName = secondname;
        //    this.Fraction = fraction;
        //    this.Gun = gun;
        //    this.Intellect = intellect;
        //    this.Cord = coord;
        //    this.IsAlive = true;
        //    this.Health = maxHealth;
        //    //this.See = false;
        //    this.Cloth = cloth;
        //    this.SystemName = systemname;
        //    this.Money = money;
        //    this.InventoryList = Inventory.Creating(7, 4, inventoryList);
        //    this.InventoryDisplay = new DBDisplay();
        //    this.IsClarity = isClarity;
        //    ListLocationCell[,] cell = new ListLocationCell[7, 4];
        //    for (int i = 0; i < 7; i++)
        //    {
        //        for (int j = 0; j < 4; j++)
        //        {
        //            cell[i, j] = new ListLocationCell();
        //            cell[i, j].AddLocationCell(new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"emptyitem.png")), 0);
        //        }
        //    }
        //    this.InventoryDisplay.Update(cell);
        //    this.FriendFranction = friendFranction;
        //    this.MaxHealth = maxHealth;
        //    picture = new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{SystemName}-map.png"));
        //    switch (rotate)
        //    {
        //        case '1': picture.Rotate90 = true; break;
        //        case '2': picture.Rotate180 = true; break;
        //        case '3': picture.Rotate270 = true; break;
        //    }

        //    GlobalActions = new BilateralQueue<IAction>();
        //    Actions = new BilateralQueue<IAction>();
        //}

        //public void Damaging(int damage)
        //{
        //    if (!IsAlive) return;
        //    Health -= (damage - Cloth.Armor);
        //    if (Health <= 0)
        //    {
        //        Health = 0;
        //        Intellect = NPSIntellect.Non;
        //        picture = new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{SystemName}-mapdead.png"));
        //        IsAlive = false;
        //    }
        //}
        //public void Healthing(int health)
        //{
        //    if (health == MaxHealth) IsAlive = true;
        //    if (!IsAlive) return;
        //    Health += health;
        //    if (Health > MaxHealth)
        //    {
        //        Health = MaxHealth;
        //    }
        //}
        //public void Going(GamePoint point, Location location)
        //{
        //    Cord = point;
        //    foreach (Vertex vertex in location.GrafLocToWatch)
        //    {
        //        if (vertex.Cord == point)
        //        {
        //            if (this.FractionInf() == NPSGroup.Mutant)
        //            {
        //                OblSee = location.GrafLocToWatch.SearchSee(Cord, 11);
        //            }
        //            else
        //            {
        //                OblSee = location.GrafLocToWatch.SearchSee(Cord, 9);
        //            }
        //            OblAttack = location.GrafLocToWatch.SearchSee(Cord, Gun.Radius);
        //            return;
        //        }
        //    }
        //    OblSee = new SortedEnum<GamePoint>();
        //    OblAttack = new SortedEnum<GamePoint>();
        //    return;
        //}

        //public void TakeGun(Gun gun, Location location)
        //{
        //    if (Gun != null)
        //    {
        //        InventoryList.Add(Gun);
        //    }
        //    Gun = gun;
        //    OblAttack = location.GrafLocToWatch.SearchSee(Cord, Gun.Radius);
        //}
        //public void TakeCloth(Cloth cloth)
        //{
        //    if (Cloth != null)
        //    {
        //        InventoryList.Add(Cloth);
        //    }
        //    Cloth = cloth;
        //    Fraction = Cloth.FractionCloth;
        //}
    }
}
