using System;
using System.Collections.Generic;

namespace TwoD_Game_RP
{
    public class Phrase : IComparable<Phrase>
    {
        public string SystemName { get; set; }
        public string Text { get; set; }
        public List<string> NextSystemNames { get; set; }
        public List<string> ComplitedTaskSystemNames { get; set; }
        public Phrase(string systemName, string text, List<string> nextSystemNames, List<string> complitedTaskSystemNames)
        {
            SystemName = systemName;
            Text = text;
            NextSystemNames = nextSystemNames;
            ComplitedTaskSystemNames = complitedTaskSystemNames;
        }
        public int CompareTo(Phrase other)
        {
            return SystemName.CompareTo(other.SystemName);
        }
    }
}
