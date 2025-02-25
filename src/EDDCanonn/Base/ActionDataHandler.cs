/*
 * Copyright © 2022-2022 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EDDCanonnPanel;

namespace EDDCanonnPanel.Base
{
    public class ActionDataHandler
    {
        #region Threading

        public Task StartTaskAsync(Action<CancellationToken> job, Action<Exception> errorCallback = null, string name = "default", Action finalAction = null, CancellationToken? token = null)
        {
            CancellationToken effectiveToken = token ?? _cts.Token;

            return StartTask(() => job(effectiveToken), name, effectiveToken, finalAction, errorCallback);
        }


        private readonly List<Task> _tasks = new List<Task>();
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly object _lock = new object();

        private Task StartTask(Action job, string name, CancellationToken token, Action finalAction = null, Action<Exception> errorCallback = null)
        {
            lock (_lock)
            {
                if (_isClosing)
                    return Task.CompletedTask;

                Task task = Task.Run(() =>
                {
                    try
                    {
                        job.Invoke();
                    }
                    catch (OperationCanceledException)
                    {
                        string error = $"EDDCanonn: Task canceled. [ID: {Task.CurrentId}, Name: {name}]";
                        Console.Error.WriteLine(error);
                        CanonnLogging.Instance.LogToFile(error);
                    }
                    catch (Exception ex)
                    {
                        errorCallback?.Invoke(ex);
                        string error = $"EDDCanonn: Error in job execution: {ex.Message}";
                        Console.Error.WriteLine(error);
                        CanonnLogging.Instance.LogToFile(error);
                    }
                }, token);

                _tasks.Add(task);
                string mgReg = $"EDDCanonn: Task registered. [ID: {task.Id}, Name: {name}, Status: {task.Status}]";
                Console.WriteLine(mgReg);
                CanonnLogging.Instance.LogToFile(mgReg);

                task.ContinueWith(t =>
                {
                    finalAction?.Invoke();
                    lock (_lock)
                    {
                        _tasks.Remove(t);
                        string mgFinished = $"EDDCanonn: Task finished. [ID: {t.Id}, Name: {name}, Final Status: {t.Status}]";
                        Console.WriteLine(mgFinished);
                        CanonnLogging.Instance.LogToFile(mgFinished);
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);

                return task;
            }
        }

        public void CancelAllTasks()
        {
            lock (_lock)
            {
                _cts.Cancel(); 

                string mg = $"EDDCanonn: Cancelling all tasks...";
                Console.WriteLine(mg);
                CanonnLogging.Instance.LogToFile(mg);
            }
        }

        private bool _isClosing = false;
        public void Closing()
        {
            lock (_lock)
                _isClosing = true;

            CancelAllTasks();

            Task.WaitAll(_tasks.Where(t => !t.IsCanceled).ToArray());
        }
        #endregion

        #region Networking 

        public (bool success, string response) FetchData(string fullUrl)
        {
            try
            {
                return PerformGetRequest(fullUrl);
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Exception in FetchData: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
                return (false, null);
            }
        }

        public (bool success, string response) PushData(string fullUrl, string jsonData)
        {
            try
            {
                return PerformPostRequest(fullUrl, jsonData, "application/json");
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Exception in PushData: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
                return (false, null);
            }
        }

        // Performs a GET request to the specified endpoint
        private (bool success, string response) PerformGetRequest(string fullUrl)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.Method = "GET";
                request.Accept = "application/json";
                request.UserAgent = "EDDCanonnPanel" + CanonnEDDClass.V;
                request.Timeout = 20000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        string error = $"EDDCanonn: GET request failed. Status: {response.StatusCode}";
                        Console.Error.WriteLine(error);
                        CanonnLogging.Instance.LogToFile(error);
                        return (false, null);
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        return (true, reader.ReadToEnd());
                    }
                }
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse httpResponse)
            {
                string error = $"EDDCanonn: GET request failed. Status: {httpResponse.StatusCode}, Error: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
                return (false, null);
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error performing GET request: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
                return (false, null);
            }
        }

        // Performs a POST request to the specified endpoint with JSON data
        private (bool success, string response) PerformPostRequest(string fullUrl, string postData, string contentType)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUrl);
                request.Method = "POST";
                request.ContentType = contentType;
                request.ContentLength = Encoding.UTF8.GetByteCount(postData);
                request.UserAgent = "EDDCanonnPanel" + CanonnEDDClass.V;
                request.Timeout = 20000;

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(postData);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                    {
                        string error = $"EDDCanonn: POST request failed. Status: {response.StatusCode}";
                        Console.Error.WriteLine(error);
                        CanonnLogging.Instance.LogToFile(error);
                        return (false, null);
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        return (true, reader.ReadToEnd());
                    }
                }
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse httpResponse)
            {
                string error = $"EDDCanonn: POST request failed. Status: {httpResponse.StatusCode}, Error: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
                return (false, null);
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error performing POST request: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
                return (false, null);
            }
        }
        #endregion

    }
}
