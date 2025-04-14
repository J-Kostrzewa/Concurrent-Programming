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
using System;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        public Ball(Data.IBall ball)
        {
            ball.NewPositionNotification += RaisePositionChangeEvent;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private

        private void RaisePositionChangeEvent(object? sender, Data.IVector position)
        {
            Data.IBall dataBall = (Data.IBall)sender!;
            WallCollisionCheck(dataBall, position);
            NewPositionNotification?.Invoke(this, new Position(position.x, position.y));
        }

        private void WallCollisionCheck(Data.IBall ball, Data.IVector position)
        {
            // Pobieramy wymiary stołu i kulki z API warstwy logiki
            double ballDiameter = BusinessLogicAbstractAPI.GetDimensions.BallDimension;
            double tableWidth = BusinessLogicAbstractAPI.GetDimensions.TableWidth;
            double tableHeight = BusinessLogicAbstractAPI.GetDimensions.TableHeight;

            // Pobieramy aktualną pozycję z parametru i prędkość kulki
            double posX = position.x;
            double posY = position.y;
            Data.IVector velocity = ((Data.IBall)ball).Velocity;

            // Promień kulki (połowa średnicy)
            //double radius = ballDiameter / 2;

            bool collisionDetected = false;
            double newVelocityX = velocity.x;
            double newVelocityY = velocity.y;
            double newPosX = posX;
            double newPosY = posY;

            // Sprawdzamy kolizję z lewą i prawą ścianą
            if (posX <= 0) // Lewa ściana
            {
                newVelocityX = -velocity.x; // Odbijamy w prawo
                newPosX = 0;
                collisionDetected = true;
            }
            else if (posX + ballDiameter >= tableWidth) // Prawa ściana
            {
                newVelocityX = -velocity.x; // Odbijamy w lewo
                newPosX = tableWidth - ballDiameter;
                collisionDetected = true;
            }

            // Sprawdzamy kolizję z górną i dolną ścianą
            if (posY <= 0) // Górna ściana
            {
                newVelocityY = -velocity.y; // Odbijamy w dół
                newPosY = 0;
                collisionDetected = true;
            }
            else if (posY + ballDiameter >= tableHeight) // Dolna ściana
            {
                newVelocityY = -velocity.y; // Odbijamy w górę
                newPosY = tableHeight - ballDiameter;
                collisionDetected = true;
            }

            // Jeśli wykryto kolizję, aktualizujemy prędkość kulki używając interfejsu abstrakcyjnego
            if (collisionDetected)
            {
                ((Data.IBall)ball).Velocity = new BusinessVector(newVelocityX, newVelocityY);
                ((Data.IBall)ball).SetPosition(new BusinessVector(newPosX, newPosY));
                collisionDetected = false;
                Console.WriteLine("TEST");
            }

        }

        // Klasa pomocnicza implementująca interfejs IVector
        private class BusinessVector : Data.IVector
        {
            public double x { get; init; }
            public double y { get; init; }

            public BusinessVector(double xComponent, double yComponent)
            {
                x = xComponent;
                y = yComponent;
            }
        }

        #endregion private
    }
}