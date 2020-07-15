using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TaskScheduler.Models;

namespace TaskScheduler
{
    public class TaskScheduler
    {
        internal Dictionary<string, ITaskArg> timers;
        public DateTimeOffset SchedulerDateTime { get; internal set; }

        public Classes.TaskAdder TaskAdder { get; }

        private Classes.TimerCreator timerCreator;

        public TaskScheduler()
        {
            this.timers = new Dictionary<string, ITaskArg>();
            this.SchedulerDateTime = DateTimeOffset.UtcNow;
            this.TaskAdder = new Classes.TaskAdder(this);
            this.timerCreator = new Classes.TimerCreator(this);
        }

        public void UpdateTimezone(TimeSpan timezone)
        {
            this.SchedulerDateTime.ToOffset(timezone);
        }

        public ITaskArg GetTasksWithId(string taskid)
        {
            if (!VerifyTaskExistWithId(taskid))
                throw new Exception("This tasks with this id not exist in list");

            return this.timers[taskid];
        }

        public bool GetTasksLaunchedStatusWithId(string taskid)
        {
            return GetTasksWithId(taskid).Launched;
        }

        public bool GetTasksFinishedStatusWithId(string taskid)
        {
            return GetTasksWithId(taskid).Finished;
        }

        public bool VerifyTaskExistWithId(string taskid)
        {
            return this.timers.ContainsKey(taskid);
        }

        public Dictionary<string, ITaskArg> GetAllTasks => this.timers.ToDictionary(entry => entry.Key,entry => entry.Value);

        internal void AddNewTask(string id, ITaskArg taskArg)
        {
            timerCreator.SetUpTimers(taskArg);
            this.timers.Add(id, taskArg);
        }

        internal void DeleteTask(string taskID)
        {
            if (this.timers.ContainsKey(taskID))
            {
                this.timers[taskID].CancellationToken.Cancel();

                if (this.timers[taskID].StartTimer != null)
                {
                    DeleteTaskStartTimer(taskID);
                }
                if (this.timers[taskID].StopTimer != null)
                {
                    DeleteTaskStopTimer(taskID);
                }

                timers.Remove(taskID);
            }
        }

        private void DeleteTaskStopTimer(string taskID)
        {
            this.timers[taskID].StopTimer.Dispose();
            if (this.timers[taskID].StopTimer != null)
                this.timers[taskID].StopTimer = null;
        }

        private void DeleteTaskStartTimer(string taskID)
        {
            this.timers[taskID].StartTimer.Dispose();
            if (this.timers[taskID].StartTimer != null)
                this.timers[taskID].StartTimer = null;
        }
    }
}
