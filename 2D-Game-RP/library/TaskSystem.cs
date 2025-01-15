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
    abstract public class GeneralTask : IComparable<GeneralTask>
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
    public class Trigger : GeneralTask
    {
        public Trigger(string systemName) : base(systemName, null, null, new List<string>(), new List<string>())
        { }
    }
    public class TriggerImp : GeneralTask
    {
        public TriggerImp(string systemName) : base(systemName, null, null, new List<string>(), new List<string>())
        { }
    }
    public class Task : GeneralTask
    {
        public Task(string systemName, DescriptionTask descriptionTask, List<string> eachOtherExclusive) : base(systemName, eachOtherExclusive, descriptionTask, new List<string>(), new List<string>())
        { }
    }
    public class TaskHid : GeneralTask
    {
        public TaskHid(string systemName, List<string> eachOtherExclusive) : base(systemName, eachOtherExclusive, null, new List<string>(), new List<string>())
        { }
    }
    public class TaskBoard
    {
        private CustomSortedEnum<string> _blockedTasks;
        private CustomSortedEnum<string> _complitedTasks;
        private CustomSortedEnum<string> _usingTask;
        private CustomSortedEnum<GeneralTask> _memoryTask;
        public TaskBoard(CustomSortedEnum<GeneralTask> memoryTask, CustomSortedEnum<(string, string)> connect, CustomSortedEnum<string> startedSystemNameTask)
        {
            _memoryTask = memoryTask;
            _usingTask = startedSystemNameTask;
            _complitedTasks = new CustomSortedEnum<string>();
            _blockedTasks = new CustomSortedEnum<string>();
            CreateNextPrevSystemTask(connect);
        }
        private void CreateNextPrevSystemTask(CustomSortedEnum<(string prev, string next)> connect)
        {
            foreach (var v in connect)
            {
                FindTask(v.prev)._nextSystemTask.Add(v.next);
                FindTask(v.next)._prevSystemTask.Add(v.prev);
            }
        }
        public CustomSortedEnum<GeneralTask> GetUsingTask()
        {
            CustomSortedEnum<GeneralTask> retur = new CustomSortedEnum<GeneralTask>();
            foreach (var v in _usingTask)
            {
                retur.Add(FindTask(v));
            }
            return retur;
        }
        public List<DescriptionTask> GetDescriptionUsingTask()
        {
            var retur = new List<DescriptionTask>();
            foreach (var task in _usingTask)
            {
                var v = FindTask(task);
                if (v.DescriptionTask != null && !retur.Contains(v.DescriptionTask))
                    retur.Add(v.DescriptionTask);
            }
            return retur;
        }
        private GeneralTask FindTask(string systemnametask)
        {
            foreach (var task in _memoryTask)
            {
                if (task.SystemName == systemnametask)
                    return task;
            }
            throw new CustomException($"Task ={systemnametask}= is not find");
        }

        public void ComplitedTask(string SysNameTask)
        {
            _usingTask.Remove(SysNameTask);
            _complitedTasks.Add(SysNameTask);

            var task = FindTask(SysNameTask);
            if (task._eachOtherExclusive != null)
            {
                foreach (var exclusion in task._eachOtherExclusive)
                {
                    _blockedTasks.Add(exclusion);
                    _usingTask.Remove(exclusion);
                }
            }

            AddNewUsingTask(SysNameTask);
        }
        private void AddNewUsingTask(string SysNameTask)
        {
            List<string> NewTask = new List<string>();
            foreach (var next in FindTask(SysNameTask)._nextSystemTask)
            {
                if (!_blockedTasks.Contains(next))
                    NewTask.Add(next);
            }
            List<string> NotComplited = new List<string>();
            foreach (var task in NewTask)
            {
                foreach (var prev in FindTask(task)._prevSystemTask)
                {
                    if (!_complitedTasks.Contains(prev))
                    {
                        NotComplited.Add(task);
                        break;
                    }
                }
            }
            foreach (var task in NewTask)
            {
                if (!NotComplited.Contains(task))
                {
                    _usingTask.Add(task);
                }
            }
        }
    }
}
