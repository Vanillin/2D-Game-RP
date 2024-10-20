using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Game_STALKER_Exclusion_Zone
{
    public class Information
    {
        public static List<string> Blocks = new List<string> { "д", "о", "с", "у", "ф", "я"};
        public static List<string> NotWatch = new List<string> { "д", "к", "с", "у", "ф", "я" };

        public List<string> NameRandom = new List<string>
        {
            "Петя", "Колян", "Митя", "Дэн", "Витя", "Гога",
            "Петя", "Колян", "Митя", "Дэн", "Витя", "Гога"
        };
        public List<string> NicknameRandom = new List<string>
        {
            "Тухлый", "Трезвый", "Лопух", "Шустрый", "Крематорий",
            "Пёс", "Повар", "Лимон", "Табуретка", "Снайпер", "Козырь"
        };
        const int sizeX = 19;
        const int sizeY = 31;

        public Task[] TasksInGame = ReadTasks("Name Tasks");
        public Phrase[] PhraseInGame = ReadPhrases("Name Phrases");

        private static Location Hub;
        private static Location Zavod;
        private static Location Boloto;
        private static Location Electr;

        public static Location GetLocationHub()
        {
            if (Hub == null) {
                Hub = new Location("Деревня сталкеров", "hab", sizeX, sizeY);
                Hub.AddLayerCells(CreateLocation("Earth Hab"), 0);
                Hub.AddLayerCells(CreateLocation("Air Hab"), 3);
                Hub.CreateGrafMove();
                Hub.CreateGrafWatch(); 
            }
            return Hub;
        }
        public static Location GetLocationZavod()
        {
            if (Zavod == null)
            {
                Zavod = new Location("Заброшенный завод", "zavod", sizeX, sizeY);
                Zavod.AddLayerCells(CreateLocation("Earth Zavod"), 0);
                Zavod.AddLayerCells(CreateLocation("Air Zavod"), 3);
                Zavod.CreateGrafMove();
                Zavod.CreateGrafWatch();
            }
            return Zavod;
        }
        public static Location GetLocationBoloto()
        {
            if (Boloto == null)
            {
                Boloto = new Location("Топи", "boloto", sizeX, sizeY);
                Boloto.AddLayerCells(CreateLocation("Earth Boloto"), 0);
                Boloto.AddLayerCells(CreateLocation("Air Boloto"), 3);
                Boloto.CreateGrafMove();
                Boloto.CreateGrafWatch();
            }
            return Boloto;
        }
        public static Location GetLocationElectr()
        {
            if (Electr == null)
            {
                Electr = new Location("Поле Электр", "electr", sizeX, sizeY);
                Electr.AddLayerCells(CreateLocation("Earth Electr"), 0);
                Electr.AddLayerCells(CreateLocation("Air Electr"), 3);
                Electr.CreateGrafMove();
                Electr.CreateGrafWatch();
            }
            return Electr;
        }

        private static Task[] ReadTasks(string nameTask)
        {
            Task[] tas;
            using (var file = new FileStream(Path.Combine(ConfigurationManager.AppSettings["Scripts"], nameTask + ".txt"), FileMode.Open))
            {
                var xml = new XmlSerializer(typeof(Task[]), new Type[] { typeof(Task) });
                tas = (Task[])xml.Deserialize(file);
            }
            return tas;
        }
        private static Phrase[] ReadPhrases(string namePhrase)
        {
            Phrase[] phrase;
            using (var file = new FileStream(Path.Combine(ConfigurationManager.AppSettings["Scripts"], namePhrase + ".txt"), FileMode.Open))
            {
                var xml = new XmlSerializer(typeof(Phrase[]), new Type[] { typeof(Phrase) });
                phrase = (Phrase[])xml.Deserialize(file);
            }
            return phrase;
        }
        private static IPictureCell[,] CreateLocation(string nameloca)
        {
            string[] loca;
            using (var file = new FileStream(Path.Combine(ConfigurationManager.AppSettings["Levels"], nameloca + ".txt"), FileMode.Open))
            {
                var xml = new XmlSerializer(typeof(string[]));
                loca = (string[])xml.Deserialize(file);
            }

            int x = loca.Length;
            int y = loca[0].Length;
            IPictureCell[,] retur = new IPictureCell[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (loca[i][j] == '-' || loca[i][j] == ' ') continue;
                    retur[i, j] = new StaticPicCell(   Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"{loca[i][j]}.png")   );
                }
            }         

            return retur;
        }
    }
}
