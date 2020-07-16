using System;
using System.Collections.Generic;
using System.Text;
using TaskScheduler.Models;

namespace TaskScheduler.Helpers
{
    public static class Extensions
    {
        public static T ConvertTaskArg<T>(this ITaskArg taskArg)
        {
            return (T)taskArg;
        }
    }
}
