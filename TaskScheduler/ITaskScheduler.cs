﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TaskScheduler.Models;

namespace TaskScheduler
{
    public interface ITaskScheduler
    {
        public Dictionary<string, ITaskArg> Timers { get; }

        Classes.TaskAdder TaskAdder { get; }

        DateTimeOffset SchedulerDateTime { get; }

        void UpdateTimezone(TimeSpan timezone);

        ITaskArg GetTasksArgWithId(string taskid);

        bool GetTasksLaunchedStatusWithId(string taskid);

        bool GetTasksFinishedStatusWithId(string taskid);

        bool VerifyTaskExistWithId(string taskid);

        CancellationToken GetTaskToken(string taskid);

        List<ITaskArg> GetAllTasks();

        Options Options { get; }

        Classes.TimerCreator TimerCreator { get; }
    }
}