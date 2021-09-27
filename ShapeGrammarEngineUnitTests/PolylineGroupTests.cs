using BasicGeometries;
using ListOperations;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ShapeGrammarEngine.UnitTests
{
	class PolylineGroupTests
	{
		[Test]
		public void TestCreateEmptyPolylineGroup()
		{
			var emptyGroup = PolylineGroup.CreateEmptyPolylineGroup();
			var emptyShape = Shape.CreateEmptyShape();
			Assert.AreEqual(0, emptyGroup.PolylinesCopy.Count);
			Assert.IsTrue(emptyShape.ConformsWithGeometry(emptyGroup, out _));
		}

		[Test]
		public void TestDoesIntersectOrOverlapWithItself_Intersect()
		{
			Assert.IsTrue(new PolylineGroup(new List<List<Point>> { 
				new List<Point> { new Point(0, -1), new Point(0, 1), new Point(1, 0), new Point(-1, 0) } }).DoesIntersectOrOverlapWithItself());
			Assert.IsTrue(new PolylineGroup(new List<List<Point>> { 
				new List<Point> { new Point(1, 0), new Point(1, 2) }, 
				new List<Point> { new Point(0, 1), new Point(2, 1) } }).DoesIntersectOrOverlapWithItself());
			Assert.IsTrue(new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1) },
				new List<Point> { new Point(0,0), new Point(1,1) } }).DoesIntersectOrOverlapWithItself());
		}

		[Test]
		public void TestDoesIntersectOrOverlapWithItself_Overlap()
		{
			Assert.IsTrue(new PolylineGroup(new List<List<Point>> { new List<Point> { new Point(0, -1), new Point(0, 1), new Point(0, 0), new Point(0, 0.5) } }).DoesIntersectOrOverlapWithItself());
		}
		
		[Test]
		public void TestErasePoint_EdgeCases()
		{
			var group0 = new PolylineGroup(new List<List<Point>>());
			Assert.Throws<ArgumentOutOfRangeException>(() => group0.ErasePoint(0, 0));

			var group1 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(0,1) } });
			Assert.Throws<ArgumentOutOfRangeException>(() => group1.ErasePoint(0, 2));
			Assert.Throws<ArgumentOutOfRangeException>(() => group1.ErasePoint(1, 0));
			Assert.Throws<ArgumentOutOfRangeException>(() => group1.ErasePoint(1, 2));
		}

		[Test]
		public void TestErasePoint_SimpleCases()
		{
			var group1 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(0,1) } });
			Assert.DoesNotThrow(() => group1.ErasePoint(0, 1));
			Assert.AreEqual(0, group1.PolylinesCopy.Count);

			var group2 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(0,1) } });
			Assert.DoesNotThrow(() => group2.ErasePoint(0, 0));
			Assert.AreEqual(0, group2.PolylinesCopy.Count);
		}

		[Test]
		public void TestErasePoint_AtEndOfPolyline_RemoveTheEndOfPolyline()
		{
			var group1 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) } });
			group1.ErasePoint(0, 0);
			Assert.AreEqual(1, group1.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group1.PolylinesCopy[0], new List<Point> { new Point(1, 1), new Point(2, 2), new Point(3, 3) }));

			group1.ErasePoint(0, 2);
			Assert.AreEqual(1, group1.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group1.PolylinesCopy[0], new List<Point> { new Point(1, 1), new Point(2, 2) }));
		}

		[Test]
		public void TestErasePoint_AtEndOfTwoPointPolyline_RemoveTheWholePolylineAlongWithTheLastPoint()
		{
			var group1 = new PolylineGroup(new List<List<Point>>{ 
				new List<Point> { new Point(0,0), new Point(1,1) },
				new List<Point> { new Point(0,0), new Point(0,1) } });
			group1.ErasePoint(0, 1);
			Assert.AreEqual(1, group1.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group1.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(0, 1) }));
		}

		[Test]
		public void TestErasePoint_InTheSecondOrSecondLastPoint_RemoveTheFirstOrLastTwoPoints()
		{
			var group1 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2, 2), new Point(3,3) },
				new List<Point> { new Point(0,0), new Point(0,1) } });
			group1.ErasePoint(0, 1);
			Assert.AreEqual(2, group1.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group1.PolylinesCopy[0], new List<Point> { new Point(2, 2), new Point(3, 3) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group1.PolylinesCopy[1], new List<Point> { new Point(0, 0), new Point(0, 1) }));

			var group2 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2, 2), new Point(3,3) },
				new List<Point> { new Point(0,0), new Point(0,1) } });
			group2.ErasePoint(0, 2);
			Assert.AreEqual(2, group2.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group2.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(1, 1) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group2.PolylinesCopy[1], new List<Point> { new Point(0, 0), new Point(0, 1) }));

			var group3 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1) },
				new List<Point> { new Point(0,0), new Point(0,1), new Point(0,2) } });
			group3.ErasePoint(1, 1);
			Assert.AreEqual(1, group3.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group3.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(1, 1) }));
		}

		[Test]
		public void TestErasePoint_InTheMiddleOfPolyline_BreakUpPolyline()
		{
			var group1 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,0) },
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2, 2), new Point(3,3), new Point(4,4) },
				new List<Point> { new Point(0,0), new Point(0,1) } });
			group1.ErasePoint(1, 2);
			Assert.AreEqual(4, group1.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group1.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(1, 0) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group1.PolylinesCopy[1], new List<Point> { new Point(0, 0), new Point(1, 1) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group1.PolylinesCopy[2], new List<Point> { new Point(3, 3), new Point(4, 4) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group1.PolylinesCopy[3], new List<Point> { new Point(0, 0), new Point(0, 1) }));
		}
	
		[Test]
		public void TestConvertToConnections()
		{
			var geometry1 = new PolylineGroup(new List<List<Point>> {
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1), new Point(-6, -6) } });

			var dic = new Dictionary<Point, int>();
			dic.Add(new Point(-5, 2.1), 1);
			dic.Add(new Point(20, 20), 2);
			dic.Add(new Point(5, 10), 3);
			dic.Add(new Point(-6, -6), 4);

			var cs = geometry1.ConvertToConnections(dic);
			Assert.AreEqual(4, cs.Count);
			Assert.IsTrue(cs.Contains(new Connection(1, 2)));
			Assert.IsTrue(cs.Contains(new Connection(3, 2)));
			Assert.IsTrue(cs.Contains(new Connection(3, 1)));
			Assert.IsTrue(cs.Contains(new Connection(1, 4)));
		}
	}
}
