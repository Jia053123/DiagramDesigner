using BasicGeometries;
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
			Assert.IsTrue(new PolylineGroup(new List<List<Point>> { new List<Point> { new Point(0, -1), new Point(0, 1), new Point(1, 0), new Point(-1, 0) } }).DoesIntersectOrOverlapWithItself());
			Assert.IsTrue(new PolylineGroup(new List<List<Point>> { new List<Point> { new Point(1, 0), new Point(1, 2) }, new List<Point> { new Point(0, 1), new Point(2, 1) } }).DoesIntersectOrOverlapWithItself());
		}

		[Test]
		public void TestDoesIntersectOrOverlapWithItself_Overlap()
		{
			Assert.IsTrue(new PolylineGroup(new List<List<Point>> { new List<Point> { new Point(0, -1), new Point(0, 1), new Point(0, 0), new Point(0, 0.5) } }).DoesIntersectOrOverlapWithItself());
		}
		
		[Test]
		public void TestErasePoint_EdgeCases()
		{
			var group1 = new PolylineGroup(new List<List<Point>>{
				new List<Point> { new Point(0,0) } });
			Assert.Throws<ArgumentException>(() => group1.ErasePoint(0, 1));
			Assert.Throws<ArgumentException>(() => group1.ErasePoint(1, 0));
			Assert.Throws<ArgumentException>(() => group1.ErasePoint(1, 1));
			Assert.DoesNotThrow(() => group1.ErasePoint(0, 0));
		}

		[Test]
		public void TestErasePoint_DoesNotBreakUpPolyline()
		{
			
		}

		[Test]
		public void TestErasePoint_BreakUpPolyline()
		{

		}

		[Test]
		public void TestErasePoint_MultipleInstances()
		{

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
