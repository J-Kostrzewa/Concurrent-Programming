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
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        private bool Disposed = false;
        private readonly UnderneathLayerAPI layerBellow;
        
        public BusinessLogicImplementation() : this(null)
        { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        //Wall collision check
        internal void WallCollision(Data.IBall ball)
        {
            double tableWidth = layerBellow.getWidth();
            double tableHeight = layerBellow.getHeight();
            
            if (ball.Position.X - ball.Radius <= 0 || ball.Position.X + ball.Radius >= tableWidth)
                ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);
            if (ball.Position.Y - ball.Radius <= 0 || ball.Position.Y + ball.Radius >= tableHeight)
                ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);             
        }

        //Ball collision check
        internal void BallCollision(Data.IBall ball)
        {
            List<Data.IBall> balls = layerBellow.getAllBalls();
            foreach (var otherBall in balls)
            {
                if (otherBall != ball)
                {
                    double distance = Math.Sqrt(Math.Pow(ball.Position.X - otherBall.Position.X, 2) + Math.Pow(ball.Position.Y - otherBall.Position.Y, 2));
                    if (distance <= ball.Radius + otherBall.Radius)
                    {
                        Vector2 newVelocity = new Vector2(-ball.Velocity.X, -ball.Velocity.Y);
                        ball.Velocity = newVelocity;
                    }
                }
            }
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            layerBellow.Start(numberOfBalls, (startingPosition, databall) => upperLayerHandler(new Position(startingPosition.x, startingPosition.x), new Ball(databall)));
        }

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

    }
}