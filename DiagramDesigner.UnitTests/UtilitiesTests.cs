using NUnit.Framework;
using WinPoint = System.Windows.Point;
using Point = BasicGeometries.Point;
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
		public void TestDistanceBetweenWinPoints()
		{
			var p1 = new WinPoint(1, 2);
			Assert.AreEqual(0, Utilities.DistanceBetweenWinPoints(p1, p1));

			var p2 = new WinPoint(1, 3);
			Assert.AreEqual(1, Utilities.DistanceBetweenWinPoints(p1, p2));
			Assert.AreEqual(1, Utilities.DistanceBetweenWinPoints(p2, p1));
			var p3 = new WinPoint(2, 2);
			Assert.AreEqual(1, Utilities.DistanceBetweenWinPoints(p1, p3));
			Assert.AreEqual(1, Utilities.DistanceBetweenWinPoints(p3, p1));

			var p4 = new WinPoint(1, -1);
			var p5 = new WinPoint(2, -2);
			Assert.AreEqual(Math.Sqrt(2), Utilities.DistanceBetweenWinPoints(p4, p5));
			Assert.AreEqual(Math.Sqrt(2), Utilities.DistanceBetweenWinPoints(p5, p4));
		}

		[Test]
		public void TestDistanceFromWinPointToLine()
		{
			var p1 = new WinPoint(0, 0);
			var endPoint1 = new WinPoint(1, -1);
			var endPoint2 = new WinPoint(1, 1);

			Assert.Throws<ArgumentException>(() => Utilities.DistanceFromWinPointToLine(p1, endPoint1, endPoint1));

			Assert.AreEqual(1, Utilities.DistanceFromWinPointToLine(p1, endPoint1, endPoint2).Item1);
			Assert.AreEqual(new WinPoint(1, 0), Utilities.DistanceFromWinPointToLine(p1, endPoint1, endPoint2).Item2);

			var p2 = new WinPoint(0, 0);
			var endPoint3 = new WinPoint(1, -2);
			var endPoint4 = new WinPoint(-1, -2);
			Assert.AreEqual(2, Utilities.DistanceFromWinPointToLine(p2, endPoint3, endPoint4).Item1);
			Assert.AreEqual(new WinPoint(0, -2), Utilities.DistanceFromWinPointToLine(p2, endPoint3, endPoint4).Item2);

			// 1  x
			//       \
			// 0       \  
			//           \
			// -1 x         x
			//   -1    0    1
			var p3 = new WinPoint(-1, -1);
			var endPoint5 = new WinPoint(-1, 1);
			var endPoint6 = new WinPoint(1, -1);
			Assert.AreEqual(Math.Sqrt(2), Utilities.DistanceFromWinPointToLine(p3, endPoint5, endPoint6).Item1);
			Assert.AreEqual(new WinPoint(0, 0), Utilities.DistanceFromWinPointToLine(p3, endPoint5, endPoint6).Item2);

			var p4 = new WinPoint(0, 0);
			var endPoint7 = new WinPoint(1, 1);
			var endPoint8 = new WinPoint(2, 2);
			Assert.IsNull(Utilities.DistanceFromWinPointToLine(p4, endPoint7, endPoint8));

			//       
			// 1      x----x  
			//           
			// 0 x         
			//   0    1    2
			var p5 = new WinPoint(0, 0);
			var endPoint9 = new WinPoint(1, 1);
			var endPoint10 = new WinPoint(2, 1);
			Assert.IsNull(Utilities.DistanceFromWinPointToLine(p5, endPoint9, endPoint10));
		}

		[Test]
		public void TestPointOrthogonal()
		{
			var p1 = new WinPoint(0, 0);
			Assert.AreEqual(new WinPoint(0,0), Utilities.PointOrthogonal(p1, p1));

			// 2    x
			//     
			// 1
			//
			// 0 X
			//   0  1
			var p2 = new WinPoint(1, 2);
			Assert.AreEqual(new WinPoint(0, 2), Utilities.PointOrthogonal(p1, p2));

			// 2    
			//     
			// 1 x      
			//
			// 0       X      
			//  -2 -1  0
			var p3 = new WinPoint(-2, 1);
			Assert.AreEqual(new WinPoint(-2, 0), Utilities.PointOrthogonal(p1, p3));
		}
	}
}