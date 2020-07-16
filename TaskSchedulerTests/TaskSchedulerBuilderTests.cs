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

namespace TaskScheduler.Tests
{
    [TestClass()]
    public class TaskSchedulerBuilderTests
    {
        [TestMethod()]
        public void Should_Build_Default_TaskScheduler()
        {
            var taskScheduler  = TaskSchedulerBuilder.CreateBuilder()
                .Build();

            Assert.IsNotNull(taskScheduler);
            Assert.IsNotNull(taskScheduler.Timers);
        }

        [TestMethod()]
        public void Should_Build_Custom_TaskScheduler()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
                .SetCustomTaskScheduler(new CustomScheduler())
                .Build();

            Assert.IsNotNull(taskScheduler);
            Assert.IsNotNull(taskScheduler.Timers);
        }

        public class CustomScheduler : ITaskScheduler
        {
            public Dictionary<string, ITaskArg> Timers { get; private set; }

            public Classes.TaskAdder TaskAdder
            {
                get
                {
                    return new Classes.TaskAdder(this);
                }
            }

            public DateTimeOffset SchedulerDateTime { get; private set; }

            public Options Options { get; private set; }

            public TimerCreator TimerCreator { get; private set; } 

            public CustomScheduler()
            {
                this.Timers = new Dictionary<string, ITaskArg>();
                this.TimerCreator = new Classes.TimerCreator(this);
                this.Options = new Options();
            }

            public List<ITaskArg> GetAllTasks()
            {
                throw new NotImplementedException();
            }

            public bool GetTasksFinishedStatusWithId(string taskid)
            {
                throw new NotImplementedException();
            }

            public bool GetTasksLaunchedStatusWithId(string taskid)
            {
                throw new NotImplementedException();
            }

            public ITaskArg GetTasksArgWithId(string taskid)
            {
                throw new NotImplementedException();
            }

            public CancellationToken GetTaskToken(string taskid)
            {
                throw new NotImplementedException();
            }

            public void UpdateTimezone(TimeSpan timezone)
            {
                throw new NotImplementedException();
            }

            public bool VerifyTaskExistWithId(string taskid)
            {
                throw new NotImplementedException();
            }
        }
    }
}