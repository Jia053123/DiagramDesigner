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

			Assert.DoesNotThrow(() => new DividableDiagramFragment(new CycleOfLineSegments(perimeter), new List<LineSegment>()));
			Assert.DoesNotThrow(() => new UndividableDiagramFragment(new CycleOfLineSegments(perimeter), new List<CycleOfLineSegments>()));
		}

		[Test]
		public void TestConstructor_1()
		{
			//  1  ____________
			//    |            |
			//    |        |\  |
			//  0 |        |  \x 
			//    |        |  /|
			//    |        |/  |
			// -1 |____________| 
			//
			//   -1     0     1    
			//
			var ls1 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls2 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls3 = new LineSegment(new Point(1, 1), new Point(1, 0));
			var ls4 = new LineSegment(new Point(1, 0), new Point(1, -1));
			var ls5 = new LineSegment(new Point(1, -1), new Point(-1, -1));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5 };

			var ls6 = new LineSegment(new Point(1, 0), new Point(0.5, 0.5));
			var ls7 = new LineSegment(new Point(1, 0), new Point(0.5, -0.5));
			var ls8 = new LineSegment(new Point(0.5, -0.5), new Point(0.5, 0.5));

			var segsWithin = new List<LineSegment> { ls6, ls7, ls8 };

			Assert.DoesNotThrow(() => new DividableDiagramFragment(new CycleOfLineSegments(perimeter), segsWithin));
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

			Assert.Throws<ArgumentException>(() => new DividableDiagramFragment(new CycleOfLineSegments(perimeter), segsWithin));
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

			Assert.Throws<ArgumentException>(() => new DividableDiagramFragment(new CycleOfLineSegments(perimeter), segsWithin));
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

			Assert.Throws<ArgumentException>(() => new DividableDiagramFragment(new CycleOfLineSegments(perimeter), segsWithin));
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

			Assert.Throws<ArgumentException>(() => new DividableDiagramFragment(new CycleOfLineSegments(perimeter), segsWithin));
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

			Assert.DoesNotThrow(() => new UndividableDiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1 }));
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

			Assert.Throws<ArgumentException>(() => new UndividableDiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1 }));
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

			Assert.Throws<ArgumentException>(() => new UndividableDiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1 }));
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

			Assert.Throws<ArgumentException>(() => new UndividableDiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1, innerPerimeter2 }));
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

			Assert.Throws<ArgumentException>(() => new UndividableDiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1, innerPerimeter2 }));
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

			Assert.Throws<ArgumentException>(() => new UndividableDiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1, innerPerimeter2 }));
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

			Assert.Throws<ArgumentException>(() => new UndividableDiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1, innerPerimeter2 }));
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

			Assert.DoesNotThrow(() => new UndividableDiagramFragment(perimeter, new List<CycleOfLineSegments> { innerPerimeter1, innerPerimeter2 }));
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
			var fragment = new DividableDiagramFragment(cycle, new List<LineSegment>());
			var result = fragment.DivideIntoSmallerFragments();
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result[0] is UndividableDiagramFragment);
			Assert.IsTrue(ListUtilities.AreContentsEqual(perimeter, result[0].GetPerimeter().GetPerimeter()));
			Assert.AreEqual(0, result[0].GetInnerPerimeters().Count);
			Assert.AreEqual(0, result[0].GetSegmentsWithin().Count);

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
			var fragment = new DividableDiagramFragment(cycle, segsWithin);
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

			var fragment = new DividableDiagramFragment(perimeter, segsWithin);
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


			var fragment = new DividableDiagramFragment(perimeter, segsWithin);
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

		[Test]
		public void TestDivideIntoSmallerFragments_5()
		{
			//   2    __________________ 
			//       |                  |
			//   1   |      ______      |
			//       |      \    /      |
			//   0   |        \/        |
			//       |        /\        |
			//  -1   |      /____\      |
			//       |                  |
			//  -2   |__________________|
			//
			//       -2   -1   0   1    2
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 2));
			var ls2 = new LineSegment(new Point(-2, 2), new Point(2, 2));
			var ls3 = new LineSegment(new Point(2, 2), new Point(2, -2));
			var ls4 = new LineSegment(new Point(2, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4 });

			var ls5 = new LineSegment(new Point(-1, -1), new Point(0, 0));
			var ls6 = new LineSegment(new Point(0, 0), new Point(1, -1));
			var ls7 = new LineSegment(new Point(1, -1), new Point(-1, -1));

			var ls8 = new LineSegment(new Point(0, 0), new Point(-1, 1));
			var ls9 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls10 = new LineSegment(new Point(1, 1), new Point(0, 0));

			var segsWithin = new List<LineSegment> { ls5, ls6, ls8, ls7, ls9, ls10 };

			var fragment = new DividableDiagramFragment(perimeter, segsWithin);
			var result = fragment.DivideIntoSmallerFragments();
			Assert.AreEqual(3, result.Count);

			var expectedPerimeter1 = new List<LineSegment> { ls1, ls2, ls3, ls4 };
			var expectedPerimeter2 = new List<LineSegment> { ls5, ls6, ls7 };
			var expectedPerimeter3 = new List<LineSegment> { ls8, ls9, ls10 };

			// these to be honest are not required to be ordered this way, but guessing each potential order would be complicated
			Assert.IsTrue(ListUtilities.AreContentsEqual(result[0].GetPerimeter().GetPerimeter(), expectedPerimeter1));
			var guess11 = (ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), expectedPerimeter2) &&
			ListUtilities.AreContentsEqual(result[2].GetPerimeter().GetPerimeter(), expectedPerimeter3));
			var guess12 = (ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), expectedPerimeter3) &&
			ListUtilities.AreContentsEqual(result[2].GetPerimeter().GetPerimeter(), expectedPerimeter2));
			Assert.IsTrue(guess11 || guess12);

			var guess21 = (ListUtilities.AreContentsEqual(result[0].GetInnerPerimeters()[0].GetPerimeter(), expectedPerimeter2) &&
				ListUtilities.AreContentsEqual(result[0].GetInnerPerimeters()[1].GetPerimeter(), expectedPerimeter3));
			var guess22 = (ListUtilities.AreContentsEqual(result[0].GetInnerPerimeters()[0].GetPerimeter(), expectedPerimeter3) &&
				ListUtilities.AreContentsEqual(result[0].GetInnerPerimeters()[1].GetPerimeter(), expectedPerimeter2));
			Assert.IsTrue(guess21 || guess22);
			Assert.AreEqual(0, result[1].GetInnerPerimeters().Count);
			Assert.AreEqual(0, result[2].GetInnerPerimeters().Count);

			Assert.AreEqual(0, result[0].GetSegmentsWithin().Count);
			Assert.AreEqual(0, result[1].GetSegmentsWithin().Count);
			Assert.AreEqual(0, result[2].GetSegmentsWithin().Count);
		}

		[Test]
		public void TestDivideIntoSmallerFragments_6()
		{
			//   2    __________________ 
			//       |                  |
			//   1   x__________________x
			//       |\                /|
			//   0   |       \  /       |
			//       |        /\        |
			//  -1   |      /____\      |
			//       |                  |
			//  -2   |__________________|
			//
			//       -2   -1   0   1    2
			//
			var ls1 = new LineSegment(new Point(-2, -2), new Point(-2, 1));
			var ls2 = new LineSegment(new Point(-2, 1), new Point(-2, 2));
			var ls3 = new LineSegment(new Point(-2, 2), new Point(2, 2));
			var ls4 = new LineSegment(new Point(2, 2), new Point(2, 1));
			var ls5 = new LineSegment(new Point(2, 1), new Point(2, -2));
			var ls6 = new LineSegment(new Point(2, -2), new Point(-2, -2));
			var perimeter = new CycleOfLineSegments(new List<LineSegment> { ls1, ls2, ls3, ls4, ls5, ls6 });

			var ls7 = new LineSegment(new Point(-1, -1), new Point(0, 0));
			var ls8 = new LineSegment(new Point(0, 0), new Point(1, -1));
			var ls9 = new LineSegment(new Point(1, -1), new Point(-1, -1));

			var ls10 = new LineSegment(new Point(0, 0), new Point(-2, 1));
			var ls11 = new LineSegment(new Point(-2, 1), new Point(2, 1));
			var ls12 = new LineSegment(new Point(2, 1), new Point(0, 0));

			var segsWithin = new List<LineSegment> { ls7, ls8, ls9, ls10, ls11, ls12 };

			var fragment = new DividableDiagramFragment(perimeter, segsWithin);
			var result = fragment.DivideIntoSmallerFragments();
			Assert.AreEqual(2, result.Count);

			// guess 1: divided along the upper single straight line
			var expectedPerimeter11 = new List<LineSegment> { ls2, ls3, ls4, ls11 };
			var expectedSegmentsWithin11 = new List<LineSegment>();
			var expectedPerimeter12 = new List<LineSegment> { ls1, ls11, ls5, ls6 };
			var expectedSegmentsWithin12 = new List<LineSegment> { ls7, ls8, ls9, ls10, ls12 };

			var guess1pA = (ListUtilities.AreContentsEqual(result[0].GetPerimeter().GetPerimeter(), expectedPerimeter11) &&
			ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), expectedPerimeter12));
			var guess1sA = (ListUtilities.AreContentsEqual(result[0].GetSegmentsWithin(), expectedSegmentsWithin11) &&
				ListUtilities.AreContentsEqual(result[1].GetSegmentsWithin(), expectedSegmentsWithin12));

			var guess1pB = (ListUtilities.AreContentsEqual(result[0].GetPerimeter().GetPerimeter(), expectedPerimeter12) &&
			ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), expectedPerimeter11));
			var guess1sB = (ListUtilities.AreContentsEqual(result[0].GetSegmentsWithin(), expectedSegmentsWithin12) &&
						ListUtilities.AreContentsEqual(result[1].GetSegmentsWithin(), expectedSegmentsWithin11));

			var guess1 = (guess1pA && guess1sA) || (guess1pB && guess1sB);

			// guess2: divided along the lower two lines
			var expectedPerimeter21 = new List<LineSegment> { ls1, ls10, ls12, ls5, ls6 };
			var expectedSegmentsWithin21 = new List<LineSegment> { ls7, ls8, ls9 };
			var expectedPerimeter22 = new List<LineSegment> { ls10, ls2, ls3, ls4, ls12 };
			var expectedSegmentsWithin22 = new List<LineSegment> { ls11 };

			var guess2pA = (ListUtilities.AreContentsEqual(result[0].GetPerimeter().GetPerimeter(), expectedPerimeter21) &&
			ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), expectedPerimeter22));
			var guess2sA = (ListUtilities.AreContentsEqual(result[0].GetSegmentsWithin(), expectedSegmentsWithin21) &&
				ListUtilities.AreContentsEqual(result[1].GetSegmentsWithin(), expectedSegmentsWithin22));

			var guess2pB = (ListUtilities.AreContentsEqual(result[0].GetPerimeter().GetPerimeter(), expectedPerimeter22) &&
			ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), expectedPerimeter21));
			var guess2sB = (ListUtilities.AreContentsEqual(result[0].GetSegmentsWithin(), expectedSegmentsWithin22) &&
						ListUtilities.AreContentsEqual(result[1].GetSegmentsWithin(), expectedSegmentsWithin21));

			var guess2 = (guess2pA && guess2sA) || (guess2pB && guess2sB);

			Assert.IsTrue(guess1 || guess2);

			Assert.AreEqual(0, result[0].GetInnerPerimeters().Count);
			Assert.AreEqual(0, result[1].GetInnerPerimeters().Count);
		}

		[Test]
		public void TestDivideIntoSmallerFragments_7()
		{
			//  1  ____________
			//    |            |
			//    |        |\  |
			//  0 |        |  \x 
			//    |        |  /|
			//    |        |/  |
			// -1 |____________| 
			//
			//   -1     0     1    
			//
			var ls1 = new LineSegment(new Point(-1, -1), new Point(-1, 1));
			var ls2 = new LineSegment(new Point(-1, 1), new Point(1, 1));
			var ls3 = new LineSegment(new Point(1, 1), new Point(1, 0));
			var ls4 = new LineSegment(new Point(1, 0), new Point(1, -1));
			var ls5 = new LineSegment(new Point(1, -1), new Point(-1, -1));

			var perimeter = new List<LineSegment> { ls1, ls2, ls3, ls4, ls5 };

			var ls6 = new LineSegment(new Point(1, 0), new Point(0.5, 0.5));
			var ls7 = new LineSegment(new Point(1, 0), new Point(0.5, -0.5));
			var ls8 = new LineSegment(new Point(0.5, -0.5), new Point(0.5, 0.5));

			var segsWithin = new List<LineSegment> { ls6, ls7, ls8 };

			var fragment = new DividableDiagramFragment(new CycleOfLineSegments(perimeter), segsWithin);
			var result = fragment.DivideIntoSmallerFragments();
			Assert.AreEqual(2, result.Count);

			Assert.IsTrue(ListUtilities.AreContentsEqual(result[0].GetPerimeter().GetPerimeter(), perimeter));
			Assert.AreEqual(1, result[0].GetInnerPerimeters().Count);
			Assert.IsTrue(ListUtilities.AreContentsEqual(result[0].GetInnerPerimeters()[0].GetPerimeter(), segsWithin));

			Assert.IsTrue(ListUtilities.AreContentsEqual(result[1].GetPerimeter().GetPerimeter(), segsWithin));
			Assert.AreEqual(0, result[1].GetInnerPerimeters().Count);

			Assert.AreEqual(0, result[0].GetSegmentsWithin().Count);
			Assert.AreEqual(0, result[1].GetSegmentsWithin().Count);
		}
	}
}
