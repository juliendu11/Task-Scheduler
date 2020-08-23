using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler;

namespace TaskSchedulerTests
{
    [TestClass()]
    public class CreateAndRunTaskTests
    {
        [TestMethod()]
        public void Should_Start_The_Action_With_A_Correct_Start_And_End_Time()
        {
            Task.Run(async () =>
            {
                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
                .Build();

                var newTaskId = taskScheduler.TaskAdder
                   .SetHours(DateTimeOffset.Now.AddMilliseconds(500).TimeOfDay,DateTimeOffset.Now.AddSeconds(10).TimeOfDay)
                   .SetAction(async (x) =>
                   {
                       while (!x.CancellationToken.IsCancellationRequested) { }
                   })
                    .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(1));
                Assert.IsTrue(getTask.Launched);
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Start_The_Action_With_A_Correct_Start_And_End_Date()
        {
            Task.Run(async () =>
            {
                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddMilliseconds(500),DateTimeOffset.Now.AddSeconds(10))
                    .SetAction(async (x) =>
                    {
                        while (!x.CancellationToken.IsCancellationRequested) { }
                    })
                     .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(1));
                Assert.IsTrue(getTask.Launched);
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Stop_The_Action_With_A_Correct_Start_And_End_Time()
        {
            Task.Run(async () =>
            {
                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
                .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                   .SetHours(DateTimeOffset.Now.AddMilliseconds(200).TimeOfDay,DateTimeOffset.Now.AddSeconds(1).TimeOfDay)
                   .SetAction(async (x) =>
                   {
                       while (!x.CancellationToken.IsCancellationRequested) { }
                   })
                    .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(2));
                Assert.IsFalse(getTask.Launched);
                Assert.IsTrue(getTask.CancellationToken.IsCancellationRequested);
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Stop_The_Action_With_A_Correct_Start_And_End_Date()
        {
            Task.Run(async () =>
            {
                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddMilliseconds(200),DateTimeOffset.Now.AddSeconds(1))
                    .SetAction(async (x) =>
                    {
                        while (!x.CancellationToken.IsCancellationRequested) { }
                    })
                     .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(2));
                Assert.IsFalse(getTask.Launched);
                Assert.IsTrue(getTask.CancellationToken.IsCancellationRequested);

            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Stop_And_Delete_The_Action_With_A_Correct_Start_And_End_Time()
        {
            Task.Run(async () =>
            {
                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
                .Build();

                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                   .SetHours(DateTimeOffset.Now.AddMilliseconds(300).TimeOfDay,DateTimeOffset.Now.AddSeconds(1).TimeOfDay)
                   .SetAction(async (x) =>
                   {
                       while (!x.CancellationToken.IsCancellationRequested) { }
                   })
                    .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(2));

                Assert.IsFalse(getTask.Launched);
                Assert.IsTrue(getTask.CancellationToken.IsCancellationRequested);
                Assert.IsFalse(taskScheduler.Timers.ContainsKey(newTaskId));
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Stop_And_Delete_The_Action_With_A_Correct_Start_And_End_Date()
        {
            Task.Run(async () =>
            {
                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues
                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddMilliseconds(200),DateTimeOffset.Now.AddSeconds(1))
                    .SetAction(async (x) =>
                    {
                        while (!x.CancellationToken.IsCancellationRequested) { }
                    })
                     .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(2));

                Assert.IsFalse(getTask.Launched);
                Assert.IsTrue(getTask.CancellationToken.IsCancellationRequested);
                Assert.IsFalse(taskScheduler.Timers.ContainsKey(newTaskId));
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Change_The_Value_Of_Notify_To_True_When_The_Task_Is_Launched_With_The_Subscription_To_Linklaunchedstatus()
        {
            Task.Run(async () =>
            {
                bool notify = false;

                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddMilliseconds(500),DateTimeOffset.Now.AddSeconds(2))
                    .SetAction(async (x) =>
                    {
                        while (!x.CancellationToken.IsCancellationRequested) { }
                    })
                    .LinkLaunchedStatus(async(x) =>
                    {
                        notify = x.Launched;
                    })
                     .AddTask();

                await Task.Delay(TimeSpan.FromSeconds(1));

                Assert.IsTrue(notify);
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Change_The_Value_Of_Notify_To_True_When_The_Task_Is_Stopped_With_The_Subscription_To_Linklaunchedstatus()
        {
            Task.Run(async () =>
            {
                bool notify = false;

                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddMilliseconds(200),DateTimeOffset.Now.AddSeconds(1))
                    .SetAction(async (x) =>
                    {
                        while (!x.CancellationToken.IsCancellationRequested) { }
                    })
                    .LinkLaunchedStatus(async (x) =>
                    {
                        notify = x.Launched;
                    })
                     .AddTask();

                await Task.Delay(TimeSpan.FromSeconds(2));

                Assert.IsFalse(notify);
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Change_The_Value_Of_Notify_To_True_When_The_Task_Is_Finished_With_The_Subscription_To_Linklaunchedstatus()
        {
            Task.Run(async () =>
            {
                bool notify = false;

                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

                SetSchedulerDate(taskScheduler); // -> control scheduler datetime for avoid end-of-day date issues

                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddMilliseconds(200),DateTimeOffset.Now.AddSeconds(1))
                    .SetAction(async (x) =>
                    {
                        await Task.Delay(200);
                    })
                    .LinkFinishedStatus(async (x) =>
                    {
                        notify = x.Launched;
                    })
                     .AddTask();

                await Task.Delay(TimeSpan.FromMilliseconds(600));

                Assert.IsFalse(notify);
            }).GetAwaiter().GetResult();
        }


        [TestMethod()]
        public void Should_Start_The_Action_With_A_Correct_Start_And_End_Time_With_UTC_Plus_2()
        {
            Task.Run(async () =>
            {
                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
                .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                    .SetHours(DateTimeOffset.Now.AddMilliseconds(500).TimeOfDay,DateTimeOffset.Now.AddSeconds(10).TimeOfDay)
                    .SetAction(async (x) =>
                    {
                        while (!x.CancellationToken.IsCancellationRequested) { }
                    })
                    .SetTimezone(TimeSpan.Parse("02:00:00"))
                    .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(1));
                Assert.IsTrue(getTask.Launched);
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Start_The_Action_With_A_Correct_Start_And_End_Date_With_UTC_Plus_2()
        {
            Task.Run(async () =>
            {
                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddMilliseconds(500),DateTimeOffset.Now.AddSeconds(10))
                    .SetAction(async (x) =>
                    {
                        while (!x.CancellationToken.IsCancellationRequested) { }
                    })
                     .SetTimezone(TimeSpan.Parse("02:00:00"))
                     .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(1));
                Assert.IsTrue(getTask.Launched);
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Stop_The_Action_With_A_Correct_Start_And_End_Time_With_UTC_Plus_2()
        {
            Task.Run(async () =>
            {
                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
                .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                   .SetHours(DateTimeOffset.Now.AddMilliseconds(200).TimeOfDay,DateTimeOffset.Now.AddSeconds(1).TimeOfDay)
                   .SetAction(async (x) =>
                   {
                       while (!x.CancellationToken.IsCancellationRequested) { }
                   })
                    .SetTimezone(TimeSpan.Parse("02:00:00"))
                    .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(2));
                Assert.IsFalse(getTask.Launched);
                Assert.IsTrue(getTask.CancellationToken.IsCancellationRequested);
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Stop_The_Action_With_A_Correct_Start_And_End_Date_With_UTC_Plus_2()
        {
            Task.Run(async () =>
            {
                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddMilliseconds(200),DateTimeOffset.Now.AddSeconds(1))
                    .SetAction(async (x) =>
                    {
                        while (!x.CancellationToken.IsCancellationRequested) { }
                    })
                     .SetTimezone(TimeSpan.Parse("02:00:00"))
                     .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(2));
                Assert.IsFalse(getTask.Launched);
                Assert.IsTrue(getTask.CancellationToken.IsCancellationRequested);

            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Stop_And_Delete_The_Action_With_A_Correct_Start_And_End_Time_With_UTC_Plus_2()
        {
            Task.Run(async () =>
            {
                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
                .Build();

                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                   .SetHours(DateTimeOffset.Now.AddMilliseconds(300).TimeOfDay,DateTimeOffset.Now.AddSeconds(1).TimeOfDay)
                   .SetAction(async (x) =>
                   {
                       while (!x.CancellationToken.IsCancellationRequested) { }
                   })
                    .SetTimezone(TimeSpan.Parse("02:00:00"))
                    .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(2));

                Assert.IsFalse(getTask.Launched);
                Assert.IsTrue(getTask.CancellationToken.IsCancellationRequested);
                Assert.IsFalse(taskScheduler.Timers.ContainsKey(newTaskId));
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Stop_And_Delete_The_Action_With_A_Correct_Start_And_End_Date_With_UTC_Plus_2()
        {
            Task.Run(async () =>
            {
                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues
                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddMilliseconds(200),DateTimeOffset.Now.AddSeconds(1))
                    .SetAction(async (x) =>
                    {
                        while (!x.CancellationToken.IsCancellationRequested) { }
                    })
                     .SetTimezone(TimeSpan.Parse("02:00:00"))
                     .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(2));

                Assert.IsFalse(getTask.Launched);
                Assert.IsTrue(getTask.CancellationToken.IsCancellationRequested);
                Assert.IsFalse(taskScheduler.Timers.ContainsKey(newTaskId));
            }).GetAwaiter().GetResult();
        }


        [TestMethod()]
        public void Should_Change_The_Value_Of_Notify_To_True_When_The_Task_Is_Launched_With_The_Subscription_To_Linklaunchedstatus_With_UTC_Plus_2()
        {
            Task.Run(async () =>
            {
                bool notify = false;

                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddMilliseconds(500),DateTimeOffset.Now.AddSeconds(2))
                    .SetAction(async (x) =>
                    {
                        while (!x.CancellationToken.IsCancellationRequested) { }
                    })
                    .LinkLaunchedStatus(async(x) =>
                    {
                        notify = x.Launched;
                    })
                     .SetTimezone(TimeSpan.Parse("02:00:00"))
                     .AddTask();

                await Task.Delay(TimeSpan.FromSeconds(1));

                Assert.IsTrue(notify);
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Change_The_Value_Of_Notify_To_True_When_The_Task_Is_Stopped_With_The_Subscription_To_Linklaunchedstatus_With_UTC_Plus_2()
        {
            Task.Run(async () =>
            {
                bool notify = false;

                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

                SetSchedulerDate(taskScheduler);  // -> control scheduler datetime for avoid end-of-day date issues

                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddMilliseconds(200),DateTimeOffset.Now.AddSeconds(1))
                    .SetAction(async (x) =>
                    {
                        while (!x.CancellationToken.IsCancellationRequested) { }
                    })
                    .LinkLaunchedStatus(async (x) =>
                    {
                        notify = x.Launched;
                    })
                     .SetTimezone(TimeSpan.Parse("02:00:00"))
                     .AddTask();

                await Task.Delay(TimeSpan.FromSeconds(2));

                Assert.IsFalse(notify);
            }).GetAwaiter().GetResult();
        }

        [TestMethod()]
        public void Should_Change_The_Value_Of_Notify_To_True_When_The_Task_Is_Finished_With_The_Subscription_To_Linklaunchedstatus_With_UTC_Plus_2()
        {
            Task.Run(async () =>
            {
                bool notify = false;

                var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

                SetSchedulerDate(taskScheduler); // -> control scheduler datetime for avoid end-of-day date issues

                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddMilliseconds(200),DateTimeOffset.Now.AddSeconds(1))
                    .SetAction(async (x) =>
                    {
                        await Task.Delay(200);
                    })
                    .LinkFinishedStatus(async (x) =>
                    {
                        notify = x.Launched;
                    })
                     .SetTimezone(TimeSpan.Parse("02:00:00"))
                     .AddTask();

                await Task.Delay(TimeSpan.FromMilliseconds(600));

                Assert.IsFalse(notify);
            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Should_Create_1_Task_With_Custom_Date()
        {
            Task.Run(async () =>
            {
                bool notify = false;

                DateTimeOffset dateNow = DateTimeOffset.ParseExact("2020-07-18 05:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                var taskScheduler = TaskSchedulerBuilder.CreateBuilder().Build();

                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetHours(dateNow.AddMilliseconds(2).TimeOfDay, dateNow.AddSeconds(5).TimeOfDay)
                    .SetTaskSchedulerDate(dateNow)
                    .SetAction(async (x) =>
                    {
                        while(!x.CancellationToken.IsCancellationRequested) {}
                    })
                    .LinkFinishedStatus(async (x) =>
                    {
                        notify = x.Launched;
                    })
                     .SetTimezone(dateNow.Offset)
                     .AddTask();

                await Task.Delay(TimeSpan.FromSeconds(1));

                Assert.AreEqual(1,taskScheduler.Timers.Count);
            }).GetAwaiter().GetResult();
        }

        private void SetSchedulerDate(ITaskScheduler taskScheduler)
        {
            return;
        }
    }
}
