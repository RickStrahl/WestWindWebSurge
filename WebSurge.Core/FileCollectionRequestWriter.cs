using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Westwind.Utilities;

namespace WebSurge
{
    /// <summary>
    /// Writes values into a collection until it reaches a certain size and t
    /// writes out to a file as serialization. Retrieving the collection
    /// reconstructs the collection.
    /// </summary>
    public class FileCollectionRequestWriter : RequestWriterBase
    {
        public string TempFolderName { get; set; }
        
        private List<HttpRequestData> _Results;
        
        private List<List<HttpRequestData>> ResultsList; 

        public int FileCount { get; set; } = 0;

        public const string BaseFilename = "WebSurgeRequests_";

        private static readonly object InsertLock = new object();

        public FileCollectionRequestWriter(StressTester tester) : base(tester)
        {
            string name = "WebSurge_" + Process.GetCurrentProcess().Id;
            TempFolderName = Path.Combine(Path.GetTempPath(), name);

            try
            {
                Directory.Delete(TempFolderName, true);
            }
            catch
            {
            }

            Directory.CreateDirectory(TempFolderName);
        }

        public string LongString = StringUtils.Replicate('1', 1000000);

        JsonSerializer jsonNet = JsonSerializationUtils.CreateJsonNet(false);

        public override void Write(HttpRequestData result)
        {
            // don't log request detail data for non errors over a certain no of requests
            if (!result.IsError && RequestsProcessed > 30000)
            {
                // always clear response
                result.ResponseContent = null;

                // detail data only if we explicitly requested
                if (_stressTester.Options.CaptureMinimalResponseData)
                {
                    result.Headers = null;
                    result.ResponseHeaders = null;
                    result.FullRequest = null;
                    result.RequestContent = null;
                }
            }

            bool writeOut = false;
            var savedResults = Results;
            int savedCount = 0;
            lock (InsertLock)
            {
                Results.Add(result);
                RequestsProcessed++;
                if (result.IsError)
                    RequestsFailed++;

                if (Results.Count >= MaxCollectionItems)
                {
                    FileCount++;
                    savedCount = FileCount;
                    writeOut = true;

                    Results = null;
                    Results = new List<HttpRequestData>();
                }
            }

            if (writeOut)
            {
                //Console.WriteLine();
                ThreadPool.QueueUserWorkItem((parm) =>
                {
                    //Console.WriteLine("QUWI Thread: " + Thread.CurrentThread.ManagedThreadId);
                    FileWriteParms parms = (FileWriteParms) parm;

                    var file = Path.Combine(TempFolderName, BaseFilename + parms.FileCount + ".json");
                    
                    //JsonSerializationUtils.SerializeToFile(parms.Requests, file, false);
          
                    using (FileStream fileStream = new FileStream(file, FileMode.CreateNew, FileAccess.Write))
                    {
                      using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.UTF8))
                      {
                        using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) streamWriter))
                        {
                          jsonTextWriter.QuoteChar = '"';
                          jsonNet.Serialize((JsonWriter) jsonTextWriter, parms.Requests);
                        }
                      }
                    }

                    //SerializationUtils.SerializeObject(parms.Requests, file, true);

                    //if(ResultsList == null)
                    //    ResultsList = new List<List<HttpRequestData>>();

                    //ResultsList.Add(parms.Requests);
                    //ResultsList.Add(null);
                    //ResultsList[ResultsList.Count-1] = parms.Requests;

                    //var r = parms.Requests;
                    //Console.WriteLine("Queued Item " + parms.FileCount  + " " + r.Count);

                    //IFormatter formatter = new BinaryFormatter();
                    //Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
                    //formatter.Serialize(stream, parms.Requests);
                    //stream.Close();

                }, new FileWriteParms {FileCount = savedCount, Requests = savedResults});
             
                //Console.WriteLine("Queue Use Worker Item time: " + swatch.ElapsedTicks.ToString("N0") + " " + Thread.CurrentThread.ManagedThreadId);
            }
        }

        /// <summary>
        /// Retrieves results from various request files
        /// </summary>
        /// <returns></returns>
        public override List<HttpRequestData> GetResults()
        {
            if (FileCount < 1)
                return Results;

            var list = new List<HttpRequestData>();

            if (!EnsureFileExists(Path.Combine(TempFolderName, "WebSurgeRequests_" + FileCount + ".json")))
                return null;

            var files = Directory.GetFiles(
                Path.Combine( TempFolderName), "WebSurgeRequests_*.json",SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                int x = 0;

                if (!EnsureFileExists(file))
                    return null;

                var reqs = JsonSerializationUtils.DeserializeFromFile(file, typeof(List<HttpRequestData>),true) as List<HttpRequestData>;
                if(reqs != null)
                    list.AddRange(reqs);
                else
                {
                    int yx = 1;
                }
            }

            list.AddRange(Results);

            return list;
        }


        bool EnsureFileExists(string file)
        {
            int x = 0;
            while (x++ < 20)
            {
                try
                {
                    // try to open the file for reading
                    using (var fstream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                    }

                    return true;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }

            return false;   
        }


        /// <summary>
        /// Explicitly remove the temp folder
        /// </summary>
        public override void Dispose()
        {
            try
            {
                if (Directory.Exists(TempFolderName))
                   Directory.Delete(TempFolderName, true);
            }
            catch
            {
            }
        }
            
      

        public int MaxCollectionItems { get; set; } = 100000;
    }

    struct FileWriteParms
    {
        public List<HttpRequestData> Requests;
        public int FileCount;
    }
}