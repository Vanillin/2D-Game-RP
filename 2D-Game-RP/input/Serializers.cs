﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;
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

        //==================================================================================================
        //==============================+==       Tasks        =============================================
        //==================================================================================================

        static CustomSortedEnum<DescriptionTask> _memoryDescTask = new CustomSortedEnum<DescriptionTask>();
        public static CustomSortedEnum<GeneralTask> GetTasks(int lengthDescription)
        {
            CustomSortedEnum<GeneralTask> retur = new CustomSortedEnum<GeneralTask>();
            using (StreamReader sr = new StreamReader(Path.Combine(ConfigurationManager.AppSettings["Scripts"], "AllTask.txt")))
            {
                bool IsOk = ReadInformationTask(sr, out CustomDictionary<string, string> inform);
                while (IsOk)
                {
                    switch (DeleteSpace( inform["class"]))
                    {
                        case "Trigger":
                            {
                                if (!inform.ContainsKey("systemName"))
                                    throw new CustomException("Not find systemName in Trigger");
                                retur.Add(new Trigger(DeleteSpace( inform["systemName"])));
                                break;
                            }
                        case "TriggerImp":
                            {
                                if (!inform.ContainsKey("systemName"))
                                    throw new CustomException("Not find systemName in TriggerImp");
                                retur.Add(new TriggerImp(DeleteSpace(inform["systemName"])));
                                break;
                            }
                        case "Task":
                            {
                                if (!inform.ContainsKey("systemName"))
                                    throw new CustomException("Not find systemName in Task");
                                DescriptionTask desc;
                                if (inform.ContainsKey("name") && inform.ContainsKey("description"))
                                    desc = new DescriptionTask(null, CutString( GetStringInBkt( inform["name"]), lengthDescription), CutString( GetStringInBkt( inform["description"]), lengthDescription));
                                else if (inform.ContainsKey("systemNameDescription"))
                                    desc = FindDescriptionTask(DeleteSpace( inform["systemNameDescription"]));
                                else
                                    throw new CustomException("Not find name and description or systemNameDescription in Task");
                                List<string> eachOther = null;
                                if (inform.ContainsKey("eachOtherName"))
                                {
                                    eachOther = new List<string>();
                                    eachOther.AddRange(inform["eachOtherName"].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                                }
                                retur.Add(new Task(DeleteSpace( inform["systemName"]), desc, eachOther));
                                break;
                            }
                        case "TaskHid":
                            {
                                if (!inform.ContainsKey("systemName"))
                                    throw new CustomException("Not find systemName in TaskHid");
                                List<string> eachOther = null;
                                if (inform.ContainsKey("eachOtherName"))
                                {
                                    eachOther = new List<string>();
                                    eachOther.AddRange(inform["eachOtherName"].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                                }
                                retur.Add(new TaskHid(DeleteSpace( inform["systemName"]), eachOther));
                                break;
                            }
                        case "Description":
                            {
                                if (inform.ContainsKey("systemName") && inform.ContainsKey("name") && inform.ContainsKey("description"))
                                    _memoryDescTask.Add(new DescriptionTask(DeleteSpace( inform["systemName"]), CutString( GetStringInBkt( inform["name"]), lengthDescription), CutString( GetStringInBkt( inform["description"]), lengthDescription)));
                                else
                                    throw new CustomException("Not find systemname, name and description in Description");
                                break;
                            }
                    }

                    IsOk = ReadInformationTask(sr, out inform);
                }
            }            

            return retur;
        }
        private static string ReadLineStreamReader(StreamReader sr)
        {
            while (!sr.EndOfStream)
            {
                string str = sr.ReadLine();
                if (str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length == 0)
                    continue;
                if (SubstringSearch.Creating().CheckSubstring(str, "///"))
                    continue;
                return str;
            }
            return null;
        }
        private static bool ReadInformationTask(StreamReader sr, out CustomDictionary<string, string> information)
        {
            information = new CustomDictionary<string, string>();
            string str = ReadLineStreamReader(sr);
            if (str == null) return false;
            while (!SubstringSearch.Creating().CheckSubstring(str, "end"))
            {
                string key = "";
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] != ' ')
                        key += str[i];
                    else
                    {
                        information.Add(key, str.Substring(key.Length));
                        break;
                    }
                }
                str = ReadLineStreamReader(sr);
                if (str == null) return false;
            }
            return true;
        }
        private static string GetStringInBkt(string str)
        {
            string retur = "";
            bool InBkt = false;
            foreach (char c in str)
            {
                if (c == '<') InBkt = true;
                else if (c == '>') return retur;
                else if (InBkt) retur += c;
            }
            throw new CustomException("Not find simbol >");
        }
        private static string DeleteSpace(string str)
        {
            return str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
        }
        private static DescriptionTask FindDescriptionTask(string systemNameDesc)
        {
            foreach (var desc in _memoryDescTask)
            {
                if (desc.SystemName == systemNameDesc)
                {
                    return desc;
                }
            }
            throw new CustomException("Not find DescriptionTask to systemNameDescription");
        }
        public static CustomSortedEnum<(string, string)> GetTaskConnection()
        {
            CustomSortedEnum<(string, string)> connect = new CustomSortedEnum<(string, string)>();
            using (StreamReader sr = new StreamReader(Path.Combine(ConfigurationManager.AppSettings["Scripts"], "AllConnectionTask.txt")))
            {
                string str = ReadLineStreamReader(sr);
                while (str != null)
                {
                    var pair = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    connect.Add((pair[0], pair[1]));

                    str = ReadLineStreamReader(sr);
                }
            }
            return connect;
        }

        //=====================================================================================================
        //=====================================================================================================
        //=====================================================================================================

        private static string CutString(string s, int length)
        {
            string retur = "";
            string[] words = s.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
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
