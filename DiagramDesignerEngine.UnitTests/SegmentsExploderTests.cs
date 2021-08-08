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

			var exploder1 = new SegmentsExploder(segments1);
			var result1 = exploder1.ExplodeSegments();

			foreach (LineSegment ls in result1)
			{
				TestContext.WriteLine(ls.ToString());
			}

			Assert.AreEqual(result1.Count, 3);
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-3, 0), new Point(-2, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-2, 0), new Point(-1, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-1, 0), new Point(1, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(1, 0), new Point(3, 0))));
		}

			[Test]
		public void TestExplodeSegments_2()
		{
			// 
			//      |
			//   -------x----x-------
			//      |     
			// -3   -2 -1    1       3 
			//
			var ls1 = new LineSegment(new Point(-1, 0), new Point(1, 0));
			var ls2 = new LineSegment(new Point(-3, 0), new Point(3, 0));
			var ls3 = new LineSegment(new Point(-2, -1), new Point(-2, 1));
			var segments1 = new List<LineSegment> { ls1, ls3, ls2 };

			var exploder1 = new SegmentsExploder(segments1);
			var result1 = exploder1.ExplodeSegments();
			Assert.AreEqual(result1.Count, 6);
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-3, 0), new Point(-2, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-2, -1), new Point(-2, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-2, 1), new Point(-2, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-2, 0), new Point(-1, 0))));
			Assert.IsTrue(result1.Contains(new LineSegment(new Point(-1, 0), new Point(1, 0))));
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

			var exploder1 = new SegmentsExploder(segments1);
			var result1 = exploder1.ExplodeSegments();
			Assert.AreEqual(result1.Count, 10);
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
	}
}
