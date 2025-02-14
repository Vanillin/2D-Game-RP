using System;
using System.Collections.Generic;

namespace TwoD_Game_RP
{
    public class DescriptionTask : IComparable<DescriptionTask>
    {
        public string SystemName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DescriptionTask(string systemName, string name, string description)
        {
            Name = name;
            Description = description;
            SystemName = systemName;
        }
        public int CompareTo(DescriptionTask other)
        {
            return Name.CompareTo(other.Name);
        }
    }
    public abstract class GeneralTask : IComparable<GeneralTask>
    {
        internal List<string> _nextSystemTask;
        internal List<string> _prevSystemTask;
        internal List<string> _eachOtherExclusive;
        public string SystemName { get; }
        public DescriptionTask DescriptionTask { get; }
        public GeneralTask(string systemName, List<string> eachOtherExclusive, DescriptionTask descriptionTask, List<string> nextSystemTask, List<string> prevSystemTask)
        {
            _nextSystemTask = nextSystemTask;
            _prevSystemTask = prevSystemTask;
            _eachOtherExclusive = eachOtherExclusive;
            SystemName = systemName;
            DescriptionTask = descriptionTask;
        }
        public int CompareTo(GeneralTask other)
        {
            return SystemName.CompareTo(other.SystemName);
        }
    }
    internal class Trigger : GeneralTask
    {
        public Trigger(string systemName) : base(systemName, null, null, new List<string>(), new List<string>())
        { }
    }
    internal class TriggerImp : GeneralTask
    {
        public TriggerImp(string systemName) : base(systemName, null, null, new List<string>(), new List<string>())
        { }
    }
    internal class Task : GeneralTask
    {
        public Task(string systemName, DescriptionTask descriptionTask, List<string> eachOtherExclusive) : base(systemName, eachOtherExclusive, descriptionTask, new List<string>(), new List<string>())
        { }
    }
    internal class TaskHid : GeneralTask
    {
        public TaskHid(string systemName, List<string> eachOtherExclusive) : base(systemName, eachOtherExclusive, null, new List<string>(), new List<string>())
        { }
    }
}
