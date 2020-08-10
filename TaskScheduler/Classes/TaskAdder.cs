using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskScheduler.Models;

namespace TaskScheduler.Classes
{
    public class TaskAdder
    {
        private ITaskScheduler taskScheduler;

        private DateTimeOffset? startTime;
        private DateTimeOffset? stopTime;

        private Models.ITaskArg taskArg;
        private Func<ITaskArg,Task> action;
        private TimeSpan timezone;

        private Enums.TaskTimeMode taskTimeMode;

        private Action<ITaskArg> actionForLaunchedStatusChanged;
        private Action<ITaskArg> actionForFinishedStatusChanged;

        public TaskAdder(ITaskScheduler taskScheduler) 
        {
            this.taskScheduler = taskScheduler;
            this.timezone = taskScheduler.SchedulerDateTime.Offset;
        }

        public TaskAdder CleanUp()
        {
            this.startTime = null;
            this.stopTime = null;

            this.taskArg = null;
            this.action = null;

            this.timezone = this.taskScheduler.SchedulerDateTime.Offset;
            this.taskTimeMode = default;

            this.actionForLaunchedStatusChanged = null;
            this.actionForFinishedStatusChanged = null;

            return this;
        }


        public TaskAdder SetCustomTaskArg(ITaskArg taskArg)
        {
            this.taskArg = taskArg;
            return this;
        }

        public TaskAdder SetHours(TimeSpan startTime, TimeSpan stopTime)
        {
            ConvertAndSetHours(startTime, stopTime);
            return this;
        }

        private void ConvertAndSetHours(TimeSpan startTime, TimeSpan stopTime)
        {
            this.startTime = this.taskScheduler.SchedulerDateTime;
            this.stopTime = this.taskScheduler.SchedulerDateTime;

            this.startTime = ((DateTimeOffset)this.startTime).Date + startTime;
            this.stopTime = ((DateTimeOffset)this.stopTime).Date + stopTime;

            this.taskTimeMode = Enums.TaskTimeMode.HoursProgram;
        }

        public TaskAdder SetDay(DateTimeOffset startTime, DateTimeOffset stopTimer)
        {
            this.startTime = startTime;
            this.stopTime = stopTimer;
            this.taskTimeMode = Enums.TaskTimeMode.DayProgram;
            return this;
        }

        public TaskAdder SetTimezone(TimeSpan timezone)
        {
            this.timezone = timezone;
            return this;
        }

        public TaskAdder SetPayload<T>(T payload)
        {
            taskArg = new Models.TaskArgWithPayload<T>(payload);
            return this;
        }

        public TaskAdder SetAction(Func<ITaskArg,Task> action)
        {
            this.action = action;
            return this;
        }

        public TaskAdder LinkFinishedStatus(Action<ITaskArg> action)
        {
            this.actionForFinishedStatusChanged = action;
            return this;
        }

        public TaskAdder LinkLaunchedStatus(Action<ITaskArg> action)
        {
            this.actionForLaunchedStatusChanged = action;
            return this;
        }

        public string AddTask()
        {
            if (this.startTime == null)
                throw new Exception("No start time added");
            if (this.stopTime == null)
                throw new Exception("No stop time added");
            if (this.action == null)
                throw new Exception("No action added");

            this.GenerateNewTaskArg();
            this.AddNewTask();

            return taskArg.TaskId;
        }

        private void GenerateNewTaskArg()
        {
            string newId = Guid.NewGuid().ToString();

            if (taskArg == null)
                taskArg = new Models.TaskArgWithPayload<object>(null);

            taskArg.StartTime = (DateTimeOffset)this.startTime;
            taskArg.StopTime = (DateTimeOffset)this.stopTime;
            taskArg.CancellationToken = new CancellationTokenSource();


            taskArg.Action = this.action;

            taskArg.TaskId = newId;
            taskArg.TaskTimeMode = this.taskTimeMode;

            if (this.timezone != null)
            {
                taskArg.Timezone = this.timezone;
                taskArg.StartTime.ToOffset(this.timezone);
                taskArg.StopTime.ToOffset(this.timezone);
            }

            taskArg.ActionWhenFinishedChanged = this.actionForFinishedStatusChanged;
            taskArg.ActionWhenLaunchedChanged = this.actionForLaunchedStatusChanged;
        }


        private void AddNewTask()
        {
            this.taskScheduler.Timers.Add(taskArg.TaskId, taskArg);
            this.taskScheduler.TimerCreator.SetUpTimers(taskArg.TaskId);
        }
    }
}
