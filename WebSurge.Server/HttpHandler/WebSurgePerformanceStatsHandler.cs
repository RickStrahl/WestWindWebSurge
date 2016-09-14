using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace WebSurge.Server
{

    public class WebSurgePerformanceStatsHandler : IHttpHandler
    {
        //private PerformanceCounterList counters;
        private int WaitTimeMs = 5000;

        public void ProcessRequest(HttpContext context)
        {
            var action = context.Request.QueryString["action"] ?? "SummaryCounters";
            if (action == "SummaryCounters")
                SummaryCounters(context);
        }

        void SummaryCounters(HttpContext context)
        {
            var stats = new PerformanceStats();
            stats.Configure();

            // Update the counters and get the list
            var counters = stats.Update(WaitTimeMs);

            var json = JsonConvert.SerializeObject(counters);
            context.Response.ContentType = "application/json";
            context.Response.Write(json);
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
    
#if true
    
    public class WebSurgePerformanceStatsHandlerXXX : HttpTaskAsyncHandler
    {
        private int WaitTimeMs = 5000;

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            await  GetPerformanceStatusTaskAsync(context);             
        }

        private async Task<object> GetPerformanceStatusTaskAsync(HttpContext context)
        {
            var stats = new PerformanceStats();
            stats.Configure();
                        
            var counters = stats.UpdateAsyncTask(WaitTimeMs);

            var json = JsonConvert.SerializeObject(counters);

            context.Response.ContentType = "application/json";
            context.Response.Write(json);

            return null;
        }
    }


    //public class WebSurgePerformanceStatsHandlerEx : IHttpAsyncHandler
    //{
    //    private PerformanceStats PerformanceStats = new PerformanceStats();        
    //    TaskCompletionSource<object> tcs;
    //    private int WaitTimeMs = 2000;


    //    public void ProcessRequest(HttpContext context)
    //    {
            
    //    }

    //    public bool IsReusable { get; private set; }
    //    public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
    //    {
            

            

    //        tcs = new TaskCompletionSource<object>(context);

    //        // call ProcessRequest method asynchronously            
    //        var task = Task<object>.Factory.StartNew(
    //            (ctx) =>
    //            {
    //                GetPerformanceStatus(ctx as HttpContext);                    
    //                return null;
    //            }, context)
    //        .ContinueWith(tsk =>
    //        {
    //            if (tsk.IsFaulted)
    //                tcs.SetException(tsk.Exception);
    //            else
    //                // Not returning a value, but TCS needs one so just use null
    //                tcs.SetResult(null);

    //        }, TaskContinuationOptions.ExecuteSynchronously);


    //        return tcs.Task;            
    //    }

    //    private void GetPerformanceStatus(HttpContext ctx)
    //    {
    //        var stats = new PerformanceStats();
            
    //        //await stats.StartAsyncTask();
    //        stats.Start(WaitTimeMs);

    //        var json = JsonConvert.SerializeObject(stats.CounterList);

    //        ctx.Response.ContentType = "application/json";
    //        ctx.Response.Write(json);
    //    }

    //    private async void GetPerformanceStatusAsyncTask(HttpContext ctx)
    //    {
    //        var stats = new PerformanceStats();

    //        //await stats.StartAsyncTask();
    //        stats.StartAsyncTask(WaitTimeMs);

    //        var json = JsonConvert.SerializeObject(stats.CounterList);

    //        ctx.Response.ContentType = "application/json";
    //        ctx.Response.Write(json);
    //    }

    //    public void EndProcessRequest(IAsyncResult result)
    //    {       
    //    }


    //}
#endif

}
