using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var counters = CounterList;

            counters.Add("Processor Load", "Processor", "% Processor Time", "_Total");
            counters.Add("Memory Usage", "Memory", "% Committed Bytes In Use");

            counters.Add("IIS Requests/sec", "Web Service", "Total Method Requests/sec", "_Total");
            //this one is very unreliable
            counters.Add("ASP.NET Request/Sec", "ASP.NET Applications", "Requests/Sec", "__Total__");

            counters.Add("ASP.NET Current Requests", "ASP.NET", "Requests Current");
            counters.Add("ASP.NET Queued Requests", "ASP.NET", "Requests Queued");
            counters.Add("ASP.NET Requests Wait Time", "ASP.NET", "Request Wait Time");

            PerformanceCounterCategory category = new PerformanceCounterCategory("Network Interface");
            String[] instanceNames = category.GetInstanceNames();

            foreach (string name in instanceNames)
            {
                counters.Add("Net IO Total: " + name, "Network Interface", "Bytes Total/sec", name);
                counters.Add("Net IO Received: " + name, "Network Interface", "Bytes Received/sec", name);
                counters.Add("Net IO Sent: " + name, "Network Interface", "Bytes Sent/sec", name);
            }
        }

        public async Task<PerformanceCounterList> UpdateAsyncTask(int waitTimeMs)
        {
            await CounterList.GetValuesTaskAsync(1500);

            // create summary counters
            CounterList.SummarizeCounters("Net IO Total", "Total Network Load bytes/sec", true);
            CounterList.SummarizeCounters("Net IO Received", "Network Received bytes/sec", true);
            CounterList.SummarizeCounters("Net IO Sent", "Network Sent bytes/sec", true);

            return CounterList;
        }

        public PerformanceCounterList Update(int waitTimeMs)
        {
            CounterList.GetSamples(waitTimeMs);

            // create summary counters
            CounterList.SummarizeCounters("Net IO Total", "Total Network Load bytes/sec", true);
            CounterList.SummarizeCounters("Net IO Received", "Network Received bytes/sec", true);
            CounterList.SummarizeCounters("Net IO Sent", "Network Sent bytes/sec", true);

            return CounterList;
        }
    }
}
