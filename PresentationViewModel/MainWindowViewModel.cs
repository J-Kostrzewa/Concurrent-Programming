//__________________________________________________________________________________________
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
        #region ctor

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;

            ModelLayer.ScaleFactor = 2.3;

            // Aktualizacja wymiarów obszaru gry
            UpdateGameAreaDimensions();

            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));

            // Inicjalizacja komend
            IncreaseBallCountCommand = new RelayCommand(IncreaseBallCount, () => (BallCount < maxBalls && !isRunning));
            DecreaseBallCountCommand = new RelayCommand(DecreaseBallCount, () => (BallCount > 1 && !isRunning));
            StartCommand = new RelayCommand(() => Start(BallCount), () => !isRunning);
        }

        #endregion ctor

        #region public API

        private double _windowWidth;
        private double _windowHeight;
        private int _ballCount = 5; // Domyślna liczba kul

        // Logiczne wymiary obszaru gry
        private const double LogicalGameAreaSize = 400;
        private const double BorderThickness = 5;
        private const double CanvasMargin = 5;

        // Skalowane wymiary obszaru gry
        private double _gameAreaSize;
        public double GameAreaSize
        {
            get => _gameAreaSize;
            set
            {
                if (_gameAreaSize != value)
                {
                    _gameAreaSize = value;
                    RaisePropertyChanged();
                }
            }
        }

        // Wymiary Canvas uwzględniające marginesy
        private double _canvasSize;
        public double CanvasSize
        {
            get => _canvasSize;
            set
            {
                if (_canvasSize != value)
                {
                    _canvasSize = value;
                    RaisePropertyChanged();
                }
            }
        }

        // Metoda aktualizująca wymiary obszaru gry na podstawie współczynnika skalowania
        private void UpdateGameAreaDimensions()
        {
            double scale = ModelLayer.ScaleFactor;
            GameAreaSize = LogicalGameAreaSize * scale;

            // Canvas ma rozmiar obszaru gry plus marginesy
            CanvasSize = GameAreaSize + (BorderThickness * 2);

            // Ustawienie wymiarów okna
            WindowWidth = CanvasSize + 120;
            WindowHeight = CanvasSize + 150;
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

        public ICommand IncreaseBallCountCommand { get; }
        public ICommand DecreaseBallCountCommand { get; }
        public ICommand StartCommand { get; }

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

        private int maxBalls = 10;
        private bool isRunning = false; // Flaga do sprawdzania, czy model jest uruchomiony

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

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(numberOfBalls);
            Observer.Dispose();
            isRunning = true;
            RaiseCanExecuteChanged();
        }

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

        #endregion public API

        #region IDisposable

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

        #endregion IDisposable

        #region private

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;

        #endregion private
    }
}