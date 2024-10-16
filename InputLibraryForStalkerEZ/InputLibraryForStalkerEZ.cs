using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;
using LibraryForStalkerEZ;

namespace InputLibraryForStalkerEZ
{
    public class Information
    {
        public List<string> BlocksToGoing = new List<string> { "з", "п", "т", "к", "р", "ю"};
        public List<string> BlocksToWatch = new List<string> { "з", "п", "т", "р", "ю", "а", "в"};
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

        public Task[] TasksInGame = ReadTasks("Name Tasks");
        public Phrase[] PhraseInGame = ReadPhrases("Name Phrases");

        public string NameLocationHab = "Деревня сталкеров";
        public string[,] LocationHab = CreateLocation("Earth Hab");
        public string[,] AirHab = CreateLocation("Air Hab");

        public string NameLocationZavod = "Заброшенный завод";
        public string[,] LocationZavod = CreateLocation("Earth Zavod");
        public string[,] AirZavod = CreateLocation("Air Zavod");

        public string NameLocationBoloto = "Топи";
        public string[,] LocationBoloto = CreateLocation("Earth Boloto");
        public string[,] AirBoloto = CreateLocation("Air Boloto");

        public string NameLocationElectr = "Поле Электр";
        public string[,] LocationElectr = CreateLocation("Earth Electr");
        public string[,] AirElectr = CreateLocation("Air Electr");


        private static Task[] ReadTasks(string nameTask)
        {
            Task[] tas;
            using (var file = new FileStream(Path.Combine("gamedata/scripts", nameTask + ".txt"), FileMode.Open))
            {
                var xml = new XmlSerializer(typeof(Task[]), new Type[] { typeof(Task) });
                tas = (Task[])xml.Deserialize(file);
            }
            return tas;
        }
        private static Phrase[] ReadPhrases(string namePhrase)
        {
            Phrase[] phrase;
            using (var file = new FileStream(Path.Combine("gamedata/scripts", namePhrase + ".txt"), FileMode.Open))
            {
                var xml = new XmlSerializer(typeof(Phrase[]), new Type[] { typeof(Phrase) });
                phrase = (Phrase[])xml.Deserialize(file);
            }
            return phrase;
        }
        private static string[,] CreateLocation(string nameloca)
        {
            string[] loca;
            using (var file = new FileStream(Path.Combine("gamedata/levels", nameloca + ".txt"), FileMode.Open))
            {
                var xml = new XmlSerializer(typeof(string[]));
                loca = (string[])xml.Deserialize(file);
            }

            int x = loca.Length;
            int y = loca[0].Length;
            string[,] retur = new string[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    retur[i, j] = loca[i][j].ToString();
                }
            }

            //Task[] TasksIndex2 = new Task[] { new Task("sysname", "name", "secondName") }; 
            //using (var file = new FileStream(Path.Combine("gamedata/scripts", "Name Tasks.txt"), FileMode.Create))
            //{
            //    var xml = new XmlSerializer(typeof(Task[]), new Type[] { typeof(Task)});
            //    xml.Serialize(file, TasksIndex2);
            //}

            //< add key = "Dictionary" value = "Lib" />
            //< add key = "Gamedata" value = "gamedata" />
            //< add key = "Configs" value = "gamedata/configs" />
            //< add key = "Levels" value = "gamedata/levels" />
            //< add key = "Textures" value = "gamedata/textures" />
            //< add key = "Scripts" value = "gamedata/scripts" />

            //using (var file = new FileStream("File.txt", FileMode.Open))
            //{
            //    var xml = new XmlSerializer(typeof(List<CatalogItem>), new Type[] { typeof(CatalogItem) });
            //    var tasks = (List<CatalogItem>)xml.Deserialize(file);  
            //}            

            return retur;
        }
    }
}
