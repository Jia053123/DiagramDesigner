using NUnit.Framework;
using System;
using BasicGeometries;

namespace DiagramDesignerEngine.UnitTests
{
	public class PointTests
	{
		[Test]
		public void TestEqualities()
		{
			var p1 = new Point(1, 2);
			var p2 = new Point(1, 2);
			
			var p3 = new Point(1, 2.02);
			var p4 = new Point(0.9, 2);

			//var p5 = new Point(1.0004, 1.9995);

			Point? p6 = null;
			
			Assert.AreEqual(p1, p2);
			Assert.AreNotEqual(p3, p1);
			Assert.AreNotEqual(p1, p4);
			//Assert.AreEqual(p1, p5);
			Assert.AreNotEqual(p1, p6);

			Assert.IsTrue(p1 == p2);
			Assert.IsTrue(p1 != p3);
			Assert.IsTrue(p4 != p1);
			//Assert.IsTrue(p1 == p5);
			Assert.IsTrue(p1 != p6);
			Assert.IsTrue(p6 != p1);

			Assert.IsTrue(p1.Equals(p2));
			Assert.IsFalse(p1.Equals(p3));
			Assert.IsFalse(p1.Equals(p6));

			Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
			Assert.AreNotEqual(p3.GetHashCode(), p1.GetHashCode());
			Assert.AreNotEqual(p1.GetHashCode(), p4.GetHashCode());
			//Assert.AreEqual(p1.GetHashCode(), p5.GetHashCode());
		}

		private bool AreClose(double num1, double num2, double tolerance)
		{
			return Math.Abs(num1 - num2) < tolerance;
		}

		[Test]
		public void TestAngleTowardsPoint()
		{
			double tolerance = 0.00000001;
			var p0 = new Point(0, 0);
			var p1 = new Point(2, 0);
			var p2 = new Point(0, 1);

			Assert.Throws<ArgumentException>(() => p1.AngleTowardsPoint(p1));
			Assert.IsTrue(AreClose(0, p0.AngleTowardsPoint(p1), tolerance));
			Assert.IsTrue(AreClose(Math.PI, p1.AngleTowardsPoint(p0), tolerance));
			Assert.IsTrue(AreClose(Math.PI * 0.5, p0.AngleTowardsPoint(p2), tolerance));
			Assert.IsTrue(AreClose(Math.PI * -0.5, p2.AngleTowardsPoint(p0), tolerance));
		}

		[Test]
		public void TestDistanceBetweenPoints()
		{
			var p1 = new Point(1, 2);
			Assert.AreEqual(0, Point.DistanceBetweenPoints(p1, p1));

			var p2 = new Point(1, 3);
			Assert.AreEqual(1, Point.DistanceBetweenPoints(p1, p2));
			Assert.AreEqual(1, Point.DistanceBetweenPoints(p2, p1));
			var p3 = new Point(2, 2);
			Assert.AreEqual(1, Point.DistanceBetweenPoints(p1, p3));
			Assert.AreEqual(1, Point.DistanceBetweenPoints(p3, p1));

			var p4 = new Point(1, -1);
			var p5 = new Point(2, -2);
			Assert.AreEqual(Math.Sqrt(2), Point.DistanceBetweenPoints(p4, p5));
			Assert.AreEqual(Math.Sqrt(2), Point.DistanceBetweenPoints(p5, p4));
		}
	}
}