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
        public void UpdateGameAreaDimensions_ShouldCalculateCorrectDimensions()
        {
            ModelSimulatorFixture modelFixture = new ModelSimulatorFixture();
            MainWindowViewModel viewModel = new MainWindowViewModel(modelFixture);

            modelFixture.ScaleFactor = 0.5;
            viewModel.UpdateGameAreaDimensions();

            Assert.AreEqual(200.0, viewModel.GameAreaSize);  // 400 * 0.5
            Assert.AreEqual(210.0, viewModel.CanvasSize);    // 200 + (5 * 2)
            Assert.AreEqual(330.0, viewModel.WindowWidth);   // 210 + 120
            Assert.AreEqual(360.0, viewModel.WindowHeight);  // 210 + 150
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