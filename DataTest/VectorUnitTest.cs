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
  public class VectorUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      Random randomGenerator = new();
      float XComponent = (float)randomGenerator.NextDouble();
      float YComponent = (float)randomGenerator.NextDouble();
      Vector2 newInstance = new(XComponent, YComponent);
      Assert.AreEqual<double>(XComponent, newInstance.X);
      Assert.AreEqual<double>(YComponent, newInstance.Y);
    }
  }
}