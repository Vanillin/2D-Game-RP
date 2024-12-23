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
        public static CustomSortedEnum<Task> GetTasks(int lengthDescription)
        {
            CustomSortedEnum<Task> retur = new CustomSortedEnum<Task>();
            foreach (var task in ReadTasks("Tasks"))
            {
                task.Description = CutString(task.Description, lengthDescription);
                retur.Add(task);
            }
            return retur;
        }
        public static CustomSortedEnum<(string, string)> GetTaskConnection()
        {
            CustomSortedEnum<(string, string)> retur = new CustomSortedEnum<(string, string)>();
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
        public static void Serialization()
        {
            (string, string)[] connect = new (string, string)[] { ( "a", "b" ) };
            using (var file = new FileStream("serialization.txt", FileMode.Create))
            {
                var xml = new XmlSerializer(typeof((string, string)[]), new Type[] { typeof((string, string)) });
                xml.Serialize(file, connect);
            }
        }
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
