using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebSurge.Server
{

    /// <summary>
    /// Class to hold many performance counters and process them in batch and
    /// return their values. This simplifies waiting on multiple counters.
    /// </summary>
    public class PerformanceCounterList : IEnumerable<PerformanceCounterItem>,IDisposable
    {
        /// <summary>
        /// List of Performance counters and their last captured values/samples
        /// </summary>
        public Dictionary<string, PerformanceCounterItem> PerfCounterItems { get; private set; }

        /// <summary>
        /// Optional machine name to connect to a remote machine - not 
        /// </summary>
        public string MachineName { get; set; }

        public PerformanceCounterList()
        {
            PerfCounterItems = new Dictionary<string, PerformanceCounterItem>();
        }

        /// <summary>
        /// Implement indexer
        /// </summary>
        /// <param name="key">Id of the entry you specified</param>
        /// <returns></returns>
        public PerformanceCounterItem this[string key]
        {
            get { return PerfCounterItems[key]; }
        }

        /// <summary>
        /// adds a new performance counter instance
        /// </summary>
        /// <param name="key">Id to identify this entry by</param>
        /// <param name="category"></param>
        /// <param name="counterName"></param>
        /// <param name="instanceName"></param>
        /// <param name="readOnly"></param>
        /// <param name="machineName"></param>
        public PerformanceCounterItem Add(string key, string category, string counterName,
            string instanceName = null, bool readOnly = true,
            string machineName = null)
        {
            PerformanceCounter perfCounter;
            if (instanceName != null)
                perfCounter = new PerformanceCounter(category, counterName, instanceName, readOnly);
            else
                perfCounter = new PerformanceCounter(category, counterName, readOnly);

            if (machineName != null)
                perfCounter.MachineName = machineName;
            if (string.IsNullOrEmpty(machineName) && !string.IsNullOrEmpty(MachineName))
                perfCounter.MachineName = MachineName;

            var item = new PerformanceCounterItem(key, perfCounter)
            {
                Description = perfCounter.CounterHelp
            };
            
            PerfCounterItems.Add(key,item);

            return item;
        }

        public PerformanceCounterItem Add(string key, PerformanceCounterItem item)
        {
            item.Id = key;
            PerfCounterItems.Add(key, item);
            return item;
        }
       
        /// <summary>
        /// Goes through the list of PerfCounters and retrieves each counter's results
        /// and stores it in LastValue
        /// </summary>
        /// <param name="waitTime">time to wait for sample</param>
        /// <param name="initialValue"></param>
        public void GetValues(int waitTime = 1000, bool initializeCounter =true)
        {
            if (initializeCounter)
            {
                foreach (PerformanceCounterItem item in PerfCounterItems.Values)
                {
                    // read the next value twice to get more reliable results
                    GetCounterValueInternal(item.PerfCounter);
                    Thread.Sleep(1);
                    GetCounterValueInternal(item.PerfCounter);
                }
            }

            Thread.Sleep(waitTime);

            foreach (PerformanceCounterItem item in PerfCounterItems.Values)
            {
                item.LastValue = GetCounterValueInternal(item.PerfCounter);
            }
        }

        /// <summary>
        /// Goes through the list of PerfCounters and retrieves each performance
        /// counter value and stores it in LastValue
        /// </summary>
        /// <param name="waitTime">time to wait for sample</param>
        public async Task<bool> GetValuesTaskAsync(int waitTime = 1000)
        {
            foreach (PerformanceCounterItem item in PerfCounterItems.Values)
            {
                // do this twice to initialize more reliably
                await GetCounterValueInternalAsyncTask(item.PerfCounter);
                await Task.Delay(1);
                await GetCounterValueInternalAsyncTask(item.PerfCounter);
            }

            await Task.Delay(waitTime);

            foreach (PerformanceCounterItem item in PerfCounterItems.Values)
            {
                item.LastValue = await GetCounterValueInternalAsyncTask(item.PerfCounter);             
            }

            return true;
        }

        /// <summary>
        /// Summarizes a number of entries that start with a given string
        /// and adds a new item with the summarized value. The detail
        /// values are removed from the collection optionally
        /// 
        /// Call this method after a perf run has completed
        /// </summary>
        public PerformanceCounterItem SummarizeCounters(string startsWith, string newId, bool removeItems = false)
        {
            decimal summaryValue = PerfCounterItems.Where(item => item.Value.Id.StartsWith(startsWith)).Sum(itm => itm.Value.LastValue);

            var summarizedItem = new PerformanceCounterItem(newId, null)
            {LastValue = summaryValue};

            PerfCounterItems.Add(newId,summarizedItem);

            if (removeItems)
            {
                var netIds =  PerfCounterItems.Where(item => item.Value.Id.StartsWith(startsWith)).ToList();
                for (int i = 0; i < netIds.Count; i++)
                {
                    var id = netIds[i].Key;
                    PerfCounterItems.Remove(id);
                }
            }

            return summarizedItem;
        }

        /// <summary>
        /// Internally retrieves a counter reliably by waiting
        /// </summary>
        /// <param name="counter"></param>
        /// <returns></returns>
        private decimal GetCounterValueInternal(PerformanceCounter counter)
        {
            decimal value = 0;

            int i = 0;
            while (i < 500)
            {
                try
                {
                    value = (decimal) counter.NextValue();                    
                }
                catch
                {
                    return 0;
                }

                if (counter.RawValue > 0)
                    break;

                // delay slightly
                Thread.Sleep(1);                
                i++;
            }
            return value;
        }

        private async Task<decimal> GetCounterValueInternalAsyncTask(PerformanceCounter counter)
        {
            decimal value = 0;

            int i = 0;
            while (i < 500)
            {
                try
                {
                    value = (decimal)counter.NextValue();
                }
                catch
                {
                    return 0;
                }

                if (counter.RawValue > 0)
                    break;

                // delay slightly
                await Task.Delay(1);
                i++;
            }
            return value;
        }


        public void GetSamples(int waitTime = 1000)
        {
            foreach (var item in PerfCounterItems.Values)
            {
                item.FirstSample = item.PerfCounter.NextSample();
            }

            Thread.Sleep(waitTime);

            foreach (var item in PerfCounterItems.Values)
            {
                item.LastSample = item.PerfCounter.NextSample();
                item.SampleResult = CounterSample.Calculate(item.FirstSample,item.LastSample);                
            }
        }

        /// <summary>
        /// Retrieve an individual value from the collection
        /// </summary>
        /// <param name="key"></param>
        /// <param name="waitTime"></param>
        public decimal GetValue(string key, int waitTime = 1000)
        {
            var item = PerfCounterItems[key];
            if (item == null)
                return -1;

            GetCounterValueInternal(item.PerfCounter);
            Thread.Sleep(waitTime);

            return (decimal)GetCounterValueInternal(item.PerfCounter); 
        }

        /// <summary>
        /// Retrieve an individual value from the collection
        /// </summary>
        /// <param name="key"></param>
        /// <param name="waitTime"></param>
        public async Task<decimal> GetValueAsyncTask(string key, int waitTime = 1000)
        {
            var item = PerfCounterItems[key];
            if (item == null)
                return -1;

            await GetCounterValueInternalAsyncTask(item.PerfCounter);
            await Task.Delay(waitTime);

            return await GetCounterValueInternalAsyncTask(item.PerfCounter); 
        }

        public IEnumerator<PerformanceCounterItem> GetEnumerator()
        {
            foreach (var item in PerfCounterItems)
                yield return item.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool isDisposing;
        public void Dispose()
        {                        
            if (!isDisposing)
            {
                isDisposing = true;
                if (PerfCounterItems != null)
                {
                    foreach (var item in PerfCounterItems)
                    {
                        if (item.Value.PerfCounter != null)
                            item.Value.PerfCounter.Dispose();
                    }
                }
            }
        }
    }

    public class PerformanceCounterItem
    {
        public PerformanceCounterItem(string id, PerformanceCounter perfCounter)
        {
            PerfCounter = perfCounter;
            Id = id;
        }

        
        
        public string Id { get; set; }
        public decimal LastValue { get; set; }
        public string Description { get; set;  }

        [JsonIgnore]
        public PerformanceCounter PerfCounter { get; set; }

        [JsonIgnore]
        public CounterSample LastSample { get; set; }

        [JsonIgnore]
        public CounterSample FirstSample { get; set; }

        public float SampleResult { get; set; }
    }
}
