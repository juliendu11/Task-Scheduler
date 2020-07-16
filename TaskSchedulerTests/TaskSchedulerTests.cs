using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.Tests
{
    [TestClass()]
    public class TaskSchedulerTests
    {
        [TestMethod()]
        public void Should_Create_New_Instance_Of_TaskScheduler_And_TimerCreator_And_Options_Is_Instancied()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            
            Assert.IsNotNull(taskScheduler.TimerCreator);
            Assert.IsNotNull(taskScheduler.Options);
            Assert.AreEqual(false,taskScheduler.Options.DeleteTaskAfterCompleted);
        }

        [TestMethod()]
        public void Should_Create_New_Instance_Of_TaskAdder_Whenever_We_Create_New_Task()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            var firstTaskAdder = taskScheduler.TaskAdder;
            var secondTaskAdder = taskScheduler.TaskAdder;


            Assert.AreEqual(firstTaskAdder, firstTaskAdder);
            Assert.AreNotEqual(firstTaskAdder, secondTaskAdder);
        }

        [TestMethod()]
        public void Should_Update_Scheduler_DateTime_Property_With_New_Timezone()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            var schedulerDateTimeBeforeUpdate = taskScheduler.SchedulerDateTime;

            var newTimezone = TimeSpan.Parse("05:00:00");

            taskScheduler.UpdateTimezone(newTimezone);

            var schedulerDateTimeAfterUpdate = taskScheduler.SchedulerDateTime;

            Assert.AreEqual(schedulerDateTimeAfterUpdate, schedulerDateTimeBeforeUpdate.ToOffset(newTimezone));
        }

        [TestMethod()]
        public void Should_Get_Task_With_Existing_TaskId_Return_Task()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskid = Guid.NewGuid().ToString();

            taskScheduler.Timers.Add(taskid, new Models.TaskArgWithPayload<object>(null));

            Assert.IsNotNull(taskScheduler.GetTasksArgWithId(taskid));
        }

        [TestMethod()]
        public void Should_Get_Task_With_Not_Existing_TaskId_Return_Exception_Error()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskid = Guid.NewGuid().ToString();

            Assert.ThrowsException<Exception>(() => taskScheduler.GetTasksArgWithId(taskid));
        }

        [TestMethod()]
        public void Should_Get_Task_Launched_Status_With_Existing_TaskId_Return_False()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskid = Guid.NewGuid().ToString();

            taskScheduler.Timers.Add(taskid, new Models.TaskArgWithPayload<object>(null));

            Assert.AreEqual(false,taskScheduler.GetTasksLaunchedStatusWithId(taskid));
        }

        [TestMethod()]
        public void Should_Get_Task_Launched_Status_With_Not_Existing_TaskId_Return_Exception_Error()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskid = Guid.NewGuid().ToString();

            Assert.ThrowsException<Exception>(() => taskScheduler.GetTasksLaunchedStatusWithId(taskid));
        }


        [TestMethod()]
        public void Should_Get_Task_Launched_Status_With_Existing_TaskId_Return_True_Because_Is_Launched()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskid = Guid.NewGuid().ToString();

            taskScheduler.Timers.Add(taskid, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.Launched = true;

            Assert.AreEqual(true, taskScheduler.GetTasksLaunchedStatusWithId(taskid));
        }

        [TestMethod()]
        public void Should_Get_Task_Finished_Status_With_Existing_TaskId_Return_False()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskid = Guid.NewGuid().ToString();

            taskScheduler.Timers.Add(taskid, new Models.TaskArgWithPayload<object>(null));

            Assert.AreEqual(false, taskScheduler.GetTasksFinishedStatusWithId(taskid));
        }

        [TestMethod()]
        public void Should_Get_Task_Finished_Status_With_Not_Existing_TaskId_Return_Exception_Error()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskid = Guid.NewGuid().ToString();

            Assert.ThrowsException<Exception>(() => taskScheduler.GetTasksFinishedStatusWithId(taskid));
        }


        [TestMethod()]
        public void Should_Get_Task_Launched_Status_With_Existing_TaskId_Return_True_Because_Is_Finished()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskid = Guid.NewGuid().ToString();

            taskScheduler.Timers.Add(taskid, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.Finished = true;

            Assert.AreEqual(true, taskScheduler.GetTasksFinishedStatusWithId(taskid));
        }

        [TestMethod()]
        public void Should_Get_Task_Token_With_Existing_TaskId_Return_Token()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskid = Guid.NewGuid().ToString();

            taskScheduler.Timers.Add(taskid, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.CancellationToken = new System.Threading.CancellationTokenSource();

            Assert.IsNotNull(taskScheduler.GetTaskToken(taskid));
        }

        [TestMethod()]
        public void Should_Get_Task_Token_With_Not_Existing_TaskId_Return_Exception_Error()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskid = Guid.NewGuid().ToString();

            Assert.ThrowsException<Exception>(() => taskScheduler.GetTaskToken(taskid));
        }

        [TestMethod()]
        public void Should_Get_TimerArgs_List_2_Timers_Created_Return_TimerArgs_List_With_2_Timers()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskID = Guid.NewGuid().ToString();
            string taskID2 = Guid.NewGuid().ToString();

            taskScheduler.Timers.Add(taskID, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.TaskId = taskID;
            taskScheduler.Timers.Add(taskID2, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.TaskId = taskID2;

            var getTimers = taskScheduler.GetAllTasks();

            Assert.IsNotNull(getTimers);
            Assert.AreEqual(getTimers.Count, 2);
            Assert.AreEqual(getTimers[0].TaskId, taskID);
            Assert.AreEqual(getTimers[1].TaskId, taskID2);
        }

        [TestMethod()]
        public void Should_Delete_All_Tasks_With_2_Task_In_List_Return_Timers_List_Empty()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskID = Guid.NewGuid().ToString();
            string taskID2 = Guid.NewGuid().ToString();

            taskScheduler.Timers.Add(taskID, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.TaskId = taskID;
            taskScheduler.Timers.Add(taskID2, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.TaskId = taskID2;

            taskScheduler.StopAndDeleteAllTasks();

            Assert.AreEqual(0,taskScheduler.Timers.Count);
        }

        [TestMethod()]
        public void Should_Delete_All_Tasks_With_Not_Existing_Task_In_List_Return_Timers_List_Empty()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            taskScheduler.StopAndDeleteAllTasks();

            Assert.AreEqual(0, taskScheduler.Timers.Count);
        }

        [TestMethod()]
        public void Should_Delete_1_Tasks_With_2_Task_In_List_Return_Timers_List_1_Task()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskID = Guid.NewGuid().ToString();
            string taskID2 = Guid.NewGuid().ToString();

            taskScheduler.Timers.Add(taskID, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.TaskId = taskID;
            taskScheduler.Timers.Add(taskID2, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.TaskId = taskID2;

            taskScheduler.StopAndDeleteTask(taskID);

            Assert.AreEqual(1, taskScheduler.Timers.Count);
        }

        [TestMethod()]
        public void Should_Delete_All_Tasks_With_2_Same_Task_Id_In_List_Return_Timers_List_0_Task()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskID = Guid.NewGuid().ToString();
            string taskID2 = Guid.NewGuid().ToString();

            taskScheduler.Timers.Add(taskID, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.TaskId = taskID;
            taskScheduler.Timers.Add(taskID2, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.TaskId = taskID2;

            taskScheduler.StopAndDeleteTask(taskID);
            taskScheduler.StopAndDeleteTask(taskID2);

            Assert.AreEqual(0, taskScheduler.Timers.Count);
        }

        [TestMethod()]
        public void Should_Delete_2_Tasks_With_2_Same_Task_Id_In_List_Return_Timers_List_0_Task()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskID = Guid.NewGuid().ToString();
            string taskID2 = Guid.NewGuid().ToString();

            taskScheduler.Timers.Add(taskID, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.TaskId = taskID;
            taskScheduler.Timers.Add(taskID2, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.TaskId = taskID2;

            taskScheduler.StopAndDeleteTask(taskID);
            taskScheduler.StopAndDeleteTask(taskID2);

            Assert.AreEqual(0, taskScheduler.Timers.Count);
        }

        [TestMethod()]
        public void Should_Delete_1_Tasks_With_Not_Task_In_Timers_List_Return_Exception_Error()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            Assert.ThrowsException<Exception>(() =>
            {
                taskScheduler.StopAndDeleteTask("54564646");
            });
        }

        [TestMethod()]
        public void Should_Delete_1_Tasks_With_1_Task_In_Timers_List_But_Not_Same_Id_Return_Exception_Error()
        {
            var taskScheduler = TaskSchedulerBuilder.CreateBuilder()
               .Build();

            string taskID = Guid.NewGuid().ToString();

            taskScheduler.Timers.Add(taskID, new Models.TaskArgWithPayload<object>(null));
            taskScheduler.Timers.Last().Value.TaskId = taskID;

            Assert.ThrowsException<Exception>(() =>
            {
                taskScheduler.StopAndDeleteTask("54564646");
            });
        }
    }
}