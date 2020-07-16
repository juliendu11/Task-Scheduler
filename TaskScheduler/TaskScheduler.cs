using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TaskScheduler.Models;

namespace TaskScheduler
{
    public class TaskScheduler :ITaskScheduler
    {
        public Dictionary<string, ITaskArg> Timers { get; private set; }
        public DateTimeOffset SchedulerDateTime { get; internal set; }

        private Action linkTasksLaunched;
        private Action linkTasksFinished;

        public Classes.TaskAdder TaskAdder 
        {
            get
            {
                return new Classes.TaskAdder(this);
            }
        }

        public Classes.TimerCreator TimerCreator { get; private set; }
        public Options Options { get; private set; }


        public TaskScheduler(Action linkTasksLaunched, Action linkTasksFinished)
        {
            this.Timers = new Dictionary<string, ITaskArg>();
            this.SchedulerDateTime = DateTimeOffset.Now;
            this.TimerCreator = new Classes.TimerCreator(this);
            this.Options = new Options();
            this.linkTasksLaunched = linkTasksLaunched;
            this.linkTasksFinished = linkTasksFinished;

        }

        

        public void UpdateTimezone(TimeSpan timezone)
        {
            this.SchedulerDateTime.ToOffset(timezone);
        }

        public ITaskArg GetTasksArgWithId(string taskid)
        {
            if (!VerifyTaskExistWithId(taskid))
                throw new Exception("This tasks with this id not exist in list");

            return this.Timers[taskid];
        }

        public bool GetTasksLaunchedStatusWithId(string taskid)
        {
            return GetTasksArgWithId(taskid).Launched;
        }

        public bool GetTasksFinishedStatusWithId(string taskid)
        {
            return GetTasksArgWithId(taskid).Finished;
        }

        public bool VerifyTaskExistWithId(string taskid)
        {
            return this.Timers.ContainsKey(taskid);
        }

        public List<ITaskArg> GetAllTasks() => this.Timers.Values.ToList();

        public CancellationToken GetTaskToken(string taskid)
        {
            return GetTasksArgWithId(taskid).CancellationToken.Token;
        }
    }
}
