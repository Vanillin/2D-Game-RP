using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_STALKER_Exclusion_Zone
{
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
        { }
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
}
