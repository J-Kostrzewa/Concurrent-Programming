//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Numerics;
namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void NewPositionNotificationTest()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            Ball newInstance = new Ball(dataBallFixture);
            int numberOfCallBackCalled = 0;
            IPosition receivedPosition = null;

            newInstance.NewPositionNotification += (sender, position) =>
            {
                Assert.IsNotNull(sender);
                Assert.IsNotNull(position);
                receivedPosition = position;
                numberOfCallBackCalled++;
            };

            dataBallFixture.RaisePositionChangeEvent();

            Assert.AreEqual(1, numberOfCallBackCalled);
            Assert.IsNotNull(receivedPosition);
            Assert.AreEqual(5.0, receivedPosition.x);
            Assert.AreEqual(10.0, receivedPosition.y);
        }

        [TestMethod]
        public void BusinessBall_ShouldHandleMultipleEvents()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            Ball newInstance = new Ball(dataBallFixture);
            int numberOfCallBackCalled = 0;

            newInstance.NewPositionNotification += (sender, position) =>
            {
                numberOfCallBackCalled++;
            };

            dataBallFixture.RaisePositionChangeEvent();
            dataBallFixture.RaisePositionChangeEvent();

            Assert.AreEqual(2, numberOfCallBackCalled);
        }

        [TestMethod]
        public void BusinessBall_ShouldNotifyWithCorrectData()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            Ball newInstance = new Ball(dataBallFixture);
            IPosition lastPosition = null;

            newInstance.NewPositionNotification += (sender, position) =>
            {
                lastPosition = position;
            };

            dataBallFixture.RaisePositionChangeEvent(new Vector2(20.0f, 30.0f));

            Assert.IsNotNull(lastPosition);
            Assert.AreEqual(20.0, lastPosition.x);
            Assert.AreEqual(30.0, lastPosition.y);
        }

        #region testing instrumentation

        private class DataBallFixture : Data.IBall
        {
            private Vector2 _position = new Vector2(5.0f, 10.0f);
            private Vector2 _velocity = new Vector2(1.0f, 1.0f);
            private bool _isMoving = false;

            public event EventHandler<Vector2>? NewPositionNotification;

            public Vector2 Position => _position;

            public Vector2 Velocity
            {
                get => _velocity;
                set => _velocity = value;
            }

            public bool IsMoving
            {
                get => _isMoving;
                set => _isMoving = value;
            }

            public int Radius => 10;

            public int Mass => 5;

            public void StartThread()
            {
                // Implementacja testowa - nic nie rób
            }

            internal void RaisePositionChangeEvent()
            {
                NewPositionNotification?.Invoke(this, _position);
            }

            internal void RaisePositionChangeEvent(Vector2 position)
            {
                _position = position;
                NewPositionNotification?.Invoke(this, _position);
            }
        }

        #endregion testing instrumentation
    }
}