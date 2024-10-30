using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TwoD_Game_RP
{
    public class Information
    {
        public static List<string> Blocks = new List<string> { "Wall", "Window"};  //{ "д", "о", "с", "у", "ф", "я"};
        public static List<string> NotWatch = new List<string> { "Wall" };  //{ "д", "к", "с", "ю", "я" };

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

        public static Task[] TasksInGame = ReadTasks("Name Tasks");
        public Phrase[] PhraseInGame = ReadPhrases("Name Phrases");

        public static Task FindTask(string Systemname)
        {
            for (int i = 0; i < TasksInGame.Length; i++)
            {
                if (TasksInGame[i].SystemName == Systemname)
                {
                    return TasksInGame[i];
                }
            }
            throw new Exception("Не найдено задание по системному имени");
        }

        /*
        0 - земля
        1 - люди ящики
        2 - аномалии
        3 - деревья 
        4 - системные знаки
        */

        private static Location Garden;
        public static Location GetGardenLocation()
        {
            if (Garden == null)
            {
                Garden = new Location("Двор", "Garden", 21, 26);
                Garden.AddLayerCells(CreateLocation("GardenFloors"), 0);
                Garden.AddLayerCells(CreateLocation("GardenFloors2"), 0);
                Garden.AddLayerCells(CreateLocation("GardenFloors3"), 0);
                Garden.AddLayerCells(CreateLocation("GardenWalls"), 0);
                Garden.CreateGrafWatch();
                Garden.CreateGrafMove();
                Garden.UpdateDisplay();
            }
            return Garden;
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
            ReadLocation rl;
            using (var file = new FileStream(Path.Combine(ConfigurationManager.AppSettings["Levels"], nameloca + ".txt"), FileMode.Open))
            {
                var xml = new XmlSerializer(typeof(ReadLocation));
                rl = (ReadLocation)xml.Deserialize(file);
            }
            IPictureCell[,] retur = new IPictureCell[rl.height, rl.wight];
            Dictionary<char, string> dict = new Dictionary<char, string>();
            foreach (var v in rl.description) { dict.Add(v.Item1[0], v.Item2); }
            for (int i = 0; i < rl.height; i++)
            {
                for (int j = 0; j < rl.wight; j++)
                {
                    char pict = rl.location[i][j];
                    if (dict.ContainsKey(pict))
                    {
                        retur[i,j] = new StaticPicCell(Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"{dict[pict]}.png"));
                        switch (rl.rotation[i][j])
                        {
                            case '1': retur[i, j].Rotate90 = true; break;
                            case '2': retur[i, j].Rotate180 = true; break;
                            case '3': retur[i, j].Rotate270 = true; break;
                        }
                    }
                }
            }
            return retur;
        }
        public static void Serialization()
        {
            ReadLocation rl = new ReadLocation();
            rl.height = 5;
            rl.wight = 3;
            rl.location = new string[] { "abc", "abc", "abc", "abc", "abc" };
            rl.description = new List<(string, string)> { ("a", "aaa"), ("b", "aaa"), ("c", "aaa") };
            rl.rotation = new List<string> { "111", "222" };

            using (var file = new FileStream("serialization.txt", FileMode.Create))
            {
                var xml = new XmlSerializer(typeof(ReadLocation));
                xml.Serialize(file, rl);
            }
        }
    }
    public class ReadLocation
    {
        public List<(string c, string s)> description;
        public int height;
        public int wight;
        public string[] location;
        public List<string> rotation;
    }
}
