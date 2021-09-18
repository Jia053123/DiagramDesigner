using BasicGeometries;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine.UnitTests
{
	class FragmentFactoryTests
	{
		[Test]
		public void TestMakeFragment_0()
		{
			//
			// 5      
			//      
			// 3     
			// 2   ____    
			//    |    |  
			// 0  |____|  
			//  
			//   0  1  2  3  4  5	

			var ls1 = new LineSegment(new Point(0, 0), new Point(0, 2));
			var ls2 = new LineSegment(new Point(0, 2), new Point(2, 2));
			var ls3 = new LineSegment(new Point(2, 2), new Point(2, 0));
			var ls4 = new LineSegment(new Point(2, 0), new Point(0, 0));
			var perimeter1 = new List<LineSegment> { ls1, ls2, ls3, ls4 };

			var segments = new List<LineSegment> { ls4, ls3, ls2, ls1};
			var result = FragmentFactory.MakeFragments(segments);
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqual(perimeter1, result[0].GetPerimeter().GetPerimeter()));
			Assert.AreEqual(0, result[0].GetInnerPerimeters().Count);
			Assert.AreEqual(0, result[0].GetSegmentsWithin().Count);
		}

		[Test]
		public void TestMakeFragment_1()
		{
			//
			// 5      ____
			//       |    |
			// 3     |____|  
			// 2   ____    
			//    |    |
			// 0  |____|  
			//  
			//   0  1  2  3  4  5	

			var ls1 = new LineSegment(new Point(0, 0), new Point(0, 2));
			var ls2 = new LineSegment(new Point(0, 2), new Point(2, 2));
			var ls3 = new LineSegment(new Point(2, 2), new Point(2, 0));
			var ls4 = new LineSegment(new Point(2, 0), new Point(0, 0));
			var perimeter1 = new List<LineSegment> { ls1, ls2, ls3, ls4 };

			var ls5 = new LineSegment(new Point(1, 3), new Point(1, 5));
			var ls6 = new LineSegment(new Point(1, 5), new Point(3, 5));
			var ls7 = new LineSegment(new Point(3, 5), new Point(3, 3));
			var ls8 = new LineSegment(new Point(3, 3), new Point(1, 3));
			var perimeter2 = new List<LineSegment> { ls5, ls6, ls7, ls8 };

			var segments = new List<LineSegment> { ls1, ls8, ls2, ls7, ls3, ls6, ls4, ls5 };
			var result = FragmentFactory.MakeFragments(segments);
			Assert.AreEqual(2, result.Count);
			var guess1 = ListUtilities.AreContentsEqual(perimeter1, result[0].GetPerimeter().GetPerimeter()) &&
				ListUtilities.AreContentsEqual(perimeter2, result[1].GetPerimeter().GetPerimeter());
			var guess2 = ListUtilities.AreContentsEqual(perimeter2, result[0].GetPerimeter().GetPerimeter()) &&
				ListUtilities.AreContentsEqual(perimeter1, result[1].GetPerimeter().GetPerimeter());
			Assert.IsTrue(guess1 || guess2);

			Assert.AreEqual(0, result[0].GetInnerPerimeters().Count);
			Assert.AreEqual(0, result[1].GetInnerPerimeters().Count);

			Assert.AreEqual(0, result[0].GetSegmentsWithin().Count);
			Assert.AreEqual(0, result[1].GetSegmentsWithin().Count);
		}

		[Test]
		public void TestMakeFragment_2()
		{
			//
			// 5      ____
			//       | __ |
			// 3     |_\/_|  
			// 2   ____    
			//    | __ |_____
			// 0  |____|  
			//  
			//   0  1  2  3  4  5	

			var ls1 = new LineSegment(new Point(0, 0), new Point(0, 2));
			var ls2 = new LineSegment(new Point(0, 2), new Point(2, 2));
			var ls3 = new LineSegment(new Point(2, 2), new Point(2, 0));
			var ls4 = new LineSegment(new Point(2, 0), new Point(0, 0));
			var perimeter1 = new List<LineSegment> { ls1, ls2, ls3, ls4 };

			var ls5 = new LineSegment(new Point(1, 3), new Point(1, 5));
			var ls6 = new LineSegment(new Point(1, 5), new Point(3, 5));
			var ls7 = new LineSegment(new Point(3, 5), new Point(3, 3));
			var ls8 = new LineSegment(new Point(3, 3), new Point(1, 3));
			var perimeter2 = new List<LineSegment> { ls5, ls6, ls7, ls8 };

			var ls9 = new LineSegment(new Point(2, 3), new Point(1.5, 4));
			var ls10 = new LineSegment(new Point(1.5, 4), new Point(2.5, 4));
			var ls11 = new LineSegment(new Point(2.5, 4), new Point(2, 3));
			var triangle = new List<LineSegment> { ls9, ls10, ls11 };

			var ls12 = new LineSegment(new Point(0.5, 1), new Point(1.5, 1));
			var ls13 = new LineSegment(new Point(2, 1), new Point(4, 1));

			var segments = new List<LineSegment> { ls1, ls8, ls2, ls7, ls3, ls6, ls4, ls5, ls9, ls13, ls10, ls12, ls11 };
			var result = FragmentFactory.MakeFragments(segments);
			Assert.AreEqual(2, result.Count);
			var guess1 = ListUtilities.AreContentsEqual(perimeter1, result[0].GetPerimeter().GetPerimeter()) &&
				ListUtilities.AreContentsEqual(perimeter2, result[1].GetPerimeter().GetPerimeter());
			var guess2 = ListUtilities.AreContentsEqual(perimeter2, result[0].GetPerimeter().GetPerimeter()) &&
				ListUtilities.AreContentsEqual(perimeter1, result[1].GetPerimeter().GetPerimeter());
			Assert.IsTrue(guess1 || guess2);

			if (guess1)
			{
				Assert.AreEqual(0, result[0].GetSegmentsWithin().Count);
				Assert.AreEqual(3, result[1].GetSegmentsWithin().Count);
				Assert.IsTrue(ListUtilities.AreContentsEqual(triangle, result[1].GetSegmentsWithin()));
			} 
			else if (guess2)
			{
				Assert.AreEqual(3, result[0].GetSegmentsWithin().Count);
				Assert.IsTrue(ListUtilities.AreContentsEqual(triangle, result[0].GetSegmentsWithin()));
				Assert.AreEqual(0, result[1].GetSegmentsWithin().Count);
			}

			Assert.AreEqual(0, result[0].GetInnerPerimeters().Count);
			Assert.AreEqual(0, result[1].GetInnerPerimeters().Count);
		}

		[Test]
		public void TestMakeFragment_3_LoopTrap()
		{
			//
			// 5      ____
			//       |    |
			// 3     |____|  
			// 2   ___\    
			//    |    |
			// 0  |____|  
			//  
			//   0  1  2  3  4  5	

			var ls1 = new LineSegment(new Point(0, 0), new Point(0, 2));
			var ls2 = new LineSegment(new Point(0, 2), new Point(2, 2));
			var ls3 = new LineSegment(new Point(2, 2), new Point(2, 0));
			var ls4 = new LineSegment(new Point(2, 0), new Point(0, 0));
			var perimeter1 = new List<LineSegment> { ls1, ls2, ls3, ls4 };

			var ls5 = new LineSegment(new Point(1, 3), new Point(1, 5));
			var ls6 = new LineSegment(new Point(1, 5), new Point(3, 5));
			var ls7 = new LineSegment(new Point(3, 5), new Point(3, 3));
			var ls8 = new LineSegment(new Point(3, 3), new Point(1, 3));
			var perimeter2 = new List<LineSegment> { ls5, ls6, ls7, ls8 };

			var ls9 = new LineSegment(new Point(2, 2), new Point(1, 3));

			var segments = new List<LineSegment> { ls1, ls8, ls2, ls7, ls3, ls6, ls4, ls5, ls9 };
			var result = FragmentFactory.MakeFragments(segments);
			Assert.AreEqual(2, result.Count);
			var guess1 = ListUtilities.AreContentsEqual(perimeter1, result[0].GetPerimeter().GetPerimeter()) &&
				ListUtilities.AreContentsEqual(perimeter2, result[1].GetPerimeter().GetPerimeter());
			var guess2 = ListUtilities.AreContentsEqual(perimeter2, result[0].GetPerimeter().GetPerimeter()) &&
				ListUtilities.AreContentsEqual(perimeter1, result[1].GetPerimeter().GetPerimeter());
			Assert.IsTrue(guess1 || guess2);

			Assert.AreEqual(0, result[0].GetInnerPerimeters().Count);
			Assert.AreEqual(0, result[1].GetInnerPerimeters().Count);

			Assert.AreEqual(0, result[0].GetSegmentsWithin().Count);
			Assert.AreEqual(0, result[1].GetSegmentsWithin().Count);
		}

		[Test]
		public void TestExtractAllFragments_1_InnerLoopButNotInnerPerimeter()
		{
			//   2   x \           x  
			//        \     \     /|
			//   1      \       x  |         
			//            \   /    x\
			//   0          x      |    x
			//            /   \    x/         
			//  -1      /       x  |
			//        /     /    \ |
			//  -2   x /           x     
			//
			//       -2   -1   0   1    2
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-1, 0));
			var ls2 = new LineSegment(new Point(-1, 0), new Point(0, 1));
			var ls3 = new LineSegment(new Point(0, 1), new Point(1, 2));

			var ls4 = new LineSegment(new Point(1, 2), new Point(1, 0.5));
			var ls5 = new LineSegment(new Point(1, 0.5), new Point(1, -0.5));
			var ls6 = new LineSegment(new Point(1, -0.5), new Point(1, -2));

			var ls7 = new LineSegment(new Point(1, -2), new Point(0, -1));
			var ls8 = new LineSegment(new Point(0, -1), new Point(-1, 0));
			var ls9 = new LineSegment(new Point(-1, 0), new Point(-2, 2));

			var ls10 = new LineSegment(new Point(-2, 2), new Point(0, 1));
			var ls11 = new LineSegment(new Point(0, 1), new Point(1, 0.5));
			var ls12 = new LineSegment(new Point(1, 0.5), new Point(2, 0));

			var ls13 = new LineSegment(new Point(2, 0), new Point(1, -0.5));
			var ls14 = new LineSegment(new Point(1, -0.5), new Point(0, -1));
			var ls15 = new LineSegment(new Point(0, -1), new Point(-2, -2));

			var allSegments = new List<LineSegment> { ls15, ls14, ls13, ls12, ls11, ls10, ls9, ls8, ls7, ls6, ls5, ls4, ls3, ls2, ls1 };

			var result = FragmentFactory.ExtractAllFragments(allSegments);
			Assert.AreEqual(6, result.Count);
			foreach (DiagramFragment udf in result)
			{
				Assert.AreEqual(0, udf.GetInnerPerimeters().Count);
				Assert.AreEqual(0, udf.GetSegmentsWithin().Count);
			}
		}
	}
}
