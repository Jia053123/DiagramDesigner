using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine.UnitTests
{
	class GeometryUtilitiesTests
	{
		[Test]
		public void TestAngleAmongThreePoints()
		{
			var p1 = new Point(-2, 0);
			var p2 = new Point(0, 0);
			var p3 = new Point(0, 1);

			Assert.AreEqual(SegmentsUtilities.AngleAmongThreePoints(p1, p2, p3), Math.PI * 0.5);
			Assert.AreEqual(SegmentsUtilities.AngleAmongThreePoints(p3, p2, p1), Math.PI * 1.5);
			Assert.AreEqual(SegmentsUtilities.AngleAmongThreePoints(p3, p2, p3), 0);

			var p4 = new Point(2, 0);
			var p5 = new Point(0, 0);
			var p6 = new Point(0, 1.5);

			Assert.AreEqual(SegmentsUtilities.AngleAmongThreePoints(p4, p5, p6), Math.PI * 1.5);
			Assert.AreEqual(SegmentsUtilities.AngleAmongThreePoints(p6, p5, p4), Math.PI * 0.5);
			Assert.AreEqual(SegmentsUtilities.AngleAmongThreePoints(p6, p5, p3), 0);

			Assert.AreEqual(SegmentsUtilities.AngleAmongThreePoints(p1, p2, p4), Math.PI);
		}

		[Test]
		public void TestAngleAmongTwoSegments()
		{
			var p0 = new Point(0, 0);
			var p1 = new Point(2, 0);
			var p2 = new Point(-2, 0);
			var p3 = new Point(0, 1);
			var p4 = new Point(0, -1.5);

			var ls1 = new LineSegment(p1, p0);
			var ls2 = new LineSegment(p2, p0);
			var ls3 = new LineSegment(p3, p0);
			var ls4 = new LineSegment(p4, p0);

			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls1, ls2), Math.PI);
			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls2, ls1), Math.PI);

			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls1, ls3), Math.PI * 1.5);
			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls3, ls1), Math.PI * 0.5);

			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls1, ls4), Math.PI * 0.5);
			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls4, ls1), Math.PI * 1.5);

			Assert.AreEqual(SegmentsUtilities.AngleAmongTwoSegments(ls1, ls1), 0);
		}
	}
}
