using System.Collections.Concurrent;
using System.Numerics;

namespace TP.ConcurrentProgramming.Data
{
    internal class BallLogger : IDisposable
    {
        private readonly string logFilePath;
        private readonly ConcurrentQueue<LogEntry> logQueue = new ConcurrentQueue<LogEntry>();
        private readonly ManualResetEvent stopEvent = new ManualResetEvent(false);
        private readonly Thread loggerThread;
        private bool isDisposed = false;

        public BallLogger()
        {
            DateTime logTime = DateTime.Now;
            this.logFilePath = logTime.ToString("yyyy-MM-dd HH_mm_ss_fff") + ".log";
            
            // Inicjalizacja pliku logu
            File.WriteAllText(logFilePath, $"BallLogger started at: {DateTime.Now}\n");
            
            // Uruchomienie osobnego w¹tku do przetwarzania logów
            loggerThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "BallLoggerThread"
            };
            loggerThread.Start();
        }

        public BallLogger(string logFilePath)
        {
            this.logFilePath = logFilePath;

            // Inicjalizacja pliku logu
            File.WriteAllText(logFilePath, $"BallLogger started at: {DateTime.Now}\n");

            // Uruchomienie osobnego w¹tku do przetwarzania logów
            loggerThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "BallLoggerThread"
            };
            loggerThread.Start();
        }

        /// <summary>
        /// Loguje pozycjê kulki na danym etapie
        /// </summary>
        public void LogPosition(int ballId, Vector2 position, Vector2 velocity)
        {
            if (isDisposed) return;

            logQueue.Enqueue(new LogEntry
            {
                BallId = ballId,
                Timestamp = DateTime.Now,
                Position = position,
                Velocity = velocity
            });
        }

        /// <summary>
        /// Metoda uruchamiana w osobnym w¹tku, zapisuj¹ca logi do pliku
        /// </summary>
        private void ProcessLogQueue()
        {
            while (!stopEvent.WaitOne(100)) // Sprawdzaj co 100ms
            {
                // Przetwarzaj wszystkie dostêpne logi
                while (logQueue.TryDequeue(out LogEntry entry))
                {
                    try
                    {
                        string logMessage = FormatLogEntry(entry);
                        File.AppendAllText(logFilePath, logMessage);
                    }
                    catch (Exception ex)
                    {
                        // W przypadku b³êdu zapisu, zapisz informacjê w dedykowanym pliku b³êdów
                        File.AppendAllText(logFilePath + ".error.log", 
                            $"{DateTime.Now}: Error writing log: {ex.Message}\n");
                    }
                }
            }

            // Zapisz pozosta³e logi przed zakoñczeniem
            while (logQueue.TryDequeue(out LogEntry entry))
            {
                try
                {
                    string logMessage = FormatLogEntry(entry);
                    File.AppendAllText(logFilePath, logMessage);
                }
                catch { /* Ignorujemy b³êdy podczas zamykania */ }
            }
        }

        /// <summary>
        /// Formatuje wpis logów do czytelnej postaci
        /// </summary>
        private string FormatLogEntry(LogEntry entry)
        {
            string timestamp = entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");

            return $"{timestamp} [POSITION] Ball {entry.BallId}: Pos({entry.Position.X:F2}, {entry.Position.Y:F2}), Vel({entry.Velocity.X:F2}, {entry.Velocity.Y:F2})\n";          

        }

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;

            // Sygnalizuj w¹tkowi aby zakoñczy³ pracê
            stopEvent.Set();
            
            // Czekaj na zakoñczenie w¹tku (z timeoutem)
            if (!loggerThread.Join(1000))
            {
                // Jeœli w¹tek nie zakoñczy³ pracy w ci¹gu 1s, koñczymy go
                try { loggerThread.Interrupt(); } 
                catch { /* Ignorujemy b³êdy */ }
            }

            // Zwolnij zasoby
            stopEvent.Dispose();
            
            // Informacja o zakoñczeniu pracy loggera
            File.AppendAllText(logFilePath, $"BallLogger stopped at: {DateTime.Now}\n");
        }

        /// <summary>
        /// Struktura przechowuj¹ca informacje o logu
        /// </summary>
        private struct LogEntry
        {
            public int BallId { get; set; }
            public DateTime Timestamp { get; set; }
            public Vector2 Position { get; set; }
            public Vector2 Velocity { get; set; }
        }
    }
}
