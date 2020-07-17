using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskScheduler.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TaskScheduler.Models;
using TaskScheduler.Enums;

namespace TaskScheduler.Classes.Tests
{
    [TestClass()]
    public class TaskAdderTests
    {
        [TestMethod()]
        [Description("Test AddTask() function")]
        public void Should_Create_New_Task_But_Action_Not_Set_Return_Exception_Error()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            Assert.ThrowsException<Exception>(() =>
            {
                var newTaskId = taskScheduler.TaskAdder
                 .SetHours(DateTimeOffset.Now.AddMinutes(3).TimeOfDay, DateTimeOffset.Now.AddMinutes(5).TimeOfDay)
                 .AddTask();
            });
        }

        [TestMethod()]
        [Description("Test AddTask() function")]
        public void Should_Create_New_Task_But_Action_And_StartTime_And_StopTime_Not_Set_Return_Exception_Error()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            Assert.ThrowsException<Exception>(() =>
            {
                var newTaskId = taskScheduler.TaskAdder
                 .AddTask();
            });
        }

        [TestMethod()]
        [Description("Test AddTask() function")]
        public void Should_Create_New_Task_With_SetDay_But_StartTime_And_StopTime_Not_Set_Return_Exception_Error()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            Assert.ThrowsException<Exception>(() =>
            {
                var newTaskId = taskScheduler.TaskAdder
                 .SetAction(async (x) =>
                 {
                     Console.WriteLine(x.TaskId);
                 })
                 .AddTask();
            });
        }


        [TestMethod()]
        public void Should_Create_Task_Good_Simple_Implementation_Return_new_TaskId()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            var newTaskId = taskScheduler.TaskAdder
                .SetHours(DateTimeOffset.Now.AddMinutes(20).TimeOfDay, DateTimeOffset.Now.AddMinutes(50).TimeOfDay)
                .SetAction(async (x) =>
                {
                    Console.WriteLine(x.TaskId);
                })
                .AddTask();

            Assert.IsNotNull(newTaskId);
        }

        [TestMethod()]
        public void Should_Create_Task_Good_Simple_Implementation_Verify_Task_Exist_In_Timers_List_Return_True()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            var newTaskId = taskScheduler.TaskAdder
                .SetHours(DateTimeOffset.Now.AddMinutes(20).TimeOfDay, DateTimeOffset.Now.AddMinutes(50).TimeOfDay)
                .SetAction(async (x) =>
                {
                    Console.WriteLine(x.TaskId);
                })
                .AddTask();

            Assert.IsTrue(taskScheduler.VerifyTaskExistWithId(newTaskId));
        }

        [TestMethod()]
        public void Should_Create_Task_Good_PayLoad_Implementation_Return_new_TaskId()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            var newTaskId = taskScheduler.TaskAdder
                .SetHours(DateTimeOffset.Now.AddMinutes(20).TimeOfDay, DateTimeOffset.Now.AddMinutes(50).TimeOfDay)
                .SetPayload<string>("hello")
                .SetAction(async (x) =>
                {
                    Console.WriteLine(x.TaskId);
                })
                .AddTask();

            Assert.IsNotNull(newTaskId);
        }

        [TestMethod()]
        public void Should_Create_Task_Good_Payload_Implementation_Verify_Task_Exist_In_Timers_List_Return_True_And_Get_Payload()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            var newTaskId = taskScheduler.TaskAdder
                .SetHours(DateTimeOffset.Now.AddMinutes(20).TimeOfDay, DateTimeOffset.Now.AddMinutes(50).TimeOfDay)
                .SetPayload<string>("hello")
                .SetAction(async (x) =>
                {
                    Console.WriteLine(x.TaskId);
                })
                .AddTask();

            var taskExist = taskScheduler.VerifyTaskExistWithId(newTaskId);
            var getTask = taskScheduler.GetTasksArgWithId(newTaskId);

            var convertTaskArg = (Models.TaskArgWithPayload<string>)getTask;

            Assert.IsTrue(taskExist);
            Assert.AreEqual("hello", convertTaskArg.Payload);
        }

        [TestMethod()]
        public void Should_Create_Task_Good_CustomTaskArg_Implementation_Return_new_TaskId()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            var newTaskId = taskScheduler.TaskAdder
                .SetHours(DateTimeOffset.Now.AddMinutes(20).TimeOfDay, DateTimeOffset.Now.AddMinutes(50).TimeOfDay)
                .SetCustomTaskArg(new CustomTaskArgs {Username = "Bob", UserId = Guid.NewGuid().ToString() })
                .SetAction(async (x) =>
                {
                    Console.WriteLine(x.TaskId);
                })
                .AddTask();

            Assert.IsNotNull(newTaskId);
        }

        [TestMethod()]
        public void Should_Create_Task_Good_CustomTaskArg_Implementation_Verify_Task_Exist_In_Timers_List_Return_True_And_Get_Payload()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            var newTaskId = taskScheduler.TaskAdder
               .SetHours(DateTimeOffset.Now.AddMinutes(20).TimeOfDay, DateTimeOffset.Now.AddMinutes(50).TimeOfDay)
                .SetCustomTaskArg(new CustomTaskArgs { Username = "Bob", UserId = Guid.NewGuid().ToString() })
                .SetAction(async (x) =>
                {
                    Console.WriteLine(x.TaskId);
                })
                .AddTask();

            var taskExist = taskScheduler.VerifyTaskExistWithId(newTaskId);
            var getTask = taskScheduler.GetTasksArgWithId(newTaskId);

            var convertTaskArg = (CustomTaskArgs)getTask;

            Assert.IsTrue(taskExist);
            Assert.AreEqual("Bob", convertTaskArg.Username);
        }

        class CustomTaskArgs : ITaskArg
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
    }
}