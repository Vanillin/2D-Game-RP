namespace TwoD_Game_RP
{
    public class SmallToz : Gun
    {
        public SmallToz() : base("Ружьё", "smallToz", 4, 4, 0, 1, 2)  { }
    }
    public class Knife : Gun
    {
        public Knife() : base("Нож", "knife", 2, 1, 0, 1, 1) { }
    }
    public class ScorpionGun : Gun
    {
        public ScorpionGun() : base("", "", 4, 1, 0, 1, 1) { }
    }
    public class Hand : Gun
    {
        public Hand() : base("", "", 1, 1, 0, 1, 1) { }
    }
}
