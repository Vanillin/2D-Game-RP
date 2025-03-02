using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace TwoD_Game_RP
{
    public abstract class AliveSkelet : StorageSkelet
    {
        private IAliveElement _health;
        private IFractionElement _fractionElement;
        private IHaveGun _inventoryGun;
        private Gun _defaultGun;
        public string Name { get; }
        public string SecondName { get; }
        public bool IsAlive => _health.IsAlive;
        public int Health => _health.Health;
        public double HealthPercent => _health.HealthPercent;
        public NPSGroup Fraction => _fractionElement.Fraction;
        public List<NPSGroup> FriendFranction => _fractionElement.FriendFranction;
        public void MinusHealth(int health)
        {
            _health.MinusHealth(health);
            if (!IsAlive) (_picture as ISomePicture).ChangeIndexPicture(1);
        }
        public void PlusHealth(int health)
        {
            _health.PlusHealth(health);
            if (IsAlive) (_picture as ISomePicture).ChangeIndexPicture(0);
        }
        public void AddFriendFraction(NPSGroup friend) => _fractionElement.AddFriendFraction(friend);
        public void RemoveFriendFraction(NPSGroup friend) => _fractionElement.RemoveFriendFraction(friend);
        public void ChangeGunInHandAndInBack() => _inventoryGun.ChangeGunInHandAndInBack();
        public Gun GetGunInHand()
        {
            Gun gun = _inventoryGun.GetGunInHand();
            if (gun == null)
                return _defaultGun;
            return gun;
        }
        public void DropGunInHand() => _inventoryGun.DropGunInHand();
        public Gun GetGunInBack() => _inventoryGun.GetGunInBack();
        public void DropGunInBack() => _inventoryGun.DropGunInBack();
        public bool GiveGunInHand(Gun gun)
        {
            if (GetGunInHand() == _defaultGun)
            {
                _inventoryGun.GiveGunInHand(gun);
                return true;
            }
            else
            {
                if (AddInBackpack(GetGunInHand()))
                {
                    _inventoryGun.GiveGunInHand(gun);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool GiveGunInBack(Gun gun)
        {
            if (GetGunInBack() == null)
            {
                _inventoryGun.GiveGunInBack(gun);
                return true;
            }
            else
            {
                if (AddInBackpack(GetGunInBack()))
                {
                    _inventoryGun.GiveGunInBack(gun);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        internal AliveSkelet(string systemName, ISomePicture alivePicture, GamePoint point, bool isClarity, IMemoryAction memoryAction, IBoxElement boxElement,
            int health, IFractionElement fractionElement, IHaveGun inventoryGun, Gun defaultgun, string name, string secondName)
            : base(systemName, alivePicture, point, isClarity, memoryAction, boxElement)
        {
            _health = new HealthSkelet(health, health);
            _fractionElement = fractionElement;
            _inventoryGun = inventoryGun;
            _defaultGun = defaultgun;
            Name = name;
            SecondName = secondName;
        }
        internal AliveSkelet(string systemNamePicture, GamePoint point, bool isClarity, IMemoryAction memoryAction, IBoxElement boxElement,
            int health, IFractionElement fractionElement, IHaveGun inventoryGun, Gun defaultgun, string name, string secondName)
            : base(systemNamePicture,
                  new AlivePicture(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{systemNamePicture}-map.png"),
                  System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesPlayer"], $"{systemNamePicture}-map-dead.png")),
            point, isClarity, memoryAction, boxElement)
        {
            _health = new HealthSkelet(health, health);
            _fractionElement = fractionElement;
            _inventoryGun = inventoryGun;
            _defaultGun = defaultgun;
            Name = name;
            SecondName = secondName;
        }
    }
    public class Skelet : AliveSkelet
    {
        public Skelet(string systemNamePicture, GamePoint point, bool isClarity, int inventorySizeH, int inventorySizeW,
            int health, NPSGroup fraction, Gun gun, string name, string secondName)
            : base(systemNamePicture, point, isClarity, new MemoryAction(), new Inventory(inventorySizeH, inventorySizeW, new List<Item>(0)),
                health, new MemoryFraction(fraction), new InventoryGun(), gun, name, secondName)
        {
        }
        public Skelet(string systemNamePicture, GamePoint point, bool isClarity, int inventorySizeH, int inventorySizeW, List<Item> items,
            int health, NPSGroup fraction, Gun gun, string name, string secondName)
            : base(systemNamePicture, point, isClarity, new MemoryAction(), new Inventory(inventorySizeH, inventorySizeW, items),
                health, new MemoryFraction(fraction), new InventoryGun(), gun, name, secondName)
        {
        }
    }
}
