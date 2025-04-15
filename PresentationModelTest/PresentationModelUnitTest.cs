//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.Presentation.Model.Test
{
    [TestClass]
    public class PresentationModelUnitTest
    {
        [TestMethod]
        public void DisposeTestMethod()
        {
            UnderneathLayerFixture underneathLayerFixture = new UnderneathLayerFixture();
            ModelImplementation? newInstance = null;
            using (newInstance = new(underneathLayerFixture))
            {
                newInstance.CheckObjectDisposed(x => Assert.IsFalse(x));
                newInstance.CheckUnderneathLayerAPI(x => Assert.AreSame(underneathLayerFixture, x));
                newInstance.CheckBallChangedEvent(x => Assert.IsTrue(x));
                Assert.IsFalse(underneathLayerFixture.Disposed);
            }
            newInstance.CheckObjectDisposed(x => Assert.IsTrue(x));
            newInstance.CheckUnderneathLayerAPI(x => Assert.AreSame(underneathLayerFixture, x));
            Assert.IsTrue(underneathLayerFixture.Disposed);
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
        }

        [TestMethod]
        public void StartTestMethod()
        {
            UnderneathLayerFixture underneathLayerFixture = new UnderneathLayerFixture();
            using (ModelImplementation newInstance = new(underneathLayerFixture))
            {
                newInstance.CheckBallChangedEvent(x => Assert.IsTrue(x));
                IDisposable subscription = newInstance.Subscribe(x => { });
                newInstance.CheckBallChangedEvent(x => Assert.IsFalse(x));
                newInstance.Start(10);
                Assert.AreEqual<int>(10, underneathLayerFixture.NumberOfBalls);
                subscription.Dispose();
                newInstance.CheckBallChangedEvent(x => Assert.IsTrue(x));
            }
        }

        [TestMethod]
        public void UpdateDimensions_ShouldCalculateCorrectDimensions()
        {
            UnderneathLayerFixture underneathFixture = new UnderneathLayerFixture();
            ModelImplementationFixture model = new ModelImplementationFixture(underneathFixture);
            double borderThickness = 5.0;
            double extraWindowWidth = 120.0;
            double extraWindowHeight = 150.0;
            double gameAreaWidthResult = 0;
            double windowWidthResult = 0;
            double windowHeightResult = 0;

            model.ScaleFactor = 0.5;
            model.UpdateDimensions(borderThickness, extraWindowWidth, extraWindowHeight,
                (gameAreaWidth, windowWidth, windowHeight) =>
                {
                    gameAreaWidthResult = gameAreaWidth;
                    windowWidthResult = windowWidth;
                    windowHeightResult = windowHeight;
                });

            Assert.AreEqual(200.0, gameAreaWidthResult);  // 400 * 0.5
            double expectedCanvasSize = 200.0 + (borderThickness * 2);
            Assert.AreEqual(expectedCanvasSize + extraWindowWidth, windowWidthResult);
            Assert.AreEqual(expectedCanvasSize + extraWindowHeight, windowHeightResult);
        }

        #region testing instrumentation

        private class ModelImplementationFixture : ModelImplementation
        {
            public ModelImplementationFixture(BusinessLogicAbstractAPI underneathLayer)
                : base(underneathLayer)
            { }
        }

        private class UnderneathLayerFixture : BusinessLogicAbstractAPI
        {
            #region testing instrumentation

            internal bool Disposed = false;
            internal int NumberOfBalls = 0;

            #endregion testing instrumentation

            #region BusinessLogicAbstractAPI

            public override void Dispose()
            {
                Disposed = true;
            }

            public override void Start(int numberOfBalls, Action<IPosition, BusinessLogic.IBall> upperLayerHandler)
            {
                NumberOfBalls = numberOfBalls;
                Assert.IsNotNull(upperLayerHandler);
            }

            #endregion BusinessLogicAbstractAPI
        }

        #endregion testing instrumentation
    }
}