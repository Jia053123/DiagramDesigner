using NUnit.Framework;
using ShapeGrammarEngine;
using System;
using System.Collections.Generic;

namespace ShapeGrammarEngineUnitTests
{
	public class ShapeTests
	{
		[Test]
		public void TestConstructor_EmptySet_CreateEmptyShape()
		{
			Assert.DoesNotThrow(() => new Shape(new HashSet<Connection>()));
			var emptyShape = new Shape(new HashSet<Connection>());
			Assert.AreEqual(0, emptyShape.Definition.Count);
		}

		[Test]
		public void TestConstructor_NormalCases()
		{
			var c1 = new Connection(1, 2);
			var c2 = new Connection(2, 3);
			var c3 = new Connection(0, 3);

			Assert.DoesNotThrow(() => new Shape(new HashSet<Connection> { c1, c2, c3 }));
			Assert.DoesNotThrow(() => new Shape(new HashSet<Connection> { c2, c3 }));

			var shape1 = new Shape(new HashSet<Connection> { c2, c3 });
			Assert.AreEqual(2, shape1.Definition.Count);
			Assert.IsTrue(shape1.Definition.Contains(c2));
			Assert.IsTrue(shape1.Definition.Contains(c3));
		}

		[Test]
		public void TestEquality()
		{
			var c1 = new Connection(1, 2);
			var c2 = new Connection(2, 3);
			var c3 = new Connection(0, 3);

			var s1 = new Shape(new HashSet<Connection> { c1, c2, c3 });
			var s2 = new Shape(new HashSet<Connection> { c1, c3, c2, c3 });
			var s3 = new Shape(new HashSet<Connection> { c3, c2 });

			Assert.IsTrue(s2 == s1);
			Assert.IsFalse(s3 == s1);
		}

