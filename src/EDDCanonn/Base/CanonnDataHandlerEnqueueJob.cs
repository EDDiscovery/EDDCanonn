using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace EDDCanonnTest
{
    public class CanonnDataHandler
    {
        private readonly ConcurrentQueue<Action> jobQueue = new ConcurrentQueue<Action>();
        private readonly AutoResetEvent jobSignal = new AutoResetEvent(false);
        private readonly Thread workerThread;

        public CanonnDataHandler()
        {
            workerThread = new Thread(WorkerLoop) { IsBackground = true };
            workerThread.Start();
        }

        // Fetches data from the Canonn endpoint asynchronously
        public void FetchCanonnAsync(string fullUrl, Action<string> callback, Action<Exception> errorCallback = null)
        {
            EnqueueJob(() =>
            {
                try
                {
                    string jsonResponse = PerformGetRequest(fullUrl);
                    callback?.Invoke(jsonResponse);
                }
                catch (Exception ex)
                {
                    errorCallback?.Invoke(ex);
                }
            });
        }

        // Pushes data to the Canonn endpoint asynchronously
        public void PushCanonnAsync(string fullUrl, string jsonData, Action<bool> callback, Action<Exception> errorCallback = null)
        {
            EnqueueJob(() =>
            {
                try
                {
                    string response = PerformPostRequest(fullUrl, jsonData, "application/json");
                    bool success = !string.IsNullOrEmpty(response);
                    callback?.Invoke(success);
                }
                catch (Exception ex)
                {
                    errorCallback?.Invoke(ex);
                }
            });
        }

        // Adds a job to the queue
        private void EnqueueJob(Action job)
        {
            jobQueue.Enqueue(job);
            jobSignal.Set();
        }

        // Worker thread loop
        private void WorkerLoop()
        {
            while (true)
            {
                jobSignal.WaitOne(); // blocks the thread if no job is present
                while (jobQueue.TryDequeue(out Action job))
                {
                    try
                    {
                        job.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Error in job execution: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        // Performs a GET request to the specified endpoint
        private string PerformGetRequest(string fullUrl)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.Method = "GET";
                request.Accept = "application/json";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.Error.WriteLine($"GET request failed. Status: {response.StatusCode}");
                        return null;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                Console.Error.WriteLine($"Error performing GET request: {ex.Message}");
                throw; 
            }
        }

        // Performs a POST request to the specified endpoint with JSON data
        private string PerformPostRequest(string fullUrl, string postData, string contentType)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.Method = "POST";
                request.ContentType = contentType;
                request.ContentLength = Encoding.UTF8.GetByteCount(postData);

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(postData);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                    {
                        Console.Error.WriteLine($"POST request failed. Status: {response.StatusCode}");
                        return null;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                Console.Error.WriteLine($"Error performing POST request: {ex.Message}");
                throw;
            }
        }
    }
}
