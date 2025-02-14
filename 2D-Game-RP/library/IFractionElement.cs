using System.Collections.Generic;

namespace TwoD_Game_RP
{
    public enum NPSGroup
    {
        People,
        Box,
        Door,
        Monster,
    }
    internal interface IFractionElement
    {
        NPSGroup Fraction { get; }
        List<NPSGroup> FriendFranction { get; }
        void AddFriendFraction(NPSGroup friend);
        void RemoveFriendFraction(NPSGroup friend);
    }
    internal class MemoryFraction : IFractionElement
    {
        private NPSGroup _fraction;
        public NPSGroup Fraction => _fraction;
        public MemoryFraction(NPSGroup fraction)
        {
            _fraction = fraction;
            FriendFranction = GetFriendFraction(fraction);
        }
        public List<NPSGroup> FriendFranction { get; set; }
        private List<NPSGroup> GetFriendFraction(NPSGroup fraction)
        {
            switch (fraction)
            {
                case NPSGroup.Monster: return new List<NPSGroup>() { NPSGroup.Monster };
                default: return new List<NPSGroup>() { NPSGroup.People };
            }
        }
        public void AddFriendFraction(NPSGroup friend)
        {
            FriendFranction.Add(friend);
        }
        public void RemoveFriendFraction(NPSGroup friend)
        {
            FriendFranction.Remove(friend);
        }
    }
}
