using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine.UnitTests
{
	class DiagramFragmentTests
	{
		[Test]
		public void TestConstructor_0()
		{
			//  
			//    ___________
			//   |           |
			//   |           |
			//   |           |
			//   |           |
			//   |           |
			//   |___________| 
			//
			//   -1    0     1    
			//

			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, -2));
			var ls4 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4 };

			var content = new List<LineSegment>();

			Assert.DoesNotThrow(() => new DiagramFragment(new CycleOfLineSegments(perimeter), content));
		}

		[Test]
		public void TestConstructor_1()
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

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5 };

			var ls6 = new LineSegment(new Point(1, 0), new Point(0.5, 0.5));
			var ls7 = new LineSegment(new Point(1, 0), new Point(0.5, -0.5));
			var ls8 = new LineSegment(new Point(0.5, -0.5), new Point(0.5, 0.5));

			var content = new List<LineSegment> { ls6, ls7, ls8 };

			Assert.DoesNotThrow(() => new DiagramFragment(new CycleOfLineSegments(perimeter), content));
		}

		[Test]
		public void TestConstructor_2()
		{
			//       
			//        ___________ 
			//       |           |
			//       |           |
			//       |         / | \
			//       |      /____|____\ 
			//       |           |
			//       |           |
			//       |           |
			//       |___________|
			//
			//       -1    0     1    2
			//

			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, -2));
			var ls4 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4 };

			var ls6 = new LineSegment(new Point(0, 0), new Point(2, 0));
			var ls7 = new LineSegment(new Point(0, 0), new Point(1, 1));
			var ls8 = new LineSegment(new Point(2, 0), new Point(1, 1));

			var content = new List<LineSegment> { ls6, ls7, ls8 };

			Assert.Throws<ArgumentException>(() => new DiagramFragment(new CycleOfLineSegments(perimeter), content));
		}

		[Test]
		public void TestConstructor_3()
		{
			//       
			//        ___________ 
			//       |           |
			//       |           |
			//       |           |
			//       |    / \    | 
			//       |   /   \   |
			//       |  /     \  |
			//       | /       \ |
			//        ===========
			//
			//       -1    0     1    
			//

			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, -2));
			var ls4 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4 };

			var ls6 = new LineSegment(new Point(-1, -2), new Point(0, 0));
			var ls7 = new LineSegment(new Point(1, -2), new Point(0, 0));
			var ls8 = new LineSegment(new Point(-1, -2), new Point(1, -2));

			var content = new List<LineSegment> { ls6, ls7, ls8 };

			Assert.Throws<ArgumentException>(() => new DiagramFragment(new CycleOfLineSegments(perimeter), content));
		}

		[Test]
		public void TestConstructor_4()
		{
			//       
			//        ___________ 
			//       |           |
			//       |           |
			//       |           |
			//       |    /|     | 
			//       |   / |     |
			//       |  /  |     |
			//       | /   |     |
			//        ======-----
			//
			//       -1    0     1    
			//

			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, -2));
			var ls4 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4 };

			var ls6 = new LineSegment(new Point(-1, -2), new Point(0, 0));
			var ls7 = new LineSegment(new Point(0, -2), new Point(0, 0));
			var ls8 = new LineSegment(new Point(-1, -2), new Point(0, -2));

			var content = new List<LineSegment> { ls6, ls7, ls8 };

			Assert.Throws<ArgumentException>(() => new DiagramFragment(new CycleOfLineSegments(perimeter), content));
		}

		[Test]
		public void TestConstructor_5()
		{
			//       
			//        ___________ 
			//       |           |
			//       |           |
			//       |           |
			//       |    /      | 
			//       |   /       |
			//       |  /        |
			//       | /         |
			//       |___________|
			//
			//       -1    0     1    
			//

			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, -2));
			var ls4 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4 };

			var ls6 = new LineSegment(new Point(-1, -2), new Point(0, 0));
			
			var content = new List<LineSegment> { ls6 };

			Assert.Throws<ArgumentException>(() => new DiagramFragment(new CycleOfLineSegments(perimeter), content));
		}

		[Test]
		public void TestDivideIntoSmallerFragments_1()
		{
			//       
			//        ___________ 
			//       |           |
			//       |           |
			//       |           |
			//       |           |
			//       |___________|
			//
			//       -1    0     1    
			//
			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, -2));
			var ls4 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4 };
			var cycle = new CycleOfLineSegments(perimeter);
			var fragment = new DiagramFragment(cycle, new List<LineSegment>());
			var result = fragment.DivideIntoSmallerFragments();
			Assert.IsNull(result);
		}

		[Test]
		public void TestDivideIntoSmallerFragments_2()
		{
			//       
			//        ___________ 
			//       |           |
			//       |           |
			//    0  |-----------|
			//       |           |
			//       |___________|
			//
			//       -1    0     1    
			//
			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, -2));
			var ls4 = new LineSegment(new Point(1, -2), new Point(-1, -2));

			var ls5 = new LineSegment(new Point(-1, 0), new Point(1, 0));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4 };
			var content = new List<LineSegment> { ls5 };

			var cycle = new CycleOfLineSegments(perimeter);
			var fragment = new DiagramFragment(cycle, content);
			var result = fragment.DivideIntoSmallerFragments();
			Assert.AreEqual(2, result.Count);
			
		}
	}
}
