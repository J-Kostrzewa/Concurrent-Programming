//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2023, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//  by introducing yourself and telling us what you do with this community.
//_____________________________________________________________________________________________________________________________________

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TP.ConcurrentProgramming.BusinessLogic;
using LogicIBall = TP.ConcurrentProgramming.BusinessLogic.IBall;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    internal class ModelBall : IBall
    {
        private double TopBackingField;
        private double LeftBackingField;
        private double _originalTop; // Oryginalna pozycja (nieskalowana)
        private double _originalLeft; // Oryginalna pozycja (nieskalowana)
        private double _scaleFactor; // Współczynnik skalowania
        public event PropertyChangedEventHandler PropertyChanged;

        public ModelBall(double top, double left, LogicIBall underneathBall, double scale = 1.0)
        {
            _originalTop = top;
            _originalLeft = left;
            _scaleFactor = scale;

            Diameter = 20.0 * _scaleFactor; 
            double radius = Diameter / 2.0;

            TopBackingField = (top - radius) * _scaleFactor;
            LeftBackingField = (left - radius) * _scaleFactor;
            underneathBall.NewPositionNotification += NewPositionNotification;
        }
        public double Diameter { get; set; } = 0;

        public double Top
        {
            get { return TopBackingField; }
            private set
            {
                if (TopBackingField == value)
                    return;
                TopBackingField = value;
                RaisePropertyChanged();
            }
        }

        public double Left
        {
            get { return LeftBackingField; }
            private set
            {
                if (LeftBackingField == value)
                    return;
                LeftBackingField = value;
                RaisePropertyChanged();
            }
        }

        // Metoda do aktualizacji skali kulki
        internal void UpdateScale(double newScaleFactor)
        {
            if (_scaleFactor != newScaleFactor && newScaleFactor > 0)
            {
                _scaleFactor = newScaleFactor;

                // Aktualizacja średnicy (tylko jeśli jest zainicjowana)
                if (Diameter > 0)
                {
                    double originalDiameter = Diameter / (this._scaleFactor / newScaleFactor);
                    Diameter = originalDiameter * _scaleFactor;
                }

                double radius = Diameter / 2.0;
                Top = (_originalTop - radius) * _scaleFactor;
                Left = (_originalLeft - radius) * _scaleFactor;
            }
        }

        private void NewPositionNotification(object sender, IPosition e)
        {
            // Zapamiętanie oryginalnych współrzędnych
            _originalTop = e.y;
            _originalLeft = e.x;

            // Ustawienie skalowanych wartości
            double radius = Diameter / 2.0;
            Top = (_originalTop - radius) * _scaleFactor;
            Left = (_originalLeft - radius) * _scaleFactor;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        internal void SetLeft(double x)
        { Left = x; }

        [Conditional("DEBUG")]
        internal void SettTop(double x)
        { Top = x; }

    }
}