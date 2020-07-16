using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskScheduler.Classes
{
    public class TimerCreator
    {
        private ITaskScheduler taskScheduler;

        public TimerCreator(ITaskScheduler taskScheduler)
        {
            this.taskScheduler = taskScheduler;
        }

        public void SetUpTimers(Models.ITaskArg taskArg)
        {
            taskArg.CancellationToken.Token.Register(() => taskArg.Launched = false);

            taskArg.StartTimer = CreateTimer(taskArg.StartTime, taskScheduler.SchedulerDateTime, () => LaunchTask(taskArg));
            taskArg.StopTimer = CreateTimer(taskArg.StopTime, taskScheduler.SchedulerDateTime, () => DeleteTask(taskArg.TaskId));
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

        private async void LaunchTask(Models.ITaskArg taskArg)
        {
            taskArg.Launched = true;

            await Task.Factory.StartNew(async () =>
            {
                await taskArg.Action.Invoke(taskArg);
                DeleteTask(taskArg.TaskId);
                taskArg.Finished = true; //The task could be finished before the time ended

            }, taskArg.CancellationToken.Token, TaskCreationOptions.LongRunning, System.Threading.Tasks.TaskScheduler.Default);
        }


        private void DeleteTask(string taskID)
        {
            if (this.taskScheduler.Timers.ContainsKey(taskID))
            {
                this.taskScheduler.Timers[taskID].CancellationToken.Cancel();
                if (this.taskScheduler.Timers[taskID].StartTimer != null)
                {
                    DeleteTaskStartTimer(taskID);
                }
                if (this.taskScheduler.Timers[taskID].StopTimer != null)
                {
                    DeleteTaskStopTimer(taskID);
                }

                if (this.taskScheduler.Options.DeleteTaskAfterCompleted)
                    taskScheduler.Timers.Remove(taskID);
            }
        }

        private void DeleteTaskStopTimer(string taskID)
        {
            this.taskScheduler.Timers[taskID].StopTimer.Dispose();
            if (this.taskScheduler.Timers[taskID].StopTimer != null)
                this.taskScheduler.Timers[taskID].StopTimer = null;
        }

        private void DeleteTaskStartTimer(string taskID)
        {
            this.taskScheduler.Timers[taskID].StartTimer.Dispose();
            if (this.taskScheduler.Timers[taskID].StartTimer != null)
                this.taskScheduler.Timers[taskID].StartTimer = null;
        }
    }
}
