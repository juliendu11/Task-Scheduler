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
using System.Globalization;

namespace RealTest
{
    class Program
    {
        static ITaskScheduler taskScheduler;
        static void Main(string[] args)
        {
            var nowDate = DateTimeOffset.Now;

            taskScheduler = TaskScheduler.TaskSchedulerBuilder.CreateBuilder()
                .Build();
           
            for(int i=0; i< 50; i++)
            {
                 var newId = GenerateId();
            taskScheduler.Timers.Add(newId, new TaskArgWithPayload<string>(""));
            Console.WriteLine(newId);
            }

            Console.ReadLine();
        }

        private static string GenerateId()
        {
            string id = string.Empty;
            do
            {
                string randomString = DateTimeOffset.UtcNow.Ticks.ToString();
                string ramdomString2 = Guid.NewGuid().ToString("N");
                id = randomString + ramdomString2;
            }
            while (taskScheduler.CheckIdExist(id));

            return id;
        }
    }
}
