﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//_____________________________________________________________________________________________________________________________________

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using TP.ConcurrentProgramming.BusinessLogic;
using UnderneathLayerAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    /// <summary>
    /// Class Model - implements the <see cref="ModelAbstractApi" />
    /// </summary>
    internal class ModelImplementation : ModelAbstractApi
    {
        internal ModelImplementation() : this(null)
        { }

        internal ModelImplementation(UnderneathLayerAPI underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetBusinessLogicLayer() : underneathLayer;
            eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(this, "BallChanged");
        }

        #region ModelAbstractApi

        private double _scaleFactor = 1.0;

        // Implementacja właściwości ScaleFactor
        public override double ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (_scaleFactor != value && value > 0)
                {
                    _scaleFactor = value;
                    // Aktualizacja istniejących kulek
                    foreach (var ball in _balls)
                    {
                        if (ball is ModelBall modelBall)
                        {
                            modelBall.UpdateScale(_scaleFactor);
                        }
                    }
                }
            }
        }

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Model));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override IDisposable Subscribe(IObserver<IBall> observer)
        {
            return eventObservable.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
        }

        public override void Start(int numberOfBalls)
        {
            layerBellow.Start(numberOfBalls, StartHandler);
        }

        public override double LogicalGameAreaWidth => BusinessLogicAbstractAPI.GetDimensions.TableWidth;
        public override double LogicalGameAreaHeight => BusinessLogicAbstractAPI.GetDimensions.TableHeight;
        public override double BallDiameter => BusinessLogicAbstractAPI.GetDimensions.BallDimension;

        public override void UpdateDimensions(double borderThickness,
                                        double extraWindowWidth,
                                        double extraWindowHeight,
                                        Action<double, double, double> dimensionsUpdatedCallback)
        {
            double gameAreaWidth = LogicalGameAreaWidth * _scaleFactor;
            double gameAreaHeight = LogicalGameAreaHeight * _scaleFactor;
            double canvasSize = Math.Max(gameAreaWidth, gameAreaHeight) + (borderThickness * 2);
            double windowWidth = canvasSize + extraWindowWidth;
            double windowHeight = canvasSize + extraWindowHeight;

            // Wywołaj callback do aktualizacji UI
            dimensionsUpdatedCallback(gameAreaWidth, windowWidth, windowHeight);
        }

        #endregion ModelAbstractApi

        #region API

        public event EventHandler<BallChaneEventArgs> BallChanged;

        #endregion API

        #region private

        private bool Disposed = false;
        private readonly IObservable<EventPattern<BallChaneEventArgs>> eventObservable = null;
        private readonly UnderneathLayerAPI layerBellow = null;
        private readonly List<IBall> _balls = new List<IBall>();

        private void StartHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball)
        {
            // Tworzymy kulkę z uwzględnieniem aktualnego współczynnika skali
            ModelBall newBall = new ModelBall(position.x, position.y, ball, _scaleFactor);
            newBall.Diameter = 20.0 * _scaleFactor;
            _balls.Add(newBall);
            BallChanged.Invoke(this, new BallChaneEventArgs() { Ball = newBall });
        }

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        [Conditional("DEBUG")]
        internal void CheckUnderneathLayerAPI(Action<UnderneathLayerAPI> returnNumberOfBalls)
        {
            returnNumberOfBalls(layerBellow);
        }

        [Conditional("DEBUG")]
        internal void CheckBallChangedEvent(Action<bool> returnBallChangedIsNull)
        {
            returnBallChangedIsNull(BallChanged == null);
        }

        #endregion TestingInfrastructure
    }

    public class BallChaneEventArgs : EventArgs
    {
        public IBall Ball { get; init; }
    }
}