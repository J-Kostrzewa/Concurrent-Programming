﻿//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.ComponentModel;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    public interface IBall : INotifyPropertyChanged
    {
        double Top { get; }
        double Left { get; }
        double Diameter { get; }
    }

    public abstract class ModelAbstractApi : IObservable<IBall>, IDisposable
    {
        public static ModelAbstractApi CreateModel()
        {
            return modelInstance.Value;
        }

        // Współczynnik skalowania dla elementów graficznych
        public abstract double ScaleFactor { get; set; }

        public abstract void Start(int numberOfBalls);

        
        public abstract double LogicalGameAreaWidth { get; }
        public abstract double LogicalGameAreaHeight { get; }
        public abstract double BallDiameter { get; }

        // Metoda do aktualizacji wymiarów UI
        public abstract void UpdateDimensions(double borderThickness,double extraWindowWidth,double extraWindowHeight,Action<double, double, double> dimensionsUpdatedCallback);

        #region IObservable

        public abstract IDisposable Subscribe(IObserver<IBall> observer);

        #endregion IObservable

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #region private

        private static Lazy<ModelAbstractApi> modelInstance = new Lazy<ModelAbstractApi>(() => new ModelImplementation());

        #endregion private
    }
}