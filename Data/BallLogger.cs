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
            
            // Uruchomienie osobnego w�tku do przetwarzania log�w
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

            // Uruchomienie osobnego w�tku do przetwarzania log�w
            loggerThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "BallLoggerThread"
            };
            loggerThread.Start();
        }

        /// <summary>
        /// Loguje pozycj� kulki na danym etapie
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
        /// Metoda uruchamiana w osobnym w�tku, zapisuj�ca logi do pliku
        /// </summary>
        private void ProcessLogQueue()
        {
            while (!stopEvent.WaitOne(100)) // Sprawdzaj co 100ms
            {
                // Przetwarzaj wszystkie dost�pne logi
                while (logQueue.TryDequeue(out LogEntry entry))
                {
                    try
                    {
                        string logMessage = FormatLogEntry(entry);
                        File.AppendAllText(logFilePath, logMessage);
                    }
                    catch (Exception ex)
                    {
                        // W przypadku b��du zapisu, zapisz informacj� w dedykowanym pliku b��d�w
                        File.AppendAllText(logFilePath + ".error.log", 
                            $"{DateTime.Now}: Error writing log: {ex.Message}\n");
                    }
                }
            }

            // Zapisz pozosta�e logi przed zako�czeniem
            while (logQueue.TryDequeue(out LogEntry entry))
            {
                try
                {
                    string logMessage = FormatLogEntry(entry);
                    File.AppendAllText(logFilePath, logMessage);
                }
                catch { /* Ignorujemy b��dy podczas zamykania */ }
            }
        }

        /// <summary>
        /// Formatuje wpis log�w do czytelnej postaci
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

            // Sygnalizuj w�tkowi aby zako�czy� prac�
            stopEvent.Set();
            
            // Czekaj na zako�czenie w�tku (z timeoutem)
            if (!loggerThread.Join(1000))
            {
                // Je�li w�tek nie zako�czy� pracy w ci�gu 1s, ko�czymy go
                try { loggerThread.Interrupt(); } 
                catch { /* Ignorujemy b��dy */ }
            }

            // Zwolnij zasoby
            stopEvent.Dispose();
            
            // Informacja o zako�czeniu pracy loggera
            File.AppendAllText(logFilePath, $"BallLogger stopped at: {DateTime.Now}\n");
        }

        /// <summary>
        /// Struktura przechowuj�ca informacje o logu
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
