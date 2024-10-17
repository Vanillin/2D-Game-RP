using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Game_STALKER_Exclusion_Zone
{
    public enum NPSIntellect
    {
        Non, //стоит вкопанный не реагирует
        StandPassive, // стоит, при приближении уходит с линии огня
        StandAgressive, // стоит, при приближении подходит
        RandomPassive, // ещё и радномно
        RandomAgressive, //
    }
    //---------------------------------------------------------------Basic Things
    public class Location
    {
        public readonly string Name;
        public readonly string SystemName;
        public Graf GrafLocToMove;
        public Graf GrafLocToWatch;
        public string[,] SignsLives;
        public string[,] Signs;
        public string[,] Blocks;
        public string[,] Air;

        public List<Skelet> Lives = new List<Skelet>();

        public Location(string name, string systemName, string[,] signs, string[,] blocks, string[,] air, Graf grafmove, Graf grafwatch, string[,] signsLives)
        {
            this.SystemName = systemName;
            this.Name = name;
            this.Signs = signs;
            this.Blocks = blocks;
            this.Air = air;
            this.GrafLocToMove = grafmove;
            this.GrafLocToWatch = grafwatch;
            this.SignsLives = signsLives;
        }
    }
    public abstract class Skelet
    {
        public readonly string Name;
        public readonly string SecondName;
        private NPSGroup Fraction;
        public NPSGroup FractionInf() { return Fraction; }
        public string FractionString() 
        {
            if (Fraction is NPSGroup.Stalker) return "Сталкер";
            else if (Fraction is NPSGroup.Naemnik) return "Наёмник";
            else if (Fraction is NPSGroup.Mutant) return "Мутант";
            else return "";
        }
        private Gun Gun;
        public Gun GunInf() { return Gun; }
        private Cloth Cloth;
        public Cloth ClothIng() { return Cloth; }
        public NPSIntellect Intellect;
        public Point Cord;
        public bool IsAlive;
        public readonly string SystemName;
        private int Health;
        public int MaxHealth;
        public int HealthInf() { return Health; }
        //public bool See;

        public List<Point> OblSee = new List<Point>();
        public List<Point> OblAttack = new List<Point>();
        public Skelet LastSeeEnemy;
        public Point LastGoingPoint = new Point(-1,-1);

        public List<Item> InventoryList;
        public int Money;
        public List<NPSGroup> FriendFranction;

        public void Damaging(int damage)
        {
            if (!IsAlive) return;
            Health -= (damage - Cloth.Armor);
            if (Health <= 0)
            {
                Health = 0;
                Intellect = NPSIntellect.Non;
                IsAlive = false;
            }
        }
        public void Healthing(int health)
        {
            if (health == MaxHealth) IsAlive = true;
            if (!IsAlive) return;
            Health += health;
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
        }
        public void Going(Point point, Location location)
        {
            Cord = point;
            foreach (Vertex vertex in location.GrafLocToWatch)
            {
                if (vertex.Cord == point)
                {
                    if (this.FractionInf() == NPSGroup.Mutant)
                    {
                        OblSee = location.GrafLocToWatch.SearchSee(Cord, 11);
                    }
                    else
                    {
                        OblSee = location.GrafLocToWatch.SearchSee(Cord, 9);
                    }
                    OblAttack = location.GrafLocToWatch.SearchSee(Cord, Gun.Radius);
                    return;
                }
            }
            OblSee = new List<Point>();
            OblAttack = new List<Point>();
            return;
        }

        public void TakeGun(Gun gun, Location location)
        {
            if (Gun != null)
            {
                InventoryList.Add(Gun);
            }
            Gun = gun;
            OblAttack = location.GrafLocToWatch.SearchSee(Cord, Gun.Radius);
        }
        public void TakeCloth(Cloth cloth)
        {
            if (Cloth != null)
            {
                InventoryList.Add(Cloth);
            }
            Cloth = cloth;
            Fraction = Cloth.FractionCloth;
        }
        public Skelet(string name, string secondname, NPSGroup fraction, Gun gun, NPSIntellect intellect, Point coord,
            Cloth cloth, string systemname, int money, List<Item> inventoryList, List<NPSGroup> friendFranction, int maxHealth)
        {
            this.Name = name;
            this.SecondName = secondname;
            this.Fraction = fraction;
            this.Gun = gun;
            this.Intellect = intellect;
            this.Cord = coord;
            this.IsAlive = true;
            this.Health = maxHealth;
            //this.See = false;
            this.Cloth = cloth;
            this.SystemName = systemname;
            this.Money = money;
            this.InventoryList = inventoryList;
            this.FriendFranction = friendFranction;
            this.MaxHealth = maxHealth;
        }
    }
    public abstract class Item
    {
        public readonly string Name;
        public readonly string SystemName;
        public int Cost;

        public abstract void Using(Skelet skelet);
        public Item(string name, string systemName, int cost)
        {
            this.Name = name;
            this.SystemName = systemName;
            this.Cost = cost;
        }
    }
    public abstract class Gun : Item
    {
        public int Damage;
        public int Radius;

        public Gun(string name, string systemName, int damage, int radius, int cost)
            : base(name, systemName, cost)
        {
            this.Damage = damage;
            this.Radius = radius;
        }

        public override void Using(Skelet skelet)
        {}
    }
    public abstract class Cloth : Item
    {
        public int Armor;
        public NPSGroup FractionCloth;

        public Cloth(string name, string systemName, int cost, int armor, NPSGroup fractioncloth)
            : base(name, systemName, cost)
        {
            this.Armor = armor;
            this.FractionCloth = fractioncloth;
        }
        public override void Using(Skelet skelet)
        { }
    }
    public class Task
    {
        public string SystemName { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public double CordOnMapX { get; set; }
        public double CordOnMapY { get; set; }
        public Task(string systemName, string name, string secondName, double cordOnMapX, double cordOnMapY)
        {
            this.Name = name;
            this.SystemName = systemName;
            this.SecondName = secondName;
            this.CordOnMapX = cordOnMapX;
            this.CordOnMapY = cordOnMapY;
        }
        private Task() { } //ДЛЯ СЕРИАЛИЗАЦИИ НЕ ТРОЖЖЖЖ
    }
    public class Phrase
    {
        public string Index { get; set; }
        public string Dialog { get; set; }
        public Phrase(string index, string dialog)
        {
            this.Index = index;
            this.Dialog = dialog;
        }
        private Phrase() { } //ДЛЯ СЕРИАЛИЗАЦИИ НЕ ТРОЖЖЖЖ
    }
    //---------------------------------------------------------------Persons
    public enum PlayerGender
    {
        Man,
        Woman,
    }
    public enum NPSGroup
    {
        Stalker,
        Naemnik,
        Mutant,
        Box,
    }
    public class Player : Skelet
    {
        public int Kills;
        public List<Task> Tasks = new List<Task>();
        public List<Task> CompliteTasks = new List<Task>();
        public readonly PlayerGender Gender;


        public Player(string name, PlayerGender gender, Point coord, Gun gun, Cloth cloth, int money, List<Item> inventoryList, List<NPSGroup> friends) :
            base(name, "", NPSGroup.Stalker, gun, NPSIntellect.Non, coord, cloth, "Player" + gender, money, inventoryList, friends, 20000) //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        {
            this.Kills = 0;
            this.Gender = gender;
        }
    }
    public class SkeletBox : Skelet
    {
        public SkeletBox(string name, string systemname, Point coord, int money, List<Item> inventoryList) :
            base(name, "", NPSGroup.Box, null, NPSIntellect.Non, coord, null, systemname, money, inventoryList, null, 100)
        { }
    }
    public class StalkerSmall : Skelet
    {
        public StalkerSmall(string name, string secondname, Gun gun, NPSIntellect intellect, Point coord, int money, List<Item> inventoryList, List<NPSGroup> friends) :
            base(name, secondname, NPSGroup.Stalker, gun, intellect, coord, new KurtkaStalker(), "StalkerSmall", money, inventoryList, friends, 100)
        { }
    }
    public class StalkerMedium : Skelet
    {
        public StalkerMedium(string name, string secondname, Gun gun, NPSIntellect intellect, Point coord, int money, List<Item> inventoryList, List<NPSGroup> friends) :
            base(name, secondname, NPSGroup.Stalker, gun, intellect, coord, new CombezStalker(), "StalkerMedium", money, inventoryList, friends, 100)
        { }
    }
    public class NaemnikMedium : Skelet
    {
        public NaemnikMedium(string name, string secondname, Gun gun, NPSIntellect intellect, Point coord, int money, List<Item> inventoryList, List<NPSGroup> friends) :
            base(name, secondname, NPSGroup.Naemnik, gun, intellect, coord, new CombezNaemnik(), "NaemnikMedium", money, inventoryList, friends, 100)
        { }
    }
    public class NaemnikHard : Skelet
    {
        public NaemnikHard(string name, string secondname, Gun gun, NPSIntellect intellect, Point coord, int money, List<Item> inventoryList, List<NPSGroup> friends) :
            base(name, secondname, NPSGroup.Naemnik, gun, intellect, coord, new ExoCombezNaemnik(), "NaemnikHard", money, inventoryList, friends, 100)
        { }
    }
    public class DealerBoris : Skelet
    {
        public DealerBoris(Gun gun, NPSIntellect intellect, Point coord, List<NPSGroup> friends) :
            base("Борис", "", NPSGroup.Stalker, gun, intellect, coord, new CombezStalker(), "DealerBoris", 0, new List<Item>(), friends, 100)
        { }
    }
    public class StalkerZelen : Skelet
    {
        public StalkerZelen(Gun gun, NPSIntellect intellect, Point coord, List<NPSGroup> friends) :
            base("Зелёный", "", NPSGroup.Stalker, gun, intellect, coord, new KurtkaStalker(), "StalkerZelen", 0, new List<Item>(), friends, 100)
        { }
    }
    public class MutantSobaka : Skelet
    {
        public MutantSobaka(Point coord, List<Item> inventoryList, List<NPSGroup> friends) :
            base("Слепой пёс", "", NPSGroup.Mutant, new Clutch(), NPSIntellect.RandomAgressive, coord, new MutantSkinCloth(), "MutantSobaka", 0,
            inventoryList, friends, 150)
        { }
    }
    public class MutantCrovosos : Skelet
    {
        public MutantCrovosos(Point coord, List<Item> inventoryList, List<NPSGroup> friends) :
            base("Кровосос", "", NPSGroup.Mutant, new Tentacles(), NPSIntellect.RandomAgressive, coord, new MutantSkinCloth(), "MutantCrovosos", 0,
            inventoryList, friends, 230)
        { }
    }
    //---------------------------------------------------------------Items
    public enum Items
    {
        AidFirstKid,
        ArmyAidFirstKid,
        ArtZabiiPuzir,
        Banknote,
        Bread,
        Stew,
        KvestGun,
        TailDog,
        MutantSkin,
    }
    public class AidFirstKid : Item
    {
        public int Healthing { get; set; }
        public override void Using(Skelet skelet)
        {
            skelet.Healthing(Healthing);
        }
        public AidFirstKid() : base("Аптечка первой помощи", "AidFirstKid", 500) { Healthing = 25; }
    }
    public class ArmyAidFirstKid : Item
    {
        public int Healthing { get; set; }
        public override void Using(Skelet skelet)
        {
            skelet.Healthing(Healthing);
        }
        public ArmyAidFirstKid() : base("Армейская аптечка первой помощи", "ArmyAidFirstKid", 800) { Healthing = 40; }
    }
    public class ArtZabiiPuzir : Item
    {
        public override void Using(Skelet skelet)
        {}
        public ArtZabiiPuzir() : base("Артефакт Жабий пузырь", "ArtZabiiPuzir", 5000) {}
    }
    public class Banknote : Item
    {
        public override void Using(Skelet skelet)
        {
            int plus = new Random().Next(3, 6);
            skelet.Money += plus*100;
        }
        public Banknote() : base("Стопка денег", "Banknote", 0) {}
    }
    public class Bread : Item
    {
        public int Healthing { get; set; }
        public override void Using(Skelet skelet)
        {
            skelet.Healthing(Healthing);
        }
        public Bread() : base("Хлеб", "Bread", 200) { Healthing = 10; }
    }
    public class Stew : Item
    {
        public int Healthing { get; set; }
        public override void Using(Skelet skelet)
        {
            skelet.Healthing(Healthing);
        }
        public Stew() : base("Тушёнка", "Stew", 500) { Healthing = 20; }
    }
    public class KvestGun : Item
    {
        public override void Using(Skelet skelet)
        { }

        public KvestGun() : base("Фамильное ружье", "KvestGun", 1000) { }
    }
    public class MutantSkin : Item
    {
        public override void Using(Skelet skelet)
        { }
        public MutantSkin() : base("Шкура кровососа", "MutantSkin", 800) { }
    }
    public class TailDog : Item
    {
        public override void Using(Skelet skelet)
        { }
        public TailDog() : base("Хвост собаки", "TailDog", 500) { }
    }
    //---------------------------------------------------------------Guns
    public enum Guns
    {
        Clutch,
        Toz34,
        Ak74ukorot,
        MP5,
        Tentacles,
    }
    public class Toz34 : Gun
    {
        public Toz34() : base("Тоз 34", "Toz34", 60, 4, 1200) { }
    }
    public class Ak74ukorot : Gun
    {
        public Ak74ukorot() : base("АК 47 (Складной)", "Ak74ukorot", 45, 6, 2300) { }
    }
    public class Clutch : Gun
    {
        public Clutch() : base("", "", 55, 1, 0) { }
    }
    public class Tentacles : Gun
    {
        public Tentacles() : base("", "", 80, 1, 0) { }
    }
    public class MP5 : Gun
    {
        public MP5() : base("МП-5", "MP5", 40, 7, 1900) { }
    }
    //---------------------------------------------------------------Cloth
    public enum Clothes
    {
        KurtkaStalker,
        CombezStalker,
        CombezNaemnik,
        MutantSkin,
    }
    public class KurtkaStalker : Cloth
    {
        public KurtkaStalker() : base("Куртка сталкера-новичка", "KurtkaStalker", 500, 5, NPSGroup.Stalker) { }
    }
    public class CombezStalker : Cloth
    {
        public CombezStalker() : base("Сталкерский комбинезон Заря", "CombezStalker", 500, 10, NPSGroup.Stalker) { }
    }
    public class CombezNaemnik : Cloth
    {
        public CombezNaemnik() : base("Комбинезон наёмника", "CombezNaemnik", 500, 10, NPSGroup.Naemnik) { }
    }
    public class ExoCombezNaemnik : Cloth
    {
        public ExoCombezNaemnik() : base("Экзоскелет наёмника", "ExoCombezNaemnik", 500, 20, NPSGroup.Naemnik) { }
    }
    public class MutantSkinCloth : Cloth
    {
        public override void Using(Skelet skelet)
        { }
        public MutantSkinCloth() : base("Шкура мутанта", "MutantSkin", 0, 0, NPSGroup.Mutant) { }
    }

    //---------------------------------------------------------------Programming Thins    
    public class Vertex : IEnumerable, ICloneable
    {
        public Point Cord = new Point();
        public List<Vertex> Near = new List<Vertex>();
        public Vertex(Point point)
        {
            Cord = point;
        }
        private Vertex(Point point, List<Vertex> near)
        {
            Cord = point;
            Near = near;
        }
        public object Clone()
        {
            return new Vertex(Cord, Near);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var point in Near)
            {
                yield return point;
            }
        }
    }
    public class Graf : IEnumerable, ICloneable
    {
        public List<Vertex> Vertexes = new List<Vertex>();
        public Graf()
        { }
        private Graf(List<Vertex> list)
        {
            Vertexes = list;
        }
        public IEnumerator GetEnumerator()
        {
            foreach (var vertex in Vertexes)
            {
                yield return vertex;
            }
        }
        public object Clone()
        {
            List<Vertex> copy = new List<Vertex>();
            foreach ( var vertex in Vertexes)
            {
                copy.Add((Vertex)vertex.Clone());
            }
            return new Graf(copy);
        }
        public List<Point> SearchWidth(Point start, Point finish)
        {
            if (start == finish)
            {
                return null;
            }
            List<Point> retur = new List<Point>();
            Vertex startVert = Find(start);
            Vertex finishVert = Find(finish);

            if (startVert == null || finishVert == null)
            {
                throw new Exception($"В графе отсутствует точка ({startVert.Cord.X},{startVert.Cord.Y};{finishVert.Cord.X},{finishVert.Cord.Y})");
            }

            List<Vertex> PartStart = new List<Vertex>() { startVert };
            List<Vertex> PartFinish = new List<Vertex>() { finishVert };
            Dictionary<Vertex, Vertex> Path = new Dictionary<Vertex, Vertex>
            {
                { startVert, null },
                { finishVert, null }
            };
            Queue<Vertex> Que = new Queue<Vertex>();
            Que.Enqueue(startVert);
            Que.Enqueue(finishVert);
            while (Que.Count > 0)
            {
                Vertex Now = Que.Dequeue();
                foreach (Vertex v in Now.Near)
                {
                    if ((PartStart.Contains(Now) && PartFinish.Contains(v)) ||
                            (PartStart.Contains(v) && PartFinish.Contains(Now)))
                    {
                        Vertex save = v;
                        if (PartFinish.Contains(Now))
                        {
                            save = Now;
                            Now = v;
                        }
                        retur.Add(Now.Cord);
                        while (Now != null)
                        {
                            if (Path[Now] != null)
                                retur.Add(Path[Now].Cord);
                            Now = Path[Now];
                        }
                        retur.Reverse();

                        retur.Add(save.Cord);
                        while (save != null)
                        {
                            if (Path[save] != null)
                                retur.Add(Path[save].Cord);
                            save = Path[save];
                        }

                        retur.Remove(startVert.Cord);
                        return retur;
                    }
                    else if (!PartStart.Contains(v) && !PartFinish.Contains(v))
                    {
                        if (PartStart.Contains(Now))
                            PartStart.Add(v);
                        else
                            PartFinish.Add(v);
                        Path.Add(v, Now);
                        Que.Enqueue(v);
                    }
                }
            }
            return null;
        }
        List<Point> SearchAround(Point start, int count)
        {
            List<Point> retur = new List<Point>();
            Vertex startVert = Find(start);

            if (startVert == null)
            {
                throw new Exception($"В графе отсутствует точка ({startVert.Cord.X},{startVert.Cord.Y})");
            }

            Dictionary<Vertex, int> Length = new Dictionary<Vertex, int> { { startVert, 0 } };
            Queue<Vertex> Que = new Queue<Vertex>();
            Que.Enqueue(startVert);
            while (Que.Count > 0)
            {
                Vertex Now = Que.Dequeue();
                foreach (Vertex v in Now.Near)
                {
                    if (!Length.ContainsKey(v))
                    {
                        if (Length[Now] + 1 > count)
                        {
                            return retur;
                        }
                        Length.Add(v, Length[Now] + 1);
                        retur.Add(v.Cord);
                        Que.Enqueue(v);
                    }
                }
            }
            return retur;
        }
        public List<Point> SearchSee(Point start, int count)
        {
            List<Point> retur = new List<Point>();
            Point startVert = Find(start).Cord;

            if (startVert == null)
            {
                throw new Exception($"В графе отсутствует точка ({startVert.X},{startVert.Y})");
            }

            Dictionary<Point, int> Length = new Dictionary<Point, int> { { startVert, 0 } };
            Queue<Point> Que = new Queue<Point>();
            Que.Enqueue(startVert);

            List<double[]> SavedObject = new List<double[]>(); //0 - max //1 - min
            bool KeyPI = true;

            while (Que.Count > 0)
            {
                Point Now = Que.Dequeue();
                List<Point> Near = new List<Point>() {new Point(Now.X+1, Now.Y), new Point(Now.X, Now.Y+1), 
                    new Point(Now.X - 1, Now.Y), new Point(Now.X, Now.Y-1) };
                foreach (Point p in Near)
                {
                    if (!Length.ContainsKey(p))
                    {
                        if (Length[Now] + 1 > count)
                        {
                            return retur;
                        }

                        double CurX = p.X - start.X;
                        double CurY = p.Y - start.Y;
                        double NewAngle = Math.Atan2(CurX, CurY);

                        if (this.Vertexes.Find(X => X.Cord == p) != null)
                        {
                            bool RadianKey = true;
                            foreach (double[] o in SavedObject)
                            {
                                if (RadianKey)
                                {
                                    if (o[0] > NewAngle && NewAngle > o[1])
                                    {
                                        RadianKey = false;
                                    }
                                }
                            }
                            if (RadianKey) //добавляет все видимые точки
                            {
                                Length.Add(p, Length[Now] + 1);
                                retur.Add(p);
                                Que.Enqueue(p);
                            }
                        }
                        else
                        {
                            List<double> Angles = new List<double> { Math.Atan2(CurX +0.5, CurY +0.5), Math.Atan2(CurX +0.5, CurY -0.5),
                            Math.Atan2(CurX -0.5, CurY +0.5), Math.Atan2(CurX -0.5, CurY -0.5) };
                            double MaxNewAngle = Angles.Max();
                            double MinNewAngle = Angles.Min();
                            
                            if (NewAngle == Math.PI)
                            {
                                if (KeyPI)
                                {
                                    SavedObject.Add(new double[2] { Math.Atan2(CurX - 0.5, CurY + 0.5), -3.2 });
                                    SavedObject.Add(new double[2] { 3.2, Math.Atan2(CurX + 0.5, CurY + 0.5) });
                                    KeyPI = false;
                                }
                                continue;
                            }

                            bool AddNewAngle = true;
                            foreach (double[] o in SavedObject)
                            {

                                if (AddNewAngle)
                                {
                                    bool MaxInside = (o[0] > MaxNewAngle && MaxNewAngle > o[1]);
                                    bool MinInside = (o[0] > MinNewAngle && MinNewAngle > o[1]);

                                    if (MaxInside && MinInside)
                                    {
                                        AddNewAngle = false;
                                    }
                                    else if (MaxInside && !MinInside)
                                    {
                                        o[1] = MinNewAngle;
                                        AddNewAngle = false;
                                    }
                                    else if (!MaxInside && MinInside)
                                    {
                                        o[0] = MaxNewAngle;
                                        AddNewAngle = false;
                                    }
                                }
                            }
                            if (AddNewAngle)
                            {
                                SavedObject.Add(new double[2] { MaxNewAngle, MinNewAngle });
                            }
                        }
                    }
                }
            }
            return retur;
        }
        public Vertex Find(Point point)
        {
            foreach (Vertex p in Vertexes)
            {
                if (p.Cord == point)
                {
                    return p;
                }
            }
            return null;
        }
    }

    //---------------------------------------------------------------

}

