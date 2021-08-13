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
			Assert.IsTrue(ls1.ContainsPoint(new Point(-1, 0)));
			Assert.IsTrue(ls1.ContainsPoint(new Point(1, 0)));

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
			var ls0 = new LineSegment(new Point(-2, 0), new Point(2, 0));
			var ps0 = new List<Point>();
			var result0 = ls0.SplitAtPoints(ps0);
			Assert.AreEqual(result0.Count, 1);
			Assert.IsTrue(result0.Contains(ls0));

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
			var ps4 = new List<Point> { new Point(-4, 2), new Point(0, 0) };
			var result4 = ls4.SplitAtPoints(ps4);
			Assert.IsTrue(result4.Contains(new LineSegment(new Point(-4, 2), new Point(0, 0))));
			Assert.IsTrue(result4.Contains(new LineSegment(new Point(0, 0), new Point(4, -2))));

			var ls42 = new LineSegment(new Point(-4, 2), new Point(4, -2));
			var ps42 = new List<Point> { new Point(4, -2), new Point(0, 0) };
			var result42 = ls42.SplitAtPoints(ps42);
			Assert.IsTrue(result42.Contains(new LineSegment(new Point(-4, 2), new Point(0, 0))));
			Assert.IsTrue(result42.Contains(new LineSegment(new Point(0, 0), new Point(4, -2))));

			var ls5 = new LineSegment(new Point(-4, 2), new Point(4, -2));
			var ps5 = new List<Point> { new Point(-2, 1), new Point(0, 0), new Point(2, -2), new Point(2, -1), new Point(0, 0) };
			Assert.Throws<ArgumentException>(() => ls5.SplitAtPoints(ps5));
		}

		[Test]
		public void TestDoOverlap_X()
		{
			// 
			//  x-----x
			//  x-----x
			//
			var ls1 = new LineSegment(new Point(-2, 0), new Point(2, 0));
			var ls2 = new LineSegment(new Point(-2, 0), new Point(2, 0)); 
			Assert.IsTrue(LineSegment.DoOverlap(ls1, ls2));

			// 
			//     x---x
			//  x----------x
			//
			var ls3 = new LineSegment(new Point(-1, 0), new Point(1, 0));
			Assert.IsTrue(LineSegment.DoOverlap(ls1, ls3));

			// 
			//  x---------x
			//    x-----x
			//
			Assert.IsTrue(LineSegment.DoOverlap(ls3, ls1));

			//
			// x--------x
			//     x-------x
			//
			var ls4 = new LineSegment(new Point(-4, 0), new Point(1, 0));
			Assert.IsTrue(LineSegment.DoOverlap(ls1, ls4));

			//
			//    x-------x
			//  x------x
			//
			Assert.IsTrue(LineSegment.DoOverlap(ls4, ls1));

			//
			// x------x
			// x---------x
			//
			var ls5 = new LineSegment(new Point(-2, 0), new Point(1, 0));
			Assert.IsTrue(LineSegment.DoOverlap(ls1, ls5));

			//
			// x--------x
			// x-----x
			//
			Assert.IsTrue(LineSegment.DoOverlap(ls5, ls1));

			//
			// x------x
			//    x---x
			//
			var ls7 = new LineSegment(new Point(-1, 0), new Point(2, 0));
			Assert.IsTrue(LineSegment.DoOverlap(ls1, ls7));

			//
			//    x---x
			// x------x
			//
			Assert.IsTrue(LineSegment.DoOverlap(ls7, ls1));

			// case with no overlap
			var ls8 = new LineSegment(new Point(100, 100), new Point(200, 200));
			Assert.IsFalse(LineSegment.DoOverlap(ls8, ls1));
			Assert.IsFalse(LineSegment.DoOverlap(ls1, ls8));

			//
			// x------x
			//        x-----x
			//
			var ls9 = new LineSegment(new Point(2, 0), new Point(3, 0));
			Assert.IsFalse(LineSegment.DoOverlap(ls9, ls1));
		}

		[Test]
		public void TestDoOverlap_Y() // test along the Y axis
		{
			var ls1 = new LineSegment(new Point(0, -2), new Point(0, 2));
			var ls2 = new LineSegment(new Point(0, -2), new Point(0, 2));
			Assert.IsTrue(LineSegment.DoOverlap(ls1, ls2));

			var ls3 = new LineSegment(new Point(0, -1), new Point(0, 1));
			Assert.IsTrue(LineSegment.DoOverlap(ls1, ls3));
			Assert.IsTrue(LineSegment.DoOverlap(ls3, ls1));

			var ls4 = new LineSegment(new Point(0, -4), new Point(0, 1));
			Assert.IsTrue(LineSegment.DoOverlap(ls1, ls4));
			Assert.IsTrue(LineSegment.DoOverlap(ls4, ls1));

			var ls5 = new LineSegment(new Point(0, -2), new Point(0, 1));
			Assert.IsTrue(LineSegment.DoOverlap(ls1, ls5));
			Assert.IsTrue(LineSegment.DoOverlap(ls5, ls1));

			var ls7 = new LineSegment(new Point(0, -1), new Point(0, 2));
			Assert.IsTrue(LineSegment.DoOverlap(ls1, ls7));
			Assert.IsTrue(LineSegment.DoOverlap(ls7, ls1));

			var ls8 = new LineSegment(new Point(100, 100), new Point(200, 200));
			Assert.IsFalse(LineSegment.DoOverlap(ls8, ls1));
			Assert.IsFalse(LineSegment.DoOverlap(ls1, ls8));

			var ls9 = new LineSegment(new Point(0, 2), new Point(0, 3));
			Assert.IsFalse(LineSegment.DoOverlap(ls9, ls1));
		}

		[Test]
		public void TestPointsToSplitIfOverlap_X()
		{
			// 
			//  x-----x
			//  x-----x
			//
			var ls1 = new LineSegment(new Point(-2, 0), new Point(2, 0));
			var ls2 = ls1;
			var result1 = LineSegment.PointsToSplitIfOverlap(ls2, ls1);
			Assert.AreEqual(result1.Count, 0);

			// 
			//     x---x
			//  x----------x
			//
			var ls3 = new LineSegment(new Point(-1, 0), new Point(1, 0));
			var result2 = LineSegment.PointsToSplitIfOverlap(ls1, ls3);
			Assert.AreEqual(result2.Count, 2);
			Assert.IsTrue(result2.Contains(new Point(-1, 0)));
			Assert.IsTrue(result2.Contains(new Point(1, 0)));

			// 
			//  x---------x
			//    x-----x
			//
			var result3 = LineSegment.PointsToSplitIfOverlap(ls3, ls1);
			Assert.AreEqual(result3.Count, 2);
			Assert.IsTrue(result3.Contains(new Point(-1, 0)));
			Assert.IsTrue(result3.Contains(new Point(1, 0)));

			//
			// x--------x
			//     x-------x
			//
			var ls4 = new LineSegment(new Point(-4, 0), new Point(1, 0));
			var result4 = LineSegment.PointsToSplitIfOverlap(ls1, ls4);
			Assert.AreEqual(result4.Count, 2);
			Assert.IsTrue(result4.Contains(new Point(-2, 0)));
			Assert.IsTrue(result4.Contains(new Point(1, 0)));

			//
			//    x-------x
			//  x------x
			//
			var result5 = LineSegment.PointsToSplitIfOverlap(ls4, ls1);
			Assert.AreEqual(result5.Count, 2);
			Assert.IsTrue(result5.Contains(new Point(-2, 0)));
			Assert.IsTrue(result5.Contains(new Point(1, 0)));

			//
			// x------x
			// x---------x
			//
			var ls5 = new LineSegment(new Point(-2, 0), new Point(1, 0));
			var result6 = LineSegment.PointsToSplitIfOverlap(ls1, ls5);
			Assert.AreEqual(result6.Count, 1);
			Assert.IsTrue(result6.Contains(new Point(1, 0)));

			//
			// x--------x
			// x-----x
			//
			var result7 = LineSegment.PointsToSplitIfOverlap(ls5, ls1);
			Assert.AreEqual(result7.Count, 1);
			Assert.IsTrue(result7.Contains(new Point(1, 0)));

			//
			// x------x
			//    x---x
			//
			var ls7 = new LineSegment(new Point(-1, 0), new Point(2, 0));
			var result8 = LineSegment.PointsToSplitIfOverlap(ls1, ls7);
			Assert.AreEqual(result8.Count, 1);
			Assert.IsTrue(result8.Contains(new Point(-1, 0)));

			// case with no overlap
			var ls8 = new LineSegment(new Point(100, 100), new Point(200, 200));
			var result9 = LineSegment.PointsToSplitIfOverlap(ls1, ls8);
			Assert.AreEqual(result9.Count, 0);

			//
			// x------x
			//        x-----x
			//
			var ls9 = new LineSegment(new Point(2, 0), new Point(3, 0));
			var result10 = LineSegment.PointsToSplitIfOverlap(ls1, ls9);
			Assert.AreEqual(result10.Count, 0);
		}
	
		[Test]
		public void TestPointsToSplitIfOverlap_Y() // test along the Y axis
		{
			var ls1 = new LineSegment(new Point(0, -2), new Point(0, 2));
			var ls2 = ls1;
			var result1 = LineSegment.PointsToSplitIfOverlap(ls2, ls1);
			Assert.AreEqual(result1.Count, 0);

			var ls3 = new LineSegment(new Point(0, -1), new Point(0, 1));
			var result2 = LineSegment.PointsToSplitIfOverlap(ls1, ls3);
			Assert.AreEqual(result2.Count, 2);
			Assert.IsTrue(result2.Contains(new Point(0, -1)));
			Assert.IsTrue(result2.Contains(new Point(0, 1)));

			var result3 = LineSegment.PointsToSplitIfOverlap(ls3, ls1);
			Assert.AreEqual(result3.Count, 2);
			Assert.IsTrue(result3.Contains(new Point(0, -1)));
			Assert.IsTrue(result3.Contains(new Point(0, 1)));

			var ls4 = new LineSegment(new Point(0, -4), new Point(0, 1));
			var result4 = LineSegment.PointsToSplitIfOverlap(ls1, ls4);
			Assert.AreEqual(result4.Count, 2);
			Assert.IsTrue(result4.Contains(new Point(0, -2)));
			Assert.IsTrue(result4.Contains(new Point(0, 1)));

			var result5 = LineSegment.PointsToSplitIfOverlap(ls4, ls1);
			Assert.AreEqual(result5.Count, 2);
			Assert.IsTrue(result5.Contains(new Point(0, -2)));
			Assert.IsTrue(result5.Contains(new Point(0, 1)));

			var ls5 = new LineSegment(new Point(0, -2), new Point(0, 1));
			var result6 = LineSegment.PointsToSplitIfOverlap(ls1, ls5);
			Assert.AreEqual(result6.Count, 1);
			Assert.IsTrue(result6.Contains(new Point(0, 1)));

			var result7 = LineSegment.PointsToSplitIfOverlap(ls5, ls1);
			Assert.AreEqual(result7.Count, 1);
			Assert.IsTrue(result7.Contains(new Point(0, 1)));

			var ls7 = new LineSegment(new Point(0, -1), new Point(0, 2));
			var result8 = LineSegment.PointsToSplitIfOverlap(ls1, ls7);
			Assert.AreEqual(result8.Count, 1);
			Assert.IsTrue(result8.Contains(new Point(0, -1)));

			var ls8 = new LineSegment(new Point(100, 100), new Point(200, 200));
			var result9 = LineSegment.PointsToSplitIfOverlap(ls1, ls8);
			Assert.AreEqual(result9.Count, 0);

			var ls9 = new LineSegment(new Point(0, 2), new Point(0, 3));
			var result10 = LineSegment.PointsToSplitIfOverlap(ls1, ls9);
			Assert.AreEqual(result10.Count, 0);
		}
	}
}
