//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using TP.ConcurrentProgramming.Presentation.Model;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel.Test
{
    [TestClass]
    public class MainWindowViewModelUnitTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            ModelNullFixture nullModelFixture = new();
            Assert.AreEqual<int>(0, nullModelFixture.Disposed);
            Assert.AreEqual<int>(0, nullModelFixture.Started);
            Assert.AreEqual<int>(0, nullModelFixture.Subscribed);
            using (MainWindowViewModel viewModel = new(nullModelFixture))
            {
                Random random = new Random();
                int numberOfBalls = random.Next(1, 10);
                viewModel.Start(numberOfBalls);
                Assert.IsNotNull(viewModel.Balls);
                Assert.AreEqual<int>(0, nullModelFixture.Disposed);
                Assert.AreEqual<int>(numberOfBalls, nullModelFixture.Started);
                Assert.AreEqual<int>(1, nullModelFixture.Subscribed);
            }
            Assert.AreEqual<int>(1, nullModelFixture.Disposed);
        }

        [TestMethod]
        public void BehaviorTestMethod()
        {
            ModelSimulatorFixture modelSimulator = new();
            MainWindowViewModel viewModel = new(modelSimulator);
            Assert.IsNotNull(viewModel.Balls);
            Assert.AreEqual<int>(0, viewModel.Balls.Count);
            Random random = new Random();
            int numberOfBalls = random.Next(1, 10);
            viewModel.Start(numberOfBalls);
            Assert.AreEqual<int>(numberOfBalls, viewModel.Balls.Count);
            viewModel.Dispose();
            Assert.IsTrue(modelSimulator.Disposed);
            Assert.AreEqual<int>(0, viewModel.Balls.Count);
        }

        [TestMethod]
        public void UpdateDimensions_CorrectlyUpdatesViewModelProperties()
        {
            // Arrange
            ModelSimulatorFixture modelFixture = new ModelSimulatorFixture();
            MainWindowViewModel viewModel = new MainWindowViewModel(modelFixture);

            // Przygotowanie wartoœci testowych
            double gameAreaSize = 200.0;
            double windowWidth = 330.0;
            double windowHeight = 360.0;

            // Act - symulacja wywo³ania UpdateDimensions z warstwy modelu
            modelFixture.SimulateUpdateDimensions(gameAreaSize, windowWidth, windowHeight);

            // Assert - sprawdzenie, czy ViewModel poprawnie zaktualizowa³ swoje w³aœciwoœci
            Assert.AreEqual(gameAreaSize, viewModel.GameAreaSize);
            Assert.AreEqual(gameAreaSize + 10, viewModel.CanvasSize);  // BorderThickness * 2 = 10
            Assert.AreEqual(windowWidth, viewModel.WindowWidth);
            Assert.AreEqual(windowHeight, viewModel.WindowHeight);
        }

        #region testing infrastructure

        private class ModelNullFixture : ModelAbstractApi
        {
            #region Test

            internal int Disposed = 0;
            internal int Started = 0;
            internal int Subscribed = 0;

            internal int ScaleFactorGetCount = 0;
            internal int ScaleFactorSetCount = 0;
            private double _scaleFactor = 1.0;
            private Action<double, double, double> _lastCallback;

            #endregion Test

            #region ModelAbstractApi

            public override double ScaleFactor
            {
                get
                {
                    ScaleFactorGetCount++;
                    return _scaleFactor;
                }
                set
                {
                    ScaleFactorSetCount++;
                    _scaleFactor = value;
                }
            }

            public override double LogicalGameAreaWidth => 400;
            public override double LogicalGameAreaHeight => 400;
            public override double BallDiameter => 20;

            public override void UpdateDimensions(double borderThickness, double extraWindowWidth, double extraWindowHeight, Action<double, double, double> dimensionsUpdatedCallback)
            {
                _lastCallback = dimensionsUpdatedCallback;
                double gameAreaSize = LogicalGameAreaWidth * ScaleFactor;
                double windowWidth = gameAreaSize + (borderThickness * 2) + extraWindowWidth;
                double windowHeight = gameAreaSize + (borderThickness * 2) + extraWindowHeight;
                dimensionsUpdatedCallback(gameAreaSize, windowWidth, windowHeight);
            }


            public override void Dispose()
            {
                Disposed++;
            }

            public override void Start(int numberOfBalls)
            {
                Started = numberOfBalls;
            }

            public override IDisposable Subscribe(IObserver<ModelIBall> observer)
            {
                Subscribed++;
                return new NullDisposable();
            }

            #endregion ModelAbstractApi

            #region private

            private class NullDisposable : IDisposable
            {
                public void Dispose()
                { }
            }

            #endregion private
        }

        private class ModelSimulatorFixture : ModelAbstractApi
        {
            #region Testing indicators

            internal bool Disposed = false;
            private double _scaleFactor = 1.0;
            private Action<double, double, double> _lastCallback;

            #endregion Testing indicators

            #region ctor

            public ModelSimulatorFixture()
            {
                eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(this, "BallChanged");
            }

            #endregion ctor

            #region ModelAbstractApi fixture

            public override double ScaleFactor
            {
                get => _scaleFactor;
                set => _scaleFactor = value;
            }

            public override double LogicalGameAreaWidth => 400;
            public override double LogicalGameAreaHeight => 400;
            public override double BallDiameter => 20;

            public override void UpdateDimensions(double borderThickness, double extraWindowWidth, double extraWindowHeight, Action<double, double, double> dimensionsUpdatedCallback)
            {
                _lastCallback = dimensionsUpdatedCallback;
                double gameAreaSize = LogicalGameAreaWidth * ScaleFactor;
                double windowWidth = gameAreaSize + (borderThickness * 2) + extraWindowWidth;
                double windowHeight = gameAreaSize + (borderThickness * 2) + extraWindowHeight;
                dimensionsUpdatedCallback(gameAreaSize, windowWidth, windowHeight);
            }

            public void SimulateUpdateDimensions(double gameAreaSize, double windowWidth, double windowHeight)
            {
                if (_lastCallback != null)
                {
                    _lastCallback(gameAreaSize, windowWidth, windowHeight);
                }
            }

            public override IDisposable? Subscribe(IObserver<ModelIBall> observer)
            {
                return eventObservable?.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
            }

            public override void Start(int numberOfBalls)
            {
                for (int i = 0; i < numberOfBalls; i++)
                {
                    ModelBall newBall = new ModelBall(0, 0) { };
                    BallChanged?.Invoke(this, new BallChaneEventArgs() { Ball = newBall });
                }
            }

            public override void Dispose()
            {
                Disposed = true;
            }

            #endregion ModelAbstractApi

            #region API

            public event EventHandler<BallChaneEventArgs> BallChanged;

            #endregion API

            #region private

            private IObservable<EventPattern<BallChaneEventArgs>>? eventObservable = null;

            private class ModelBall : ModelIBall
            {
                public ModelBall(double top, double left)
                { }

                #region IBall

                public double Diameter => throw new NotImplementedException();

                public double Top => throw new NotImplementedException();

                public double Left => throw new NotImplementedException();

                #region INotifyPropertyChanged

                public event PropertyChangedEventHandler? PropertyChanged;

                #endregion INotifyPropertyChanged

                #endregion IBall
            }

            #endregion private
        }

        #endregion testing infrastructure
    }
}