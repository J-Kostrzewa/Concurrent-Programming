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
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            using (BusinessLogicImplementation newInstance = new(new DataLayerConstructorFixture()))
            {
                bool newInstanceDisposed = true;
                newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
                Assert.IsFalse(newInstanceDisposed);
            }
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            DataLayerDisposeFixture dataLayerFixture = new DataLayerDisposeFixture();
            BusinessLogicImplementation newInstance = new(dataLayerFixture);

            Assert.IsFalse(dataLayerFixture.Disposed);
            bool newInstanceDisposed = true;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsFalse(newInstanceDisposed);

            newInstance.Dispose();

            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed);
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
            Assert.IsTrue(dataLayerFixture.Disposed);
        }

        [TestMethod]
        public void StartTestMethod()
        {
            DataLayerStartFixture dataLayerFixture = new();
            using (BusinessLogicImplementation newInstance = new(dataLayerFixture))
            {
                int called = 0;
                int numberOfBalls2Create = 10;

                newInstance.Start(
                    numberOfBalls2Create,
                    (startingPosition, ball) =>
                    {
                        called++;
                        Assert.IsNotNull(startingPosition);
                        Assert.IsNotNull(ball);
                    });

                Assert.AreEqual(1, called);
                Assert.IsTrue(dataLayerFixture.StartCalled);
                Assert.AreEqual(numberOfBalls2Create, dataLayerFixture.NumberOfBallsCreated);
            }
        }

        #region testing instrumentation

        private class DataLayerConstructorFixture : Data.DataAbstractAPI
        {
            public override int getWidth() => 400;
            public override int getHeight() => 400;
            public override List<Data.IBall> getAllBalls()
            {
                return new List<Data.IBall>();
            }

            public override void Dispose() { }

            public override void Start(int numberOfBalls, Action<Vector2, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }
        }

        private class DataLayerDisposeFixture : Data.DataAbstractAPI
        {
            internal bool Disposed = false;

            public override int getWidth() => 400;
            public override int getHeight() => 400;
            public override List<Data.IBall> getAllBalls()
            {
                return new List<Data.IBall>();
            }

            public override void Dispose()
            {
                Disposed = true;
            }

            public override void Start(int numberOfBalls, Action<Vector2, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }
        }

        private class DataLayerStartFixture : Data.DataAbstractAPI
        {
            internal bool StartCalled = false;
            internal int NumberOfBallsCreated = -1;

            public override int getWidth() => 400;
            public override int getHeight() => 400;
            public override List<Data.IBall> getAllBalls()
            {
                return new List<Data.IBall>();
            }

            public override void Dispose() { }

            public override void Start(int numberOfBalls, Action<Vector2, Data.IBall> upperLayerHandler)
            {
                StartCalled = true;
                NumberOfBallsCreated = numberOfBalls;
                upperLayerHandler(new Vector2(10.0f, 10.0f), new DataBallFixture());
            }

            private class DataBallFixture : Data.IBall
            {
                public Vector2 Position => new Vector2(10.0f, 10.0f);
                public Vector2 Velocity
                {
                    get => new Vector2(1.0f, 1.0f);
                    set { /* Implementacja testowa */ }
                }
                public bool IsMoving
                {
                    get => false;
                    set { /* Implementacja testowa */ }
                }
                public int Radius => 10;
                public int Mass => 5;

                public event EventHandler<Vector2>? NewPositionNotification;

                public void StartThread()
                {
                    // Implementacja testowa - nic nie rób
                }
            }
        }

        #endregion testing instrumentation
    }
}