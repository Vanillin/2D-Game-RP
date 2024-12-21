using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{
    public class Task : IComparable<Task>
    {
        public bool IsSystem { get; set; }
        public string SystemName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SysNameParent { get; set; }
        public Task(string systemname, string name, string description, string sysnameparent, bool isSystem)
        {
            SystemName = systemname;
            Name = name;
            Description = description;
            SysNameParent = sysnameparent;
            IsSystem = isSystem;
        }
        private Task() { }
        public int CompareTo(Task other)
        {
            if (SystemName.CompareTo(other.SystemName) > 0)
                return 1;
            if (SystemName.CompareTo(other.SystemName) < 0)
                return -1;
            return 0;
        }
    }
    public class TaskBoard
    {
        private SortedEnum<Task> UsingTask { get; set; }
        private SortedEnum<Task> MemoryTask { get; set; }
        private SortedEnum<(string comp, string usin)> Connection { get; set; }
        public TaskBoard(SortedEnum<Task> AllTask, SortedEnum<(string comp, string usin)> connect, List<string> StartedSystemNameTask)
        {
            MemoryTask = AllTask;
            UsingTask = new SortedEnum<Task>();
            Connection = connect;

            foreach (var sysname in StartedSystemNameTask)
            {
                UsingTask.Add(FindTask(sysname));
                foreach (var andertask in MemoryTask)
                {
                    if (andertask.SysNameParent == sysname)
                        UsingTask.Add(andertask);
                }
            }
        }
        public List<Task> GetUsingTask()
        {
            var retur = new List<Task>();
            foreach (var task in UsingTask)
            {
                if(!task.IsSystem) retur.Add(task);
            }
            return retur;
        }
        private Task FindTask(string systemnametask)
        {
            foreach (var task in MemoryTask)
            {
                if (task.SystemName == systemnametask)
                    return task;
            }
            throw new Exception("Task is not find");
        }
        public void ComplitedTask(string SysNameTask)
        {
            Task compliteTask = FindTask(SysNameTask);
            if (compliteTask.SysNameParent != null) //=> children
            {
                UsingTask.Remove(compliteTask);
                bool isParentComplite = true;
                foreach (var task in UsingTask)
                {
                    if (task.SysNameParent == compliteTask.SysNameParent)
                    {
                        isParentComplite = false;
                        break;
                    }
                }
                if (isParentComplite)
                {
                    UsingTask.Remove(FindTask(compliteTask.SysNameParent));
                    AddNewUsingTask(compliteTask.SysNameParent);
                }
            }
            else
            {
                UsingTask.Remove(compliteTask);
                AddNewUsingTask(compliteTask.SystemName);
            }
        }
        private void AddNewUsingTask(string SysNameTask)
        {
            List<string> NewTask = new List<string>();
            foreach (var pair in Connection)
            {
                if (pair.comp == SysNameTask)
                    NewTask.Add(pair.usin);
            }
            List<string> NotComplited = new List<string>();
            foreach (var task in NewTask)
            {
                foreach (var pair in Connection)
                {
                    if (pair.usin == task && UsingTask.Contains(FindTask(pair.comp)))
                    { 
                        NotComplited.Add(pair.usin);
                        break;
                    }
                }
            }
            foreach (var task in NewTask)
            {
                if (!NotComplited.Contains(task)) 
                { 
                    UsingTask.Add(FindTask(task));
                    foreach (var andertask in MemoryTask)
                    {
                        if (andertask.SysNameParent == task)
                            UsingTask.Add(andertask);
                    }
                }
            }
        }
    }
}
