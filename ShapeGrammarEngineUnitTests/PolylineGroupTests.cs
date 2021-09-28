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
		public void TestEraseSegment_EdgeCases()
		{
			var group0 = new PolylineGroup(new List<List<Point>>());
			Assert.Throws<ArgumentOutOfRangeException>(() => group0.EraseSegment(0, 0));

			var group1 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(0,1) } });
			Assert.Throws<ArgumentOutOfRangeException>(() => group1.EraseSegment(0, 2));
			Assert.Throws<ArgumentOutOfRangeException>(() => group1.EraseSegment(0, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => group1.EraseSegment(1, 0));
			Assert.Throws<ArgumentOutOfRangeException>(() => group1.EraseSegment(1, 1));
		}

		[Test]
		public void TestEraseSegment_SegmentAtTheBeginning_RemoveTheFirstPoint()
		{
			var group1 = new PolylineGroup(new List<List<Point>>{
				new List<Point> {new Point(0,0), new Point(1,0)},
				new List<Point> { new Point(0,0), new Point(0,1) } });
			Assert.DoesNotThrow(() => group1.EraseSegment(1, 0));
			Assert.AreEqual(1, group1.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group1.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(1, 0)}));

			var group2 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) } });
			Assert.DoesNotThrow(() => group2.EraseSegment(0, 0));
			Assert.AreEqual(1, group2.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group2.PolylinesCopy[0], new List<Point> {new Point(1,1), new Point(2, 2), new Point(3, 3) }));
		}

		[Test]
		public void TestEraseSegment_SegmentAtTheEnd_RemoveTheLastPoint()
		{
			var group3 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) } });
			Assert.DoesNotThrow(() => group3.EraseSegment(0, 2));
			Assert.AreEqual(1, group3.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group3.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(1, 1), new Point(2, 2) }));
		}

		[Test]
		public void TestEraseSegment_SegmentInTheMiddle_BreaksUp()
		{
			var group4 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(0,1), new Point(0,2) },
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) },
				new List<Point> { new Point(0,0), new Point(1,0), new Point(2,0) },});
			Assert.DoesNotThrow(() => group4.EraseSegment(1, 1));
			Assert.AreEqual(4, group4.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group4.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(0, 1), new Point(0, 2) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group4.PolylinesCopy[1], new List<Point> { new Point(0, 0), new Point(1, 1) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group4.PolylinesCopy[2], new List<Point> { new Point(2, 2), new Point(3, 3) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(group4.PolylinesCopy[3], new List<Point> { new Point(0, 0), new Point(1, 0), new Point(2, 0) }));
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
