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

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        public event EventHandler<Vector2>? NewPositionNotification;
        private Vector2 position;
        private Vector2 velocity;
        private readonly int radius = 10;
        private readonly object positionLock = new object();
        private readonly object velocityLock = new object();
        private bool isMoving;

        internal Ball(Vector2 initialPosition)
        {
            Random random = new Random();
            position = initialPosition;
            velocity = new Vector2(random.Next(1,10), random.Next(1,10));
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

        public void StartThread()
        {
            Thread thread = new Thread(Move);
            thread.Start();
        }
        private async void Move()
        {
            isMoving = true;
            while (isMoving)
            {
                lock (positionLock)
                {
                    //position = new Vector2(position.X + velocity.X, position.Y + velocity.Y);
                    position += velocity;
                }
                double calculatedVelocity = Math.Sqrt(Math.Pow(velocity.X, 2) + Math.Pow(velocity.Y, 2));
                await Task.Delay((int)(1000 / calculatedVelocity));
            }

            RaiseNewPositionChangeNotification();
        }
        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

    }
}