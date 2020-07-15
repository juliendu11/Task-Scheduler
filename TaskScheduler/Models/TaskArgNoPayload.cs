using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskScheduler.Models
{
    public class TaskArgNoPayload : ITaskArg
    {
        public string TaskId { get;  set; }

        public CancellationTokenSource CancellationToken { get;  set; }

        public Task Action { get;  set; }

        public TimeSpan? Timezone { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset StopTime { get; set; }

        public Enums.TaskTimeMode TaskTimeMode { get; set; }

        public Timer StartTimer { get; set; }

        public Timer StopTimer { get; set; }

        public bool Launched { get; set; }

        public bool Finished { get; set; }
    }
}
