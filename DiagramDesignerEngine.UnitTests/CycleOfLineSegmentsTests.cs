using BasicGeometries;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine.UnitTests
{
	class CycleOfLineSegmentsTests
	{
		[Test]
		public void TestConstructor_0()
		{
			var ls1 = new LineSegment(new Point(0, 0), new Point(1, 1));
			var ls2 = new LineSegment(new Point(0, 0), new Point(1, 1));
			var ls3 = new LineSegment(new Point(1, 1), new Point(1, -1));
			var ls4 = new LineSegment(new Point(1, -1), new Point(0, 0));

			var segments1 = new List<LineSegment> { ls1, ls3 };
			Assert.Throws<ArgumentException>(() => new CycleOfLineSegments(segments1));

			var segments2 = new List<LineSegment> { ls1, ls2, ls3, ls4 };
			Assert.Throws<ArgumentException>(() => new CycleOfLineSegments(segments2));

			var segments3 = new List<LineSegment> { ls1, ls2 };
			Assert.Throws<ArgumentException>(() => new CycleOfLineSegments(segments3));

			var segments4 = new List<LineSegment> { ls1, ls3, ls4 };
			Assert.DoesNotThrow(() => new CycleOfLineSegments(segments4));
		}

		[Test]
		public void TestConstructor_1()
		{
			//   ____        
			//  |     \        /|   
			//  |       \    /  |    
			//    \       \/    |
			//      \___________|
			//
			// -2   -1    0     1      
			//

			var ls1 = new LineSegment(new Point(0, 0), new Point(1, 1));
			var ls2 = new LineSegment(new Point(1, 1), new Point(1, -0.5));
			var ls3 = new LineSegment(new Point(1, -0.5), new Point(-1, -0.5));
			var ls4 = new LineSegment(new Point(-1, -0.5), new Point(-2, 0.5));
			var ls5 = new LineSegment(new Point(-2, 0.5), new Point(-2, 1));
			var ls6 = new LineSegment(new Point(-2, 1), new Point(-1, 1));
			var ls7 = new LineSegment(new Point(-1, 1), new Point(0, 0));

			var segments1 = new List<LineSegment> { ls1, ls7, ls2, ls6, ls3, ls5, ls4 };
			Assert.DoesNotThrow(() => new CycleOfLineSegments(segments1));
		}

		[Test]
		public void TestConstructor_2()
		{
			//   ____        
			//  |     \        /|   
			//  |       \    /  |    
			//    \       \/    |
			//      \___________x______
			//
			// -2   -1    0     1      2
			//

			var ls1 = new LineSegment(new Point(0, 0), new Point(1, 1));
			var ls2 = new LineSegment(new Point(1, 1), new Point(1, -0.5));
			var ls3 = new LineSegment(new Point(1, -0.5), new Point(-1, -0.5));
			var ls4 = new LineSegment(new Point(-1, -0.5), new Point(-2, 0.5));
			var ls5 = new LineSegment(new Point(-2, 0.5), new Point(-2, 1));
			var ls6 = new LineSegment(new Point(-2, 1), new Point(-1, 1));
			var ls7 = new LineSegment(new Point(-1, 1), new Point(0, 0));
			var ls8 = new LineSegment(new Point(1, -0.5), new Point(2, -0.5));

			var segments1 = new List<LineSegment> { ls1, ls7, ls2, ls6, ls3, ls5, ls4, ls8 };
			Assert.Throws<ArgumentException>(() => new CycleOfLineSegments(segments1));
		}

		[Test]
		public void TestConstructor_3()
		{
			//   ____        
			//  |     \        /|   
			//  |       \    /  |    
			//    \       \/    |
			//      \   /    \  |
			//
			// -2   -1    0     1      2
			//

			var ls1 = new LineSegment(new Point(0, 0), new Point(1, 1));
			var ls2 = new LineSegment(new Point(1, 1), new Point(1, -0.5));
			var ls3 = new LineSegment(new Point(1, -0.5), new Point(0, 0));
			var ls4 = new LineSegment(new Point(0, 0), new Point(-1, -0.5));
			var ls5 = new LineSegment(new Point(-1, -0.5), new Point(-2, 0.5));
			var ls6 = new LineSegment(new Point(-2, 0.5), new Point(-2, 1));
			var ls7 = new LineSegment(new Point(-2, 1), new Point(-1, 1));
			var ls8 = new LineSegment(new Point(-1, 1), new Point(0, 0));

			var segments = new List<LineSegment> { ls1, ls7, ls2, ls6, ls3, ls5, ls4, ls8 };
			Assert.Throws<ArgumentException>(() => new CycleOfLineSegments(segments));
		}

		[Test]
		public void TestConstructor_4()
		{
			//  
			//    ___________
			//   |           |
			//   |       |\  |
			//   |       |  \x 
			//   |       |  /|
			//   |       |/  |
			//   |___________| 
			//
			//   -1    0     1    
			//

			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, 0));
			var ls4 = new LineSegment(new Point(1, 0), new Point(1, -2));
			var ls5 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var ls6 = new LineSegment(new Point(1, 0), new Point(0.5, 0.5));
			var ls7 = new LineSegment(new Point(1, 0), new Point(0.5, -0.5));
			var ls8 = new LineSegment(new Point(0.5, -0.5), new Point(0.5, 0.5));

			var segments = new List<LineSegment> { ls1, ls7, ls2, ls6, ls3, ls5, ls4, ls8 };
			Assert.Throws<ArgumentException>(() => new CycleOfLineSegments(segments));
		}

		[Test]
		public void TestConstructor_5()
		{
			//  
			//  2   ___________
			//  1  |           |
			//     | /|        |
			//  0  x/ |        | 
			//     |\ |        |
			// -1  | \|        |
			// -2  |___________| 
			//
			//   -1    0     1    
			//

			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 0));
			var ls2 = new LineSegment(new Point(-1, 0), new Point(-1, 2));
			var ls3 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls4 = new LineSegment(new Point(1, 2), new Point(1, -2));
			var ls5 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var ls6 = new LineSegment(new Point(-1, 0), new Point(-0.5, 1));
			var ls7 = new LineSegment(new Point(-1, 0), new Point(-0.5, -1));
			var ls8 = new LineSegment(new Point(-0.5, -1), new Point(-0.5, 1));

			var segments = new List<LineSegment> { ls1, ls7, ls2, ls6, ls3, ls5, ls4, ls8 };
			Assert.Throws<ArgumentException>(() => new CycleOfLineSegments(segments));
		}

		[Test]
		public void TestConstructor_6()
		{
			//  
			//    ___________
			//   |           |
			//   |     |\    |
			//   |     |  \  x 
			//   |     |  /  |
			//   |     |/    |
			//   |___________| 
			//
			//   -1    0     1    
			//

			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, 0));
			var ls4 = new LineSegment(new Point(1, 0), new Point(1, -2));
			var ls5 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var ls6 = new LineSegment(new Point(0.5, 0), new Point(0, 0.5));
			var ls7 = new LineSegment(new Point(0.5, 0), new Point(0, -0.5));
			var ls8 = new LineSegment(new Point(0, -0.5), new Point(0, 0.5));

			var segments = new List<LineSegment> { ls1, ls7, ls2, ls6, ls3, ls5, ls4, ls8 };
			Assert.Throws<ArgumentException>(() => new CycleOfLineSegments(segments));
		}

		[Test]
		public void TestIsLineSegmentInCycle_1()
		{
			//  
			//    ___________
			//   |           |
			//   |     |\    |
			//   |     |  \  x 
			//   |     |  /  |
			//   |     |/    |
			//   |___________| 
			//
			//   -1    0     1    
			//

			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, 0));
			var ls4 = new LineSegment(new Point(1, 0), new Point(1, -2));
			var ls5 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var segments = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5 };
			var cycle = new CycleOfLineSegments(segments);

			var ls6 = new LineSegment(new Point(0.5, 0), new Point(0, 0.5));
			var ls7 = new LineSegment(new Point(0.5, 0), new Point(0, -0.5));
			var ls8 = new LineSegment(new Point(0, -0.5), new Point(0, 0.5));

			Assert.IsTrue(cycle.IsLineSegmentInCycle(ls6));
			Assert.IsTrue(cycle.IsLineSegmentInCycle(ls7));
			Assert.IsTrue(cycle.IsLineSegmentInCycle(ls8));
		}

		[Test]
		public void TestIsLineSegmentInCycle_2()
		{
			//  
			//    ___________
			//   |           |
			//   |       |\  |
			//   |       |  \x 
			//   |       |  /|
			//   |       |/  |
			//   |___________| 
			//
			//   -1    0     1    
			//

			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, 0));
			var ls4 = new LineSegment(new Point(1, 0), new Point(1, -2));
			var ls5 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var segments = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5 };
			var cycle = new CycleOfLineSegments(segments);

			var ls6 = new LineSegment(new Point(1, 0), new Point(0.5, 0.5));
			var ls7 = new LineSegment(new Point(1, 0), new Point(0.5, -0.5));
			var ls8 = new LineSegment(new Point(0.5, -0.5), new Point(0.5, 0.5));

			Assert.IsTrue(cycle.IsLineSegmentInCycle(ls6));
			Assert.IsTrue(cycle.IsLineSegmentInCycle(ls7));
			Assert.IsTrue(cycle.IsLineSegmentInCycle(ls8));
		}

		[Test]
		public void TestIsLineSegmentInCycle_3()
		{
			//       |
			//       |
			//  _____|___________   ___
			//       x           |
			//       |           |
			//       |           |
			//       |      _____x_____ 
			//       |           |
			//       |           |
			//       |           |
			//       |___________x_____
			//
			//       -1    0     1    
			//

			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, -2));
			var ls4 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var segments = new List<LineSegment> { ls1, ls2, ls3, ls4 };
			var cycle = new CycleOfLineSegments(segments);

			var ls61 = new LineSegment(new Point(0, 0), new Point(1, 0));
			var ls62 = new LineSegment(new Point(1, 0), new Point(2, 0));
			var ls7 = new LineSegment(new Point(1, -2), new Point(2, -2));
			var ls8 = new LineSegment(new Point(1.5, 2), new Point(2, 2));
			var ls9 = new LineSegment(new Point(-2, 2), new Point(-1, 2));
			var ls10 = new LineSegment(new Point(-2, 2), new Point(-2, 3));

			Assert.IsTrue(cycle.IsLineSegmentInCycle(ls61));
			Assert.IsFalse(cycle.IsLineSegmentInCycle(ls62));
			Assert.IsFalse(cycle.IsLineSegmentInCycle(ls7));
			Assert.IsFalse(cycle.IsLineSegmentInCycle(ls8));
			Assert.IsFalse(cycle.IsLineSegmentInCycle(ls9));
			Assert.IsFalse(cycle.IsLineSegmentInCycle(ls10));
		}

		[Test]
		public void TestIsLineSegmentInCycle_4()
		{
			//       
			//        ___________ 
			//       |           |
			//       |           |
			//    0  x-----------x
			//       |           |
			//       |___________|
			//
			//       -1    0     1    
			//
			var ls1 = new LineSegment(new Point(-1, 0), new Point(-1, 1));
			var ls2 = new LineSegment(new Point(-1, 0), new Point(-1, -1));
			var ls3 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls4 = new LineSegment(new Point(1, 0), new Point(1, 1));
			var ls5 = new LineSegment(new Point(1, 0), new Point(1, -1));
			var ls6 = new LineSegment(new Point(1, -1), new Point(-1, -1));

			var ls7 = new LineSegment(new Point(-1, 0), new Point(1, 0));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6 };

			var cycle = new CycleOfLineSegments(perimeter);
			Assert.IsTrue(cycle.IsLineSegmentInCycle(ls7));
		}

		[Test]
		public void TestIsLineSegmentInCycle_5()
		{
			//       
			//        _____x_____ 
			//       |     |     |
			//       |     |     |
			//    0  |     |     |
			//       |     |     |
			//       |_____x_____|
			//
			//       -1    0     1    
			//
			var ls1 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls2 = new LineSegment(new Point(-1, 1), new Point(0, 1));
			var ls3 = new LineSegment(new Point(1, 1), new Point(0, 1));
			var ls4 = new LineSegment(new Point(1, 1), new Point(1, -1));
			var ls5 = new LineSegment(new Point(1, -1), new Point(0, -1));
			var ls6 = new LineSegment(new Point(-1, -1), new Point(0, -1));

			var ls7 = new LineSegment(new Point(0, -1), new Point(0, 1));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6 };
			var cycle = new CycleOfLineSegments(perimeter);
			Assert.IsTrue(cycle.IsLineSegmentInCycle(ls7));
		}

		[Test]
		public void TestIsLineSegmentInCycle_6()
		{
			//   ____ x_________x       
			//  |     \        /|   
			//  |       \    /  |    
			//    \       \/    |
			//      \___________|
			//
			// -2   -1    0     1      
			//

			var ls1 = new LineSegment(new Point(0, 0), new Point(1, 1));
			var ls2 = new LineSegment(new Point(1, 1), new Point(1, -0.5));
			var ls3 = new LineSegment(new Point(1, -0.5), new Point(-1, -0.5));
			var ls4 = new LineSegment(new Point(-1, -0.5), new Point(-2, 0.5));
			var ls5 = new LineSegment(new Point(-2, 0.5), new Point(-2, 1));
			var ls6 = new LineSegment(new Point(-2, 1), new Point(-1, 1));
			var ls7 = new LineSegment(new Point(-1, 1), new Point(0, 0));

			var ls8 = new LineSegment(new Point(-1, 1), new Point(1, 1));

			var segments1 = new List<LineSegment> { ls1, ls7, ls2, ls6, ls3, ls5, ls4 };
			var cycle = new CycleOfLineSegments(segments1);
			Assert.IsFalse(cycle.IsLineSegmentInCycle(ls8));
		}

		[Test]
		public void TestIsLineSegmentInCycle_7()
		{
			//   2    
			//                         
			//   1   x__________________x
			//       |\                /|
			//   0   |   \          /   |
			//       |      \    /      |
			//  -1   |        \/        |
			//       |                  |
			//  -2   |__________________|
			//
			//       -2   -1   0   1    2
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 1));
			var ls2 = new LineSegment(new Point(-2, 1), new Point(0, -1));
			var ls3 = new LineSegment(new Point(0, -1), new Point(2, 1));
			var ls4 = new LineSegment(new Point(2, 1), new Point(2, -2));
			var ls5 = new LineSegment(new Point(2, -2), new Point(-2, -2));
			var perimeter = new List<LineSegment> { ls5, ls1, ls4, ls2, ls3 };
			
			var ls6 = new LineSegment(new Point(-2, 1), new Point(2, 1));

			var cycle = new CycleOfLineSegments(perimeter);
			Assert.IsFalse(cycle.IsLineSegmentInCycle(ls6));
		}

		[Test]
		public void TestIsLineSegmentInCycle_8()
		{
			//   2          ____________
			//             |            |
			//   1    _____x____________x 
			//       |                  |
			//   0   |                  |
			//       |                  |
			//  -1   |                  |
			//       |                  |
			//  -2   |__________________|
			//
			//       -2   -1   0   1    2
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 1));
			var ls2 = new LineSegment(new Point(-2, 1), new Point(-1, 1));
			var ls3 = new LineSegment(new Point(-1, 1), new Point(-1, 2));
			var ls4 = new LineSegment(new Point(-1, 2), new Point(2, 2));
			var ls5 = new LineSegment(new Point(2, 2), new Point(2, 1));
			var ls6 = new LineSegment(new Point(2, 1), new Point(2, -2));
			var ls7 = new LineSegment(new Point(2, -2), new Point(-2, -2));
			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6, ls7 };

			var ls8 = new LineSegment(new Point(-1, 1), new Point(2, 1));

			var cycle = new CycleOfLineSegments(perimeter);
			Assert.IsTrue(cycle.IsLineSegmentInCycle(ls8));
		}

		[Test]
		public void TestIsLineSegmentInCycle_9()
		{
			//       
			//        ___________ 
			//       |           |
			//       |           |
			//    0  |           |
			//       |           |
			//       |===========|
			//
			//       -1    0     1    
			//
			var ls1 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls2 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls3 = new LineSegment(new Point(1, 1), new Point(1, -1));
			var ls4 = new LineSegment(new Point(1, -1), new Point(-1, -1));

			var ls5 = new LineSegment(new Point(-1, -1), new Point(1, -1));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4 };
			var cycle = new CycleOfLineSegments(perimeter);
			Assert.IsFalse(cycle.IsLineSegmentInCycle(ls5));
		}
	}
}
