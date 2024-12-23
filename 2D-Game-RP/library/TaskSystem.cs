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
        private CustomSortedEnum<string> _complitedTasks;
        private CustomSortedEnum<Task> _usingTask;
        private CustomSortedEnum<Task> _memoryTask;
        private CustomSortedEnum<(string comp, string usin)> _connection;
        public TaskBoard(CustomSortedEnum<Task> memoryTask, CustomSortedEnum<(string comp, string usin)> connect, List<string> startedSystemNameTask)
        {
            _memoryTask = memoryTask;
            _usingTask = new CustomSortedEnum<Task>();
            _connection = connect;
            _complitedTasks = new CustomSortedEnum<string>();

            foreach (var sysname in startedSystemNameTask)
            {
                _usingTask.Add(FindTask(sysname));
                foreach (var andertask in _memoryTask)
                {
                    if (andertask.SysNameParent == sysname)
                        _usingTask.Add(andertask);
                }
            }
        }
        public CustomSortedEnum<Task> GetUsingTask()
        {
            return _usingTask;
        }
        public List<Task> GetUsingTaskNotSystem()
        {
            var retur = new List<Task>();
            foreach (var task in _usingTask)
            {
                if (!task.IsSystem) retur.Add(task);
            }
            return retur;
        }
        private Task FindTask(string systemnametask)
        {
            foreach (var task in _memoryTask)
            {
                if (task.SystemName == systemnametask)
                    return task;
            }
            throw new CustomException("Task is not find");
        }
        public void ComplitedTask(string SysNameTask)
        {
            Task compliteTask = FindTask(SysNameTask);
            if (compliteTask.SysNameParent != null) //=> children
            {
                _usingTask.Remove(compliteTask);
                _complitedTasks.Add(compliteTask.SystemName);
                bool isParentComplite = true;
                foreach (var task in _usingTask)
                {
                    if (task.SysNameParent == compliteTask.SysNameParent)
                    {
                        isParentComplite = false;
                        break;
                    }
                }
                if (isParentComplite)
                {
                    _usingTask.Remove(FindTask(compliteTask.SysNameParent));
                    _complitedTasks.Add(compliteTask.SysNameParent);
                    AddNewUsingTask(compliteTask.SysNameParent);
                }
            }
            else
            {
                _usingTask.Remove(compliteTask);
                _complitedTasks.Add(compliteTask.SystemName);
                AddNewUsingTask(compliteTask.SystemName);
            }
        }
        private void AddNewUsingTask(string SysNameTask)
        {
            List<string> NewTask = new List<string>();
            foreach (var pair in _connection)
            {
                if (pair.comp == SysNameTask)
                    NewTask.Add(pair.usin);
            }
            List<string> NotComplited = new List<string>();
            foreach (var task in NewTask)
            {
                foreach (var pair in _connection)
                {
                    if (pair.usin == task)
                    { 
                        if (!_complitedTasks.Contains(pair.comp))
                        {
                            NotComplited.Add(pair.usin);
                            break;
                        }
                    }
                }
            }
            foreach (var task in NewTask)
            {
                if (!NotComplited.Contains(task)) 
                { 
                    _usingTask.Add(FindTask(task));
                    foreach (var andertask in _memoryTask)
                    {
                        if (andertask.SysNameParent == task)
                            _usingTask.Add(andertask);
                    }
                }
            }
        }
    }
}
