using System.Collections.Generic;

namespace TwoD_Game_RP
{
    public class PlayerFirst : Player
    {
        public PlayerFirst(string name, string systemName, GamePoint coord, int inventoryHeight, int inventoryWight, List<Item> inventoryList, CustomSortedEnum<string> startTask) :
            base(name, systemName, PlayerGender.Man, coord, inventoryHeight, inventoryWight, inventoryList, new TaskBoard(Information.GetTasks(70), Information.GetTaskConnection(), startTask))
        { }
    }
    public class Girl : Enemy
    {
        public Girl(int lenWatch) :
            base("", "", NPSGroup.People, NPSIntellect.Non, new GamePoint(15, 10), 0, "girl", new List<Item> { new Knife(), new Water() }, 2, 2, true, 4, lenWatch)
        { }
    }
    public class Grandma : Enemy
    {
        public Grandma(int lenWatch) :
            base("", "", NPSGroup.People, NPSIntellect.Non, new GamePoint(5, 22), 0, "grandma", new List<Item> { }, 1, 1, true, 4, lenWatch)
        { }
    }
    public class Grandpa : Enemy
    {
        public Grandpa(int lenWatch) :
            base("", "", NPSGroup.People, NPSIntellect.Non, new GamePoint(3, 10), 0, "grandpa", new List<Item> { new Knife() }, 2, 2 , true, 4, lenWatch)
        { }
    }
    public class WoodDoor : Door
    {
        public WoodDoor(GamePoint coord, int rotate, bool isLock) :
            base("woodZaborDoorSkelet", coord, rotate, isLock)
        { }
    }
    public class Scorpion : Enemy
    {
        public Scorpion(GamePoint point, int lenWatch) :
            base("Scorpion", "", NPSGroup.Monster, NPSIntellect.Attack, point, 0, "scorpion", new List<Item>(), 1, 1, false, 6, lenWatch)
        {
            GiveGunInHand(new Knife());
        }
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
    public class Perecati : SystemSkelet
    {
        public Perecati(GamePoint coord, int rotate) :
            base(coord, rotate, "perecati", true)
        { }
    }
}
