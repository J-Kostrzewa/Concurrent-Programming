//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void MoveTestMethod()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            Ball newInstance = new(dataBallFixture);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
            dataBallFixture.Move();
            Assert.AreEqual<int>(2, numberOfCallBackCalled);
        }

        #region testing instrumentation

        private class DataBallFixture : Data.IBall
        {
            private Data.IVector _velocity = new VectorFixture(0.0, 0.0);
            private bool _isUpdating = false;

            public Data.IVector Velocity
            {
                get => _velocity;
                set
                {
                    if (value == null)
                        throw new ArgumentNullException(nameof(value), "Velocity cannot be null.");
                    _velocity = value;
                }
            }

            public event EventHandler<Data.IVector>? NewPositionNotification;

            internal void Move()
            {
                NewPositionNotification?.Invoke(this, _velocity);
            }

            void Data.IBall.SetPosition(Data.IVector position)
            {
                if (position == null)
                    throw new ArgumentNullException(nameof(position), "Position cannot be null.");

                // Zabezpieczenie przed zapętleniem
                if (_isUpdating)
                    return;

                try
                {
                    _isUpdating = true;
                    // Możesz tutaj aktualizować wewnętrzną pozycję jeśli jest potrzebna
                    NewPositionNotification?.Invoke(this, position);
                }
                finally
                {
                    _isUpdating = false;
                }
            }
        }

        private class VectorFixture : Data.IVector
        {
            internal VectorFixture(double X, double Y)
            {
                x = X; y = Y;
            }

            public double x { get; init; }
            public double y { get; init; }
        }

        #endregion testing instrumentation
    }
}