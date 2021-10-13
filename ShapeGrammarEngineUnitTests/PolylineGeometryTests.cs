using BasicGeometries;
using ListOperations;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ShapeGrammarEngine.UnitTests
{
	class PolylineGeometryTests
	{
		[Test]
		public void TestCreateEmptyPolylineGeometry()
		{
			var emptyGeo = PolylineGeometry.CreateEmptyPolylineGeometry();
			var emptyShape = Shape.CreateEmptyShape();
			Assert.AreEqual(0, emptyGeo.PolylinesCopy.Count);
			Assert.IsTrue(emptyShape.ConformsWithGeometry(emptyGeo, out _));
		}

		[Test]
		public void TestGetAllPoints()
		{
			var geo = new PolylineGeometry(new List<List<Point>> {
				new List<Point> { new Point(0, -1), new Point(0, 1), new Point(1, 0), new Point(0, -1) } });

			var allPoints = geo.GetAllPoints();
			Assert.AreEqual(3, allPoints.Count);
			Assert.IsTrue(allPoints.Contains(new Point(0, -1)));
			Assert.IsTrue(allPoints.Contains(new Point(0, 1)));
			Assert.IsTrue(allPoints.Contains(new Point(1, 0)));
		}

		[Test]
		public void TestDoesIntersectOrOverlapWithItself_Intersect()
		{
			Assert.Throws<ArgumentException>(() => new PolylineGeometry(new List<List<Point>> { 
				new List<Point> { new Point(0, -1), new Point(0, 1), new Point(1, 0), new Point(-1, 0) } }));
			Assert.Throws<ArgumentException>(() => new PolylineGeometry(new List<List<Point>> { 
				new List<Point> { new Point(1, 0), new Point(1, 2) }, 
				new List<Point> { new Point(0, 1), new Point(2, 1) } }));
			Assert.Throws<ArgumentException>(() => new PolylineGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1) },
				new List<Point> { new Point(0,0), new Point(1,1) } }));
		}

		[Test]
		public void TestDoesIntersectOrOverlapWithItself_Overlap()
		{
			Assert.Throws<ArgumentException>(() => new PolylineGeometry(new List<List<Point>> { 
				new List<Point> { new Point(0, -1), new Point(0, 1), new Point(0, 0), new Point(0, 0.5) } }));
		}

		[Test]
		public void TestAddSegmentByPoints_IndenticalEndPoints_ThrowException()
		{
			var geo = PolylineGeometry.CreateEmptyPolylineGeometry();
			Assert.Throws<ArgumentException>(() => geo.AddSegmentByPoints(new Point(0, 0), new Point(0, 0)));
		}

		[Test]
		public void TestAddSegmentByPoints_DoesNotConnectToEndsOfExistingPolylines_AddNewPolyline()
		{
			var geo = PolylineGeometry.CreateEmptyPolylineGeometry();
			geo.AddSegmentByPoints(new Point(0, 0), new Point(0, 1));
			Assert.AreEqual(1, geo.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(0, 1) }));
			
			geo.AddSegmentByPoints(new Point(0, 0), new Point(1, 1));
			Assert.AreEqual(2, geo.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(0, 1) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo.PolylinesCopy[1], new List<Point> { new Point(0, 0), new Point(1, 1) }));
		}

		[Test]
		public void TestEraseSegmentByPoints_IdenticalEndPoints_ThrowException()
		{
			var geo = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(0,1)}});
			Assert.Throws<ArgumentException>(() => geo.EraseSegmentByPoints(new Point(0, 0), new Point(0, 0)));
		}

		[Test]
		public void TestEraseSegmentByPoints_NotASegment()
		{
			var geo4 = new PolylineGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(0,1), new Point(0,2) },
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) },
				new List<Point> { new Point(0,0), new Point(1,0), new Point(2,0) }});
			var s1 = geo4.EraseSegmentByPoints(new Point(0, 0), new Point(2, 2));
			Assert.IsFalse(s1);
			Assert.AreEqual(3, geo4.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(0, 1), new Point(0, 2) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[1], new List<Point> { new Point(0, 0), new Point(1, 1), new Point(2, 2), new Point(3, 3) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[2], new List<Point> { new Point(0, 0), new Point(1, 0), new Point(2, 0) }));

			var s2 = geo4.EraseSegmentByPoints(new Point(0, 2), new Point(2, 0));
			Assert.IsFalse(s2);

			Assert.Throws<ArgumentException>(() => geo4.EraseSegmentByPoints(new Point(0, 0), new Point(0, 0)));
		}

		[Test]
		public void TestEraseSegmentByPoints_InTheMiddle()
		{
			var geo4 = new PolylineGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(0,1), new Point(0,2) },
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) },
				new List<Point> { new Point(0,0), new Point(1,0), new Point(2,0) }});
			var s = geo4.EraseSegmentByPoints(new Point(1,1), new Point(2,2));
			Assert.IsTrue(s);
			Assert.AreEqual(4, geo4.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(0, 1), new Point(0, 2) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[1], new List<Point> { new Point(0, 0), new Point(1, 1) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[2], new List<Point> { new Point(2, 2), new Point(3, 3) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[3], new List<Point> { new Point(0, 0), new Point(1, 0), new Point(2, 0) }));
		}

		[Test]
		public void TestEraseSegmentByPoints_AtTheBeginning()
		{
			var geo4 = new PolylineGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(0,1), new Point(0,2) },
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) } });
			var s = geo4.EraseSegmentByPoints(new Point(1, 1), new Point(0, 0));
			Assert.IsTrue(s);
			Assert.AreEqual(2, geo4.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(0, 1), new Point(0, 2) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[1], new List<Point> { new Point(1, 1), new Point(2, 2), new Point(3, 3) }));
		}

		[Test]
		public void TestEraseSegmentByPoints_AtTheEnd()
		{
			var geo4 = new PolylineGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) },
				new List<Point> { new Point(0,0), new Point(1,0), new Point(2,0) }});
			var s = geo4.EraseSegmentByPoints(new Point(3, 3), new Point(2, 2));
			Assert.IsTrue(s);
			Assert.AreEqual(2, geo4.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(1, 1), new Point(2, 2)}));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[1], new List<Point> { new Point(0, 0), new Point(1, 0), new Point(2, 0) }));
		}

		[Test]
		public void TestEraseSegmentByIndexes_EdgeCases()
		{
			var geo0 = new PolylineGeometry(new List<List<Point>>());
			Assert.Throws<ArgumentOutOfRangeException>(() => geo0.EraseSegmentByIndexes(0, 0));

			var geo1 = new PolylineGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(0,1) } });
			Assert.Throws<ArgumentOutOfRangeException>(() => geo1.EraseSegmentByIndexes(0, 2));
			Assert.Throws<ArgumentOutOfRangeException>(() => geo1.EraseSegmentByIndexes(0, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => geo1.EraseSegmentByIndexes(1, 0));
			Assert.Throws<ArgumentOutOfRangeException>(() => geo1.EraseSegmentByIndexes(1, 1));
		}

		[Test]
		public void TestEraseSegmentByIndexes_SegmentAtTheBeginning_RemoveTheFirstPoint()
		{
			var geo1 = new PolylineGeometry(new List<List<Point>>{
				new List<Point> {new Point(0,0), new Point(1,0)},
				new List<Point> { new Point(0,0), new Point(0,1) } });
			Assert.DoesNotThrow(() => geo1.EraseSegmentByIndexes(1, 0));
			Assert.AreEqual(1, geo1.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo1.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(1, 0)}));

			var geo2 = new PolylineGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) } });
			Assert.DoesNotThrow(() => geo2.EraseSegmentByIndexes(0, 0));
			Assert.AreEqual(1, geo2.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo2.PolylinesCopy[0], new List<Point> {new Point(1,1), new Point(2, 2), new Point(3, 3) }));
		}

		[Test]
		public void TestEraseSegmentByIndexes_SegmentAtTheEnd_RemoveTheLastPoint()
		{
			var geo3 = new PolylineGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) } });
			Assert.DoesNotThrow(() => geo3.EraseSegmentByIndexes(0, 2));
			Assert.AreEqual(1, geo3.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo3.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(1, 1), new Point(2, 2) }));
		}

		[Test]
		public void TestEraseSegmentByIndexes_SegmentInTheMiddle_BreaksUp()
		{
			var geo4 = new PolylineGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(0,1), new Point(0,2) },
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) },
				new List<Point> { new Point(0,0), new Point(1,0), new Point(2,0) }});
			Assert.DoesNotThrow(() => geo4.EraseSegmentByIndexes(1, 1));
			Assert.AreEqual(4, geo4.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(0, 1), new Point(0, 2) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[1], new List<Point> { new Point(0, 0), new Point(1, 1) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[2], new List<Point> { new Point(2, 2), new Point(3, 3) }));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo4.PolylinesCopy[3], new List<Point> { new Point(0, 0), new Point(1, 0), new Point(2, 0) }));
		}

		[Test]
		public void TestConvertToConnections()
		{
			var geometry1 = new PolylineGeometry(new List<List<Point>> {
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1), new Point(-6, -6) } });

			var dic = new LabelingDictionary();
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
