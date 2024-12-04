using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TwoD_Game_RP
{
    public class TestSkelet : Skelet
    {
        public TestSkelet(string name, string secondname, Gun gun, NPSIntellect intellect, GamePoint coord, int money, List<Item> inventoryList, List<NPSGroup> friends) :
            base(name, secondname, NPSGroup.Stalker, gun, intellect, coord, '0', null, "test", money, inventoryList, friends, 100, true)
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
