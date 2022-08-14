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
		public void TestConstructor_NonIntersectingOrOverlappingInput_polylineNotModified()
		{
			//  1      |\
			//  0      | >
			// -1      |/   
			//
			//         0  1       

			var testPolyline = new List<List<Point>> {
				new List<Point> { new Point(0, -1), new Point(0, 1), new Point(1, 0), new Point(0, -1) } };
			var geo = new PolylinesGeometry(testPolyline);
			Assert.AreEqual(1, geo.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(testPolyline, geo.PolylinesCopy));
		}

		[Test]
		public void TestConstructor_IntersectingInput_polylineExploded()
		{
			//  1      |\
			//  0    --|--
			// -1      |   
			//
			//         0  1       

			var testPolyline = new List<List<Point>> 
			{
				new List<Point> { new Point(0, -1), new Point(0, 1), new Point(1, 0), new Point(-1, 0) } 
			};
			var geo = new PolylinesGeometry(testPolyline);

			Assert.AreEqual(5, geo.PolylinesCopy.Count);
		}

		[Test]
		public void TestConstructor_OverlappingInput_polylineExploded()
		{
			var testPolyline = new List<List<Point>>
			{
				new List<Point> { new Point(0, -1), new Point(0, 1), new Point(0, 0) }
			};
			var geo = new PolylinesGeometry(testPolyline);

			Assert.AreEqual(2, geo.PolylinesCopy.Count);
		}

		[Test]
		public void TestCreateEmptyPolylineGeometry()
		{
			var emptyGeo = PolylinesGeometry.CreateEmptyPolylineGeometry();
			var emptyShape = Shape.CreateEmptyShape();
			Assert.AreEqual(0, emptyGeo.PolylinesCopy.Count);
			Assert.IsTrue(emptyShape.ConformsWithGeometry(emptyGeo, out _));
		}

		[Test]
		public void TestGetAllPoints()
		{
			var geo = new PolylinesGeometry(new List<List<Point>> {
				new List<Point> { new Point(0, -1), new Point(0, 1), new Point(1, 0), new Point(0, -1) } });

			var allPoints = geo.GetAllPoints();
			Assert.AreEqual(3, allPoints.Count);
			Assert.IsTrue(allPoints.Contains(new Point(0, -1)));
			Assert.IsTrue(allPoints.Contains(new Point(0, 1)));
			Assert.IsTrue(allPoints.Contains(new Point(1, 0)));
		}

		[Test]
		public void TestDoesIntersectOrOverlapWithItself_Intersect_NoExceptionThrown()
		{
			Assert.DoesNotThrow(() => new PolylinesGeometry(new List<List<Point>> { 
				new List<Point> { new Point(0, -1), new Point(0, 1), new Point(1, 0), new Point(-1, 0) } }));
			Assert.DoesNotThrow(() => new PolylinesGeometry(new List<List<Point>> { 
				new List<Point> { new Point(1, 0), new Point(1, 2) }, 
				new List<Point> { new Point(0, 1), new Point(2, 1) } }));
			Assert.DoesNotThrow(() => new PolylinesGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1) },
				new List<Point> { new Point(0,0), new Point(1,1) } }));
		}

		[Test]
		public void TestDoesIntersectOrOverlapWithItself_Overlap_NoExceptionThrown()
		{
			Assert.DoesNotThrow(() => new PolylinesGeometry(new List<List<Point>> { 
				new List<Point> { new Point(0, -1), new Point(0, 1), new Point(0, 0), new Point(0, 0.5) } }));
		}

		[Test]
		public void TestAddSegmentByPoints_IndenticalEndPoints_ThrowException()
		{
			var geo = PolylinesGeometry.CreateEmptyPolylineGeometry();
			Assert.Throws<ArgumentException>(() => geo.AddSegmentByPoints(new Point(0, 0), new Point(0, 0)));
		}

		[Test]
		public void TestAddSegmentByPoints_DoesNotConnectToEndsOfExistingPolylines_AddNewPolyline()
		{
			var geo = PolylinesGeometry.CreateEmptyPolylineGeometry();
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
			var geo = new PolylinesGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(0,1)}});
			Assert.Throws<ArgumentException>(() => geo.EraseSegmentByPoints(new Point(0, 0), new Point(0, 0)));
		}

		[Test]
		public void TestEraseSegmentByPoints_NotASegment()
		{
			var geo4 = new PolylinesGeometry(new List<List<Point>>{
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
			var geo4 = new PolylinesGeometry(new List<List<Point>>{
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
			var geo4 = new PolylinesGeometry(new List<List<Point>>{
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
			var geo4 = new PolylinesGeometry(new List<List<Point>>{
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
			var geo0 = new PolylinesGeometry(new List<List<Point>>());
			Assert.Throws<ArgumentOutOfRangeException>(() => geo0.EraseSegmentByIndexes(0, 0));

			var geo1 = new PolylinesGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(0,1) } });
			Assert.Throws<ArgumentOutOfRangeException>(() => geo1.EraseSegmentByIndexes(0, 2));
			Assert.Throws<ArgumentOutOfRangeException>(() => geo1.EraseSegmentByIndexes(0, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => geo1.EraseSegmentByIndexes(1, 0));
			Assert.Throws<ArgumentOutOfRangeException>(() => geo1.EraseSegmentByIndexes(1, 1));
		}

		[Test]
		public void TestEraseSegmentByIndexes_SegmentAtTheBeginning_RemoveTheFirstPoint()
		{
			var geo1 = new PolylinesGeometry(new List<List<Point>>{
				new List<Point> {new Point(0,0), new Point(1,0)},
				new List<Point> { new Point(0,0), new Point(0,1) } });
			Assert.DoesNotThrow(() => geo1.EraseSegmentByIndexes(1, 0));
			Assert.AreEqual(1, geo1.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo1.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(1, 0)}));

			var geo2 = new PolylinesGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) } });
			Assert.DoesNotThrow(() => geo2.EraseSegmentByIndexes(0, 0));
			Assert.AreEqual(1, geo2.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo2.PolylinesCopy[0], new List<Point> {new Point(1,1), new Point(2, 2), new Point(3, 3) }));
		}

		[Test]
		public void TestEraseSegmentByIndexes_SegmentAtTheEnd_RemoveTheLastPoint()
		{
			var geo3 = new PolylinesGeometry(new List<List<Point>>{
				new List<Point> { new Point(0,0), new Point(1,1), new Point(2,2), new Point(3,3) } });
			Assert.DoesNotThrow(() => geo3.EraseSegmentByIndexes(0, 2));
			Assert.AreEqual(1, geo3.PolylinesCopy.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(geo3.PolylinesCopy[0], new List<Point> { new Point(0, 0), new Point(1, 1), new Point(2, 2) }));
		}

		[Test]
		public void TestEraseSegmentByIndexes_SegmentInTheMiddle_BreaksUp()
		{
			var geo4 = new PolylinesGeometry(new List<List<Point>>{
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
			var geometry1 = new PolylinesGeometry(new List<List<Point>> {
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

		[Test]
		public void TestGetPointByIndex_InputRepresetsAPointInPolylinesGeometry_OutputPoint()
		{
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) }
			});

			Point point = geometry1.GetPointByIndex(0, 1);
			Assert.AreEqual(5, point.coordinateX);
			Assert.AreEqual(10, point.coordinateY);
		}

		[Test]
		public void TestFindIndexForNextPoint_InputIsAPointInGeometry_OutputTheNextPointInPolylineOrder_1()
		{
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) }
			});

			var result = geometry1.FindIndexForNextPoint(0, 2);
			Assert.AreEqual(1, result.nextPointIndex);
			Assert.AreEqual(2, result.nextPolylineIndex);
		}

		[Test]
		public void TestFindIndexForNextPoint_InputIsAPointInGeometry_OutputTheNextPointInPolylineOrder_2()
		{
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) }
			});

			var result = geometry1.FindIndexForNextPoint(1, 1);
			Assert.AreEqual(0, result.nextPointIndex);
			Assert.AreEqual(2, result.nextPolylineIndex);
		}

		[Test]
		public void TestFindIndexForNextPoint_InputIsAtTheEndOfGeometry_SpecialOutput()
		{
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) }
			});

			var result = geometry1.FindIndexForNextPoint(1, 2);
			Assert.AreEqual(-1, result.nextPointIndex);
			Assert.AreEqual(-1, result.nextPolylineIndex);
		}


		[Test]
		public void TestFindIndexForNextPoint_InputIsNotAPointInGeometry_Throws_1()
		{
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) }
			});

			Assert.Throws<ArgumentException>(() => geometry1.FindIndexForNextPoint(0, 3));
		}

		[Test]
		public void TestFindIndexForNextPoint_InputIsNotAPointInGeometry_Throws_2()
		{
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) }
			});

			Assert.Throws<ArgumentException>(() => geometry1.FindIndexForNextPoint(2, 1));
		}
	}
}
