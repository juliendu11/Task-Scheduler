using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
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
                   .SetHours(DateTimeOffset.Now.AddSeconds(3).TimeOfDay, DateTimeOffset.Now.AddSeconds(10).TimeOfDay)
                   .SetAction(async (x) =>
                   {
                       while (!x.CancellationToken.IsCancellationRequested) { }
                   })
                    .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(5));
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

                var newTaskId = taskScheduler.TaskAdder
                    .SetDay(DateTimeOffset.Now.AddSeconds(3), DateTimeOffset.Now.AddSeconds(10))
                    .SetAction(async (x) =>
                    {
                        while (!x.CancellationToken.IsCancellationRequested) { }
                    })
                     .AddTask();

                var getTask = taskScheduler.GetTasksArgWithId(newTaskId);
                await Task.Delay(TimeSpan.FromSeconds(4));
                Assert.IsTrue(getTask.Launched);
            }).GetAwaiter().GetResult();
        }
    }
}
