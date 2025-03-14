using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                Console.WriteLine($"Logging Error: {ex.Message}");
            }
        }

        private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private readonly AutoResetEvent _logSignal = new AutoResetEvent(false);
        private readonly object _logLock = new object();

        private Task _logTask;
        private CancellationTokenSource _logCTS;
        private string _logDirectory;
        private string _logFilePath;

        public CanonnLogging()
        {
            //Logging should start right away. No delays.
            StartLogging();
        }

        private void StartLogging()
        {
            //Log directory setup. This will be the default location.
            _logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EDDiscovery", "AddonFiles", "Canonn", "Log");
            Directory.CreateDirectory(_logDirectory);

            CleanLogs(10);

            //Creating a timestamped log file. Avoids overwriting.
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            _logFilePath = Path.Combine(_logDirectory, $"CanonnLog_{timestamp}.log");

            _logSignal.Reset();
            _logCTS = new CancellationTokenSource();

            //Start the log processing worker.
            _logTask = Task.Run(ProcessLogQueue, _logCTS.Token);
        }

        public void LogToFile(string message)
        {
            lock (_logLock)
            {
                _logQueue.Enqueue(message);
                //Signal the logging worker that new entries exist.
                _logSignal.Set();
            }
        }

        private void ProcessLogQueue()
        {
            while (!_logCTS.Token.IsCancellationRequested)
            {
                //Blocks the thread. No unnecessary CPU usage.
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
                        Console.WriteLine($"Logging Error: {ex.Message}");
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
                _logTask.Wait(); //Don't leave unfinished work behind.
            }
        }
    }
}
