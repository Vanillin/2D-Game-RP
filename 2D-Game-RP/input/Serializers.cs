using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace TwoD_Game_RP
{
    public class DialogInform
    {
        public List<(string, string)> StartedTasksPerson { get; }
        public List<(string, string)> StartedTasksPlayer { get; }
        public CustomDictionary<string, Phrase> Phrases { get; }
        public DialogInform(List<(string, string)> startPers, List<(string, string)> startplayer, CustomDictionary<string, Phrase> phrases)
        {
            StartedTasksPerson = startPers;
            StartedTasksPlayer = startplayer;
            Phrases = phrases;
        }
    }
    public class Information
    {

        public static List<string> Blocks = new List<string> { "Wall", "Window" };
        public static List<string> NotWatch = new List<string> { "Wall", "Shrub" };

        //==================================================================================================
        //=================================      Location      =============================================
        //==================================================================================================

        public static ReadLocation GetLocation(string namelocation)
        {
            CustomDictionary<char, string> description = new CustomDictionary<char, string>();
            List<string> location = new List<string>();
            List<string> rotation = new List<string>();
            using (StreamReader sr = new StreamReader(Path.Combine(ConfigurationManager.AppSettings["Levels"], namelocation + ".txt")))
            {
                string str;
                str = ReadLineStreamReader(sr);
                while (DeleteSpace(str) != "start")
                {
                    string[] desc = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    description.Add(desc[0][0], desc[1]);

                    str = ReadLineStreamReader(sr);
                }
                str = ReadLineStreamReader(sr);
                while (DeleteSpace(str) != "end")
                {
                    location.Add(DeleteSpace(str));
                    str = ReadLineStreamReader(sr);
                }
                str = ReadLineStreamReader(sr);
                while (DeleteSpace(str) != "end")
                {
                    rotation.Add(DeleteSpace(str));
                    str = ReadLineStreamReader(sr);
                }
            }
            return new ReadLocation(location.Count, location[0].Length, description, location, rotation);
        }

        //==================================================================================================
        //=============================       Tasks and Phrases     ========================================
        //==================================================================================================

        static CustomSortedEnum<DescriptionTask> _memoryDescTask = new CustomSortedEnum<DescriptionTask>();
        public static CustomSortedEnum<GeneralTask> GetTasks(int lengthDescription)
        {
            CustomSortedEnum<GeneralTask> retur = new CustomSortedEnum<GeneralTask>();
            using (StreamReader sr = new StreamReader(Path.Combine(ConfigurationManager.AppSettings["Scripts"], "AllTask.txt")))
            {
                bool IsOk = ReadKeyValueInformation(sr, out CustomDictionary<string, string> inform);
                while (IsOk)
                {
                    switch (DeleteSpace(inform["class"]))
                    {
                        case "Trigger":
                            {
                                if (!inform.ContainsKey("systemName"))
                                    throw new CustomException("Not find systemName in Trigger");
                                retur.Add(new Trigger(DeleteSpace(inform["systemName"])));
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
                                    desc = new DescriptionTask(null, CutString(GetStringInBkt(inform["name"]), lengthDescription), CutString(GetStringInBkt(inform["description"]), lengthDescription));
                                else if (inform.ContainsKey("systemNameDescription"))
                                    desc = FindDescriptionTask(DeleteSpace(inform["systemNameDescription"]));
                                else
                                    throw new CustomException("Not find name and description or systemNameDescription in Task");
                                List<string> eachOther = null;
                                if (inform.ContainsKey("eachOtherName"))
                                {
                                    eachOther = new List<string>();
                                    eachOther.AddRange(inform["eachOtherName"].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                                }
                                retur.Add(new Task(DeleteSpace(inform["systemName"]), desc, eachOther));
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
                                retur.Add(new TaskHid(DeleteSpace(inform["systemName"]), eachOther));
                                break;
                            }
                        case "Description":
                            {
                                if (inform.ContainsKey("systemName") && inform.ContainsKey("name") && inform.ContainsKey("description"))
                                    _memoryDescTask.Add(new DescriptionTask(DeleteSpace(inform["systemName"]), CutString(GetStringInBkt(inform["name"]), lengthDescription), CutString(GetStringInBkt(inform["description"]), lengthDescription)));
                                else
                                    throw new CustomException("Not find systemname, name and description in Description");
                                break;
                            }
                        default:
                            {
                                throw new CustomException($"Not find name class {inform["class"]}");
                            }
                    }

                    IsOk = ReadKeyValueInformation(sr, out inform);
                }
            }

            return retur;
        }

        public static DialogInform GetPhrasesPerson(string skeletSystemName, int lengthDescription)
        {
            List<(string phrase, string task)> startedTasksPerson = new List<(string, string)>();
            List<(string phrase, string task)> startedTasksPlayer = new List<(string, string)>();
            CustomDictionary<string, Phrase> phrases = new CustomDictionary<string, Phrase>();
            using (StreamReader sr = new StreamReader(Path.Combine(ConfigurationManager.AppSettings["Scripts"], $"Phrases_{skeletSystemName}.txt")))
            {
                string str;
                str = ReadLineStreamReader(sr);
                while (DeleteSpace(str) != "end")
                {
                    string[] phraseTask = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    startedTasksPerson.Add((phraseTask[0], phraseTask[1]));

                    str = ReadLineStreamReader(sr);
                }

                str = ReadLineStreamReader(sr);
                while (DeleteSpace(str) != "end")
                {
                    string[] phraseTask = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    startedTasksPlayer.Add((phraseTask[0], phraseTask[1]));

                    str = ReadLineStreamReader(sr);
                }

                bool IsOk = ReadKeyValueInformation(sr, out CustomDictionary<string, string> inform);
                while (IsOk)
                {
                    if (!inform.ContainsKey("systemName") || !inform.ContainsKey("text"))
                        throw new CustomException("Not find systemName or text");

                    string text;
                    try
                    {
                        text = GetStringInBkt(inform["text"]);
                    }
                    catch (CustomException)
                    {
                        text = GetStringInBktWithEnter(inform["text"]);
                        bool IsRead = false;
                        int i = 1;
                        string newtext;
                        while (!IsRead)
                        {
                            try
                            {
                                newtext = GetStringInBkt(inform[$"text{i}"]);
                                text += newtext;
                                IsRead = true;
                            }
                            catch (CustomException)
                            {
                                newtext = GetStringInBktWithEnter(inform[$"text{i}"]);
                                text += newtext;
                                i++;
                            }
                        }
                    }

                    List<string> next = new List<string>();
                    if (inform.ContainsKey("nextSystemName"))
                    {
                        foreach (var v in GetStringInBkt(inform["nextSystemName"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            next.Add(v);
                        }
                    }
                    List<string> complite = new List<string>();
                    if (inform.ContainsKey("compliteTaskSystemName"))
                    {
                        foreach (var v in GetStringInBkt(inform["compliteTaskSystemName"]).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            complite.Add(v);
                        }
                    }
                    phrases.Add(DeleteSpace(inform["systemName"]), new Phrase(DeleteSpace(inform["systemName"]), CutString(text, lengthDescription), next, complite));

                    IsOk = ReadKeyValueInformation(sr, out inform);
                }
            }

            return new DialogInform(startedTasksPerson, startedTasksPlayer, phrases);
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
        private static bool ReadKeyValueInformation(StreamReader sr, out CustomDictionary<string, string> information)
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
        private static string GetStringInBktWithEnter(string str)
        {
            string retur = "";
            bool InBkt = false;
            foreach (char c in str)
            {
                if (c == '<') InBkt = true;
                else if (c == '>' || c == ']') return retur;
                else if (InBkt) retur += c;
            }
            throw new CustomException("Not find simbol >");
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
            return str.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[0];
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
        public static void Serialization()
        {
            (string, string)[] connect = new (string, string)[] { ("a", "b") };
            using (var file = new FileStream("serialization.txt", FileMode.Create))
            {
                var xml = new XmlSerializer(typeof((string, string)[]), new Type[] { typeof((string, string)) });
                xml.Serialize(file, connect);
            }
        }
    }
}
