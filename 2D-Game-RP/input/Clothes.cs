using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{
    public enum Clothes
    {
        KurtkaStalker,
        CombezStalker,
        CombezNaemnik,
        MutantSkin,
    }
    public class KurtkaStalker : Cloth
    {
        public KurtkaStalker() : base("Куртка сталкера-новичка", "KurtkaStalker", 500, 5, NPSGroup.Stalker, 3, 2, 
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"KurtkaStalker.png"))) { }
    }
    public class CombezStalker : Cloth
    {
        public CombezStalker() : base("Сталкерский комбинезон Заря", "CombezStalker", 500, 10, NPSGroup.Stalker, 3, 2,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"CombezStalker.png")))
        { }
    }
    public class CombezNaemnik : Cloth
    {
        public CombezNaemnik() : base("Комбинезон наёмника", "CombezNaemnik", 500, 10, NPSGroup.Naemnik, 3, 2,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"CombezNaemnik.png")))
        { }
    }
    public class ExoCombezNaemnik : Cloth
    {
        public ExoCombezNaemnik() : base("Экзоскелет наёмника", "ExoCombezNaemnik", 500, 20, NPSGroup.Naemnik, 3, 2,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"ExoCombezNaemnik.png")))
        { }
    }
    public class MutantSkinCloth : Cloth
    {
        public override void Using(Skelet skelet)
        { }
        public MutantSkinCloth() : base("Шкура мутанта", "MutantSkin", 0, 0, NPSGroup.Mutant, 1, 1,
            new StaticPicCell(System.IO.Path.Combine(ConfigurationManager.AppSettings["TexturesItems"], $"MutantSkin.png")))
        { }
    }
}
