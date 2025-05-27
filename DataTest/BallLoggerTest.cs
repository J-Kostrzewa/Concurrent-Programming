using System.Numerics;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallLoggerTest
    {

        [TestMethod]
        public void BallLogger_ShouldLogBallPosition()
        {
            string testLogPath = "test_ball_logger.log";

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

                // Czekamy chwilę, aby logger miał czas na zapisanie danych
                Thread.Sleep(300);
            } // Dispose zamyka logger i zapisuje wszystkie pozostałe logi

            Assert.IsTrue(File.Exists(testLogPath), "Plik logu powinien zostać utworzony");
            string logContent = File.ReadAllText(testLogPath);

            // Sprawdzamy czy plik logu zawiera informacje o starcie loggera
            Assert.IsTrue(logContent.Contains("BallLogger started at:"),
                "Log powinien zawierać informację o starcie loggera");

            // Sprawdzamy czy plik logu zawiera informacje o pozycji kulki
            Assert.IsTrue(logContent.Contains("[POSITION] Ball 42"),
                "Log powinien zawierać wpisy o pozycji kulki");
            Assert.IsTrue(logContent.Contains("Pos(100,00, 150,00)"),
                "Log powinien zawierać poprawną pozycję");
            Assert.IsTrue(logContent.Contains("Vel(5,00, 7,00)"),
                "Log powinien zawierać poprawną prędkość");
            Assert.IsTrue(logContent.Contains("BallLogger stopped at:"),
                "Log powinien zawierać informację o zakończeniu pracy loggera");
        }
    }
}
