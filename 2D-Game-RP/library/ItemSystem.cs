using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{
    public abstract class Item : IComparable<Item>, IEquatable<Item>
    {
        public readonly string Name;
        public readonly string SystemName;
        public int Cost;
        public readonly int SizeH;
        public readonly int SizeW;
        public IPictureCell Picture;
                      
        public Item(string name, string systemName, int cost, int sizeH, int sizeW)
        {
            this.Name = name;
            this.SystemName = systemName;
            this.Cost = cost;
            this.SizeH = sizeH;
            this.SizeW = sizeW;
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
    public abstract class Gun : Item
    {
        public int Damage;
        public int Radius;

        public Gun(string name, string systemName, int damage, int radius, int cost, int sizeH, int sizeW)
            : base(name, systemName, cost, sizeH, sizeW)
        {
            this.Damage = damage;
            this.Radius = radius;
        }
    }
    public abstract class Cloth : Item
    {
        public int Armor;
        public NPSGroup FractionCloth;

        public Cloth(string name, string systemName, int cost, int armor, NPSGroup fractioncloth, int sizeH, int sizeW)
            : base(name, systemName, cost, sizeH, sizeW)
        {
            this.Armor = armor;
            this.FractionCloth = fractioncloth;
        }
    }
}
