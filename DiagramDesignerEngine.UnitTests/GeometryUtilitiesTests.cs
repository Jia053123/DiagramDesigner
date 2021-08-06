using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine.UnitTests
{
	class GeometryUtilitiesTests
	{
		[Test]
		public void TestAngleBetweenConnectedSegments()
		{
			var p1 = new Point(-2, 0);
			var p2 = new Point(0, 0);
			var p3 = new Point(0, 1);

			Assert.AreEqual(GeometryUtilities.AngleBetweenConnectedSegments(p1, p2, p3), Math.PI * 0.5);
			Assert.AreEqual(GeometryUtilities.AngleBetweenConnectedSegments(p3, p2, p1), Math.PI * 1.5);
			Assert.AreEqual(GeometryUtilities.AngleBetweenConnectedSegments(p3, p2, p3), 0);

			var p4 = new Point(2, 0);
			var p5 = new Point(0, 0);
			var p6 = new Point(0, 1.5);

			Assert.AreEqual(GeometryUtilities.AngleBetweenConnectedSegments(p4, p5, p6), Math.PI * 1.5);
			Assert.AreEqual(GeometryUtilities.AngleBetweenConnectedSegments(p6, p5, p4), Math.PI * 0.5);
			Assert.AreEqual(GeometryUtilities.AngleBetweenConnectedSegments(p6, p5, p3), 0);

			Assert.AreEqual(GeometryUtilities.AngleBetweenConnectedSegments(p1, p2, p4), Math.PI);
		}
	}
}
