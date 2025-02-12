using System.Configuration;

namespace TwoD_Game_RP
{
    public class SmallToz : Gun
    {
        public SmallToz() : base("Ружьё", "smallToz", 4, 4, 0, 1, 2,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"System/ShootGun.png")))
        { }
    }
    public class Knife : Gun
    {
        public Knife() : base("Нож", "knife", 2, 1, 0, 1, 1,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"System/Shoot.png")))
        { }
    }
    public class ScorpionGun : Gun
    {
        public ScorpionGun() : base("", "", 4, 1, 0, 1, 1,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"System/Shoot.png")))
        { }
    }
    public class Hand : Gun
    {
        public Hand() : base("", "", 1, 1, 0, 1, 1,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"System/Shoot.png")))
        { }
    }

}
