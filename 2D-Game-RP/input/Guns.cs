using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{
    public enum Guns
    {
        Clutch,
        Toz34,
        Ak74ukorot,
        MP5,
        Tentacles,
    }
    public class Toz34 : Gun
    {
        public Toz34() : base("Тоз 34", "Toz34", 60, 4, 1200, 1, 3,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"Toz34.png"))) { }
    }
    public class Ak74ukorot : Gun
    {
        public Ak74ukorot() : base("АК 47 (Складной)", "Ak74ukorot", 45, 6, 2300, 1, 3,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"Ak74ukorot.png"))) { }
    }
    public class Clutch : Gun
    {
        public Clutch() : base("", "", 55, 1, 0, 1, 1,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"emptyitem.png"))) { }
    }
    public class Tentacles : Gun
    {
        public Tentacles() : base("", "", 80, 1, 0, 1, 1,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"emptyitem.png"))) { }
    }
    public class MP5 : Gun
    {
        public MP5() : base("МП-5", "MP5", 40, 7, 1900, 1, 2,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"MP5.png"))) { }
    }
}
