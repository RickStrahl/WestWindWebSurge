using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSurge.Server
{
    public class PerformanceStats
    {
        public PerformanceCounterList CounterList { get; private set; }
        public int IntervalMs { get; set; }

        public string MachineName
        {
            get { return CounterList.MachineName; }
            set { CounterList.MachineName = value; }
        }

        public PerformanceStats()
        {
            CounterList = new PerformanceCounterList();
            IntervalMs = 3000;
        }

        public void Configure()
        {
            CounterList.Add("Processor Load", "Processor", "% Processor Time", "_Total");
            CounterList.Add("Memory Usage", "Memory", "% Committed Bytes In Use");
            CounterList.Add("IIS Get Requests", "Web Service", "Get Requests/sec", "_Total");
            CounterList.Add("IIS Post Requests", "Web Service", "Post Requests/sec", "_Total");
            CounterList.Add("ASP.NET Current Requests", "ASP.NET", "Requests Current");
        }

        public async Task<bool> StartAsyncTask(int waitTimeMs)
        {
            await CounterList.GetValuesTaskAsync(1500);         
            return true;
        }

        public void Start(int waitTimeMs)
        {
            CounterList.GetValues(waitTimeMs);
        }
    }
}
