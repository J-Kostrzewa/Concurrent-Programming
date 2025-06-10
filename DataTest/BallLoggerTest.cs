using System.Numerics;
using System.Text.Json;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallLoggerTest
    {
        [TestMethod]
        public void BallLogger_ShouldLogBallPositionInJSON()
        {
            string testLogPath = "json_ball_logger_test.json";

            // Usunięcie istniejącego pliku testowego, jeśli istnieje
            if (File.Exists(testLogPath))
                File.Delete(testLogPath);

            using (BallLogger logger = new BallLogger(testLogPath))
            {
                // Symulujemy logowanie pozycji kulki
                Vector2 position = new Vector2(100, 150);
                Vector2 velocity = new Vector2(5, 7);
                int ballId = 42;

                logger.LogPosition(ballId, position, velocity);

                // Czekamy chwilę, aby logger miał czas na zapisanie danych (dłuższy czas niż _flushIntervalMs)
                Thread.Sleep(1200);
            } // Dispose zamyka logger i zapisuje wszystkie pozostałe logi

            Assert.IsTrue(File.Exists(testLogPath), "Plik JSON powinien zostać utworzony");
            string jsonContent = File.ReadAllText(testLogPath);

            // Sprawdzamy poprawność formatu JSON
            Assert.IsTrue(jsonContent.StartsWith("["), "Plik JSON powinien zaczynać się od znaku [");
            Assert.IsTrue(jsonContent.TrimEnd().EndsWith("]"), "Plik JSON powinien kończyć się znakiem ]");

            // Sprawdzamy zawartość JSON
            Assert.IsTrue(jsonContent.Contains("\"BallId\":42"), "JSON powinien zawierać ID kulki");
            Assert.IsTrue(jsonContent.Contains("\"Position\":\"100, 150\""), "JSON powinien zawierać pozycję");
            Assert.IsTrue(jsonContent.Contains("\"Velocity\":\"5, 7\""), "JSON powinien zawierać prędkość");
        }
    }
}