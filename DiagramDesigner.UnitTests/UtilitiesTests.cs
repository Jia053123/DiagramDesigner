using NUnit.Framework;
using WinPoint = System.Windows.Point;
using Point = DiagramDesignerEngine.Point;
using System;

namespace DiagramDesigner.UnitTests
{
	public class UtilitiesTests
	{
		[Test]
		public void TestConvertPointToWindowsPoint()
		{
			var p = new Point(-1, 1.5);
			var winPointUnitOverPointUnit = 2;
			var wp = Utilities.ConvertPointToWindowsPoint(p, winPointUnitOverPointUnit);

			Assert.AreEqual(wp.X, -2);
			Assert.AreEqual(wp.Y, 3);
		}

		[Test]
		public void TestConvertWindowsPointToPoint()
		{
			var wp = new WinPoint(5, -2);
			var winPointUnitOverPointUnit = 2;
			var p = Utilities.ConvertWindowsPointToPoint(wp, winPointUnitOverPointUnit);

			Assert.AreEqual(p.coordinateX, 2.5);
			Assert.AreEqual(p.coordinateY, -1);
		}


		[Test]
		public void TestDistanceFromWinPoint()
		{
			var p1 = new WinPoint(1, 2);
			Assert.AreEqual(Utilities.DistanceBetweenWinPoints(p1, p1), 0);

			var p2 = new WinPoint(1, 3);
			Assert.AreEqual(Utilities.DistanceBetweenWinPoints(p1, p2), 1);
			Assert.AreEqual(Utilities.DistanceBetweenWinPoints(p2, p1), 1);
			var p3 = new WinPoint(2, 2);
			Assert.AreEqual(Utilities.DistanceBetweenWinPoints(p1, p3), 1);
			Assert.AreEqual(Utilities.DistanceBetweenWinPoints(p3, p1), 1);

			var p4 = new WinPoint(1, -1);
			var p5 = new WinPoint(2, -2);
			Assert.AreEqual(Utilities.DistanceBetweenWinPoints(p4, p5), Math.Sqrt(2));
			Assert.AreEqual(Utilities.DistanceBetweenWinPoints(p5, p4), Math.Sqrt(2));
		}
	}
}