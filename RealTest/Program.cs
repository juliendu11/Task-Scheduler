using System;
using System.Threading.Tasks;
using System.Linq;
using TaskScheduler;
using TaskScheduler.Models;
using System.Threading;
using TaskScheduler.Enums;
using TaskScheduler.Helpers;

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
            var nowDate = DateTimeOffset.Now;

            newTaskId = taskScheduler.TaskAdder
                .SetHours("14:00", "15:00") //In 24hr format
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
                .SetTimezone(nowDate.Offset)//Optional
                .LinkFinishedStatus(taskArg => { Console.WriteLine($"Tâche: {taskArg.TaskId}, finished: {taskArg.Finished}"); })//Optional
                .LinkLaunchedStatus(taskArg => { Console.WriteLine($"Tâche: {taskArg.TaskId}, launched: {taskArg.Launched}"); })//Optional
                .AddTask();
        }

        static void NewTaskSimpleTaskArgsWithPayload()
        {
            string newTaskId = "";
            var nowDate = DateTimeOffset.Now;

            newTaskId = taskScheduler.TaskAdder
                .SetHours("14:00", "15:00") //In 24hr format
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
                .SetTimezone(nowDate.Offset)//Optional
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
                .SetHours("14:00", "15:00") //In 24hr format
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

        struct MyCustomPayload
        {
            public string Username { get; set; }
            public string UserId { get; set; }
        }
        
    }
}
