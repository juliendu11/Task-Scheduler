using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskScheduler.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.CustomExceptions;

namespace TaskScheduler.Classes.Tests
{
    [TestClass()]
    public class TimerCreatorTests
    {
        [TestMethod()]
        public void Should_Implement_A_Timer_For_StartTimer_And_Stoptimer_With_Date_Not_Smaller_Than_Now_Returns_A_Timer_For_StartTimer_And_StopTimer_Not_Null()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
           .Build();

            var timerCreator = new Classes.TimerCreator(taskScheduler);

            var task1 = BuildFakeTaskArg();
            task1.Action = new Func<Models.ITaskArg, Task>(async (x) => { });
            task1.StartTime = DateTimeOffset.Now.AddMinutes(5);
            task1.StopTime = task1.StartTime.AddMinutes(5);

            taskScheduler.Timers.Add(task1.TaskId, task1);
            timerCreator.SetUpTimers(task1.TaskId);

            Assert.IsNotNull(task1.StartTimer);
            Assert.IsNotNull(task1.StopTimer);
        }

        [TestMethod()]
        public void Should_Implement_A_Timer_For_StartTimer_And_Stoptimer_With_Start_Time_Smaller_Than_Now_Returns_Exception_SchedulerTimeError()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
                .Build();

            var timerCreator = new Classes.TimerCreator(taskScheduler);
            var task1 = BuildFakeTaskArg();
            task1.Action = new Func<Models.ITaskArg, Task>(async (x) => { });
            task1.StartTime = DateTimeOffset.Now.AddMinutes(-5);
            task1.StopTime = task1.StartTime.AddMinutes(5);
            taskScheduler.Timers.Add(task1.TaskId, task1);

            Assert.ThrowsException<SchedulerTimeError>(() =>
            {
                timerCreator.SetUpTimers(task1.TaskId);
            });
        }

        [TestMethod()]
        public void Should_Implement_A_Timer_For_StartTimer_And_Stoptimer_With_Stop_Time_Smaller_Than_Start_Time_Returns_Exception_SchedulerTimeError()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
              .Build();

            var timerCreator = new Classes.TimerCreator(taskScheduler);
            var task1 = BuildFakeTaskArg();
            task1.Action = new Func<Models.ITaskArg, Task>(async (x) => { });
            task1.StartTime = DateTimeOffset.Now.AddMinutes(5);
            task1.StopTime = task1.StartTime.AddMinutes(-10);
            taskScheduler.Timers.Add(task1.TaskId, task1);

            Assert.ThrowsException<SchedulerTimeError>(() =>
            {
                timerCreator.SetUpTimers(task1.TaskId);
            });
        }

        private Models.TaskArgWithPayload<object> BuildFakeTaskArg()
        {
            var task1Arg = new Models.TaskArgWithPayload<object>(null)
            {
                TaskId = Guid.NewGuid().ToString(),
                CancellationToken = new System.Threading.CancellationTokenSource(),
            };

            return task1Arg;
        }
    }
}