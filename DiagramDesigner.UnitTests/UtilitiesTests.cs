using NUnit.Framework;
using WinPoint = System.Windows.Point;
using Point = DiagramDesignerEngine.Point;

namespace DiagramDesigner.UnitTests
{
	public class UtilitiesTests
	{
		[SetUp]
		public void Setup()
		{
		}

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
	}
}