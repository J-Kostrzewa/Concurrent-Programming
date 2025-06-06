﻿//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private double _windowWidth;
        private double _windowHeight;
        private int _ballCount = 5;
        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;
        private double _canvasSize;
        private int maxBalls = 10;
        private bool isRunning = false; // Flaga do sprawdzania, czy model jest uruchomiony
        // Skalowane wymiary obszaru gry
        private double _gameAreaSize;

        public ICommand IncreaseBallCountCommand { get; }
        public ICommand DecreaseBallCountCommand { get; }
        public ICommand StartCommand { get; }

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;

            ModelLayer.ScaleFactor = 1.0;

            const double BorderThickness = 5;
            const double ExtraWindowWidth = 120;
            const double ExtraWindowHeight = 150;

            ModelLayer.UpdateDimensions(BorderThickness, ExtraWindowWidth, ExtraWindowHeight,
           (gameAreaSize, windowWidth, windowHeight) =>
           {
               GameAreaSize = gameAreaSize;
               CanvasSize = gameAreaSize + (BorderThickness * 2);
               WindowWidth = windowWidth;
               WindowHeight = windowHeight;
           });

            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));

            // Inicjalizacja komend
            IncreaseBallCountCommand = new RelayCommand(IncreaseBallCount, () => (BallCount < maxBalls && !isRunning));
            DecreaseBallCountCommand = new RelayCommand(DecreaseBallCount, () => (BallCount > 1 && !isRunning));
            StartCommand = new RelayCommand(() => Start(BallCount), () => !isRunning);
        }
        
        public double GameAreaSize
        {
            get => _gameAreaSize;
            set
            {
                if (_gameAreaSize != value)
                {
                    _gameAreaSize = value;
                    //RaisePropertyChanged();
                }
            }
        }

        public double CanvasSize
        {
            get => _canvasSize;
            set
            {
                if (_canvasSize != value)
                {
                    _canvasSize = value;
                    //RaisePropertyChanged();
                }
            }
        }

        public int BallCount
        {
            get => _ballCount;
            set
            {
                if (_ballCount != value)
                {
                    _ballCount = value;
                    RaisePropertyChanged(nameof(BallCount));
                    RaiseCanExecuteChanged();
                }
            }
        }

        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                _windowWidth = value;
            }
        }

        public double WindowHeight
        {
            get => _windowHeight;
            set
            {
                _windowHeight = value;
            }
        }

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(numberOfBalls);
            Observer.Dispose();
            isRunning = true;
            RaiseCanExecuteChanged();
        }
        private void IncreaseBallCount()
        {
            if (BallCount < maxBalls) // Maksymalna liczba kul
                BallCount++;
        }

        private void DecreaseBallCount()
        {
            if (BallCount > 1) // Minimalna liczba kul to 1
                BallCount--;
        }

        private void RaiseCanExecuteChanged()
        {
            (IncreaseBallCountCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DecreaseBallCountCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (StartCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    Observer.Dispose();
                    ModelLayer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}