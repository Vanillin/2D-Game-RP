using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_STALKER_Exclusion_Zone
{
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
        { }
        public ArtZabiiPuzir() : base("Артефакт Жабий пузырь", "ArtZabiiPuzir", 5000) { }
    }
    public class Banknote : Item
    {
        public override void Using(Skelet skelet)
        {
            int plus = new Random().Next(3, 6);
            skelet.Money += plus * 100;
        }
        public Banknote() : base("Стопка денег", "Banknote", 0) { }
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
}
