/******************************************************************************
 * 
 * Copyright © 2022-2022 EDDiscovery development team
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at:
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ******************************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace EDDCanonnPanel.Base
{
    public sealed class CanonnLogging
    {
        //I'm not a fan of singleton implementations.
        private static Lazy<CanonnLogging> _instance = new Lazy<CanonnLogging>(() => new CanonnLogging());

        public static CanonnLogging Instance
        {
            get
            {
                //Make sure logging is _journalLock before we return the instance.
                CanonnLogging canonnLogging = _instance.Value;
                if (canonnLogging._logTask == null || canonnLogging._logTask.IsCompleted)
                    canonnLogging.StartLogging();
                return _instance.Value;
            }
        }

        private void CleanLogs(int maxLogs)
        {
            try
            {
                List <FileInfo> logFiles = Directory.GetFiles(_logDirectory, "CanonnLog_*.log")
                                        .Select(f => new FileInfo(f))
                                        .OrderBy(f => f.CreationTime) 
                                        .ToList();

                while (logFiles.Count > maxLogs)
                {
                    logFiles[0].Delete(); 
                    logFiles.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Logging Error: {ex}");
            }
        }

        private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private readonly AutoResetEvent _logSignal = new AutoResetEvent(false);
        private readonly object _logLock = new object();

        private Task _logTask;
        private CancellationTokenSource _logCTS;
        private string _logDirectory;
        private string _logFilePath;
        private string _currentLogDate = string.Empty;

        private void StartLogging()
        {
            //Log directory setup. This will be the default location.
            _logDirectory = Path.Combine(CanonnEDDClass.DLLPath, "AddonFiles", "Canonn", "Log");
            Directory.CreateDirectory(_logDirectory);

            _currentLogDate = DateTime.Now.ToString("yyyy-MM-dd");

            //Check if a log file for today already exists
            string existingLog = Directory.GetFiles(_logDirectory, $"CanonnLog_{_currentLogDate}_*.log").FirstOrDefault();

            if (!string.IsNullOrEmpty(existingLog))
            {
                _logFilePath = existingLog;
            }
            else
            {
                _logFilePath = Path.Combine(_logDirectory, $"CanonnLog_{_currentLogDate}.log");
            }

            CleanLogs(10);

            _logSignal.Reset();
            _logCTS = new CancellationTokenSource();

            //Start the log processing worker.
            _logTask = Task.Run(ProcessLogQueue, _logCTS.Token);

            Logging = true;
        }

        private bool Logging = false;

        public void Log(string message)
        {
            lock (_logLock)
            {
                if(!Logging)
                    StartLogging();
                _logQueue.Enqueue(message);
                //Signal the logging worker that new entries exist.
                _logSignal.Set();
            }
        }

        private void ProcessLogQueue()
        {
            while (!_logCTS.Token.IsCancellationRequested)
            {
                //Blocks the thread.
                _logSignal.WaitOne();

                while (_logQueue.TryDequeue(out string logEntry))
                {
                    try
                    {
                        File.AppendAllText(_logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {logEntry}" + Environment.NewLine);
                        Debug.WriteLine(logEntry);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Logging Error: {ex}");
                    }
                }
            }
        }

        public void StopLogging()
        {
            lock (_logLock)
            {
                //Ensure we properly shut down the logging task.
                _logCTS.Cancel();
                _logSignal.Set();
                _logTask.Wait();
            }
        }
    }
}
