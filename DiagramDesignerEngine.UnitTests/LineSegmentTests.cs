using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using DiagramDesignerEngine;
using System.Diagnostics;

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
			Assert.Throws<ArgumentException>(() => new LineSegment(p1, p2));
			Assert.Throws<ArgumentException>(() => new LineSegment(p4, p4));
			Assert.DoesNotThrow(() => new LineSegment(p1, p3));

			var ls1 = new LineSegment(p3, p1);
			Assert.AreEqual(ls1.FirstPoint, p1);
			Assert.AreEqual(ls1.SecondPoint, p3);

			var p5 = new Point(1, -1);
			var ls2 = new LineSegment(p5, p1);
			Assert.AreEqual(ls2.FirstPoint, p1);
			Assert.AreEqual(ls2.SecondPoint, p5);

			var ls3 = new LineSegment(p3, p4);
			Assert.AreEqual(ls3.FirstPoint, p3);
			Assert.AreEqual(ls3.SecondPoint, p4);
		}

		[Test]
		public void TestEquality()
		{
			var ls1 = new LineSegment(new Point(1, 2), new Point(3, 4));
			var ls2 = new LineSegment(new Point(1, 2), new Point(3, 4));
			var ls3 = new LineSegment(new Point(1, 2), new Point(2, 4));
			var ls4 = new LineSegment(new Point(1, 1), new Point(3, 4));
			var ls5 = new LineSegment(new Point(1, -1), new Point(4, 4));

			Assert.AreEqual(ls1, ls1);
			Assert.AreEqual(ls1, ls2);
			Assert.AreNotEqual(ls1, ls3);
			Assert.AreNotEqual(ls1, ls4);
			Assert.AreNotEqual(ls1, ls5);

			Assert.IsTrue(ls1 == ls2);
			Assert.IsTrue(ls1 != ls3);
			Assert.IsTrue(ls1 != ls4);
			Assert.IsTrue(ls1 != ls5);

			Assert.IsTrue(ls1.Equals(ls1));
			Assert.IsTrue(ls1.Equals(ls2));
			Assert.IsFalse(ls1.Equals(ls3));
			Assert.IsFalse(ls1.Equals(ls4));
			Assert.IsFalse(ls1.Equals(ls5));

			Assert.AreEqual(ls1.GetHashCode(), ls2.GetHashCode());
			Assert.AreNotEqual(ls1.GetHashCode(), ls3.GetHashCode());
			Assert.AreNotEqual(ls1.GetHashCode(), ls4.GetHashCode());
			Assert.AreNotEqual(ls1.GetHashCode(), ls5.GetHashCode());
		}

		[Test]
		public void TestFindIntersection()
		{
			var ls1 = new LineSegment(new Point(-1, 0), new Point(1, 0));
			var ls2 = new LineSegment(new Point(0, -1), new Point(0, 1));
			Assert.AreEqual(ls1.FindIntersection(ls2), new Point(0, 0));
			Assert.AreEqual(ls2.FindIntersection(ls1), new Point(0, 0));

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

		[Test]
		public void TestSplitIfOverlap_X()
		{
			var ls1 = new LineSegment(new Point(-2, 0), new Point(2, 0));
			var ls2 = ls1;
			var result1 = LineSegment.SplitIfOverlap(ls2, ls1);
			Assert.AreEqual(result1.Count, 1);
			Assert.AreEqual(result1[0], ls1);

			var ls3 = new LineSegment(new Point(-1, 0), new Point(1, 0));
			var result2 = LineSegment.SplitIfOverlap(ls1, ls3);
			TestContext.WriteLine(result2[0]);
			TestContext.WriteLine(result2[1]);
			Assert.AreEqual(result2.Count, 3);
			Assert.IsTrue(result2.Contains(new LineSegment(new Point(-2, 0), new Point(-1, 0))));
			Assert.IsTrue(result2.Contains(new LineSegment(new Point(-1, 0), new Point(1, 0))));
			Assert.IsTrue(result2.Contains(new LineSegment(new Point(1, 0), new Point(2, 0))));

			var result3 = LineSegment.SplitIfOverlap(ls3, ls1);
			Assert.AreEqual(result3.Count, 3);
			Assert.IsTrue(result3.Contains(new LineSegment(new Point(-2, 0), new Point(-1, 0))));
			Assert.IsTrue(result3.Contains(new LineSegment(new Point(-1, 0), new Point(1, 0))));
			Assert.IsTrue(result3.Contains(new LineSegment(new Point(1, 0), new Point(2, 0))));

			var ls4 = new LineSegment(new Point(-4, 0), new Point(1, 0));
			var result4 = LineSegment.SplitIfOverlap(ls1, ls4);
			Assert.AreEqual(result4.Count, 3);
			Assert.IsTrue(result4.Contains(new LineSegment(new Point(-4, 0), new Point(-2, 0))));
			Assert.IsTrue(result4.Contains(new LineSegment(new Point(-2, 0), new Point(1, 0))));
			Assert.IsTrue(result4.Contains(new LineSegment(new Point(1, 0), new Point(2, 0))));

			var result5 = LineSegment.SplitIfOverlap(ls4, ls1);
			Assert.AreEqual(result5.Count, 3);
			Assert.IsTrue(result5.Contains(new LineSegment(new Point(-4, 0), new Point(-2, 0))));
			Assert.IsTrue(result5.Contains(new LineSegment(new Point(-2, 0), new Point(1, 0))));
			Assert.IsTrue(result5.Contains(new LineSegment(new Point(1, 0), new Point(2, 0))));

			var ls5 = new LineSegment(new Point(-2, 0), new Point(1, 0));
			var result6 = LineSegment.SplitIfOverlap(ls1, ls5);
			Assert.AreEqual(result6.Count, 2);
			Assert.IsTrue(result6.Contains(new LineSegment(new Point(-2, 0), new Point(1, 0))));
			Assert.IsTrue(result6.Contains(new LineSegment(new Point(1, 0), new Point(2, 0))));

			var result7 = LineSegment.SplitIfOverlap(ls5, ls1);
			Assert.AreEqual(result7.Count, 2);
			Assert.IsTrue(result7.Contains(new LineSegment(new Point(-2, 0), new Point(1, 0))));
			Assert.IsTrue(result7.Contains(new LineSegment(new Point(1, 0), new Point(2, 0))));

			var ls7 = new LineSegment(new Point(-1, 0), new Point(2, 0));
			var result8 = LineSegment.SplitIfOverlap(ls1, ls7);
			Assert.AreEqual(result8.Count, 2);
			Assert.IsTrue(result8.Contains(new LineSegment(new Point(-2, 0), new Point(-1, 0))));
			Assert.IsTrue(result8.Contains(new LineSegment(new Point(-1, 0), new Point(2, 0))));
		}
	}
}
