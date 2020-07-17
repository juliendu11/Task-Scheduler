using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
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

                taskScheduler.UpdateTaskSchedulerDate(DateTimeOffset.Parse("2020-07-18 15:00:00", CultureInfo.InvariantCulture)); // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                   .SetHours(taskScheduler.SchedulerDateTime.AddMilliseconds(500).TimeOfDay, taskScheduler.SchedulerDateTime.AddSeconds(10).TimeOfDay)
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

               taskScheduler.UpdateTaskSchedulerDate(DateTimeOffset.Parse("2020-07-18 15:00:00", CultureInfo.InvariantCulture)); // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(taskScheduler.SchedulerDateTime.AddMilliseconds(500), taskScheduler.SchedulerDateTime.AddSeconds(10))
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

                taskScheduler.UpdateTaskSchedulerDate(DateTimeOffset.Parse("2020-07-18 15:00:00", CultureInfo.InvariantCulture)); // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                   .SetHours(taskScheduler.SchedulerDateTime.AddMilliseconds(200).TimeOfDay, taskScheduler.SchedulerDateTime.AddSeconds(1).TimeOfDay)
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

                taskScheduler.UpdateTaskSchedulerDate(DateTimeOffset.Parse("2020-07-18 15:00:00", CultureInfo.InvariantCulture)); // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(taskScheduler.SchedulerDateTime.AddMilliseconds(200), taskScheduler.SchedulerDateTime.AddSeconds(1))
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

                taskScheduler.UpdateTaskSchedulerDate(DateTimeOffset.Parse("2020-07-18 15:00:00", CultureInfo.InvariantCulture)); // -> control scheduler datetime for avoid end-of-day date issues

                var newTaskId = taskScheduler.TaskAdder
                   .SetHours(taskScheduler.SchedulerDateTime.AddMilliseconds(300).TimeOfDay, taskScheduler.SchedulerDateTime.AddSeconds(1).TimeOfDay)
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

                taskScheduler.UpdateTaskSchedulerDate(DateTimeOffset.Parse("2020-07-18 15:00:00", CultureInfo.InvariantCulture)); // -> control scheduler datetime for avoid end-of-day date issues
                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(taskScheduler.SchedulerDateTime.AddMilliseconds(200), taskScheduler.SchedulerDateTime.AddSeconds(1))
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

                taskScheduler.UpdateTaskSchedulerDate(DateTimeOffset.Parse("2020-07-18 15:00:00", CultureInfo.InvariantCulture)); // -> control scheduler datetime for avoid end-of-day date issues

                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(taskScheduler.SchedulerDateTime.AddMilliseconds(500), taskScheduler.SchedulerDateTime.AddSeconds(2))
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

                taskScheduler.UpdateTaskSchedulerDate(DateTimeOffset.Parse("2020-07-18 15:00:00", CultureInfo.InvariantCulture)); // -> control scheduler datetime for avoid end-of-day date issues

                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(taskScheduler.SchedulerDateTime.AddMilliseconds(200), taskScheduler.SchedulerDateTime.AddSeconds(1))
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

                taskScheduler.UpdateTaskSchedulerDate(DateTimeOffset.Parse("2020-07-18 15:00:00", CultureInfo.InvariantCulture)); // -> control scheduler datetime for avoid end-of-day date issues

                taskScheduler.Options.DeleteTaskAfterCompleted = true;

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(taskScheduler.SchedulerDateTime.AddMilliseconds(200), taskScheduler.SchedulerDateTime.AddSeconds(1))
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
    }
}
