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
namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class DataImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                IEnumerable<IBall>? ballsList = null;
                newInstance.CheckBallsList(x => ballsList = x);
                Assert.IsNotNull(ballsList);
                int numberOfBalls = 0;
                newInstance.CheckNumberOfBalls(x => numberOfBalls = x);
                Assert.AreEqual(0, numberOfBalls);
            }
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            DataImplementation newInstance = new DataImplementation();
            bool newInstanceDisposed = false;

            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsFalse(newInstanceDisposed);

            newInstance.Dispose();

            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed);

            // Sprawdzenie czy lista piłek jest pusta
            IEnumerable<IBall>? ballsList = null;
            newInstance.CheckBallsList(x => ballsList = x);
            Assert.IsNotNull(ballsList);
            newInstance.CheckNumberOfBalls(x => Assert.AreEqual(0, x));

            // Sprawdzenie czy ponowne wywołanie Dispose rzuca wyjątek
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());

            // Sprawdzenie czy wywołanie Start rzuca wyjątek po Dispose
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
        }

        [TestMethod]
        public void StartTestMethod()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                int numberOfCallbackInvoked = 0;
                int numberOfBalls2Create = 10;

                newInstance.Start(
                  numberOfBalls2Create,
                  (startingPosition, ball) =>
                  {
                      numberOfCallbackInvoked++;
                      Assert.IsTrue(startingPosition.X >= 0);
                      Assert.IsTrue(startingPosition.Y >= 0);
                      Assert.IsNotNull(ball);
                  });

                Assert.AreEqual(numberOfBalls2Create, numberOfCallbackInvoked);
                newInstance.CheckNumberOfBalls(x => Assert.AreEqual(10, x));
            }
        }

        [TestMethod]
        public void GetWidth_ReturnsCorrectValue()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                int width = newInstance.getWidth();

                Assert.AreEqual(400, width);
            }
        }

        [TestMethod]
        public void GetHeight_ReturnsCorrectValue()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                int height = newInstance.getHeight();

                Assert.AreEqual(400, height);
            }
        }

        [TestMethod]
        public void GetAllBalls_ReturnsAllCreatedBalls()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                int numberOfBalls = 5;
                List<IBall> createdBalls = new List<IBall>();

                newInstance.Start(numberOfBalls, (pos, ball) =>
                {
                    createdBalls.Add(ball);
                });

                List<IBall> returnedBalls = newInstance.getAllBalls();

                Assert.AreEqual(numberOfBalls, returnedBalls.Count);

                // Sprawdzamy czy zwrócona lista zawiera wszystkie utworzone piłki
                foreach (var ball in createdBalls)
                {
                    Assert.IsTrue(returnedBalls.Contains(ball));
                }
            }
        }

        [TestMethod]
        public void Start_CreatesPositionsWithinBounds()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                int numberOfBalls = 5;
                List<Vector2> positions = new List<Vector2>();

                newInstance.Start(numberOfBalls, (pos, ball) =>
                {
                    positions.Add(pos);
                });

                int width = newInstance.getWidth();
                int height = newInstance.getHeight();

                foreach (var pos in positions)
                {
                    // Sprawdzamy, czy pozycje piłek mieszczą się w obszarze gry 
                    // (z uwzględnieniem marginesu 10 pikseli)
                    Assert.IsTrue(pos.X >= 10 && pos.X <= width - 10);
                    Assert.IsTrue(pos.Y >= 10 && pos.Y <= height - 10);
                }
            }
        }

        [TestMethod]
        public void Start_ThrowsArgumentNullException_WhenHandlerIsNull()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                Assert.ThrowsException<ArgumentNullException>(() => newInstance.Start(5, null));
            }
        }
    }
}