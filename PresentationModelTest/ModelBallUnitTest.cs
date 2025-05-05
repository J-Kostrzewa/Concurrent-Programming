//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.Presentation.Model.Test
{
    [TestClass]
    public class ModelBallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            ModelBall ball = new ModelBall(0.0, 0.0, new BusinessLogicIBallFixture());

            // Dla środka kulki w (0,0) oraz domyślnej średnicy 20, pozycja górnego lewego rogu to (-10, -10)
            Assert.AreEqual(-10.0, ball.Top);
            Assert.AreEqual(-10.0, ball.Left);
        }

        [TestMethod]
        public void PositionChangeNotificationTestMethod()
        {
            int notificationCounter = 0;
            ModelBall ball = new ModelBall(0, 0.0, new BusinessLogicIBallFixture());
            ball.PropertyChanged += (sender, args) => notificationCounter++;

            Assert.AreEqual(0, notificationCounter);

            ball.SetLeft(1.0);

            Assert.AreEqual(1, notificationCounter);
            Assert.AreEqual(1.0, ball.Left);
            Assert.AreEqual(-10.0, ball.Top); // Początkowa wartość Top nie zmieniła się

            ball.SettTop(1.0);

            Assert.AreEqual(2, notificationCounter);
            Assert.AreEqual(1.0, ball.Left);
            Assert.AreEqual(1.0, ball.Top);
        }

        [TestMethod]
        public void UpdateScale_ShouldRecalculatePositionAndDiameter()
        {
            double originalDiameter = 10.0;
            double originalTop = 50.0;
            double originalLeft = 100.0;
            double initialScale = 1.0;
            double newScale = 2.0;

            ModelBall ball = new ModelBall(originalTop, originalLeft, new BusinessLogicIBallFixture(), initialScale);
            ball.Diameter = originalDiameter;

            ball.UpdateScale(newScale);

            double newDiameter = originalDiameter * newScale;
            double newRadius = newDiameter / 2.0;

            double expectedTop = (originalTop - newRadius / initialScale * newScale) * newScale;
            double expectedLeft = (originalLeft - newRadius / initialScale * newScale) * newScale;

            Assert.AreEqual(80.0, ball.Top, "Top position value");
            Assert.AreEqual(180.0, ball.Left, "Left position value");
            Assert.AreEqual(20.0, ball.Diameter, "Diameter value");
        }

        #region testing instrumentation

        private class BusinessLogicIBallFixture : BusinessLogic.IBall
        {
            public event EventHandler<IPosition>? NewPositionNotification;

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        #endregion testing instrumentation
    }
}