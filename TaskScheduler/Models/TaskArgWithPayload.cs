using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskScheduler.Models
{
    public class TaskArgWithPayload<T> :ITaskArg
    {
        private bool launched;
        private bool finished;

        public string TaskId { get;  set; }

        public CancellationTokenSource CancellationToken { get; set; }

        public T Payload { get;  }

        public Func<ITaskArg,Task> Action { get;  set; }

        public TimeSpan? Timezone { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset StopTime { get; set; }

        public Enums.TaskTimeMode TaskTimeMode { get; set; }

        public Timer StartTimer { get; set; }

        public Timer StopTimer { get; set; }

        public Action<ITaskArg> ActionWhenLaunchedChanged { get; set; }

        public Action<ITaskArg> ActionWhenFinishedChanged { get; set; }

        public bool Launched
        {
            get => launched;
            set
            {
                launched = value;
                if (ActionWhenLaunchedChanged !=null)
                    ActionWhenLaunchedChanged.Invoke(this);
            }
        }

        public bool Finished
        {
            get => finished;
            set
            {
                finished = value;
                if (ActionWhenFinishedChanged !=null)
                    ActionWhenFinishedChanged.Invoke(this);
            }
        }

        public TaskArgWithPayload(T payload)
        {
            this.Payload = payload;
        }
    }
}
