using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace TwoD_Game_RP
{    
    public enum PlayerGender
    {
        Man,
        Woman,
    }
    
    public class Player : Skelet
    {
        public List<Task> Tasks = new List<Task>();
        public List<Task> CompliteTasks = new List<Task>();
        public readonly PlayerGender Gender;


        public Player(string name, PlayerGender gender, GamePoint coord, Gun gun, Cloth cloth, int money, List<Item> inventoryList, List<NPSGroup> friends) :
            base(name, "", NPSGroup.Stalker, gun, NPSIntellect.Non, coord, '0', cloth, "player", money, inventoryList, friends, 20000, true) //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  "Player" + gender
        {
            this.Gender = gender;
        }
    }
}

