using NUnit.Framework;
using ShapeGrammarEngine;
using System;
using System.Collections.Generic;

namespace ShapeGrammarEngineUnitTests
{
	public class ShapeTests
	{
		[Test]
		public void TestCreateShapeFromPolylines_EdgeCases()
		{
			Assert.Throws<ArgumentNullException>(() => Shape.CreateShapeFromPolylines(null));

			var result1 = Shape.CreateShapeFromPolylines(new List<List<(double, double)>> { new List<(double, double)>() });
			Assert.AreEqual(0, result1.Definition.Count);
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
		public void TestCreateShapeFromPolylines_OnePolyline()
		{
			var result1 = Shape.CreateShapeFromPolylines(new List<List<(double, double)>> { new List<(double, double)> { (0, 0), (0, 1) } });
			Assert.AreEqual(1, result1.Definition.Count);
			Assert.IsTrue(result1.Definition.Contains(new Connection(0, 1)));

			var result2 = Shape.CreateShapeFromPolylines(new List<List<(double, double)>> { new List<(double, double)> { (0, 0), (0, 1), (0, 2) } });
			Assert.AreEqual(2, result2.Definition.Count);
			Assert.IsTrue(result2.Definition.Contains(new Connection(0, 1)));
			Assert.IsTrue(result2.Definition.Contains(new Connection(1, 2)));

			var result3 = Shape.CreateShapeFromPolylines(new List<List<(double, double)>> { new List<(double, double)> { (0, 0), (1, 1), (1, 0), (0, 0) } });
			Assert.AreEqual(3, result3.Definition.Count);
			Assert.IsTrue(result3.Definition.Contains(new Connection(0, 1)));
			Assert.IsTrue(result3.Definition.Contains(new Connection(1, 2)));
			Assert.IsTrue(result3.Definition.Contains(new Connection(2, 0)));
		}

		[Test]
		public void TestCreateShapeFromPolylines_MultiplePolylines()
		{
			var result1 = Shape.CreateShapeFromPolylines(new List<List<(double, double)>> { 
				new List<(double, double)> { (0, 0), (0, 1) },
				new List<(double, double)> { (0, 0), (1, 0), (0, 1) }});
			Assert.AreEqual(3, result1.Definition.Count);
			Assert.IsTrue(result1.Definition.Contains(new Connection(0, 1)));
			Assert.IsTrue(result1.Definition.Contains(new Connection(0, 2)));
			Assert.IsTrue(result1.Definition.Contains(new Connection(2, 1)));
		}

		[Test]
		public void TestCreateShapeFromPolylines_InputGeometryIntersectsWithItself_ThrowArgumentException()
		{
			Assert.Throws<ArgumentException>(() => Shape.CreateShapeFromPolylines(new List<List<(double, double)>> { new List<(double, double)> { (0, -1), (0, 1), (1, 0), (-1, 0) } }));
			Assert.Throws<ArgumentException>(() => Shape.CreateShapeFromPolylines(new List<List<(double, double)>> { new List<(double, double)> { (1, 0), (1, 2) }, new List<(double, double)> { (0, 1), (2, 1) } }));
		}

		[Test]
		public void TestCreateShapeFromPolylines_InputGeometryOverlapWithItself_ThrowArgumentException()
		{
			Assert.Throws<ArgumentException>(() => Shape.CreateShapeFromPolylines(new List<List<(double, double)>> { new List<(double, double)> { (0, -1), (0, 1), (0, 0), (0, 0.5) } }));
		}

		[Test]
		public void TestConformsWithGeometry_EdgeCases()
		{
			var shape = new Shape(new HashSet<Connection> { new Connection(0, 1) });

			Assert.Throws<ArgumentNullException>(() => shape.ConformsWithGeometry(null));

			Assert.IsFalse(shape.ConformsWithGeometry(new List<List<(double X, double Y)>>()));
			Assert.IsFalse(shape.ConformsWithGeometry(new List<List<(double X, double Y)>> { new List<(double X, double Y)>() }));
		}

		[Test]
		public void TestConformsWithGeometry_OnePolyline()
		{
			var shape1 = new Shape(new HashSet<Connection> { new Connection(0, 1) });
			var geometry1 = new List<List<(double X, double Y)>> { new List<(double X, double Y)> { (-5, 2.1), (20, 20) } };
			Assert.IsTrue(shape1.ConformsWithGeometry(geometry1));
		}
	}
}