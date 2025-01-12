using System.Collections.Generic;

namespace TwoD_Game_RP
{
    internal class Inventory
    {
        Backpack _backpack;
        Gun _gunInHand;
        Gun _gunInBack;
        public CustomDictionary<Item, List<GamePoint>> ReferenceItem => _backpack.ReferenceItem;
        public Inventory(int maxH, int maxW, List<Item> list)
        {
            _backpack = Backpack.Creating(maxH, maxW, list);
            _gunInHand = null;
            _gunInBack = null;
        }

        public void ChangeGunInHandAndInBack()
        {
            (_gunInHand, _gunInBack) = (_gunInBack, _gunInHand);
        }

        // ============================== GunInHand =======================
        public Gun GetGunInHand()
        {
            return _gunInHand;
        }
        public bool GiveGunInHand(Gun gun)
        {
            if (_gunInHand == null)
            {
                _gunInHand = gun;
                return true;
            }
            if (AddInBackpack(_gunInHand))
            {
                _gunInHand = gun;
                return true;
            }
            return false;
        }
        public bool TakeAwayGunInHand()
        {
            if (AddInBackpack(_gunInHand))
            {
                _gunInHand = null;
                return true;
            }
            return false;
        }
        public void DropGunInHand()
        {
            _gunInHand = null;
        }

        // ============================== GunInBack =======================
        public Gun GetGunInBack()
        {
            return _gunInBack;
        }
        public bool GiveGunInBack(Gun gun)
        {
            if (_gunInBack == null)
            {
                _gunInBack = gun;
                return true;
            }
            if (AddInBackpack(_gunInBack))
            {
                _gunInBack = gun;
                return true;
            }
            return false;
        }
        public bool TakeAwayGunInBack()
        {
            if (AddInBackpack(_gunInBack))
            {
                _gunInBack = null;
                return true;
            }
            return false;
        }
        public void DropGunInBack()
        {
            _gunInBack = null;
        }

        // ============================== method Backpack =======================
        public bool AddInBackpack(Item item)
        {
            return _backpack.Add(item);
        }
        public void RemoveInBackpack(Item item)
        {
            _backpack.Remove(item);
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
