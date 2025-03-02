using System;
using System.Collections.Generic;

namespace TwoD_Game_RP
{
    public abstract class EnemySkelet : AliveSkelet
    {
        private int _lenWatch;
        public int LenWatch => _lenWatch;
        internal EnemySkelet(string systemNamePicture, GamePoint point, bool isClarity, IMemorySmartAction memoryAction, IBoxElement boxElement,
            int health, IFractionElement fractionElement, IHaveGun inventoryGun, Gun defaultGun, string name, string secondName, int lenWatch)
            : base(systemNamePicture, point, isClarity, memoryAction, boxElement,
                health, fractionElement, inventoryGun, defaultGun, name, secondName)
        {
            _lenWatch = lenWatch;
        }
    }

    public class Enemy : EnemySkelet
    {
        public Enemy(string systemNamePicture, GamePoint point, bool isClarity, int inventoryHeight, int inventoryWight, List<Item> items,
                int health, NPSGroup fraction, string name, string secondName, int lenWatch)
            : base(systemNamePicture, point, isClarity, new MemoryAttackAction(), new Inventory(inventoryHeight, inventoryWight, items),
                health, new MemoryFraction(fraction), new InventoryGun(), null, name, secondName, lenWatch)
        { }

    }
}

