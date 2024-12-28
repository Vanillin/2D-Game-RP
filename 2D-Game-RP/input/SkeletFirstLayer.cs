using System.Collections.Generic;

namespace TwoD_Game_RP
{
    public class PlayerFirst : Player
    {
        public PlayerFirst(string name, string systemName, GamePoint coord, int inventoryHeight, int inventoryWight, List<Item> inventoryList, CustomSortedEnum<string> startTask) :
            base(name, systemName, PlayerGender.Man, coord, inventoryHeight, inventoryWight, inventoryList, new TaskBoard(Information.GetTasks(70), Information.GetTaskConnection(), startTask))
        { }
    }
    public class Kristina : Skelet
    {
        public Kristina(GamePoint point, char rotate) :
            base("Кристина", "", NPSGroup.People, NPSIntellect.Non, point, rotate, "Kristina", new List<Item>(), 3, 3, false)
        { }
    }
    public class Agency : Skelet
    {
        public Agency(GamePoint point, char rotate) :
            base("Агентство", "", NPSGroup.People, NPSIntellect.Non, point, rotate, "Agency", new List<Item>(), 3, 3, false)
        { }
    }
    public class Dead : Skelet
    {
        public Dead(GamePoint point, char rotate) :
            base("Убитый", "", NPSGroup.People, NPSIntellect.Non, point, rotate, "Dead", new List<Item>(), 3, 3, true)
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
    public class WoodDoor : Door
    {
        public WoodDoor(GamePoint coord, char rotate, bool isLock) :
            base("Двер", "woodZaborDoorSkelet", coord, rotate, isLock)
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
