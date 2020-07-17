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
        public void Should_Delete_And_Stop_Task_Send_Valid_Taskid_2_Task_In_The_List_There_Must_Be_Only_1()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
             .Build();

            var timerCreator = new Classes.TimerCreator(taskScheduler);

            var task1 = BuildFakeTaskArg();
            var task2 = BuildFakeTaskArg();

            taskScheduler.Timers.Add(task1.TaskId, task1);
            taskScheduler.Timers.Add(task2.TaskId, task2);

            timerCreator.DeleteTask(task1.TaskId, true);

            Assert.AreEqual(1, taskScheduler.Timers.Count);
        }

        [TestMethod()]
        public void Should_Delete_And_Stop_Task_Sends_A_Valid_Taskid_2_Task_In_The_List_It_Must_Have_The_Same_Number_But_1_Task_With_Its_Timers_At_Null_And_A_Cancellation_Request_For_The_Token()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
             .Build();

            var timerCreator = new Classes.TimerCreator(taskScheduler);

            var task1 = BuildFakeTaskArg();
            var task2 = BuildFakeTaskArg();

            taskScheduler.Timers.Add(task1.TaskId, task1);
            taskScheduler.Timers.Add(task2.TaskId, task2);

            timerCreator.DeleteTask(task1.TaskId, false);

            Assert.AreEqual(2, taskScheduler.Timers.Count);
            Assert.IsNull(task1.StartTimer);
            Assert.IsNull(task1.StopTimer);
            Assert.AreEqual(true, task1.CancellationToken.IsCancellationRequested);
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

            timerCreator.SetUpTimers(task1);

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

            Assert.ThrowsException<SchedulerTimeError>(() =>
            {
                timerCreator.SetUpTimers(task1);
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

            Assert.ThrowsException<SchedulerTimeError>(() =>
            {
                timerCreator.SetUpTimers(task1);
            });
        }
    }
}