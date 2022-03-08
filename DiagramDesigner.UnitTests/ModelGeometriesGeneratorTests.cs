using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ShapeGrammarEngine;
using WinPoint = System.Windows.Point;
using MyPoint = BasicGeometries.Point;

namespace DiagramDesigner.UnitTests
{
	class ModelGeometriesGeneratorTests
	{
		ModelGeometriesGenerator mgg;
		List<List<WinPoint>> allGeo1;

		[SetUp]
		public void SetUp()
		{
			var displayUnitOverRealUnit = 0.5;
			mgg = new ModelGeometriesGenerator(displayUnitOverRealUnit);

			allGeo1 = new List<List<WinPoint>>();
			allGeo1.Add(new List<WinPoint> { new WinPoint(1, 1), new WinPoint(2, 2), new WinPoint(3, 3) });
			allGeo1.Add(new List<WinPoint>());
			allGeo1.Add(new List<WinPoint> { new WinPoint(1, 2), new WinPoint(2, 3) });
		}

		[Test]
		public void TestGenerateLeftHandGeometryFromContext_FirstIndexOutOfRange_ThrowArgumentOutOfRangeException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(3, 1, 2));

			Assert.Throws<ArgumentOutOfRangeException>(() => mgg.GenerateLeftHandGeometryFromContext(allGeo1,contextGeoIndex));
		}

		[Test]
		public void TestGenerateLeftHandGeometryFromContext_SecondIndexOutOfRange_ThrowArgumentOutOfRangeException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(2, 2, 3));

			Assert.Throws<ArgumentOutOfRangeException>(() => mgg.GenerateLeftHandGeometryFromContext(allGeo1, contextGeoIndex));
		}

		[Test]
		public void TestGenerateLeftHandGeometryFromContext_ThirdIndexOutOfRange_ThrowArgumentOutOfRangeException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(2, 1, 2));

			Assert.Throws<ArgumentOutOfRangeException>(() => mgg.GenerateLeftHandGeometryFromContext(allGeo1, contextGeoIndex));
		}

		[Test]
		public void TestGenerateLeftHandGeometryFromContext_SecondAndThirdIndexNotConsecutive_ThrowArgumentException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 0, 2));

			Assert.Throws<ArgumentException>(() => mgg.GenerateLeftHandGeometryFromContext(allGeo1, contextGeoIndex));
		}

		[Test]
		public void TestGenerateLeftHandGeometryFromContext_SecondAndThirdIndexNotAscending_ThrowArgumentException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 1, 2));
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 1, 0));

			Assert.Throws<ArgumentException>(() => mgg.GenerateLeftHandGeometryFromContext(allGeo1, contextGeoIndex));
		}

		[Test]
		public void TestGenerateLeftHandGeometryFromContext_ValidArguments_CorrectOutput()
		{
			var allGeo = new List<List<WinPoint>>();
			allGeo.Add(new List<WinPoint> { new WinPoint(1, 1), new WinPoint(2, 2), new WinPoint(3, 3), new WinPoint(4, 4) });
			allGeo.Add(new List<WinPoint>());
			allGeo.Add(new List<WinPoint> { new WinPoint(1, 2), new WinPoint(2, 3) });

			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 0, 1));
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 2, 3));
			contextGeoIndex.Add(new Tuple<int, int, int>(2, 0, 1));

			PolylinesGeometry pg;
			Assert.DoesNotThrow(() => mgg.GenerateLeftHandGeometryFromContext(allGeo, contextGeoIndex));

			pg = mgg.GenerateLeftHandGeometryFromContext(allGeo, contextGeoIndex);
			var lines = pg.PolylinesCopy;
			Assert.AreEqual(lines.Count, 3);
			foreach(List<MyPoint> line in lines)
			{
				Assert.AreEqual(line.Count, 2);
			}

			Assert.AreEqual(lines[0][0], new MyPoint(2, 2));
			Assert.AreEqual(lines[0][1], new MyPoint(4, 4));

			Assert.AreEqual(lines[1][0], new MyPoint(6, 6));
			Assert.AreEqual(lines[1][1], new MyPoint(8, 8));

			Assert.AreEqual(lines[2][0], new MyPoint(2, 4));
			Assert.AreEqual(lines[2][1], new MyPoint(4, 6));
		}

		[Test]
		public void TestGenerateGeometriesFromContextAndAdditions_ContextIndexOutOfRange_ThrowArgumentException()
		{
			
		}

		[Test]
		public void TestGenerateGeometriesFromContextAndAdditions_AdditionIndexOutOfRange_ThrowArgumentException()
		{
			var displayUnitOverRealUnit = 2;
		}
	}
}
