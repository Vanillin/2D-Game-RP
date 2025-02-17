using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TwoD_Game_RP
{
    internal interface IBoxElement
    {
        CustomDictionary<Item, List<GamePoint>> ReferenceItem { get; }
        int MaxSizeH { get; }
        int MaxSizeW { get; }
        bool AddInBackpack(Item item);
        GamePoint RemoveInBackpack(Item item);
        bool ContainsInBackpack(Item item);
        Item SearchInBackpack(int h, int w);
    }

    internal class Inventory : IBoxElement
    {
        IStorageItem _backpack;
        IHaveGun haveGun;
        int _maxSizeH;
        int _maxSizeW;
        public int MaxSizeH => _maxSizeH;
        public int MaxSizeW => _maxSizeW;
        public CustomDictionary<Item, List<GamePoint>> ReferenceItem => _backpack.ReferenceItem;
        public Inventory(int maxH, int maxW, List<Item> list)
        {
            _maxSizeH = maxH;
            _maxSizeW = maxW;
            _backpack = Backpack.Creating(_maxSizeH, _maxSizeW, list);
            haveGun = new InventoryGun();
        }

        public bool GiveGunInHand(Gun gun)
        {
            if (haveGun.GetGunInHand() == null)
            {
                haveGun.GiveGunInHand(gun);
                return true;
            }
            if (AddInBackpack(haveGun.GetGunInHand()))
            {
                haveGun.GiveGunInHand(gun);
                return true;
            }
            return false;
        }
        public bool TakeAwayGunInHand()
        {
            if (AddInBackpack(haveGun.GetGunInHand()))
            {
                haveGun.DropGunInHand();
                return true;
            }
            return false;
        }
        public bool GiveGunInBack(Gun gun)
        {
            if (haveGun.GetGunInBack() == null)
            {
                haveGun.GiveGunInBack(gun);
                return true;
            }
            if (AddInBackpack(haveGun.GetGunInBack()))
            {
                haveGun.GiveGunInBack(gun);
                return true;
            }
            return false;
        }
        public bool TakeAwayGunInBack()
        {
            if (AddInBackpack(haveGun.GetGunInBack()))
            {
                haveGun.DropGunInBack();
                return true;
            }
            return false;
        }


        // ============================== method Backpack =======================
        public bool AddInBackpack(Item item)
        {
            return _backpack.Add(item);
        }
        public GamePoint RemoveInBackpack(Item item)
        {
            return _backpack.Remove(item);
        }
        public bool ContainsInBackpack(Item item)
        {
            return _backpack.Contains(item);
        }
        public Item SearchInBackpack(int h, int w)
        {
            return _backpack.SearchItem(h, w);
        }
    }
}
