using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskScheduler.CustomExceptions;
using TaskScheduler.Models;

namespace TaskScheduler.Classes
{
    public class TimerCreator
    {
        private ITaskScheduler taskScheduler;

        public TimerCreator(ITaskScheduler taskScheduler)
        {
            this.taskScheduler = taskScheduler;
        }

        public void SetUpTimers(string taskid)
        {
            Models.ITaskArg taskArg = this.taskScheduler.Timers[taskid];

            var nowDate = DateTimeOffset.Now;
            if (taskArg.Timezone !=null)
            {
                nowDate.ToOffset((TimeSpan)taskArg.Timezone);
            }

            taskScheduler.UpdateTaskSchedulerDate(nowDate);

            VerifyTaskTimers(taskArg);

            taskArg.CancellationToken.Token.Register(() => taskArg.Launched = false);

            if (taskArg.TaskTimeMode == Enums.TaskTimeMode.DayProgram)
            {
                taskArg.StartTimer = CreateTimer(taskArg.StartTime, taskScheduler.SchedulerDateTime, () => LaunchTask(taskArg));
                taskArg.StopTimer = CreateTimer(taskArg.StopTime, taskScheduler.SchedulerDateTime, () => ManageTaskTermination(taskArg));
            }
            else
            {
                taskArg.StartTimer = CreateTimer(taskArg.StartTime.TimeOfDay, taskScheduler.SchedulerDateTime.TimeOfDay, () => LaunchTask(taskArg));
                taskArg.StopTimer = CreateTimer(taskArg.StopTime.TimeOfDay, taskScheduler.SchedulerDateTime.TimeOfDay, () => ManageTaskTermination(taskArg));
            }

        }

        private void VerifyTaskTimers(ITaskArg taskArg)
        {
            if (DateTimeOffset.Compare(taskArg.StartTime, this.taskScheduler.SchedulerDateTime) < 0)
                throw new SchedulerTimeError($"Startup time is smaller than TaskScheduler date (now date) - Startup time: {taskArg.StartTime},  TaskScheduler date: {this.taskScheduler.SchedulerDateTime}");

            if (DateTimeOffset.Compare(taskArg.StopTime, taskArg.StartTime) < 0 )
                throw new SchedulerTimeError($"End time is greater than start date - Startup time: {taskArg.StartTime},  End time: {taskArg.StopTime}");
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

        private Timer CreateTimer(TimeSpan timer, TimeSpan current, Action toRun)
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
                ManageTaskTermination(taskArg);
                taskArg.Finished = true; //The task could be finished before the time ended

            }, taskArg.CancellationToken.Token, TaskCreationOptions.LongRunning, System.Threading.Tasks.TaskScheduler.Default);
        }


        public void ManageTaskTermination(ITaskArg taskArg)
        {
            if (taskArg.CancellationToken != null)
                taskArg.CancellationToken.Cancel();

            DeleteTaskStartTimer(taskArg);
            DeleteTaskStopTimer(taskArg);
            DeleteTaskIfOptionIsEnabled(taskArg);
        }


        private void DeleteTaskStartTimer(ITaskArg taskArg)
        {
            if (taskArg.StartTimer != null)
                taskArg.StartTimer.Dispose();
            if (taskArg.StartTimer != null)
                taskArg.StartTimer = null;
        }

        private void DeleteTaskStopTimer(ITaskArg taskArg)
        {
            if (taskArg.StopTimer != null)
                taskArg.StopTimer.Dispose();
            if (taskArg.StopTimer != null)
                taskArg.StopTimer = null;
        }

        private void DeleteTaskIfOptionIsEnabled(ITaskArg taskArg)
        {
            if (this.taskScheduler.Options.DeleteTaskAfterCompleted
                && taskScheduler.Timers.ContainsKey(taskArg.TaskId))
                taskScheduler.Timers.Remove(taskArg.TaskId);
        }
    }
}
