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

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector2 testingVector = new Vector2(0.0f, 0.0f);
            Ball newInstance = new(testingVector);

            Assert.AreEqual(testingVector, newInstance.Position);
            Assert.AreEqual(10, ((IBall)newInstance).Radius);
            Assert.AreEqual(5, newInstance.Mass);
            Assert.IsFalse(newInstance.IsMoving);
            Assert.IsNotNull(newInstance.Velocity);
        }

        [TestMethod]
        public void VelocitySetterTest()
        {
            Vector2 initialPosition = new Vector2(10.0f, 10.0f);
            Ball newInstance = new(initialPosition);
            Vector2 newVelocity = new Vector2(5.0f, 7.0f);

            newInstance.Velocity = newVelocity;

            Assert.AreEqual(newVelocity, newInstance.Velocity);
        }

        [TestMethod]
        public void IsMovingSetterTest()
        {
            Vector2 initialPosition = new Vector2(10.0f, 10.0f);
            Ball newInstance = new(initialPosition);

            Assert.IsFalse(newInstance.IsMoving);

            // Zmiana wartości
            newInstance.IsMoving = true;
            Assert.IsTrue(newInstance.IsMoving);
        }

        [TestMethod]
        public void RadiusAndMassPropertiesTest()
        {
            Vector2 initialPosition = new Vector2(10.0f, 10.0f);
            Ball newInstance = new(initialPosition);

            Assert.AreEqual(10, ((IBall)newInstance).Radius);
            Assert.AreEqual(5, newInstance.Mass);
        }
    }
}
