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

        private Action linkTasksLaunched;
        private Action linkTasksFinished;

        private Classes.TaskAdder taskAdder;
        public Classes.TaskAdder TaskAdder 
        {
            get
            {
                return taskAdder.CleanUp();
            }
        }

        public Classes.TimerCreator TimerCreator { get; private set; }
        public Options Options { get; private set; }


        public TaskScheduler(Action linkTasksLaunched, Action linkTasksFinished)
        {
            this.Timers = new Dictionary<string, ITaskArg>();
            this.TimerCreator = new Classes.TimerCreator(this);
            this.Options = new Options();
            this.linkTasksLaunched = linkTasksLaunched;
            this.linkTasksFinished = linkTasksFinished;
            this.taskAdder = new Classes.TaskAdder(this);
        }
        

        [Obsolete("Use SetScheduler Date directly when you create a task and if you want to put a specific date")]
        public void UpdateTimezone(TimeSpan timezone)
        {
            return;
        }

        [Obsolete("Use SetScheduler Date directly when you create a task and if you want to put a specific date")]
        public void UpdateTaskSchedulerDate(DateTimeOffset newDate)
        {
            return;
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

        public void StopAndDeleteAllTasks()
        {
            if (this.Timers.Count != 0)
                this.Timers.Keys.ToList().ForEach(taskid => this.StopAndDeleteTask(taskid));
        }

        public void StopAndDeleteTask(string taskId)
        {
            if (!VerifyTaskExistWithId(taskId))
                throw new Exception("This tasks with this id not exist in list");

            this.TimerCreator.ManageTaskTermination(this.Timers[taskId]);
            this.Timers.Remove(taskId);
        }

        public bool CheckIdExist(string id)
        {
            return this.Timers != null && this.Timers.Count != 0 && this.Timers.ContainsKey(id);
        }
    }
}