		[Test]
		public void TestGetAllLabels_EmptyShape()
		{
			var emptyShape = new Shape(new HashSet<Connection>());
			var result = emptyShape.GetAllLabels();
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void TestGetAllLabels_NonEmptyShape()
		{
			var c1 = new Connection(1, 2);
			var c2 = new Connection(2, 4);
			var c3 = new Connection(0, 4);
			var shape1 = new Shape(new HashSet<Connection> {c1, c2, c3 });
			var result1 = shape1.GetAllLabels();
			Assert.AreEqual(4, result1.Count);
			Assert.IsTrue(result1.Contains(1));
			Assert.IsTrue(result1.Contains(2));
			Assert.IsTrue(result1.Contains(4));
			Assert.IsTrue(result1.Contains(0));
		}

		[Test]
		public void TestCreateShapeFromPolylines_EdgeCases()
		{
			Assert.Throws<ArgumentNullException>(() => Shape.CreateShapeFromPolylines(null));

			var pls1 = new PolylineGroup(new List<List<(double, double)>>());
			Assert.DoesNotThrow(() => Shape.CreateShapeFromPolylines(pls1));
			var shape1 = Shape.CreateShapeFromPolylines(pls1);
			Assert.AreEqual(0, shape1.Definition.Count);

			var pls2 = new PolylineGroup( new List<List<(double, double)>> { new List<(double, double)>() });
			Assert.DoesNotThrow(() => Shape.CreateShapeFromPolylines(pls2));
			var shape2 = Shape.CreateShapeFromPolylines(pls2);
			Assert.AreEqual(0, shape2.Definition.Count);

			var pls3 = new PolylineGroup( new List<List<(double, double)>> { new List<(double, double)> { (0, 1) } });
			Assert.DoesNotThrow(() => Shape.CreateShapeFromPolylines(pls3));
			var shape3 = Shape.CreateShapeFromPolylines(pls3);
			Assert.AreEqual(0, shape3.Definition.Count);
		}

		[Test]
		public void TestCreateShapeFromPolylines_OnePolyline()
		{
			var result1 = Shape.CreateShapeFromPolylines(new PolylineGroup( new List<List<(double, double)>> { new List<(double, double)> { (0, 0), (0, 1) } }));
			Assert.AreEqual(1, result1.Definition.Count);
			Assert.IsTrue(result1.Definition.Contains(new Connection(0, 1)));

			var result2 = Shape.CreateShapeFromPolylines(new PolylineGroup( new List<List<(double, double)>> { new List<(double, double)> { (0, 0), (0, 1), (0, 3) } }));
			Assert.AreEqual(2, result2.Definition.Count);
			Assert.IsTrue(result2.Definition.Contains(new Connection(0, 1)));
			Assert.IsTrue(result2.Definition.Contains(new Connection(1, 2)));

			var result3 = Shape.CreateShapeFromPolylines(new PolylineGroup( new List<List<(double, double)>> { new List<(double, double)> { (0, 0), (1, 1), (1, 0), (0, 0) } }));
			Assert.AreEqual(3, result3.Definition.Count);
			Assert.IsTrue(result3.Definition.Contains(new Connection(0, 1)));
			Assert.IsTrue(result3.Definition.Contains(new Connection(1, 2)));
			Assert.IsTrue(result3.Definition.Contains(new Connection(2, 0)));
		}

		[Test]
		public void TestCreateShapeFromPolylines_MultiplePolylines()
		{
			var result1 = Shape.CreateShapeFromPolylines( new PolylineGroup( new List<List<(double, double)>> { 
				new List<(double, double)> { (0, 0), (0, 1) },
				new List<(double, double)> { (0, 0), (1, 0), (0, 1) }}));
			Assert.AreEqual(3, result1.Definition.Count);
			Assert.IsTrue(result1.Definition.Contains(new Connection(0, 1)));
			Assert.IsTrue(result1.Definition.Contains(new Connection(0, 2)));
			Assert.IsTrue(result1.Definition.Contains(new Connection(2, 1)));
		}

		[Test]
		public void TestCreateShapeFromPolylines_InputGeometryIntersectsWithItself_ThrowArgumentException()
		{
			Assert.Throws<ArgumentException>(() => Shape.CreateShapeFromPolylines(new PolylineGroup( new List<List<(double, double)>> { new List<(double, double)> { (0, -1), (0, 1), (1, 0), (-1, 0) } })));
			Assert.Throws<ArgumentException>(() => Shape.CreateShapeFromPolylines(new PolylineGroup( new List<List<(double, double)>> { new List<(double, double)> { (1, 0), (1, 2) }, new List<(double, double)> { (0, 1), (2, 1) } })));
		}

		[Test]
		public void TestCreateShapeFromPolylines_InputGeometryOverlapWithItself_ThrowArgumentException()
		{
			Assert.Throws<ArgumentException>(() => Shape.CreateShapeFromPolylines(new PolylineGroup( new List<List<(double, double)>> { new List<(double, double)> { (0, -1), (0, 1), (0, 0), (0, 0.5) } })));
		}

		[Test]
		public void TestConformsWithGeometry_EdgeCases()
		{
			var shape1 = Shape.CreateEmptyShape();
			Assert.Throws<ArgumentNullException>(() => shape1.ConformsWithGeometry(null));
			Assert.IsTrue(shape1.ConformsWithGeometry(new PolylineGroup(new List<List<(double X, double Y)>>())));
			Assert.IsTrue(shape1.ConformsWithGeometry(new PolylineGroup(new List<List<(double X, double Y)>> { new List<(double X, double Y)>() })));
			Assert.IsTrue(shape1.ConformsWithGeometry(new PolylineGroup(new List<List<(double X, double Y)>> { new List<(double X, double Y)> { (1, 1) } })));
			Assert.IsFalse(shape1.ConformsWithGeometry(new PolylineGroup(new List<List<(double X, double Y)>> { new List<(double X, double Y)> { (1, 1), (1, 2) }})));

			var shape2 = new Shape(new HashSet<Connection> { new Connection(0, 1) });
			Assert.Throws<ArgumentNullException>(() => shape2.ConformsWithGeometry(null));
			Assert.DoesNotThrow(() => shape2.ConformsWithGeometry(new PolylineGroup(new List<List<(double X, double Y)>>())));
			Assert.DoesNotThrow(() => shape2.ConformsWithGeometry(new PolylineGroup(new List<List<(double X, double Y)>> { new List<(double X, double Y)>() })));
		}

		[Test]
		public void TestConformsWithGeometry_OnePolylineAndNonConsecutiveLabels()
		{
			var shape0 = new Shape(new HashSet<Connection> { new Connection(0, 1) });
			var geometry0 = new PolylineGroup(new List<List<(double X, double Y)>> { new List<(double X, double Y)> { (-5, 2.1) } });
			Assert.IsFalse(shape0.ConformsWithGeometry(geometry0));

			var shape1 = new Shape(new HashSet<Connection> { new Connection(0, 1) });
			var geometry1 = new PolylineGroup(new List<List<(double X, double Y)>> { new List<(double X, double Y)> { (-5, 2.1), (20, 20) } });
			Assert.IsTrue(shape1.ConformsWithGeometry(geometry1));

			var shape2 = new Shape(new HashSet<Connection> { new Connection(0, 1), new Connection(1, 3), new Connection(3, 0) });
			var geometry2 = new PolylineGroup(new List<List<(double X, double Y)>> { new List<(double X, double Y)> { (-5, 2.1), (20, 20), (5, 10), (-5, 2.1) } });
			Assert.IsTrue(shape2.ConformsWithGeometry(geometry2));

			var geometry3 = new PolylineGroup(new List<List<(double X, double Y)>> { new List<(double X, double Y)> { (-5, 2.1), (20, 20), (5, 10), (2, 5), (-5, 2.1) } });
			Assert.IsFalse(shape2.ConformsWithGeometry(geometry3));

			var geometry4 = new PolylineGroup(new List<List<(double X, double Y)>> { new List<(double X, double Y)> { (-5, 2.1), (20, 20), (5, 10), (2, 5) } });
			Assert.IsFalse(shape2.ConformsWithGeometry(geometry4));
		}

		[Test]
		public void TestConformsWithGeometry_MultiplePolylinesAndNonConsecutiveLabels()
		{
			var shape1 = new Shape(new HashSet<Connection> { new Connection(0, 4), new Connection(4, 2), new Connection(2, 0) });
			var geometry1 = new PolylineGroup(new List<List<(double X, double Y)>> { 
				new List<(double X, double Y)> { (-5, 2.1), (20, 20) }, 
				new List<(double X, double Y)> { (5, 10), (20, 20) }, 
				new List<(double X, double Y)>{ (5, 10), (-5, 2.1) } });
			Assert.IsTrue(shape1.ConformsWithGeometry(geometry1));

			var geometry2 = new PolylineGroup(new List<List<(double X, double Y)>> {
				new List<(double X, double Y)> { (-5, 2.1), (20, 20) },
				new List<(double X, double Y)> { (5, 10), (19, 20) },
				new List<(double X, double Y)>{ (5, 10), (-5, 2.1) } });
			Assert.IsFalse(shape1.ConformsWithGeometry(geometry2));
		}
	}
}