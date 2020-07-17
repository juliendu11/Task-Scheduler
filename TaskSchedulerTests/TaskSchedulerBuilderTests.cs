using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Models;
using TaskScheduler.Classes;
using System.Threading;
using TaskScheduler.Helpers;
using TaskScheduler.Enums;

namespace TaskScheduler.Tests
{
    [TestClass()]
    public class TaskSchedulerBuilderTests
    {
        [TestMethod()]
        public void Should_Build_Default_TaskScheduler()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
                .Build();

            Assert.IsNotNull(taskScheduler);
            Assert.IsNotNull(taskScheduler.Timers);
        }

        [TestMethod()]
        public void Should_Build_Custom_TaskScheduler()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
                .SetCustomTaskScheduler(new CustomTaskScheduler(null, null))
                .Build();

            Assert.IsNotNull(taskScheduler);
            Assert.IsNotNull(taskScheduler.Timers);
        }

        public class CustomTaskArgs : ITaskArg
        {
            private bool launched;
            private bool finished;

            public string TaskId { get; set; }
            public CancellationTokenSource CancellationToken { get; set; }
            public Func<ITaskArg, Task> Action { get; set; }
            public TimeSpan? Timezone { get; set; }
            public DateTimeOffset StartTime { get; set; }
            public DateTimeOffset StopTime { get; set; }
            public TaskTimeMode TaskTimeMode { get; set; }
            public Timer StartTimer { get; set; }
            public Timer StopTimer { get; set; }
            public Action<ITaskArg> ActionWhenLaunchedChanged { get; set; }
            public Action<ITaskArg> ActionWhenFinishedChanged { get; set; }


            public bool Launched
            {
                get => launched;
                set
                {
                    launched = value;
                    if (ActionWhenLaunchedChanged != null)
                        ActionWhenLaunchedChanged.Invoke(this);
                }
            }

            public bool Finished
            {
                get => finished;
                set
                {
                    finished = value;
                    if (ActionWhenFinishedChanged != null)
                        ActionWhenFinishedChanged.Invoke(this);
                }
            }

            //...Your custom prop, field,...
            public string Username { get; set; }
            public string UserId { get; set; }
        }

        public class CustomTaskScheduler : ITaskScheduler
        {
            private TimeSpan defaultTimezone;

            public Dictionary<string, ITaskArg> Timers { get; private set; }
            public DateTimeOffset SchedulerDateTime { get; internal set; }

            private Action linkTasksLaunched;
            private Action linkTasksFinished;

            private Classes.TaskAdder taskAdder;
            public Classes.TaskAdder TaskAdder
            {
                get
                {
                    SchedulerDateTime.ToOffset(defaultTimezone);
                    return taskAdder.CleanUp();
                }
            }

            public Classes.TimerCreator TimerCreator { get; private set; }
            public Options Options { get; private set; }


            public CustomTaskScheduler(Action linkTasksLaunched, Action linkTasksFinished)
            {
                this.Timers = new Dictionary<string, ITaskArg>();
                this.SchedulerDateTime = DateTimeOffset.Now;
                this.defaultTimezone = DateTimeOffset.Now.Offset;
                this.TimerCreator = new Classes.TimerCreator(this);
                this.Options = new Options();
                this.linkTasksLaunched = linkTasksLaunched;
                this.linkTasksFinished = linkTasksFinished;
                this.taskAdder = new Classes.TaskAdder(this);
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

            // -> Custom implementation for get by Username with my CustomTaskArg
            // -> Use ConvertTaskArg extension methods to easily convert
            public List<ITaskArg> GetTasksArgWithUsername(string username)
            {
                return this.Timers.Values.Where(taskArg => taskArg.ConvertTaskArg<CustomTaskArgs>().Username == username).ToList();
            }
            //


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
                this.TimerCreator.DeleteTask(taskId, true);
            }
        }
    }
}
