using System;
using System.Threading.Tasks;

namespace RealTest
{
    class Program
    {
        static TaskScheduler.TaskScheduler taskScheduler;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            taskScheduler = new TaskScheduler.TaskScheduler();

            string newtTaskId = taskScheduler.TaskAdder
                .SetHours(TimeSpan.Parse("00:40"), TimeSpan.Parse("00:43"))
                //.SetDay(DateTimeOffset.Now.AddMinutes(5), DateTimeOffset.Now.AddDays(5))
                //.SetTimezone(TimeSpan.Parse("02:00:00"))
                .SetAction(() => { Console.WriteLine("Salut"); })
                .SetPayload("salut")
                .AddTask();
            

            while (Console.ReadKey().Key != ConsoleKey.Escape) 
            {
                CheckTasksNumber();
            }
        }

        static async void CheckTasksNumber()
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            Console.WriteLine(taskScheduler.GetAllTasks.Count);
        }
    }
}
