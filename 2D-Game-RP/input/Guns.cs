using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{
    public class SomeGun : Gun
    {
        public int Healthing { get; set; }
        public override void Using(Skelet skelet)
        {
            skelet.Healthing(Healthing);
        }
        public SomeGun() : base("Предмет", "gun", 0, 1, 500, 1, 1,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"gun.png")))
        { }
    }
}
