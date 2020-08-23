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
            Console.WriteLine($"Test: {nowDate}");

            while (Console.ReadKey().Key != ConsoleKey.Escape) 
            {
            }
        }
    }
}
