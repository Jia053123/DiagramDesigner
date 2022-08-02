using System;
using System.Collections.Generic;
using NUnit.Framework;
using ShapeGrammarEngine;
using WinPoint = System.Windows.Point;
using MyPoint = BasicGeometries.Point;

namespace DiagramDesigner.UnitTests
{
	class ModelScreenGeometriesConvertorTests
	{
		ModelScreenGeometriesConverter msgc;
		List<List<WinPoint>> allGeo1;

		[SetUp]
		public void SetUp()
		{
			var displayUnitOverRealUnit = 0.5;
			msgc = new ModelScreenGeometriesConverter(displayUnitOverRealUnit);

			allGeo1 = new List<List<WinPoint>>();
			allGeo1.Add(new List<WinPoint> { new WinPoint(1, 1), new WinPoint(2, 2), new WinPoint(3, 3) });
			allGeo1.Add(new List<WinPoint>());
			allGeo1.Add(new List<WinPoint> { new WinPoint(1, 2), new WinPoint(2, 3) });
		}

		[Test]
		public void TestConvertPolylinesInPointsToGeometriesOnScreen_NullInput_ThrowArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => msgc.ConvertPolylinesInPointsToGeometriesOnScreen(null));
		}

		[Test]
		public void TestConvertPolylinesInPointsToGeometriesOnScreen_ValidInputContainingLinesPointsWithEmptyEntries_CorrectlyConvertedOutputInSameOrder()
		{
			var pip = new List<List<MyPoint>>();
			pip.Add(new List<MyPoint> { new MyPoint(2, 2), new MyPoint(4, 4) });
			pip.Add(new List<MyPoint>());
			pip.Add(new List<MyPoint> { new MyPoint(6, 4) });

			var piwp = msgc.ConvertPolylinesInPointsToGeometriesOnScreen(pip);
			Assert.AreEqual(2, piwp[0].Count);
			Assert.AreEqual(0, piwp[1].Count);
			Assert.AreEqual(1, piwp[2].Count);

			Assert.AreEqual(new WinPoint(1, 1), piwp[0][0]);
			Assert.AreEqual(new WinPoint(2, 2), piwp[0][1]);
			Assert.AreEqual(new WinPoint(3, 2), piwp[2][0]);
		}

		[Test]
		public void TestGenerateLeftHandGeometryFromContext_FirstIndexOutOfRange_ThrowArgumentOutOfRangeException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(3, 1, 2));

			Assert.Throws<ArgumentOutOfRangeException>(() => msgc.GenerateLeftHandGeometryFromContext(allGeo1,contextGeoIndex));
		}

		[Test]
		public void TestGenerateLeftHandGeometryFromContext_SecondIndexOutOfRange_ThrowArgumentOutOfRangeException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(2, 2, 3));

			Assert.Throws<ArgumentOutOfRangeException>(() => msgc.GenerateLeftHandGeometryFromContext(allGeo1, contextGeoIndex));
		}

		[Test]
		public void TestGenerateLeftHandGeometryFromContext_ThirdIndexOutOfRange_ThrowArgumentOutOfRangeException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(2, 1, 2));

			Assert.Throws<ArgumentOutOfRangeException>(() => msgc.GenerateLeftHandGeometryFromContext(allGeo1, contextGeoIndex));
		}

		[Test]
		public void TestGenerateLeftHandGeometryFromContext_SecondAndThirdIndexNotConsecutive_ThrowArgumentException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 0, 2));

			Assert.Throws<ArgumentException>(() => msgc.GenerateLeftHandGeometryFromContext(allGeo1, contextGeoIndex));
		}

		[Test]
		public void TestGenerateLeftHandGeometryFromContext_SecondAndThirdIndexNotAscending_ThrowArgumentException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 1, 2));
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 1, 0));

			Assert.Throws<ArgumentException>(() => msgc.GenerateLeftHandGeometryFromContext(allGeo1, contextGeoIndex));
		}

		[Test]
		public void TestGenerateLeftHandGeometryFromContext_ValidArguments_NoIntersectionOrOverlap_PolylineGeometryWithCorrectLineSegmentsInSameOrder()
		{
			var allGeo = new List<List<WinPoint>>();
			allGeo.Add(new List<WinPoint> { new WinPoint(1, 1), new WinPoint(2, 2), new WinPoint(3, 3), new WinPoint(4, 4) });
			allGeo.Add(new List<WinPoint>());
			allGeo.Add(new List<WinPoint> { new WinPoint(1, 2), new WinPoint(2, 3) });

			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 0, 1));
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 2, 3));
			contextGeoIndex.Add(new Tuple<int, int, int>(2, 0, 1));

			Assert.DoesNotThrow(() => msgc.GenerateLeftHandGeometryFromContext(allGeo, contextGeoIndex));

			var pg = msgc.GenerateLeftHandGeometryFromContext(allGeo, contextGeoIndex);
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
		public void TestGenerateLeftHandGeometryFromContext_ValidArguments_WithIntersectionsNoOverlap_PolylineGeometryWithCorrectLineSegmentsInSameOrder()
		{
			var allGeo = new List<List<WinPoint>>();
			allGeo.Add(new List<WinPoint> { new WinPoint(-1, 0), new WinPoint(1, 0) });
			allGeo.Add(new List<WinPoint>());
			allGeo.Add(new List<WinPoint> { new WinPoint(0, -1), new WinPoint(0, 1) });

			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 0, 1));
			contextGeoIndex.Add(new Tuple<int, int, int>(2, 0, 1));

			Assert.DoesNotThrow(() => msgc.GenerateLeftHandGeometryFromContext(allGeo, contextGeoIndex));

			//var pg = msgc.GenerateLeftHandGeometryFromContext(allGeo, contextGeoIndex);
			//var lines = pg.PolylinesCopy;
			//Assert.AreEqual(lines.Count, 4);
			//foreach (List<MyPoint> line in lines)
			//{
			//	Assert.AreEqual(line.Count, 2);
			//}

			//Assert.AreEqual(lines[0][0], new MyPoint(2, 2));
			//Assert.AreEqual(lines[0][1], new MyPoint(4, 4));

			//Assert.AreEqual(lines[1][0], new MyPoint(6, 6));
			//Assert.AreEqual(lines[1][1], new MyPoint(8, 8));

			//Assert.AreEqual(lines[2][0], new MyPoint(2, 4));
			//Assert.AreEqual(lines[2][1], new MyPoint(4, 6));
		}

		[Test]
		public void TestGenerateGeometriesFromContextAndAdditions_IndexOutOfRange1_ThrowArgumentOutOfRangeException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 1, 2));

			var additionGeoIndex = new List<Tuple<int, int, int>>();
			additionGeoIndex.Add(new Tuple<int, int, int>(3, 1, 2));

			Assert.Throws<ArgumentOutOfRangeException>(() => msgc.GenerateGeometriesFromContextAndAdditions(allGeo1, contextGeoIndex, additionGeoIndex));
		}

		[Test]
		public void TestGenerateGeometriesFromContextAndAdditions_IndexOutOfRange2_ThrowArgumentOutOfRangeException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 1, 2));

			var additionGeoIndex = new List<Tuple<int, int, int>>();
			additionGeoIndex.Add(new Tuple<int, int, int>(1, 0, 1));

			Assert.Throws<ArgumentOutOfRangeException>(() => msgc.GenerateGeometriesFromContextAndAdditions(allGeo1, contextGeoIndex, additionGeoIndex));
		}

		[Test]
		public void TestGenerateGeometriesFromContextAndAdditions_SecondAndThirdIndexNotConsecutive_ThrowArgumentException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 1, 2));

			var additionGeoIndex = new List<Tuple<int, int, int>>();
			additionGeoIndex.Add(new Tuple<int, int, int>(2, 1, 0));

			Assert.Throws<ArgumentException>(() => msgc.GenerateGeometriesFromContextAndAdditions(allGeo1, contextGeoIndex, additionGeoIndex));
		}

		[Test]
		public void TestGenerateGeometriesFromContextAndAdditions_ValidInputsWithOverlappingContextAndAddition_ThrowArgumentException()
		{
			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 1, 2));
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 0, 1));

			var additionGeoIndex = new List<Tuple<int, int, int>>();
			additionGeoIndex.Add(new Tuple<int, int, int>(0, 0, 1));
			additionGeoIndex.Add(new Tuple<int, int, int>(2, 0, 1));

			Assert.Throws<ArgumentException>(() => msgc.GenerateGeometriesFromContextAndAdditions(allGeo1, contextGeoIndex, additionGeoIndex));
		}

		[Test]
		public void TestGenerateGeometriesFromContextAndAdditions_ValidInputsWithMutuallyExclusiveContextAndAddition_LeftHandGeometryIsConvertedFromContextAndRightHandGeometryIsConvertedFromTheUnionOfContextAndAddition()
		{
			var allGeo = new List<List<WinPoint>>();
			allGeo.Add(new List<WinPoint> { new WinPoint(1, 1), new WinPoint(2, 2), new WinPoint(3, 3), new WinPoint(4, 4) });
			allGeo.Add(new List<WinPoint>());
			allGeo.Add(new List<WinPoint> { new WinPoint(1, 2), new WinPoint(2, 3), new WinPoint(3, 4) });

			var contextGeoIndex = new List<Tuple<int, int, int>>();
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 1, 2));
			contextGeoIndex.Add(new Tuple<int, int, int>(0, 0, 1));

			var additionGeoIndex = new List<Tuple<int, int, int>>();
			additionGeoIndex.Add(new Tuple<int, int, int>(0, 2, 3));
			additionGeoIndex.Add(new Tuple<int, int, int>(2, 0, 1));
			additionGeoIndex.Add(new Tuple<int, int, int>(2, 1, 2));

			Assert.DoesNotThrow(() => msgc.GenerateGeometriesFromContextAndAdditions(allGeo, contextGeoIndex, additionGeoIndex));

			var result = msgc.GenerateGeometriesFromContextAndAdditions(allGeo, contextGeoIndex, additionGeoIndex);
			var leftHandPg = result.Item1;
			var rightHandPg = result.Item2;

			var lLines = leftHandPg.PolylinesCopy;
			Assert.AreEqual(lLines.Count, 2);
			foreach (List<MyPoint> line in lLines)
			{
				Assert.AreEqual(line.Count, 2);
			}
			Assert.AreEqual(lLines[0][0], new MyPoint(4, 4));
			Assert.AreEqual(lLines[0][1], new MyPoint(6, 6));
			Assert.AreEqual(lLines[1][0], new MyPoint(2, 2));
			Assert.AreEqual(lLines[1][1], new MyPoint(4, 4));

			var rLines = rightHandPg.PolylinesCopy;
			Assert.AreEqual(rLines.Count, 5);
			foreach (List<MyPoint> line in rLines)
			{
				Assert.AreEqual(line.Count, 2);
			}
			Assert.AreEqual(rLines[0][0], new MyPoint(4, 4));
			Assert.AreEqual(rLines[0][1], new MyPoint(6, 6));
			Assert.AreEqual(rLines[1][0], new MyPoint(2, 2));
			Assert.AreEqual(rLines[1][1], new MyPoint(4, 4));
			Assert.AreEqual(rLines[2][0], new MyPoint(6, 6));
			Assert.AreEqual(rLines[2][1], new MyPoint(8, 8));
			Assert.AreEqual(rLines[3][0], new MyPoint(2, 4));
			Assert.AreEqual(rLines[3][1], new MyPoint(4, 6));
			Assert.AreEqual(rLines[4][0], new MyPoint(4, 6));
			Assert.AreEqual(rLines[4][1], new MyPoint(6, 8));
		}
	}
}
