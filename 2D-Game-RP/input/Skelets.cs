using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TwoD_Game_RP
{
    public class StalkerSmall : Skelet
    {
        public StalkerSmall(string name, string secondname, Gun gun, NPSIntellect intellect, GamePoint coord, int money, List<Item> inventoryList, List<NPSGroup> friends) :
            base(name, secondname, NPSGroup.Stalker, gun, intellect, coord, '0', new KurtkaStalker(), "StalkerSmall", money, inventoryList, friends, 100, true)
        { }
    }
    public class StalkerMedium : Skelet
    {
        public StalkerMedium(string name, string secondname, Gun gun, NPSIntellect intellect, GamePoint coord, int money, List<Item> inventoryList, List<NPSGroup> friends) :
            base(name, secondname, NPSGroup.Stalker, gun, intellect, coord, '0', new CombezStalker(), "StalkerMedium", money, inventoryList, friends, 100, true)
        { }
    }
    public class NaemnikMedium : Skelet
    {
        public NaemnikMedium(string name, string secondname, Gun gun, NPSIntellect intellect, GamePoint coord, int money, List<Item> inventoryList, List<NPSGroup> friends) :
            base(name, secondname, NPSGroup.Naemnik, gun, intellect, coord, '0', new CombezNaemnik(), "NaemnikMedium", money, inventoryList, friends, 100, true)
        { }
    }
    public class NaemnikHard : Skelet
    {
        public NaemnikHard(string name, string secondname, Gun gun, NPSIntellect intellect, GamePoint coord, int money, List<Item> inventoryList, List<NPSGroup> friends) :
            base(name, secondname, NPSGroup.Naemnik, gun, intellect, coord, '0', new ExoCombezNaemnik(), "NaemnikHard", money, inventoryList, friends, 100, true)
        { }
    }
    public class DealerBoris : Skelet
    {
        public DealerBoris(Gun gun, NPSIntellect intellect, GamePoint coord, List<NPSGroup> friends) :
            base("Борис", "", NPSGroup.Stalker, gun, intellect, coord, '0', new CombezStalker(), "DealerBoris", 0, new List<Item>(), friends, 100, true)
        { }
    }
    public class StalkerZelen : Skelet
    {
        public StalkerZelen(Gun gun, NPSIntellect intellect, GamePoint coord, List<NPSGroup> friends) :
            base("Зелёный", "", NPSGroup.Stalker, gun, intellect, coord, '0', new KurtkaStalker(), "StalkerZelen", 0, new List<Item>(), friends, 100, true)
        { }
    }
    public class MutantSobaka : Skelet
    {
        public MutantSobaka(GamePoint coord, List<Item> inventoryList, List<NPSGroup> friends) :
            base("Слепой пёс", "", NPSGroup.Mutant, new Clutch(), NPSIntellect.RandomAgressive, coord, '0', new MutantSkinCloth(), "MutantSobaka", 0,
            inventoryList, friends, 150, true)
        { }
    }
    public class MutantCrovosos : Skelet
    {
        public MutantCrovosos(GamePoint coord, List<Item> inventoryList, List<NPSGroup> friends) :
            base("Кровосос", "", NPSGroup.Mutant, new Tentacles(), NPSIntellect.RandomAgressive, coord, '0', new MutantSkinCloth(), "MutantCrovosos", 0,
            inventoryList, friends, 230, true)
        { }
    }
    public class Door : Skelet
    {
        public Door(GamePoint coord, char rotate) :
            base("Двер", "Двер", NPSGroup.Door, null, NPSIntellect.Non, coord, rotate, null, "woodZaborDoorSkelet", 0, new List<Item>(0), new List<NPSGroup>(0), 1000, false)
        { }
    }
    public class SkeletBox : Skelet
    {
        public SkeletBox(string name, string systemname, GamePoint coord, int money, List<Item> inventoryList) :
            base(name, "", NPSGroup.Box, null, NPSIntellect.Non, coord, '0', null, systemname, money, inventoryList, null, 100, true)
        { }
    }
}
