using System;
using System.Threading.Tasks;
using System.Linq;
using TaskScheduler;
using TaskScheduler.Models;
using System.Threading;
using TaskScheduler.Enums;
using TaskScheduler.Helpers;
using System.Collections.Generic;
using TaskScheduler.Classes;

namespace RealTest
{
    class Program
    {
        static ITaskScheduler taskScheduler;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            taskScheduler = TaskSchedulerBuilder.CreateBuilder()
                .Build();


            var nowDate = DateTimeOffset.Now;

            string newTaskId = "";

            newTaskId = taskScheduler.TaskAdder
                .SetHours(nowDate.AddSeconds(8).TimeOfDay, nowDate.AddMinutes(3).TimeOfDay)
                .SetAction(async (taskArg) =>
                {
                    var token = taskArg.CancellationToken.Token;
                    int i = 0;
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), token);
                        Console.WriteLine("Hello: " + i);
                        i++;
                    }
                })
                .SetCustomTaskArg(new CustomTaskArgs { Username = "Bob", UserId = Guid.NewGuid().ToString() })
                .SetTimezone(nowDate.Offset)
                .LinkFinishedStatus(taskArg => { Console.WriteLine($"Tâche: {taskArg.TaskId}, finished: {taskArg.Finished}"); })
                  .LinkLaunchedStatus(taskArg =>
                  {
                      var taskArgWithPayload = taskArg.ConvertTaskArg<CustomTaskArgs>();
                      Console.WriteLine($"Tâche: {taskArg.TaskId}, launched: {taskArg.Launched}, user: {taskArgWithPayload.Username}");
                  })
                .AddTask();

            while (Console.ReadKey().Key != ConsoleKey.Escape) 
            {
            }
        }

        static void NewTaskSimpleTaskArgs()
        {
            string newTaskId = "";

            newTaskId = taskScheduler.TaskAdder
                .SetHours(DateTimeOffset.Now.AddMinutes(20).TimeOfDay, DateTimeOffset.Now.AddMinutes(50).TimeOfDay) //In 24hr format
                .SetDay(DateTimeOffset.Parse("2020-07-16 16:00:00"), DateTimeOffset.Parse("2020-07-18 12:00:00")) //Or user DateTime for set with specific date. Do not use SetHours and SetDay at the same time !!!
                .SetAction(async (taskArg) =>
                {
                    var token = taskArg.CancellationToken.Token;
                    int i = 0;
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), token);
                        Console.WriteLine("Hello: " + i);
                        i++;
                    }
                })
                .SetTimezone(DateTimeOffset.Now.Offset)//Optional
                .LinkFinishedStatus(taskArg => { Console.WriteLine($"Tâche: {taskArg.TaskId}, finished: {taskArg.Finished}"); })//Optional
                .LinkLaunchedStatus(taskArg => { Console.WriteLine($"Tâche: {taskArg.TaskId}, launched: {taskArg.Launched}"); })//Optional
                .AddTask();
        }

        static void NewTaskSimpleTaskArgsWithPayload()
        {
            string newTaskId = "";

            newTaskId = taskScheduler.TaskAdder
                .SetHours(DateTimeOffset.Now.AddMinutes(20).TimeOfDay, DateTimeOffset.Now.AddMinutes(50).TimeOfDay) //In 24hr format
                .SetDay(DateTimeOffset.Parse("2020-07-16 16:00:00"), DateTimeOffset.Parse("2020-07-18 12:00:00")) //Or user DateTime for set with specific date. Do not use SetHours and SetDay at the same time !!!
                .SetAction(async (taskArg) =>
                {
                    var token = taskArg.CancellationToken.Token;
                    int i = 0;
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), token);
                        Console.WriteLine("Hello: " + i);
                        i++;
                    }
                })
                .SetTimezone(DateTimeOffset.Now.Offset)//Optional
                .SetPayload(new MyCustomPayload { UserId = Guid.NewGuid().ToString(), Username = "Bob" })//Optional
                .LinkFinishedStatus(taskArg => { Console.WriteLine($"Tâche: {taskArg.TaskId}, finished: {taskArg.Finished}"); })//Optional
                .LinkLaunchedStatus(taskArg =>
                {
                    var taskArgWithPayload = taskArg.ConvertTaskArg<TaskArgWithPayload<MyCustomPayload>>(); //Convert TaskArg to payload object (MyCustomPayload) because is interface
                    Console.WriteLine($"Tâche: {taskArg.TaskId}, launched: {taskArg.Launched}, user: {taskArgWithPayload.Payload.Username}");
                })//Optional
                .AddTask();
        }

        static void NewTaskWithCustomTaskArg()
        {
            string newTaskId = "";
            var nowDate = DateTimeOffset.Now;

            newTaskId = taskScheduler.TaskAdder
                .SetHours(DateTimeOffset.Now.AddMinutes(20).TimeOfDay, DateTimeOffset.Now.AddMinutes(50).TimeOfDay) //In 24hr format
                 .SetDay(nowDate.AddDays(2), nowDate.AddDays(4)) //Or user DateTime for set with specific date. Do not use SetHours and SetDay at the same time !!!
                .SetAction(async (taskArg) =>
                {
                    var token = taskArg.CancellationToken.Token;
                    int i = 0;
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), token);
                        Console.WriteLine("Hello: " + i);
                        i++;
                    }
                })
                .SetTimezone(nowDate.Offset) //Optional
                .SetCustomTaskArg(new CustomTaskArgs { Username = "Bob", UserId = Guid.NewGuid().ToString() }) //Optional
                .LinkFinishedStatus(taskArg =>
                {
                    var taskArgWithPayload = taskArg.ConvertTaskArg<CustomTaskArgs>(); //Convert TaskArg to custom TaskArg object (CustomTaskArgs) because is interface
                    Console.WriteLine($"Tâche: {taskArg.TaskId}, finished: {taskArg.Launched}, user: {taskArgWithPayload.Username}");
                })//Optional
                .LinkLaunchedStatus(taskArg =>
                  {
                      var taskArgWithPayload = taskArg.ConvertTaskArg<CustomTaskArgs>(); //Convert TaskArg to custom TaskArg object (CustomTaskArgs) because is interface
                      Console.WriteLine($"Tâche: {taskArg.TaskId}, launched: {taskArg.Launched}, user: {taskArgWithPayload.Username}");
                  })//Optional
                .AddTask();
        }

        class CustomTaskArgs : ITaskArg
        {
            private bool launched;
            private bool finished;

            public string TaskId { get;  set; }
            public CancellationTokenSource CancellationToken { get;  set; }
            public Func<ITaskArg, Task> Action { get;  set; }
            public TimeSpan? Timezone { get;  set; }
            public DateTimeOffset StartTime { get;  set; }
            public DateTimeOffset StopTime { get;  set; }
            public TaskTimeMode TaskTimeMode { get;  set; }
            public Timer StartTimer { get;  set; }
            public Timer StopTimer { get;  set; }
            public Action<ITaskArg> ActionWhenLaunchedChanged { get;  set; }
            public Action<ITaskArg> ActionWhenFinishedChanged { get;  set; }


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

        class CustomTaskScheduler : ITaskScheduler
        {
            private TimeSpan defaultTimezone;

            public Dictionary<string, ITaskArg> Timers { get; private set; }
            public DateTimeOffset SchedulerDateTime { get; internal set; }

            private Action linkTasksLaunched;
            private Action linkTasksFinished;

            private TaskScheduler.Classes.TaskAdder taskAdder;
            public TaskScheduler.Classes.TaskAdder TaskAdder
            {
                get
                {
                    SchedulerDateTime.ToOffset(defaultTimezone);
                    return taskAdder.CleanUp();
                }
            }

            public TaskScheduler.Classes.TimerCreator TimerCreator { get; private set; }
            public Options Options { get; private set; }


            public CustomTaskScheduler(Action linkTasksLaunched, Action linkTasksFinished)
            {
                this.Timers = new Dictionary<string, ITaskArg>();
                this.SchedulerDateTime = DateTimeOffset.Now;
                this.defaultTimezone = DateTimeOffset.Now.Offset;
                this.TimerCreator = new TaskScheduler.Classes.TimerCreator(this);
                this.Options = new Options();
                this.linkTasksLaunched = linkTasksLaunched;
                this.linkTasksFinished = linkTasksFinished;
                this.taskAdder = new TaskScheduler.Classes.TaskAdder(this);
            }


            public void UpdateTimezone(TimeSpan timezone)
            {
                this.SchedulerDateTime.ToOffset(timezone);
            }

            public void UpdateTaskSchedulerDate(DateTimeOffset newDate)
            {
                this.SchedulerDateTime = newDate;
            }


            public ITaskArg GetTasksArgWithId(string taskid)
            {
                if (!VerifyTaskExistWithId(taskid))
                    throw new Exception("This tasks with this id not exist in list");

                return this.Timers[taskid];
            }

            // -> Custom implementation for get by Username with my CustomTaskArg
            // -> Use ConvertTaskArg extension methods to easily convert
            public List<ITaskArg> GetTasksArgWithUsername(string username) 
            {
                return this.Timers.Values.Where(taskArg=> taskArg.ConvertTaskArg<CustomTaskArgs>().Username == username).ToList();
            }
            //
          

            public bool GetTasksLaunchedStatusWithId(string taskid)
            {
                return GetTasksArgWithId(taskid).Launched;
            }

            public bool GetTasksFinishedStatusWithId(string taskid)
            {
                return GetTasksArgWithId(taskid).Finished;
            }

            public bool VerifyTaskExistWithId(string taskid)
            {
                return this.Timers.ContainsKey(taskid);
            }


            public List<ITaskArg> GetAllTasks() => this.Timers.Values.ToList();

            public CancellationToken GetTaskToken(string taskid)
            {
                return GetTasksArgWithId(taskid).CancellationToken.Token;
            }

            public void StopAndDeleteAllTasks()
            {
                if (this.Timers.Count != 0)
                    this.Timers.Keys.ToList().ForEach(taskid => this.StopAndDeleteTask(taskid));
            }

            public void StopAndDeleteTask(string taskId)
            {
                if (!VerifyTaskExistWithId(taskId))
                    throw new Exception("This tasks with this id not exist in list");

                this.TimerCreator.DeleteTask(taskId);
                this.Timers.Remove(taskId);
            }
        }

        struct MyCustomPayload
        {
            public string Username { get; set; }
            public string UserId { get; set; }
        }
        
    }
}
