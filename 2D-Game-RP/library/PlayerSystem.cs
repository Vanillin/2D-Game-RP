using System;
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
            base(name, "", NPSGroup.People, NPSIntellect.Non, coord, 0, systemName, inventoryList, inventoryHeight, inventoryWight, true, 15)
        {
            Gender = gender;
            Tasks = tasks;
        }
    }
    public abstract class Enemy : Skelet
    {
        private int _lenWatch;
        public int LenWatch => _lenWatch;
        public Enemy(string name, string secondName, NPSGroup fraction, NPSIntellect intellect, GamePoint coord, int rotate,
            string systemName, List<Item> inventoryList, int inventoryHeight, int inventoryWidth, bool isClarity, int health, int lenWatch) :
            base(name, secondName, fraction, intellect, coord, rotate, systemName, inventoryList, inventoryHeight, inventoryWidth, isClarity, health)
        {
            _lenWatch = lenWatch;
        }
        public override bool DoAction(Location location)
        {
            switch (Intellect)
            {
                case NPSIntellect.Non: break;
                case NPSIntellect.Attack:
                    {
                        if (_globalActions.Count != 0 && _globalActions.Peek() is ActionAttack)
                            break;
                        var OblWatch = location.GetWatchCirlce(Cord, _lenWatch - 0.1,
                            Math.Max((int)Cord.X - _lenWatch, 0), Math.Max((int)Cord.Y - _lenWatch, 0),
                            Math.Min((int)Cord.X + _lenWatch + 1, location.Height), Math.Min((int)Cord.Y + _lenWatch + 1, location.Width));
                        foreach (SystemSkelet skelet in location.GetLives())
                        {
                            if (skelet is Skelet && OblWatch.Contains(skelet.Cord))
                                if (!FriendFranction.Contains((skelet as Skelet).Fraction))
                                {
                                    EnqueueUpGlobalAction(new ActionAttack(skelet as Skelet, false));
                                    break;
                                }
                        }
                        break;
                    }
            }
            return base.DoAction(location);
        }
    }
}

