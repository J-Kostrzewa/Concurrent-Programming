using System.Collections.Concurrent;
using System.Numerics;
using System.Text.Json;

namespace TP.ConcurrentProgramming.Data
{
    internal class BallLogger : IDisposable
    {
        private static readonly Lazy<BallLogger> _instance = new(() => new BallLogger());
        internal static BallLogger Instance => _instance.Value;

        private readonly ConcurrentQueue<LogEntry> _logQueue = new();
        private readonly ManualResetEvent _stopEvent = new(false);
        private readonly Thread _loggerThread;
        private readonly StreamWriter _writer;
        private bool _isDisposed = false;
        private readonly int _flushIntervalMs = 1000;

        private BallLogger() : this(null)
        {
        }

        internal BallLogger(string logFilePath = null)
        {
            // Jeœli nie podano œcie¿ki, u¿yj domyœlnej opartej na czasie
            if (string.IsNullOrEmpty(logFilePath))
            {
                logFilePath = $"ball_log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";
            }

            _writer = new StreamWriter(logFilePath, append: false);
            _writer.WriteLine("[");

            _loggerThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "BallLoggerThread"
            };
            _loggerThread.Start();
        }

        public void LogPosition(int ballId, Vector2 position, Vector2 velocity)
        {
            if (_isDisposed) return;
            _logQueue.Enqueue(new LogEntry
            {
                BallId = ballId,
                Timestamp = DateTime.Now,
                Position = position.X.ToString() + ", " + position.Y.ToString(),
                Velocity = velocity.X.ToString() + ", " + velocity.Y.ToString()
            });
        }

        private void ProcessLogQueue()
        {
            bool first = true;
            while (!_stopEvent.WaitOne(_flushIntervalMs))
            {
                FlushBuffer(ref first);
            }
            // Ostatni flush po zakoñczeniu
            FlushBuffer(ref first);
            _writer.Flush();
        }

        private void FlushBuffer(ref bool first)
        {
            while (_logQueue.TryDequeue(out LogEntry entry))
            {
                string json = JsonSerializer.Serialize(entry);
                if (!first) _writer.WriteLine(",");
                _writer.Write(json);
                first = false;
            }
            _writer.Flush();
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _stopEvent.Set();
            _loggerThread.Join();
            _writer.WriteLine("\n]");
            Thread.Sleep(500);
            _writer.Dispose();
            _stopEvent.Dispose();
        }

        private struct LogEntry
        {
            public int BallId { get; set; }
            public DateTime Timestamp { get; set; }
            public String Position { get; set; }
            public String Velocity { get; set; }
        }
    }
}