using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using DiagramDesignerEngine;

namespace DiagramDesignerEngine.UnitTests
{
	class LineSegmentTests
	{
		[Test]
		public void TestConstructor()
		{
			var p1 = new Point(1, -2);
			var p2 = new Point(1, -2);
			var p3 = new Point(2, -1);
			var p4 = new Point(2, 3);
			Assert.Throws<ArgumentException>(() => new LineSegment(null, p1));
			Assert.Throws<ArgumentException>(() => new LineSegment(p1, p2));
			Assert.Throws<ArgumentException>(() => new LineSegment(p4, p4));
			Assert.DoesNotThrow(() => new LineSegment(p1, p3));
		}

		[Test]
		public void TestFindIntersection()
		{
			var ls1 = new LineSegment(new Point(-1, 0), new Point(1, 0));
			var ls2 = new LineSegment(new Point(0, -1), new Point(0, 1));
			Assert.AreEqual(ls1.FindIntersection(ls2), new Point(0, 0));
			Assert.AreEqual(ls2.FindIntersection(ls1), new Point(0, 0));

			Assert.Throws<ArgumentException>(() => ls1.FindIntersection(null));

			var ls3 = new LineSegment(new Point(-2, -1), new Point(0, 3));
			var ls4 = new LineSegment(new Point(1, -1), new Point(-3, 3));
			Assert.AreEqual(ls3.FindIntersection(ls4), new Point(-1, 1));

			// sharing end point
			var ls5 = new LineSegment(new Point(-1, 0), new Point(0, 0));
			var ls6 = new LineSegment(new Point(0, -1), new Point(0, 0));
			Assert.IsNull(ls5.FindIntersection(ls6));

			// T shape
			Assert.AreEqual(ls2.FindIntersection(ls5), new Point(0, 0));

			// parallel
			var ls7 = new LineSegment(new Point(-1, -1), new Point(1, -1));
			Assert.IsNull(ls7.FindIntersection(ls1));

			// not parallel but no intersection
			var ls8 = new LineSegment(new Point(-1, -1.6), new Point(1, -1.2));
			Assert.IsNull(ls1.FindIntersection(ls8));
		}
	}
}
