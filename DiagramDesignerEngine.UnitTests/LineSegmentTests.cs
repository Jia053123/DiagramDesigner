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
			//Assert.Throws<ArgumentException>(() => new LineSegment(null, p1));
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

			//Assert.Throws<ArgumentException>(() => ls1.FindIntersection(null));

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

		[Test]
		public void TestContainsPoint()
		{
			var ls1 = new LineSegment(new Point(-1, 0), new Point(1, 0));
			Assert.IsTrue(ls1.ContainsPoint(new Point(0, 0)));
			Assert.IsFalse(ls1.ContainsPoint(new Point(0, 1)));
			Assert.IsFalse(ls1.ContainsPoint(new Point(-1, 0)));
			Assert.IsFalse(ls1.ContainsPoint(new Point(1, 0)));
			
			var ls2 = new LineSegment(new Point(0, -1), new Point(0, 1));
			Assert.IsTrue(ls2.ContainsPoint(new Point(0, 0)));
			Assert.IsFalse(ls2.ContainsPoint(new Point(1, 0)));

			var ls3 = new LineSegment(new Point(-1, -2), new Point(1, 2));
			Assert.IsTrue(ls3.ContainsPoint(new Point(0, 0)));
			Assert.IsFalse(ls3.ContainsPoint(new Point(1, 1)));
		}

		[Test]
		public void TestSplitAtPoints()
		{
			var ls1 = new LineSegment(new Point(-2, 0), new Point(2, 0));
			var ps1 = new List<Point> { new Point(-1, 0), new Point(1, 0), new Point(0, 0) };
			var result1 = ls1.SplitAtPoints(ps1);
			Assert.AreEqual(result1.Count, 4);

			var ls2 = new LineSegment(new Point(0, 2), new Point(0, -2));
			var ps2 = new List<Point> { new Point(0, 1), new Point(0, -1), new Point(0, 0) };
			var result2 = ls2.SplitAtPoints(ps2);
			Assert.AreEqual(result2.Count, 4);

			var ls3 = new LineSegment(new Point(-4, 2), new Point(4, -2));
			var ps3 = new List<Point> { new Point(-2, 1), new Point(0, 0), new Point(2, -1), new Point(0, 0) };
			var result3 = ls3.SplitAtPoints(ps3);
			Assert.AreEqual(result3.Count, 4);
			Assert.IsTrue(result3.Contains(new LineSegment(new Point(-4, 2), new Point(-2, 1))));
			Assert.IsTrue(result3.Contains(new LineSegment(new Point(-2, 1), new Point(0, 0))));
			Assert.IsTrue(result3.Contains(new LineSegment(new Point(0, 0), new Point(2, -1))));
			Assert.IsTrue(result3.Contains(new LineSegment(new Point(2, -1), new Point(4, -2))));

			var ls4 = new LineSegment(new Point(-4, 2), new Point(4, -2));
			var ps4 = new List<Point> { new Point(-2, 1), new Point(0, 0), new Point(2, -2), new Point(2, -1), new Point(0, 0) };
			Assert.Throws<ArgumentException>(() => ls4.SplitAtPoints(ps4));
		}
	}
}
