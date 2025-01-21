using System.Collections.Generic;

namespace TwoD_Game_RP
{
    public class PlayerFirst : Player
    {
        public PlayerFirst(string name, string systemName, GamePoint coord, int inventoryHeight, int inventoryWight, List<Item> inventoryList, CustomSortedEnum<string> startTask) :
            base(name, systemName, PlayerGender.Man, coord, inventoryHeight, inventoryWight, inventoryList, new TaskBoard(Information.GetTasks(70), Information.GetTaskConnection(), startTask))
        { }
    }
    public class Girl : Skelet
    {
        public Girl() :
            base("", "", NPSGroup.People, NPSIntellect.Non, new GamePoint(15, 10), 0, "girl", new List<Item> { new Knife(), new Water() }, 2, 2, false, 10)
        { }
    }
    public class Grandma : Skelet
    {
        public Grandma() :
            base("", "", NPSGroup.People, NPSIntellect.Non, new GamePoint(5, 22), 0, "grandma", new List<Item> { }, 1, 1, false, 10)
        { }
    }
    public class Grandpa : Skelet
    {
        public Grandpa() :
            base("", "", NPSGroup.People, NPSIntellect.Non, new GamePoint(3, 10), 0, "grandpa", new List<Item> { new Knife() }, 2, 2 , false, 10)
        { }
    }
    public class WoodDoor : Door
    {
        public WoodDoor(GamePoint coord, int rotate, bool isLock) :
            base("woodZaborDoorSkelet", coord, rotate, isLock)
        { }
    }
    //public class Trash : Skelet
    //{
    //    public Trash(GamePoint coord, int rotate, int heightInventory, int weightInventory, List<Item> inventoryList) :
    //        base("Мусорка", "Мусорка", NPSGroup.Box, NPSIntellect.Non, coord, rotate, "trashSkelet", inventoryList, heightInventory, weightInventory, true, 1)
    //    { }
    //}
    //public class Box : Skelet
    //{
    //    public Box(string name, string systemname, GamePoint coord, int rotate, int heightInventory, int weightInventory, List<Item> inventoryList) :
    //        base(name, "", NPSGroup.Box, NPSIntellect.Non, coord, rotate, systemname, inventoryList, heightInventory, weightInventory, true, 1)
    //    { }
    //}
    public class Perecati : SystemSket
    {
        public Perecati(GamePoint coord, int rotate) :
            base(coord, rotate, "perecati", true)
        { }
    }
}
