using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{
    public class SomeItem : Item
    {
        public int Healthing { get; set; }
        public override void Using(Skelet skelet)
        {
            //skelet.Healthing(Healthing);
        }
        public SomeItem() : base("Вещь", "item", 500, 1, 1,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"item.png")))
        { }
    }
}
