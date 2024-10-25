﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Game_STALKER_Exclusion_Zone
{
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
}
