using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TaskScheduler.Classes;
using TaskScheduler.Models;

namespace TaskScheduler
{
    public class TaskSchedulerBuilder
    {
        private ITaskScheduler taskScheduler;

        private Action linkTasksLaunched;
        private Action linkTasksFinished;

        private TaskSchedulerBuilder() { }

        public static TaskSchedulerBuilder CreateBuilder()
        {
            return new TaskSchedulerBuilder();
        }

        public TaskSchedulerBuilder SetCustomTaskScheduler(ITaskScheduler taskScheduler)
        {
            this.taskScheduler = taskScheduler;
            return this;
        }

        public TaskSchedulerBuilder LinkWhenTasksLaunchedStatusChanged(Action action)
        {
            this.linkTasksLaunched = action;
            return this;
        }

        public TaskSchedulerBuilder LinkWhenTasksFinishedStatusChanged(Action action)
        {
            this.linkTasksFinished = action;
            return this;
        }


        public ITaskScheduler Build()
        {
            if (taskScheduler == null)
                taskScheduler = new TaskScheduler(linkTasksLaunched, linkTasksFinished);

            return taskScheduler;
        }
    }
}
