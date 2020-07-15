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
        private DateTimeOffset startTime;
        private DateTimeOffset stopTimer;

        private Models.ITaskArg taskArg;
        private Action action;
        private TimeSpan timezone;

        private Enums.TaskTimeMode taskTimeMode;

        private TaskScheduler taskScheduler;

        internal TaskAdder(TaskScheduler taskScheduler) 
        {
            this.taskScheduler = taskScheduler;
        }

        public TaskAdder SetHours(TimeSpan startTime, TimeSpan stopTimer)
        {
            this.startTime = DateTimeOffset.UtcNow.ToOffset(startTime);
            this.stopTimer = DateTimeOffset.UtcNow.ToOffset(stopTimer);
            this.taskTimeMode = Enums.TaskTimeMode.HoursProgram;
            return this;
        }

        public TaskAdder SetDay(DateTimeOffset startTime, DateTimeOffset stopTimer)
        {
            this.startTime = startTime;
            this.stopTimer = stopTimer;
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

        public TaskAdder SetAction(Action action)
        {
            this.action = action;
            return this;
        }

        public string AddTask()
        {
            if (this.startTime == null)
                throw new Exception("No start time added");
            if (this.stopTimer == null)
                throw new Exception("No stop time added");
            if (this.action == null)
                throw new Exception("No action added");

            GenerateNewTask();

            return taskArg.TaskId;
        }

        private void GenerateNewTask()
        {
            string newId = Guid.NewGuid().ToString();

            if (taskArg == null)
                taskArg = new Models.TaskArgNoPayload();

            taskArg.StartTime = this.startTime;
            taskArg.StopTime = this.stopTimer;
            taskArg.Action = Task.Factory.StartNew(action, taskArg.CancellationToken.Token, TaskCreationOptions.LongRunning, System.Threading.Tasks.TaskScheduler.Default);

            taskArg.CancellationToken = new CancellationTokenSource();
            taskArg.TaskId = newId;
            taskArg.TaskTimeMode = this.taskTimeMode;

            if (this.timezone != null)
            {
                taskArg.Timezone = this.timezone;
                taskArg.StartTime.ToOffset(this.timezone);
                taskArg.StopTime.ToOffset(this.timezone);
            }

            this.taskScheduler.AddNewTask(newId, taskArg);
        }
    }
}
