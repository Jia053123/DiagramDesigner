using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine.UnitTests
{
	class SegmentsExploderTests
	{
		[Test]
		public void TestExplodeSegments_1()
		{
			// 
			//       ---------
			//          ------
			//   ----x--x----x-------
			//          
			// -3   -2 -1    1       3 
			//
			var ls1 = new LineSegment(new Point(-1, 0), new Point(1, 0));
			var ls2 = new LineSegment(new Point(-3, 0), new Point(3, 0));
			var ls3 = new LineSegment(new Point(-2, 0), new Point(1, 0));
			var segments1 = new List<LineSegment> { ls1, ls2, ls3};

			var exploder1 = new LineSegmentsExploder(segments1);
			var result1 = exploder1.MergeAndExplodeSegments();
			Assert.AreEqual(result1.Count, 4);
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-3, 0), new Point(-2, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-2, 0), new Point(-1, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-1, 0), new Point(1, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(1, 0), new Point(3, 0))));
		}

		[Test]
		public void TestExplodeSegments_2()
		{
			// 
			//            |
			//   -------x----x-------
			//            |     
			// -3      -1 0  1       3 
			//
			var ls1 = new LineSegment(new Point(-1, 0), new Point(1, 0));
			var ls2 = new LineSegment(new Point(-3, 0), new Point(3, 0));
			var ls3 = new LineSegment(new Point(0, -1), new Point(0, 1));
			var segments1 = new List<LineSegment> { ls1, ls3, ls2 };

			var exploder1 = new LineSegmentsExploder(segments1);
			var result1 = exploder1.MergeAndExplodeSegments();
			Assert.AreEqual(result1.Count, 6);
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-3, 0), new Point(-1, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-1, 0), new Point(0, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(0, -1), new Point(0, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(0, 1), new Point(0, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(0, 0), new Point(1, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(1, 0), new Point(3, 0))));
		}

		[Test]	
		public void TestExplodeSegments_3()
		{
			//  
			//      |        |       | |
			//   -------x----x-------- |
			//      |        |       | |
			// -3   -2 -1    1       3 4
			//
			var ls1 = new LineSegment(new Point(-1, 0), new Point(1, 0));
			var ls2 = new LineSegment(new Point(-3, 0), new Point(3, 0));
			var ls3 = new LineSegment(new Point(-2, -1), new Point(-2, 1));
			var ls4 = new LineSegment(new Point(1, -1), new Point(1, 1));
			var ls5 = new LineSegment(new Point(3, -1), new Point(3, 1));
			var ls6 = new LineSegment(new Point(4, -1), new Point(4, 1));
			var segments1 = new List<LineSegment> { ls1, ls3, ls2, ls4, ls5, ls6 };

			var exploder1 = new LineSegmentsExploder(segments1);
			var result1 = exploder1.MergeAndExplodeSegments();
			Assert.AreEqual(result1.Count, 11);
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-3, 0), new Point(-2, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-2, -1), new Point(-2, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-2, 1), new Point(-2, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-2, 0), new Point(-1, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-1, 0), new Point(1, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(1, -1), new Point(1, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(1, 1), new Point(1, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(1, 0), new Point(3, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(3, -1), new Point(3, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(3, 0), new Point(3, 1))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(4, -1), new Point(4, 1))));
		}

		[Test]
		public void TestExplideSegments_4()
		{
			//4
			//3  |
			//   x\
			//2  |  \
			//   |  /
			//1  x/
			//0  | 
			//   0  1
			var ls1 = new LineSegment(new Point(0, 0), new Point(0, 4));
			var ls2 = new LineSegment(new Point(0, 1), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(0, 3));
			var segments1 = new List<LineSegment> { ls1, ls2, ls3 };
			var exploder1 = new LineSegmentsExploder(segments1);
			var result1 = exploder1.MergeAndExplodeSegments();
			Assert.AreEqual(result1.Count, 5);
			Assert.IsTrue(result1.Contains(ls2));
			Assert.IsTrue(result1.Contains(ls3));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(0, 4), new Point(0, 3))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(0, 3), new Point(0, 1))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(0, 1), new Point(0, 0))));
		}

		[Test]
		public void TestExplideSegments_5()
		{
			//
			//  |
			//  x\
			//  |  \
			//  |  /
			//  x/
			//  | 
			var ls1 = new LineSegment(new Point(49.6, 41.6), new Point(50.2, 70.8));
			var ls2 = new LineSegment(new Point(49.79315357561548, 51.000140679953105), new Point(72.4, 58));
			var ls3 = new LineSegment(new Point(50.072572098475966, 64.59850879249707), new Point(72.4, 58));
			var segments1 = new List<LineSegment> { ls1, ls2, ls3 };
			var exploder1 = new LineSegmentsExploder(segments1);
			var result1 = exploder1.MergeAndExplodeSegments();
			Assert.AreEqual(result1.Count, 5);
			Assert.IsTrue(result1.Contains(ls2));
			Assert.IsTrue(result1.Contains(ls3));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(49.6, 41.6), new Point(49.79315357561548, 51.000140679953105))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(49.79315357561548, 51.000140679953105), new Point(50.072572098475966, 64.59850879249707))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(50.072572098475966, 64.59850879249707), new Point(50.2, 70.8))));
		}
	}
}
