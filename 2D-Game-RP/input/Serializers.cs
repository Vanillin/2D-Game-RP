using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace TwoD_Game_RP
{
    public class Information
    {
        public static List<string> Blocks = new List<string> { "Wall", "Window" };
        public static List<string> NotWatch = new List<string> { "Wall", "Shrub" };

        //public List<string> NameRandom = new List<string>
        //{
        //    "Петя", "Колян", "Митя", "Дэн", "Витя", "Гога",
        //    "Петя", "Колян", "Митя", "Дэн", "Витя", "Гога"
        //};
        //public List<string> NicknameRandom = new List<string>
        //{
        //    "Тухлый", "Трезвый", "Лопух", "Шустрый", "Крематорий",
        //    "Пёс", "Повар", "Лимон", "Табуретка", "Снайпер", "Козырь"
        //};

        //public static Task[] TasksInGame = ReadTasks("Tasks");

        //public static Task FindTask(string Systemname)
        //{
        //    for (int i = 0; i < TasksInGame.Length; i++)
        //    {
        //        if (TasksInGame[i].SystemName == Systemname)
        //        {
        //            return TasksInGame[i];
        //        }
        //    }
        //    throw new Exception("Не найдено задание по системному имени");
        //}

        public static void CreateDarkenPicCell()
        {
            if (DarkenPicCell.Taking() == null)
            {
                DarkenPicCell.Creating(Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"System/Dark.png"));
            }
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
                Garden.AddLayerCells(CreateLocation("GardenObject1"), 0);
                Garden.AddLayerCells(CreateLocation("GardenAir"), 3);
                Garden.CreateGrafWatch();
                Garden.CreateGrafMove();
                Garden.CreateGrafAll();

                Garden.AddBoxOrAnomalyWithCell(new Door(new GamePoint(4, 14), '0', false));
                Garden.AddBoxOrAnomalyWithCell(new Door(new GamePoint(5, 9), '0', false));
                Garden.AddBoxOrAnomalyWithCell(new Door(new GamePoint(7, 11), '1', false));
                Garden.AddBoxOrAnomalyWithCell(new Door(new GamePoint(9, 9), '0', false));
                Garden.AddBoxOrAnomalyWithCell(new Door(new GamePoint(14, 15), '0', true));
                Garden.AddBoxOrAnomalyWithCell(new Door(new GamePoint(16, 17), '1', false));

                Garden.UpdateDisplay();
            }
            return Garden;
        }
        private static PhrasesStart phrasesStart;
        public static List<string> GetStartPhrases(string systemname)
        {
            if (phrasesStart == null)
            {
                using (var file = new FileStream(Path.Combine(ConfigurationManager.AppSettings["Scripts"], "Phrases.txt"), FileMode.Open))
                {
                    var xml = new XmlSerializer(typeof(PhrasesStart));
                    phrasesStart = (PhrasesStart)xml.Deserialize(file);
                }
            }
            return phrasesStart.GetStartDialogs(systemname);
        }
        public static Phrase[] GetPhrase(string sstemnameDialogPerson, int length)
        {
            var allphrases = ReadPhrases($"Phrases_{sstemnameDialogPerson}");
            foreach (var ph in allphrases)
            {
                ph.Dialog = CutString(ph.Dialog, length);
            }
            return allphrases;
        }
        public static SortedEnum<Task> GetTasks()
        {
            SortedEnum<Task> retur = new SortedEnum<Task>();
            foreach (var task in ReadTasks("Tasks"))
            {
                retur.Add(task);
            }
            return retur;
        }
        public static SortedEnum<(string, string)> GetTaskConnection()
        {
            SortedEnum<(string, string)> retur = new SortedEnum<(string, string)>();
            foreach (var pair in ReadTaskConnection("TaskConnections"))
            {
                retur.Add(pair);
            }
            return retur;
        }
        private static string CutString(string s, int length)
        {
            string retur = "";
            string[] words = s.Split(new char[] {' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            foreach (string word in words)
            {
                if (i + word.Length > length)
                {
                    retur += '\n';
                    i = 0;
                }
                retur += word + ' ';
                i += word.Length + 1;
            }
            return retur;
        }
        private static (string, string)[] ReadTaskConnection(string nameTaskConnect)
        {
            (string, string)[] connect;
            using (var file = new FileStream(Path.Combine(ConfigurationManager.AppSettings["Scripts"], nameTaskConnect + ".txt"), FileMode.Open))
            {
                var xml = new XmlSerializer(typeof((string, string)[]), new Type[] { typeof((string, string)) });
                connect = ((string, string)[])xml.Deserialize(file);
            }
            return connect;
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
                        retur[i, j] = new StaticPicCell(Path.Combine(ConfigurationManager.AppSettings["TexturesMap"], $"{dict[pict]}.png"));
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
            (string, string)[] connect = new (string, string)[] { ( "a", "b" ) };
            using (var file = new FileStream("serialization.txt", FileMode.Create))
            {
                var xml = new XmlSerializer(typeof((string, string)[]), new Type[] { typeof((string, string)) });
                xml.Serialize(file, connect);
            }


            //Task task = new Task("a", "b", "c", "d", false);
            //using (var file = new FileStream("serialization.txt", FileMode.Create))
            //{
            //    var xml = new XmlSerializer(typeof(Task));
            //    xml.Serialize(file, task);
            //}

            //PhrasesStart phrasesStart = new PhrasesStart();
            //phrasesStart.values = new List<(string systemname, List<string> startdialogs)>()
            //{
            //    ("Kristina",  new List<string> {"start1", "start2" }),
            //    ("Maksim",  new List<string> {"start3", "start4" })
            //};

            //using (var file = new FileStream("serialization.txt", FileMode.Create))
            //{
            //    var xml = new XmlSerializer(typeof(PhrasesStart));
            //    xml.Serialize(file, phrasesStart);
            //}

            //Phrase rl = new Phrase();
            //rl.Index = "Ind";
            //rl.Dialog = "Dia";
            //rl.NextIndexes = new List<string>() { "Ind1", "Ind2" };

            //Phrase[] phrases = new Phrase[] { rl };

            //using (var file = new FileStream("serialization.txt", FileMode.Create))
            //{
            //    var xml = new XmlSerializer(typeof(Phrase[]), new Type[] { typeof(Phrase) });
            //    xml.Serialize(file, phrases);
            //}

            //ReadLocation rl = new ReadLocation();
            //rl.height = 5;
            //rl.wight = 3;
            //rl.location = new string[] { "abc", "abc", "abc", "abc", "abc" };
            //rl.description = new List<(string, string)> { ("a", "aaa"), ("b", "aaa"), ("c", "aaa") };
            //rl.rotation = new List<string> { "111", "222" };

            //using (var file = new FileStream("serialization.txt", FileMode.Create))
            //{
            //    var xml = new XmlSerializer(typeof(ReadLocation));
            //    xml.Serialize(file, rl);
            //}
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
    public class PhrasesStart
    {
        public List<(string systemname, List<string> startdialogs)> values;
        public List<string> GetStartDialogs(string systemname)
        {
            foreach (var v in values)
            {
                if (v.systemname == systemname)
                    return v.startdialogs;
            }
            throw new Exception("Systemname не существует");
        }
    }
}
