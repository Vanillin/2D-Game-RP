using System;
using System.Collections.Generic;
using System.Windows;

namespace TwoD_Game_RP
{
    public class PlayerFirst : Player
    {
        public PlayerFirst(string name, string systemNamePicture, int inventoryHeight, int inventoryWight, List<Item> items, CustomSortedEnum<string> startTask, int health) :
            base(systemNamePicture, new GamePoint(5, 22), false, inventoryHeight, inventoryWight, items, new Hand(), health, NPSGroup.People, name, "", PlayerGender.Man, startTask)
        { }
    }
    public class Girl : Enemy
    {
        public Girl(int lenWatch) :
            base("girl", new GamePoint(15, 10), true,  2, 2, new List<Item> { new Knife(), new Potato() }, 4, NPSGroup.People, "Вероника", "", lenWatch)
        { }
    }
    public class Grandma : Enemy
    {
        public Grandma(int lenWatch) :
            base("grandma", new GamePoint(5,22), true, 1, 1, new List<Item>(0), 4, NPSGroup.People, "", "", lenWatch)
        { }
    }
    public class Grandpa : Enemy
    {
        public Grandpa(int lenWatch) :
            base("grandpa", new GamePoint(3, 10), true, 2, 2, new List<Item> { new Knife() }, 4, NPSGroup.People, "Хулио", "", lenWatch)
        { }
    }
    //public class WoodDoor : Door
    //{
    //    public WoodDoor(GamePoint coord, int rotate, bool isLock) :
    //        base("woodZaborDoorSkelet", coord, rotate, isLock)
    //    { }
    //}
    public class Scorpion : Enemy
    {
        public Scorpion(GamePoint point, int lenWatch) :
            base("scorpion", point, false, 1, 1, new List<Item>(1) { new ScorpionPart()}, 6, NPSGroup.Monster, "Scorpion", "", lenWatch)
        {
            GiveGunInHand(new ScorpionGun());
        }
    }
    public class Perecati : SimpleElement
    {
        public Perecati(GamePoint coord, int rotate) :
            base("perecati", coord, true)
        { }
    }
}
