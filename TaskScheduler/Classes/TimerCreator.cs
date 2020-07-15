using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TaskScheduler.Classes
{
    public class TimerCreator
    {
        private TaskScheduler taskScheduler;

        internal TimerCreator(TaskScheduler taskScheduler)
        {
            this.taskScheduler = taskScheduler;
        }

        public void SetUpTimers(Models.ITaskArg taskArg)
        {
            taskArg.StartTimer = CreateTimer(taskArg.StartTime, taskScheduler.SchedulerDateTime, () => LaunchTask(taskArg));
            taskArg.StopTimer = CreateTimer(taskArg.StopTime, taskScheduler.SchedulerDateTime, () => DeleteTask(taskArg));
        }


        private Timer CreateTimer(DateTimeOffset timer, DateTimeOffset current, Action toRun)
        {
            TimeSpan timeToGo = timer - current;
            if (timeToGo < TimeSpan.Zero)
            {
                return null;//time already passed
            }
            return new System.Threading.Timer(x =>
            {
                toRun.Invoke();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private void LaunchTask(Models.ITaskArg taskArg)
        {
            taskArg.Launched = true;
            taskArg.Action.GetAwaiter();

            DeleteTask(taskArg);
            taskArg.Finished = true; //The task could be finished before the time ended
        }

        private void DeleteTask(Models.ITaskArg taskArg)
        {
            this.taskScheduler.DeleteTask(taskArg.TaskId);
        }
    }
}
