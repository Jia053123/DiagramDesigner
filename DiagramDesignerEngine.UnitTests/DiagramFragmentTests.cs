using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine.UnitTests
{
	class DiagramFragmentTests
	{
		[Test]
		public void TestConstructor_0()
		{
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

			var segsWithin = new List<LineSegment>();

			Assert.DoesNotThrow(() => new DiagramFragment(new CycleOfLineSegments(perimeter), new List<CycleOfLineSegments>(), segsWithin));
		}

		[Test]
		public void TestConstructor_1()
		{
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

			var segsWithin = new List<LineSegment> { ls6, ls7, ls8 };

			Assert.DoesNotThrow(() => new DiagramFragment(new CycleOfLineSegments(perimeter), new List<CycleOfLineSegments>(), segsWithin));
		}

		[Test]
		public void TestConstructor_2()
		{
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

			var segsWithin = new List<LineSegment> { ls6, ls7, ls8 };

			Assert.Throws<ArgumentException>(() => new DiagramFragment(new CycleOfLineSegments(perimeter), new List<CycleOfLineSegments>(), segsWithin));
		}

		[Test]
		public void TestConstructor_3()
		{
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

			var segsWithin = new List<LineSegment> { ls6, ls7, ls8 };

			Assert.Throws<ArgumentException>(() => new DiagramFragment(new CycleOfLineSegments(perimeter), new List<CycleOfLineSegments>(), segsWithin));
		}

		[Test]
		public void TestConstructor_4()
		{
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

			var segsWithin = new List<LineSegment> { ls6, ls7, ls8 };

			Assert.Throws<ArgumentException>(() => new DiagramFragment(new CycleOfLineSegments(perimeter), new List<CycleOfLineSegments>(), segsWithin));
		}

		[Test]
		public void TestConstructor_5()
		{
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
			
			var segsWithin = new List<LineSegment> { ls6 };

			Assert.Throws<ArgumentException>(() => new DiagramFragment(new CycleOfLineSegments(perimeter), new List<CycleOfLineSegments>(), segsWithin));
		}

		[Test]
		public void TestConstructor_6()
		{
			//       
			//        __________________ 
			//       |                  |
			//       |     ________     |
			//       |    |        |    | (an inner perimeter)
			//       |    |        |    | 
			//       |    |        |    |
			//       |    |________|    |
			//       |                  |
			//       |__________________|
			//
			//       -2   -1   0   1    2
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 2));
			var ls2 = new LineSegment(new Point(-2, 2), new Point(2, 2));
			var ls3 = new LineSegment(new Point(2, 2), new Point(2, -2));
			var ls4 = new LineSegment(new Point(2, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4 });

			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls6 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls7 = new LineSegment(new Point(1, 1), new Point(1, -1));
			var ls8 = new LineSegment(new Point(1, -1), new Point(-1, -1));
			var innerPerimeter1 = new CycleOfLineSegments(new List<LineSegment> { ls5, ls6, ls7, ls8 });

			var segsWithin = new List<LineSegment>();

			Assert.DoesNotThrow(() => new DiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1 }, segsWithin));
		}

		[Test]
		public void TestConstructor_7()
		{
			//       
			//        __________________ 
			//       |                  |
			//       |     _____________|____
			//       |    |             |    | (an inner perimeter)
			//       |    |             |    |
			//       |    |             |    |
			//       |    |_____________|____|
			//       |                  |
			//       |__________________|
			//
			//       -2   -1   0   1    2    3
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 2));
			var ls2 = new LineSegment(new Point(-2, 2), new Point(2, 2));
			var ls3 = new LineSegment(new Point(2, 2), new Point(2, -2));
			var ls4 = new LineSegment(new Point(2, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4 });

			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls6 = new LineSegment(new Point(-1, 1), new Point(3, 1));
			var ls7 = new LineSegment(new Point(3, 1), new Point(3, -1));
			var ls8 = new LineSegment(new Point(3, -1), new Point(-1, -1));
			var innerPerimeter1 = new CycleOfLineSegments(new List<LineSegment> { ls5, ls6, ls7, ls8 });

			var segsWithin = new List<LineSegment>();

			Assert.Throws<ArgumentException>(() => new DiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1 }, segsWithin));
		}

		[Test]
		public void TestConstructor_8()
		{
			//       
			//        __________________ 
			//       |                  |
			//       |     ____________ | 
			//       |    |            ||   (an inner perimeter)
			//       |    |            ||    
			//       |    |            ||    
			//       |    |____________||
			//       |                  |
			//       |__________________|
			//
			//       -2   -1   0   1    2    3
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 2));
			var ls2 = new LineSegment(new Point(-2, 2), new Point(2, 2));
			var ls3 = new LineSegment(new Point(2, 2), new Point(2, -2));
			var ls4 = new LineSegment(new Point(2, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4 });

			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls6 = new LineSegment(new Point(-1, 1), new Point(2, 1));
			var ls7 = new LineSegment(new Point(2, 1), new Point(2, -1));
			var ls8 = new LineSegment(new Point(2, -1), new Point(-1, -1));
			var innerPerimeter1 = new CycleOfLineSegments(new List<LineSegment> { ls5, ls6, ls7, ls8 });

			var segsWithin = new List<LineSegment>();

			Assert.Throws<ArgumentException>(() => new DiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1 }, segsWithin));
		}

		[Test]
		public void TestConstructor_9()
		{
			//       
			//        __________________ 
			//       |                  |
			//       |     ____     ____|____
			//       |    |    |   |    |    |  (both are inner perimeters) 
			//       |    |    |   |    |    |
			//       |    |    |   |    |    |
			//       |    |____|   |____|____|
			//       |                  |
			//       |__________________|
			//
			//       -2   -1   0   1    2    3
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 2));
			var ls2 = new LineSegment(new Point(-2, 2), new Point(2, 2));
			var ls3 = new LineSegment(new Point(2, 2), new Point(2, -2));
			var ls4 = new LineSegment(new Point(2, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4 });

			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls6 = new LineSegment(new Point(-1, 1), new Point(0, 1));
			var ls7 = new LineSegment(new Point(0, 1), new Point(0, -1));
			var ls8 = new LineSegment(new Point(0, -1), new Point(-1, -1));
			var innerPerimeter1 = new CycleOfLineSegments(new List<LineSegment> { ls5, ls6, ls7, ls8 });

			var ls9 = new LineSegment(new Point(1, -1), new Point(1, 1));
			var ls10 = new LineSegment(new Point(1, 1), new Point(3, 1));
			var ls11 = new LineSegment(new Point(3, 1), new Point(3, -1));
			var ls12 = new LineSegment(new Point(3, -1), new Point(1, -1));
			var innerPerimeter2 = new CycleOfLineSegments(new List<LineSegment> { ls9, ls10, ls11, ls12 });

			var segsWithin = new List<LineSegment>();

			Assert.Throws<ArgumentException>(() => new DiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1, innerPerimeter2 }, segsWithin));
		}

		[Test]
		public void TestConstructor_10()
		{
			//       
			//        _______________________ 
			//       |                       |
			//       |     _______  ____     |
			//       |    |       ||    |    | (both are inner perimeters)
			//       |    |       ||    |    |
			//       |    |       ||    |    |
			//       |    |_______||____|    |
			//       |                       |
			//       |_______________________|
			//
			//       -2   -1   0   1    2    3
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 2));
			var ls2 = new LineSegment(new Point(-2, 2), new Point(3, 2));
			var ls3 = new LineSegment(new Point(3, 2), new Point(3, -2));
			var ls4 = new LineSegment(new Point(3, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4 });

			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls6 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls7 = new LineSegment(new Point(1, 1), new Point(1, -1));
			var ls8 = new LineSegment(new Point(1, -1), new Point(-1, -1));
			var innerPerimeter1 = new CycleOfLineSegments(new List<LineSegment> { ls5, ls6, ls7, ls8 });

			var ls9 = new LineSegment(new Point(1, -1), new Point(1, 1));
			var ls10 = new LineSegment(new Point(1, 1), new Point(2, 1));
			var ls11 = new LineSegment(new Point(2, 1), new Point(2, -1));
			var ls12 = new LineSegment(new Point(2, -1), new Point(1, -1));
			var innerPerimeter2 = new CycleOfLineSegments(new List<LineSegment> { ls9, ls10, ls11, ls12 });

			var segsWithin = new List<LineSegment>();

			Assert.Throws<ArgumentException>(() => new DiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1, innerPerimeter2 }, segsWithin));
		}

		[Test]
		public void TestConstructor_11_NestedInnerPerimeter()
		{
			//       
			//        _______________________ 
			//       |                       |
			//       |     ________          |
			//       |    |   ___  |         | (both are inner perimeters)
			//       |    |  |   | |         |
			//       |    |  |___| |         |
			//       |    |________|         |
			//       |                       |
			//       |_______________________|
			//
			//       -2   -1   0   1    2    3
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 2));
			var ls2 = new LineSegment(new Point(-2, 2), new Point(3, 2));
			var ls3 = new LineSegment(new Point(3, 2), new Point(3, -2));
			var ls4 = new LineSegment(new Point(3, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4 });

			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls6 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls7 = new LineSegment(new Point(1, 1), new Point(1, -1));
			var ls8 = new LineSegment(new Point(1, -1), new Point(-1, -1));
			var innerPerimeter1 = new CycleOfLineSegments(new List<LineSegment> { ls5, ls6, ls7, ls8 });

			var ls9 = new LineSegment(new Point(-0.5, -0.5), new Point(-0.5, 0.5));
			var ls10 = new LineSegment(new Point(-0.5, 0.5), new Point(0.5, 0.5));
			var ls11 = new LineSegment(new Point(0.5, 0.5), new Point(0.5, -0.5));
			var ls12 = new LineSegment(new Point(0.5, -0.5), new Point(-0.5, -0.5));
			var innerPerimeter2 = new CycleOfLineSegments(new List<LineSegment> { ls9, ls10, ls11, ls12 });

			var segsWithin = new List<LineSegment>();

			Assert.Throws<ArgumentException>(() => new DiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1, innerPerimeter2 }, segsWithin));
		}

		[Test]
		public void TestConstructor_12()
		{
			//       
			//   2    _______________________ 
			//       |                       |
			//   1   |     ________          |
			//  0.5  |    |     ___|___      | (both are inner perimeters)
			//   0   |    |    |   |   |     | 
			// -0.5  |    |    |___|___|     |    
			//  -1   |    |________|         |
			//       |                       |
			//  -2   |_______________________|
			//
			//       -2   -1   0   1    2    3
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 2));
			var ls2 = new LineSegment(new Point(-2, 2), new Point(3, 2));
			var ls3 = new LineSegment(new Point(3, 2), new Point(3, -2));
			var ls4 = new LineSegment(new Point(3, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4 });

			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls6 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls7 = new LineSegment(new Point(1, 1), new Point(1, -1));
			var ls8 = new LineSegment(new Point(1, -1), new Point(-1, -1));
			var innerPerimeter1 = new CycleOfLineSegments(new List<LineSegment> { ls5, ls6, ls7, ls8 });

			var ls9 = new LineSegment(new Point(0, -0.5), new Point(0, 0.5));
			var ls10 = new LineSegment(new Point(0, 0.5), new Point(2, 0.5));
			var ls11 = new LineSegment(new Point(2, 0.5), new Point(2, -0.5));
			var ls12 = new LineSegment(new Point(2, -0.5), new Point(0, -0.5));
			var innerPerimeter2 = new CycleOfLineSegments(new List<LineSegment> { ls9, ls10, ls11, ls12 });

			var segsWithin = new List<LineSegment>();

			Assert.Throws<ArgumentException>(() => new DiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1, innerPerimeter2 }, segsWithin));
		}

		[Test]
		public void TestConstructor_13()
		{
			//       
			//        _______________________ 
			//       |                       |
			//       |     ____     ____     |
			//       |    |    |   |    |    | (both are inner perimeters)
			//       |    |    |   |    |    |
			//       |    |    |   |    |    |
			//       |    |____|   |____|    |
			//       |                       |
			//       |_______________________|
			//
			//       -2   -1   0   1    2    3
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 2));
			var ls2 = new LineSegment(new Point(-2, 2), new Point(3, 2));
			var ls3 = new LineSegment(new Point(3, 2), new Point(3, -2));
			var ls4 = new LineSegment(new Point(3, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4 });

			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls6 = new LineSegment(new Point(-1, 1), new Point(0, 1));
			var ls7 = new LineSegment(new Point(0, 1), new Point(0, -1));
			var ls8 = new LineSegment(new Point(0, -1), new Point(-1, -1));
			var innerPerimeter1 = new CycleOfLineSegments(new List<LineSegment> { ls5, ls6, ls7, ls8 });

			var ls9 = new LineSegment(new Point(1, -1), new Point(1, 1));
			var ls10 = new LineSegment(new Point(1, 1), new Point(2, 1));
			var ls11 = new LineSegment(new Point(2, 1), new Point(2, -1));
			var ls12 = new LineSegment(new Point(2, -1), new Point(1, -1));
			var innerPerimeter2 = new CycleOfLineSegments(new List<LineSegment> { ls9, ls10, ls11, ls12 });

			var segsWithin = new List<LineSegment>();

			Assert.DoesNotThrow(() => new DiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1, innerPerimeter2 }, segsWithin));
		}

		[Test]
		public void TestConstructor_14()
		{
			//       
			//        _______________________ 
			//       |                       |
			//       |     ____     ____     |
			//       |    |    |   |    |    |
			//       |    |    |   |    |    |
			//       |    |    |   |    |    |
			//       |    |____|   |____|    |
			//       |    (peri) (not peri)  |
			//       |_______________________|
			//
			//       -2   -1   0   1    2    3
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 2));
			var ls2 = new LineSegment(new Point(-2, 2), new Point(3, 2));
			var ls3 = new LineSegment(new Point(3, 2), new Point(3, -2));
			var ls4 = new LineSegment(new Point(3, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4 });

			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls6 = new LineSegment(new Point(-1, 1), new Point(0, 1));
			var ls7 = new LineSegment(new Point(0, 1), new Point(0, -1));
			var ls8 = new LineSegment(new Point(0, -1), new Point(-1, -1));
			var innerPerimeter = new CycleOfLineSegments(new List<LineSegment> { ls5, ls6, ls7, ls8 });

			var ls9 = new LineSegment(new Point(1, -1), new Point(1, 1));
			var ls10 = new LineSegment(new Point(1, 1), new Point(2, 1));
			var ls11 = new LineSegment(new Point(2, 1), new Point(2, -1));
			var ls12 = new LineSegment(new Point(2, -1), new Point(1, -1));
			var segsWithin = new List<LineSegment> { ls9, ls10, ls11, ls12 };

			Assert.Throws<ArgumentException>(() => new DiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter }, segsWithin));
		}

		[Test]
		public void TestConstructor_15_ThisIsWhyCannotHaveInnerPerimetersAlongWithOtherSegments()
		{
			//       
			//   2    _______________________ 
			//       |                       |
			//   1   |____x____x___x____x____|
			//       |    |    |   |    |    |
			//   0   |    |    |   |    |    |
			//       |    |    |   |    |    |
			//  -1   |    |____|   |____|    |
			//       |    (peri)   (peri)    |
			//  -2   |_______________________|
			//
			//       -2   -1   0   1    2    3
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 2));
			var ls2 = new LineSegment(new Point(-2, 2), new Point(3, 2));
			var ls3 = new LineSegment(new Point(3, 2), new Point(3, -2));
			var ls4 = new LineSegment(new Point(3, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4 });

			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls6 = new LineSegment(new Point(-1, 1), new Point(0, 1));
			var ls7 = new LineSegment(new Point(0, 1), new Point(0, -1));
			var ls8 = new LineSegment(new Point(0, -1), new Point(-1, -1));
			var innerPerimeter1 = new CycleOfLineSegments(new List<LineSegment> { ls5, ls6, ls7, ls8 });

			var ls9 = new LineSegment(new Point(1, -1), new Point(1, 1));
			var ls10 = new LineSegment(new Point(1, 1), new Point(2, 1));
			var ls11 = new LineSegment(new Point(2, 1), new Point(2, -1));
			var ls12 = new LineSegment(new Point(2, -1), new Point(1, -1));
			var innerPerimeter2 = new CycleOfLineSegments(new List<LineSegment> { ls9, ls10, ls11, ls12 });

			var ls13 = new LineSegment(new Point(-2, 1), new Point(-1, 1));
			var ls14 = new LineSegment(new Point(0, 1), new Point(1, 1));
			var ls15 = new LineSegment(new Point(2, 1), new Point(3, 1));
			var segmentsWithin = new List<LineSegment> { ls13, ls14, ls15 };

			Assert.Throws<ArgumentException>(() => new DiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1, innerPerimeter2 }, segmentsWithin));
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
			var ls1 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls2 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls3 = new LineSegment(new Point(1, 1), new Point(1, -1));
			var ls4 = new LineSegment(new Point(1, -1), new Point(-1, -1));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4 };
			var cycle = new CycleOfLineSegments(perimeter);
			var fragment = new DiagramFragment(cycle, new List<CycleOfLineSegments>(), new List<LineSegment>());
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
			//    0  x-----------x
			//       |           |
			//       |___________|
			//
			//       -1    0     1    
			//
			var ls1 = new LineSegment(new Point(-1, -1), new Point(-1, 0));
			var ls2 = new LineSegment(new Point(-1, 0), new Point(-1, 1));
			var ls3 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls4 = new LineSegment(new Point(1, 1), new Point(1, 0));
			var ls5 = new LineSegment(new Point(1, 0), new Point(1, -1));
			var ls6 = new LineSegment(new Point(1, -1), new Point(-1, -1));

			var ls7 = new LineSegment(new Point(-1, 0), new Point(1, 0));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6 };
			var segsWithin = new List<LineSegment> { ls7 };

			var cycle = new CycleOfLineSegments(perimeter);
			var fragment = new DiagramFragment(cycle, new List<CycleOfLineSegments>(), segsWithin);
			var result = fragment.DivideIntoSmallerFragments();
			Assert.AreEqual(2, result.Count);

			var expectedPerimeter1 = new List<LineSegment> { ls2, ls3, ls4, ls7 };
			var expectedPerimeter2 = new List<LineSegment> { ls1, ls6, ls5, ls7 };
			var guess1 = (ListUtilities.AreContentsEqual(result[0].GetPerimeter().GetPerimeter(), expectedPerimeter1) &&
				ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), expectedPerimeter2));
			var guess2 = (ListUtilities.AreContentsEqual(result[0].GetPerimeter().GetPerimeter(), expectedPerimeter2) &&
				ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), expectedPerimeter1));
			Assert.IsTrue(guess1 || guess2);

			Assert.AreEqual(0, result[0].GetInnerPerimeters().Count);
			Assert.AreEqual(0, result[1].GetInnerPerimeters().Count);

			Assert.AreEqual(0, result[0].GetSegmentsWithin().Count);
			Assert.AreEqual(0, result[1].GetSegmentsWithin().Count);
		}

		[Test]
		public void TestDivideIntoSmallerFragments_3()
		{
			//       
			//   1    ___________ 
			//       |           |
			//  0.5  x-----------x
			//   0   |   _____   |
			// -0.5  |  |_____|  |  (not an inner perimeter)
			//       |___________|
			//  -1
			//       -1    0     1    
			//
			var ls1 = new LineSegment(new Point(-1, -1), new Point(-1, 0.5));
			var ls2 = new LineSegment(new Point(-1, 0.5), new Point(-1, 1));
			var ls3 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls4 = new LineSegment(new Point(1, 1), new Point(1, 0.5));
			var ls5 = new LineSegment(new Point(1, 0.5), new Point(1, -1));
			var ls6 = new LineSegment(new Point(1, -1), new Point(-1, -1));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6 });

			var ls7 = new LineSegment(new Point(-1, 0.5), new Point(1, 0.5));

			var ls8 = new LineSegment(new Point(-0.5, -0.5), new Point(-0.5, 0));
			var ls9 = new LineSegment(new Point(-0.5, 0), new Point(0.5, 0));
			var ls10 = new LineSegment(new Point(0.5, 0), new Point(0.5, -0.5));
			var ls11 = new LineSegment(new Point(0.5, -0.5), new Point(-0.5, -0.5));

			var segsWithin = new List<LineSegment> { ls7, ls8, ls9, ls10, ls11 };

			var fragment = new DiagramFragment(perimeter, new List<CycleOfLineSegments>(), segsWithin);
			var result = fragment.DivideIntoSmallerFragments();
			Assert.AreEqual(2, result.Count);

			var expectedPerimeter1 = new List<LineSegment> { ls2, ls3, ls4, ls7 };
			var expectedPerimeter2 = new List<LineSegment> { ls1, ls6, ls5, ls7 };
			var guess1 = (ListUtilities.AreContentsEqual(result[0].GetPerimeter().GetPerimeter(), expectedPerimeter1) &&
				ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), expectedPerimeter2));
			var guess2 = (ListUtilities.AreContentsEqual(result[0].GetPerimeter().GetPerimeter(), expectedPerimeter2) &&
				ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), expectedPerimeter1));
			Assert.IsTrue(guess1 || guess2);

			Assert.AreEqual(0, result[0].GetInnerPerimeters().Count);
			Assert.AreEqual(0, result[1].GetInnerPerimeters().Count);

			if (guess1)
			{
				Assert.AreEqual(0, result[0].GetSegmentsWithin().Count);
				Assert.AreEqual(4, result[1].GetSegmentsWithin().Count);
			}
			else if (guess2)
			{
				Assert.AreEqual(4, result[0].GetSegmentsWithin().Count);
				Assert.AreEqual(0, result[1].GetSegmentsWithin().Count);
			}
		}

		[Test]
		public void TestDivideIntoSmallerFragments_4()
		{
			//        __________________ 
			//       |                  |
			//       |     ________     |
			//       |    |        |    |
			//       |    |        |    | 
			//       |    |        |    |
			//       |    |________|    |
			//       |                  |
			//       |__________________|
			//
			//       -2   -1   0   1    2
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 2));
			var ls2 = new LineSegment(new Point(-2, 2), new Point(2, 2));
			var ls3 = new LineSegment(new Point(2, 2), new Point(2, -2));
			var ls4 = new LineSegment(new Point(2, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4 });

			var ls5 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls6 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls7 = new LineSegment(new Point(1, 1), new Point(1, -1));
			var ls8 = new LineSegment(new Point(1, -1), new Point(-1, -1));
			var segsWithin = new List<LineSegment> { ls5, ls6, ls7, ls8 };


			var fragment = new DiagramFragment(perimeter, new List<CycleOfLineSegments>(), segsWithin);
			var result = fragment.DivideIntoSmallerFragments();
			Assert.AreEqual(2, result.Count);

			var expectedPerimeter1 = new List<LineSegment> { ls1, ls2, ls3, ls4 };
			var expectedInnerPerimeter1 = new List<LineSegment> { ls5, ls6, ls7, ls8 };
			var expectedPerimeter2 = new List<LineSegment> { ls5, ls6, ls7, ls8 };

			var guess1 = (ListUtilities.AreContentsEqual(result[0].GetPerimeter().GetPerimeter(), expectedPerimeter1) &&
				ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), expectedPerimeter2));
			var guess2 = (ListUtilities.AreContentsEqual(result[0].GetPerimeter().GetPerimeter(), expectedPerimeter2) &&
				ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), expectedPerimeter1));
			Assert.IsTrue(guess1 || guess2);

			Assert.AreEqual(0, result[0].GetSegmentsWithin().Count);
			Assert.AreEqual(0, result[1].GetSegmentsWithin().Count);
			if (guess1)
			{
				Assert.AreEqual(1, result[0].GetInnerPerimeters().Count);
				ListUtilities.AreContentsEqual(result[0].GetInnerPerimeters().First().GetPerimeter(), expectedInnerPerimeter1);
				Assert.AreEqual(0, result[1].GetInnerPerimeters().Count);
			}
			else if (guess2)
			{
				Assert.AreEqual(0, result[0].GetInnerPerimeters().Count);
				Assert.AreEqual(1, result[1].GetInnerPerimeters().Count);
				ListUtilities.AreContentsEqual(result[1].GetInnerPerimeters().First().GetPerimeter(), expectedInnerPerimeter1);
			}
		}
	}
}
