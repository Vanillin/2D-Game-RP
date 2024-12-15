using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TwoD_Game_RP
{
    public class Kristina : Skelet
    {
        public Kristina(GamePoint point, char rotate) :
            base("Кристина", "", NPSGroup.People, NPSIntellect.Non, point, rotate, "Kristina", new List<Item>(), 3, 3, false)
        { }
    }
    public class Maksim : Skelet
    {
        public Maksim(GamePoint point, char rotate) :
            base("Максим", "", NPSGroup.People, NPSIntellect.Non, point, rotate, "Maksim", new List<Item>(), 3, 3, false)
        { }
    }
    public class Nura : Skelet
    {
        public Nura(GamePoint point, char rotate) :
            base("Баб Нюра", "", NPSGroup.People, NPSIntellect.Non, point, rotate, "Nura", new List<Item>(), 3, 3, false)
        { }
    }
    public class Vanya : Skelet
    {
        public Vanya(GamePoint point, char rotate) :
            base("Ванька", "", NPSGroup.People, NPSIntellect.Non, point, rotate, "Vanya", new List<Item>(), 3, 3, false)
        { }
    }
    public class TestSkelet : Skelet
    {
        public TestSkelet(string name, string secondname, NPSIntellect intellect, GamePoint coord, char rotate, List<Item> inventoryList) :
            base(name, secondname, NPSGroup.People, intellect, coord, rotate, "test", inventoryList, 3, 3, true)
        { }
    }
    public class Door : Skelet
    {
        public Door(GamePoint coord, char rotate) :
            base("Двер", "Двер", NPSGroup.Door, NPSIntellect.Non, coord, rotate, "woodZaborDoorSkelet", new List<Item>(0), 0, 0, false)
        { }
    }
    public class Trash : Skelet
    {
        public Trash(GamePoint coord, char rotate, int heightInventory, int weightInventory, List<Item> inventoryList) :
            base("Мусорка", "Мусорка", NPSGroup.Box, NPSIntellect.Non, coord, rotate, "trashSkelet", inventoryList, heightInventory, weightInventory, true)
        { }
    }
    public class Box : Skelet
    {
        public Box(string name, string systemname, GamePoint coord, char rotate, int heightInventory, int weightInventory, List<Item> inventoryList) :
            base(name, "", NPSGroup.Box, NPSIntellect.Non, coord, rotate, systemname, inventoryList, heightInventory, weightInventory, true)
        { }
    }
}
