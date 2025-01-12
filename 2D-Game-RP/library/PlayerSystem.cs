using System.Collections.Generic;

namespace TwoD_Game_RP
{
    public enum PlayerGender
    {
        Man,
        Woman,
    }
    public abstract class Player : Skelet
    {
        public TaskBoard Tasks { get; set; }
        public PlayerGender Gender { get; }

        public Player(string name, string systemName, PlayerGender gender, GamePoint coord, int inventoryHeight, int inventoryWight, List<Item> inventoryList, TaskBoard tasks) :
            base(name, "", NPSGroup.People, NPSIntellect.Non, coord, 0, systemName, inventoryList, inventoryHeight, inventoryWight, true, 10)
        {
            Gender = gender;
            Tasks = tasks;
        }
    }
}

