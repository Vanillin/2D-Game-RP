using System;
using System.Configuration;

namespace TwoD_Game_RP
{
    public abstract class Item : IComparable<Item>, IEquatable<Item>
    {
        public string Name { get; private set; }
        public string SystemName { get; private set; }
        public int SizeH { get; private set; }
        public int SizeW { get; private set; }
        public int Cost { get; set; }
        internal IPicture Picture { get; set; }

        public Item(string name, string systemName, int cost, int sizeH, int sizeW)
        {
            Name = name;
            SystemName = systemName;
            Cost = cost;
            SizeH = sizeH;
            SizeW = sizeW;
            Picture = new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], systemName + ".png"));
        }
        public int CompareTo(Item other)
        {
            if (SizeH * SizeW > other.SizeW * other.SizeH)
                return 1;
            else if (SizeH * SizeW < other.SizeW * other.SizeH)
                return -1;
            else if (SizeW > other.SizeW)
                return 1;
            else if (SizeW < other.SizeW)
                return -1;

            else if (Equals(other))
                return 0;

            return SystemName.CompareTo(other.SystemName);
        }
        public bool Equals(Item other)
        {
            return SystemName == other.SystemName;
        }
    }
    public abstract class ItemMedicine : Item
    {
        private int _healthPlus;
        public ItemMedicine(string name, string systemName, int cost, int sizeH, int sizeW, int healthPlus)
            : base (name, systemName, cost, sizeH, sizeW)
        {
            _healthPlus = healthPlus;
        }
        public void CureSkelet(AliveSkelet skelet)
        {
            skelet.PlusHealth(_healthPlus);
        }
    }
    public abstract class Gun : Item
    {
        public int Damage;
        public int Radius;
        private IPicture _pictureAttack;
        public IPicture PictureAttack => _pictureAttack;

        public Gun(string name, string systemName, int damage, int radius, int cost, int sizeH, int sizeW, IPicture pictureAttack)
            : base(name, systemName, cost, sizeH, sizeW)
        {
            Damage = damage;
            Radius = radius;
            _pictureAttack = pictureAttack;
        }
    }
    //public abstract class Cloth : Item
    //{
    //    public int Armor;
    //    public NPSGroup FractionCloth;

    //    public Cloth(string name, string systemName, int cost, int armor, NPSGroup fractioncloth, int sizeH, int sizeW)
    //        : base(name, systemName, cost, sizeH, sizeW)
    //    {
    //        Armor = armor;
    //        FractionCloth = fractioncloth;
    //    }
    //}
}
