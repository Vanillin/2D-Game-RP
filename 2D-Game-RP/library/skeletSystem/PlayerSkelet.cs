using System.Collections.Generic;

namespace TwoD_Game_RP
{
    public enum PlayerGender
    {
        Man,
        Woman,
    }
    public enum Actuals
    {
        Mechanic,
        Sniper
    }
    public abstract class PlayerSkelet : AliveSkelet
    {
        public IMemoryTask Tasks { get; set; }
        public PlayerGender Gender { get; }
        public List<Actuals> Actuals { get; }
        public void AddActual(Actuals actuals) => Actuals.Add(actuals);

        internal PlayerSkelet(string systemNamePicture, GamePoint point, bool isClarity, IMemoryAction memoryAction, IBoxElement boxElement,
            int health, IFractionElement fractionElement, IHaveGun inventoryGun, string name, string secondName, PlayerGender gender, IMemoryTask tasks)
            : base(systemNamePicture, point, isClarity, memoryAction, boxElement,
                health, fractionElement, inventoryGun, name, secondName)
        {
            Gender = gender;
            Tasks = tasks;
            Actuals = new List<Actuals>();
        }

    }
    public class Player : PlayerSkelet
    {
        public Player(string systemNamePicture, GamePoint point, bool isClarity, int inventoryHeight, int inventoryWight, List<Item> items,
                int health, NPSGroup fraction, string name, string secondName, PlayerGender gender, CustomSortedEnum<string> startTask)
            : base(systemNamePicture, point, isClarity, new MemoryAction(), new Inventory(inventoryHeight, inventoryWight, items),
                health, new MemoryFraction(fraction), new InventoryGun(), name, secondName, gender, new TaskBoard(Information.GetTasks(70), Information.GetTaskConnection(), startTask))
        { }

    }
}
