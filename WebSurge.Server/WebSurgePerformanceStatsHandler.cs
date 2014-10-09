using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace WebSurge.Server
{

    public class WebSurgePerformanceStatsHandler : IHttpHandler
    {
        private int WaitTimeMs = 2000;

        public void ProcessRequest(HttpContext context)
        {

            var counters = new PerformanceCounterList();

            counters.Add("Processor Load", "Processor", "% Processor Time", "_Total");            
            counters.Add("Memory Usage", "Memory", "% Committed Bytes In Use");

            counters.Add("IIS Requests/sec", "Web Service", "Total Method Requests/sec", "_Total");
            counters.Add("ASP.NET Current Requests", "ASP.NET", "Requests Current");
            
            // this one is very unreliable
            //counters.Add("ASP.NET Request/Sec", "ASP.NET Applications", "Requests/Sec", "__Total__");


            //counters.Add("ASP.NET Applications\Requests/Sec","")

            counters.GetValues(2000);

            //var stats = new PerformanceStats();
            //stats.Configure();

            //stats.Start(1000);

            var json = JsonConvert.SerializeObject(counters);

            context.Response.ContentType = "application/json";
            context.Response.Write(json);

        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
    
#if false
    
    public class WebSurgePerformanceStatsHandlerXXXX : HttpTaskAsyncHandler
    {
        private int WaitTimeMs = 2000;

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            await  GetPerformanceStatusTaskAsync(context);             
        }

        private async Task<object> GetPerformanceStatusTaskAsync(HttpContext ctx)
        {
            var stats = new PerformanceStats();
            stats.Configure();
            
            stats.StartAsyncTask(WaitTimeMs);

            var json = await JsonConvert.SerializeObjectAsync(stats.CounterList);

            ctx.Response.ContentType = "application/json";
            ctx.Response.Write(json);   

            return null;
        }
    }


    public class WebSurgePerformanceStatsHandlerEx : IHttpAsyncHandler
    {
        private PerformanceStats PerformanceStats = new PerformanceStats();        
        TaskCompletionSource<object> tcs;
        private int WaitTimeMs = 2000;


        public void ProcessRequest(HttpContext context)
        {
            
        }

        public bool IsReusable { get; private set; }
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            

            

            tcs = new TaskCompletionSource<object>(context);

            // call ProcessRequest method asynchronously            
            var task = Task<object>.Factory.StartNew(
                (ctx) =>
                {
                    GetPerformanceStatus(ctx as HttpContext);                    
                    return null;
                }, context)
            .ContinueWith(tsk =>
            {
                if (tsk.IsFaulted)
                    tcs.SetException(tsk.Exception);
                else
                    // Not returning a value, but TCS needs one so just use null
                    tcs.SetResult(null);

            }, TaskContinuationOptions.ExecuteSynchronously);


            return tcs.Task;            
        }

        private void GetPerformanceStatus(HttpContext ctx)
        {
            var stats = new PerformanceStats();
            
            //await stats.StartAsyncTask();
            stats.Start(WaitTimeMs);

            var json = JsonConvert.SerializeObject(stats.CounterList);

            ctx.Response.ContentType = "application/json";
            ctx.Response.Write(json);
        }

        private async void GetPerformanceStatusAsyncTask(HttpContext ctx)
        {
            var stats = new PerformanceStats();

            //await stats.StartAsyncTask();
            stats.StartAsyncTask(WaitTimeMs);

            var json = JsonConvert.SerializeObject(stats.CounterList);

            ctx.Response.ContentType = "application/json";
            ctx.Response.Write(json);
        }

        public void EndProcessRequest(IAsyncResult result)
        {       
        }


    }
#endif

}
