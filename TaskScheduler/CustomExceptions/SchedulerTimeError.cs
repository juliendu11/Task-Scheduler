using System;

namespace TaskScheduler.CustomExceptions
{
    public class SchedulerTimeError : Exception
    {
        private static string baseMessage = "Time issue for task scheduling, {0}";

        public SchedulerTimeError()
        {
        }

        public SchedulerTimeError(string message)
            : base(string.Format(baseMessage, message))
        {
        }

        public SchedulerTimeError(string message, Exception inner)
            : base(string.Format(baseMessage, message), inner)
        {
        }
    }
}
