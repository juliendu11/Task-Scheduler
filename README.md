# Task scheduler
Task programming module with times or date, run a Task action for C# (.net core)

#### Everything is extensible to implement your own behavior and methods

## Install

```
PM> Install-Package SimplyTaskScheduler -Version 1.1.0
```

## How to use ? 

Create a instance of ITaskScheduler, I advise you to create only one instance for the whole application

```c#
static ITaskScheduler taskScheduler;

static void Main(string[] args)
{
  Console.WriteLine("Hello World!");
  
  taskScheduler = TaskSchedulerBuilder.CreateBuilder()
    .Build();
}
```
---

### 🛠️ Create a simple Task

```c#
static void NewTaskSimpleTaskArgs()
{
      string newTaskId = "";

      newTaskId = taskScheduler.TaskAdder
        .SetHours("14:00", "15:00") //In 24hr format, it's for current Day
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
```
---

### 🛠️ Create a simple Task with Payload

```c#
static void NewTaskSimpleTaskArgsWithPayload()
{
      string newTaskId = "";

      newTaskId = taskScheduler.TaskAdder
        .SetHours("14:00", "15:00") //In 24hr format, it's for current Day
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
        .SetPayload(new MyCustomPayload { UserId = Guid.NewGuid().ToString(), Username = "Bob" })//Optional
        .SetTimezone(DateTimeOffset.Now.Offset)//Optional
        .LinkFinishedStatus(taskArg => { Console.WriteLine($"Tâche: {taskArg.TaskId}, finished: {taskArg.Finished}"); })//Optional
        .LinkLaunchedStatus(taskArg =>
            {
            var taskArgWithPayload = taskArg.ConvertTaskArg<TaskArgWithPayload<MyCustomPayload>>(); //Convert TaskArg to payload object (MyCustomPayload) because is interface
            Console.WriteLine($"Tâche: {taskArg.TaskId}, launched: {taskArg.Launched}, user: {taskArgWithPayload.Payload.Username}");
            })//Optional
          
        .AddTask();
}
```

- The payload is just used to add additional information on the task, when you retrieve the task, more precisely the TaskArg you can access this information. You can for example put a name of a user there to know for whom the task was scheduled and launched

---

### 🛠️ Create a simple Task with Custom Arg

```c#
static void NewTaskWithCustomTaskArg()
{
      string newTaskId = "";

      newTaskId = taskScheduler.TaskAdder
        .SetHours("14:00", "15:00") //In 24hr format, it's for current Day
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
```

- You can create your own TaskArg by implementing the ITaskArg interface on it.
- If you want to add behavior related to your personal TaskArg such as for example getting tasks relative to a property in your personal TaskArg (you can also do this for the version with payload) you can create your own TaskScheduler by implementing the interface ITaskScheduler and add your methods

## ⚠️ Warning 

 - Do not use SetHours and SetDay at the same time it is either one or the other
 - Do not use SetCustomTaskArg and SetPayload at the same time it is either one or the other
 - You can manage the timezone with SetTimezone, the create task will be created in relation to this timezone, if no timezone is specified, the timezone will be from the current system
 - SetAction must be a task action, you must leave the "async" if you put a synchronous action
 - If you put an asynchronous action or a loop for SetAction, use the taskArg token as in the examples above, this will force the task to stop once the schedule has ended
 
## LinkFinishedStatus and LinkLaunchedStatus
 
This is optional, you do not have to implement LinkFinishedStatus and LinkLaunchedStatus.

As for the examples below you can link to the change of status of Finished and Launched of the new task, you take action. Like sending a message when the task is finished (Launched = true => Launched = false == Send message).

The Finished status in TaskArg is there to inform if the task has been completed before the end of time (stopTime)

The Launched status in TaskArg is there to inform if the task is currently launched

## Example Custom TaskArg AND Custom TaskScheduler

```c#
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
```

```c#
public class CustomTaskScheduler : ITaskScheduler
{
    public Dictionary<string, ITaskArg> Timers { get; private set; }
    public DateTimeOffset SchedulerDateTime { get; internal set; }

    public TaskScheduler.Classes.TaskAdder TaskAdder
    {
        get
        {
            return new TaskScheduler.Classes.TaskAdder(this);
        }
    }

    public TaskScheduler.Classes.TimerCreator TimerCreator { get; private set; }
    public Options Options { get; private set; }


    public CustomTaskScheduler()
    {
        this.Timers = new Dictionary<string, ITaskArg>();
        this.SchedulerDateTime = DateTimeOffset.Now;
        this.TimerCreator = new TaskScheduler.Classes.TimerCreator(this);
        this.Options = new Options();
    }


    public void UpdateTimezone(TimeSpan timezone)
    {
        this.SchedulerDateTime.ToOffset(timezone);
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
        this.TimerCreator.DeleteTask(taskId, true);
      }
}
```

```c#
static ITaskScheduler taskScheduler;

static void Main(string[] args)
{
  Console.WriteLine("Hello World!");
  
  taskScheduler = TaskSchedulerBuilder.CreateBuilder()
                .SetCustomTaskScheduler(new CustomTaskScheduler())
                .Build();
}
```

## Stop and Delete tasks

Stop and Delete task with ```taskScheduler.StopAndDeleteTask(taskid)``` for one task

Stop and Delete tasks with ```taskScheduler.StopAndDeleteAllTasks()``` for all tasks in List
