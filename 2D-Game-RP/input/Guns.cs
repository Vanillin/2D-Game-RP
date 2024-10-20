using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_STALKER_Exclusion_Zone
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
        public Toz34() : base("Тоз 34", "Toz34", 60, 4, 1200) { }
    }
    public class Ak74ukorot : Gun
    {
        public Ak74ukorot() : base("АК 47 (Складной)", "Ak74ukorot", 45, 6, 2300) { }
    }
    public class Clutch : Gun
    {
        public Clutch() : base("", "", 55, 1, 0) { }
    }
    public class Tentacles : Gun
    {
        public Tentacles() : base("", "", 80, 1, 0) { }
    }
    public class MP5 : Gun
    {
        public MP5() : base("МП-5", "MP5", 40, 7, 1900) { }
    }
}
