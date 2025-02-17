using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{
    public interface IMemoryTask
    {
        //private void CreateNextPrevSystemTask(CustomSortedEnum<(string prev, string next)> connect);
        //private void AddNewUsingTask(string SysNameTask);
        //private GeneralTask FindTask(string systemnametask);
        CustomSortedEnum<GeneralTask> GetUsingTask();
        List<DescriptionTask> GetDescriptionUsingTask();
        void ComplitedTask(string SysNameTask);
    }
    internal class TaskBoard : IMemoryTask
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
                    if (!_complitedTasks.Contains(prev) && !_blockedTasks.Contains(prev))
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
