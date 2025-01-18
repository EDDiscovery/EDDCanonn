using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EDDCanonn
{
    public class ActionDataHandler
    {
        #region Threading
        public void StartTaskAsync(Action job, Action<Exception> errorCallback = null, string name = "default")
        {
            StartTask(() =>
            {
                try
                {                 
                    job?.Invoke();
                }
                catch (Exception ex)
                {
                    errorCallback?.Invoke(ex);
                }
            },name);
        }

        private readonly List<Task> _tasks = new List<Task>();
        private readonly object _lock = new object();

        private void StartTask(Action job,string name)
        {
            lock (_lock)
            {
                if (_isClosing)
                    return;

                Task task = Task.Run(() =>
            {
                try
                {          
                    job.Invoke();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"EDDCanonn: Error in job execution: {ex.Message}");
                    throw;
                }
            });
                _tasks.Add(task);

                Console.WriteLine($"EDDCanonn: Task registered. [ID: {task.Id}, Name: {name}, Status: {task.Status}]");

                task.ContinueWith(t =>
                {
                    _tasks.Remove(t);
                    Console.WriteLine($"EDDCanonn: Task finished. [ID: {t.Id}, Name: {name}, Final Status: {t.Status}]");
                });
            }
        }

        private bool _isClosing = false;
        public void Closing()
        {
            lock (_lock)
                _isClosing = true;

            Task.WaitAll(_tasks.ToArray());
        }
        #endregion

        #region Networking 
        public void FetchDataAsync(string fullUrl, Action<string> callback, Action<Exception> errorCallback = null, string name = "default")
        {
            StartTask(() =>
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
            }, name);
        }

        public string FetchData(string fullUrl)
        {
            try
            {
                return PerformGetRequest(fullUrl);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void PushDataAsync(string fullUrl, string jsonData, Action<bool> callback, Action<Exception> errorCallback = null, string name = "default")
        {
            StartTask(() =>
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
            }, name);
        }

        public bool PushData(string fullUrl, string jsonData)
        {
            try
            {
                string response = PerformPostRequest(fullUrl, jsonData, "application/json");
                return !string.IsNullOrEmpty(response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Performs a GET request to the specified endpoint
        private string PerformGetRequest(string fullUrl)//wip
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.Method = "GET";
                request.Accept = "application/json";
                request.Timeout = 5000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.Error.WriteLine($"EDDCanonn: GET request failed. Status: {response.StatusCode}");
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
                Console.Error.WriteLine($"EDDCanonn: Error performing GET request: {ex.Message}");
                throw;
            }
        }

        // Performs a POST request to the specified endpoint with JSON data
        private string PerformPostRequest(string fullUrl, string postData, string contentType)//wip
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.Method = "POST";
                request.ContentType = contentType;
                request.ContentLength = Encoding.UTF8.GetByteCount(postData);
                request.Timeout = 5000;

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(postData);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                    {
                        Console.Error.WriteLine($"EDDCanonn: POST request failed. Status: {response.StatusCode}");
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
                Console.Error.WriteLine($"EDDCanonn: Error performing POST request: {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
