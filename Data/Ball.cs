//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using System.Numerics;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        public event EventHandler<Vector2>? NewPositionNotification;
        private Vector2 position;
        private Vector2 velocity;
        private readonly int radius = 10;
        private readonly int mass = 5;
        private readonly object positionLock = new object();
        private readonly object velocityLock = new object();
        private bool isMoving;

        private readonly int _ballId;
        private static int _ballCounter = 0;
        private static readonly object _counterLock = new object();

        internal Ball(Vector2 initialPosition)
        {
            Random random = new Random();
            position = initialPosition;
            velocity = new Vector2(random.Next(1,10), random.Next(1,10));

            // Przypisanie unikalnego ID dla kulki
            lock (_counterLock)
            {
                _ballId = _ballCounter++;
            }
        }

        int IBall.Radius => radius;
        public Vector2 Position => position;
        public Vector2 Velocity
        {
            get => velocity;
            set
            {
                lock (velocityLock)
                {
                    velocity = value;
                }
            }
        }
        public bool IsMoving
        {
            get => isMoving;
            set
            {
                isMoving = value;
            }
        }
        public int Mass => mass;

        public void StartThread()
        {
            Thread thread = new Thread(Move);
            thread.IsBackground = true;
            thread.Start();
        }
        private async void Move()
        {
            isMoving = true;
            Stopwatch stopwatch = new();
            stopwatch.Start();
            float startingTime = 0f;
            DateTime lastLogTime = DateTime.MinValue;

            while (isMoving)
            {
                float currentTime = stopwatch.ElapsedMilliseconds;
                float delta = currentTime - startingTime;

                if (delta >= 1f / 120f)
                {

                    lock (positionLock)
                    {
                        position += velocity * 0.4f;
                    }

                    // Logowanie co 1 sekundę
                    if ((DateTime.Now - lastLogTime).TotalSeconds >= 1)
                    {
                        BallLogger.Instance.LogPosition(_ballId, position, velocity);
                        lastLogTime = DateTime.Now;
                    }


                    RaiseNewPositionChangeNotification();

                    // Oblicz opóźnienie na podstawie prędkości
                    double speed;
                    lock (velocityLock)
                    {
                        speed = Math.Sqrt(Math.Pow(velocity.X, 2) + Math.Pow(velocity.Y, 2));
                    }
                    await Task.Delay(TimeSpan.FromSeconds(1f / 120f));
                }
                
            }

        }
        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

    }
}