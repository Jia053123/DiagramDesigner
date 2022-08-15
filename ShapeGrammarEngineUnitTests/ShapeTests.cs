using BasicGeometries;
using NUnit.Framework;
using ShapeGrammarEngine;
using System;
using System.Collections.Generic;
using ListOperations;
using System.Linq;

namespace ShapeGrammarEngineUnitTests
{
	public class ShapeTests
	{
		[Test]
		public void TestConstructor_EmptySet_CreateEmptyShape()
		{
			Assert.DoesNotThrow(() => new Shape(new HashSet<Connection>()));
			var emptyShape = new Shape(new HashSet<Connection>());
			Assert.AreEqual(0, emptyShape.DefiningConnections.Count);
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
			Assert.AreEqual(2, shape1.DefiningConnections.Count);
			Assert.IsTrue(shape1.DefiningConnections.Contains(c2));
			Assert.IsTrue(shape1.DefiningConnections.Contains(c3));
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
			var shape1 = new Shape(new HashSet<Connection> { c1, c2, c3 });
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
			LabelingDictionary newLabeling;

			Assert.Throws<ArgumentNullException>(() => Shape.CreateShapeFromPolylines(null, null, out _));

			var pls1 = new PolylinesGeometry(new List<List<Point>>());
			Assert.DoesNotThrow(() => Shape.CreateShapeFromPolylines(pls1, null, out _));
			var shape1 = Shape.CreateShapeFromPolylines(pls1, null, out newLabeling);
			Assert.AreEqual(0, shape1.DefiningConnections.Count);
			Assert.AreEqual(0, newLabeling.Count);

			var pls2 = new PolylinesGeometry(new List<List<Point>> { new List<Point>() });
			Assert.DoesNotThrow(() => Shape.CreateShapeFromPolylines(pls2, null, out _));
			var shape2 = Shape.CreateShapeFromPolylines(pls2, null, out newLabeling);
			Assert.AreEqual(0, shape2.DefiningConnections.Count);
			Assert.AreEqual(0, newLabeling.Count);

			var pls3 = new PolylinesGeometry(new List<List<Point>> { new List<Point> { new Point(0, 1) } });
			Assert.DoesNotThrow(() => Shape.CreateShapeFromPolylines(pls3, null, out _));
			var shape3 = Shape.CreateShapeFromPolylines(pls3, null, out newLabeling);
			Assert.AreEqual(0, shape3.DefiningConnections.Count);
			Assert.AreEqual(0, newLabeling.Count);
		}

		[Test]
		public void TestCreateShapeFromPolylines_OnePolylineWithoutPredefinedLabeling_1()
		{
			LabelingDictionary newLabeling;

			var result1 = Shape.CreateShapeFromPolylines(new PolylinesGeometry(new List<List<Point>> { 
				new List<Point> { new Point(0, 0), new Point(0, 1) } }), null, out newLabeling);
			Assert.AreEqual(1, result1.DefiningConnections.Count);
			Assert.IsTrue(result1.DefiningConnections.Contains(new Connection(0, 1)));

			Assert.AreEqual(2, newLabeling.Count);
			Assert.IsTrue(newLabeling.DoesContainPair(new Point(0, 0), 0));
			Assert.IsTrue(newLabeling.DoesContainPair(new Point(0, 1), 1));
		}

		[Test]
		public void TestCreateShapeFromPolylines_OnePolylineWithoutPredefinedLabeling_2()
		{
			LabelingDictionary newLabeling;

			var result2 = Shape.CreateShapeFromPolylines(new PolylinesGeometry(new List<List<Point>> {
				new List<Point> { new Point(0, 0), new Point(0, 1), new Point(0, 3) } }), null, out newLabeling);
			Assert.AreEqual(2, result2.DefiningConnections.Count);
			Assert.IsTrue(result2.DefiningConnections.Contains(new Connection(0, 1)));
			Assert.IsTrue(result2.DefiningConnections.Contains(new Connection(1, 2)));

			Assert.AreEqual(3, newLabeling.Count);
			Assert.IsTrue(newLabeling.DoesContainPair(new Point(0, 0), 0));
			Assert.IsTrue(newLabeling.DoesContainPair(new Point(0, 1), 1));
			Assert.IsTrue(newLabeling.DoesContainPair(new Point(0, 3), 2));
		}

		[Test]
		public void TestCreateShapeFromPolylines_OnePolylineWithoutPredefinedLabeling_3()
		{
			LabelingDictionary newLabeling;

			var result3 = Shape.CreateShapeFromPolylines(new PolylinesGeometry(new List<List<Point>> {
				new List<Point> { new Point(0, 0), new Point(1, 1), new Point(1, 0), new Point(0, 0) } }), null, out newLabeling);
			Assert.AreEqual(3, result3.DefiningConnections.Count);
			Assert.IsTrue(result3.DefiningConnections.Contains(new Connection(0, 1)));
			Assert.IsTrue(result3.DefiningConnections.Contains(new Connection(1, 2)));
			Assert.IsTrue(result3.DefiningConnections.Contains(new Connection(2, 0)));

			Assert.AreEqual(3, newLabeling.Count);
			Assert.IsTrue(newLabeling.DoesContainPair(new Point(0, 0), 0));
			Assert.IsTrue(newLabeling.DoesContainPair(new Point(1, 1), 1));
			Assert.IsTrue(newLabeling.DoesContainPair(new Point(1, 0), 2));
		}

		[Test]
		public void TestCreateShapeFromPolylines_OnePolylineWithPredefinedLabeling()
		{
			var labeling = new LabelingDictionary();
			labeling.Add(new Point(0, 0), 100);
			labeling.Add(new Point(0, 1), 110);
			labeling.Add(new Point(0, 3), 130);
			labeling.Add(new Point(0, 10), 140);

			var result1 = Shape.CreateShapeFromPolylines(new PolylinesGeometry(new List<List<Point>> {
				new List<Point> { new Point(0, 0), new Point(0, 1) } }), labeling, out _);
			Assert.AreEqual(1, result1.DefiningConnections.Count);
			Assert.IsTrue(result1.DefiningConnections.Contains(new Connection(100, 110)));

			var result2 = Shape.CreateShapeFromPolylines(new PolylinesGeometry(new List<List<Point>> {
				new List<Point> { new Point(0, 0), new Point(0, 1), new Point(0, 3) } }), labeling, out _);
			Assert.AreEqual(2, result2.DefiningConnections.Count);
			Assert.IsTrue(result2.DefiningConnections.Contains(new Connection(100, 110)));
			Assert.IsTrue(result2.DefiningConnections.Contains(new Connection(110, 130)));

			var result3 = Shape.CreateShapeFromPolylines(new PolylinesGeometry(new List<List<Point>> {
				new List<Point> { new Point(0, 0), new Point(1, 1), new Point(1, 0), new Point(0, 0) } }), labeling, out _);
			Assert.AreEqual(3, result3.DefiningConnections.Count);
			Assert.IsTrue(result3.DefiningConnections.Contains(new Connection(100, 141)));
			Assert.IsTrue(result3.DefiningConnections.Contains(new Connection(141, 142)));
			Assert.IsTrue(result3.DefiningConnections.Contains(new Connection(142, 100)));
		}

		[Test]
		public void TestCreateShapeFromPolylines_MultiplePolylinesWithoutPredefinedLabeling()
		{
			var result1 = Shape.CreateShapeFromPolylines(new PolylinesGeometry(new List<List<Point>> {
				new List<Point> { new Point(0, 0), new Point(0, 1) },
				new List<Point> { new Point(0, 0), new Point(1, 0), new Point(0, 1) }}), null, out _);
			Assert.AreEqual(3, result1.DefiningConnections.Count);
			Assert.IsTrue(result1.DefiningConnections.Contains(new Connection(0, 1)));
			Assert.IsTrue(result1.DefiningConnections.Contains(new Connection(0, 2)));
			Assert.IsTrue(result1.DefiningConnections.Contains(new Connection(2, 1)));
		}

		[Test]
		public void TestCreateShapeFromPolylines_MultiplePolylinesWithPredefinedLabeling()
		{
			var labeling = new Dictionary<Point, int>();
			labeling.Add(new Point(0, 0), 100);
			labeling.Add(new Point(0, 1), 110);
			labeling.Add(new Point(0, 3), 130);
			labeling.Add(new Point(0, 10), 140);

			var result1 = Shape.CreateShapeFromPolylines(new PolylinesGeometry(new List<List<Point>> {
				new List<Point> { new Point(0, 0), new Point(0, 1) },
				new List<Point> { new Point(0, 0), new Point(1, 0), new Point(0, 1) }}), null, out _);
			Assert.AreEqual(3, result1.DefiningConnections.Count);
			Assert.IsTrue(result1.DefiningConnections.Contains(new Connection(0, 1)));
			Assert.IsTrue(result1.DefiningConnections.Contains(new Connection(0, 2)));
			Assert.IsTrue(result1.DefiningConnections.Contains(new Connection(2, 1)));
		}

		[Test]
		public void TestCreateShapeFromPolylines_InputGeometryIntersectsWithItself_DoesNotThrowArgumentException()
		{
			Assert.DoesNotThrow(() => Shape.CreateShapeFromPolylines(new PolylinesGeometry( new List<List<Point>> { 
				new List<Point> { new Point(0, -1), new Point(0, 1), new Point(1, 0), new Point(-1, 0) } }), null, out _));
			Assert.DoesNotThrow(() => Shape.CreateShapeFromPolylines(new PolylinesGeometry( new List<List<Point>> { 
				new List<Point> { new Point(1, 0), new Point(1, 2) }, new List<Point> { new Point(0, 1), new Point(2, 1) } }), null, out _));
		}

		[Test]
		public void TestCreateShapeFromPolylines_InputGeometryOverlapWithItself_DoesNotThrowArgumentException()
		{
			Assert.DoesNotThrow(() => Shape.CreateShapeFromPolylines(new PolylinesGeometry( new List<List<Point>> { 
				new List<Point> { new Point(0, -1), new Point(0, 1), new Point(0, 0), new Point(0, 0.5) } }), null, out _));
		}

		[Test]
		public void TestConformsWithGeometry_EdgeCases()
		{
			var emptyShape = Shape.CreateEmptyShape();
			Assert.Throws<ArgumentNullException>(() => emptyShape.ConformsWithGeometry(null, out _));

			LabelingDictionary output;
			Assert.IsTrue(emptyShape.ConformsWithGeometry(new PolylinesGeometry(new List<List<Point>>()), out output));
			Assert.AreEqual(0, output.Count);

			Assert.IsTrue(emptyShape.ConformsWithGeometry(new PolylinesGeometry(new List<List<Point>> { new List<Point>() }), out output));
			Assert.AreEqual(0, output.Count);

			Assert.IsTrue(emptyShape.ConformsWithGeometry(new PolylinesGeometry(new List<List<Point>> { new List<Point> { new Point(1, 1) } }), out output));
			Assert.AreEqual(0, output.Count);

			Assert.IsFalse(emptyShape.ConformsWithGeometry(new PolylinesGeometry(new List<List<Point>> { new List<Point> { new Point(1, 1), new Point(1, 2) }}), out output));
			Assert.IsNull(output);

			var shape2 = new Shape(new HashSet<Connection> { new Connection(0, 1) });
			Assert.Throws<ArgumentNullException>(() => shape2.ConformsWithGeometry(null, out _));
			Assert.DoesNotThrow(() => shape2.ConformsWithGeometry(new PolylinesGeometry(new List<List<Point>>()), out _));
			Assert.DoesNotThrow(() => shape2.ConformsWithGeometry(new PolylinesGeometry(new List<List<Point>> { new List<Point>() }), out _));
		}

		[Test]
		public void TestConformsWithGeometry_OnePolylineAndNonConsecutiveLabels()
		{
			LabelingDictionary output;

			var shape0 = new Shape(new HashSet<Connection> { new Connection(0, 1) });
			var geometry0 = new PolylinesGeometry(new List<List<Point>> { new List<Point> { new Point(-5, 2.1) } });
			Assert.IsFalse(shape0.ConformsWithGeometry(geometry0, out output));
			Assert.IsNull(output);

			var shape1 = new Shape(new HashSet<Connection> { new Connection(0, 1) });
			var geometry1 = new PolylinesGeometry(new List<List<Point>> { new List<Point> { new Point(-5, 2.1), new Point(20, 20) } });
			Assert.IsTrue(shape1.ConformsWithGeometry(geometry1, out output));
			Assert.AreEqual(2, output.Count);
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(-5, 2.1)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(20, 20)));

			// cycle
			var shape2 = new Shape(new HashSet<Connection> { new Connection(0, 1), new Connection(1, 3), new Connection(3, 0) });
			var geometry2 = new PolylinesGeometry(new List<List<Point>> { 
				new List<Point> { new Point(-5, 2.1), new Point(20, 20), new Point(5, 10), new Point(-5, 2.1) } });
			Assert.IsTrue(shape2.ConformsWithGeometry(geometry2, out output));
			Assert.AreEqual(3, output.Count);
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(-5, 2.1)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(20, 20)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(5, 10)));

			var geometry3 = new PolylinesGeometry(new List<List<Point>> { 
				new List<Point> { new Point(-5, 2.1), new Point(20, 20), new Point(10, 5), new Point(2, -5), new Point(-5, 2.1) } });
			Assert.IsFalse(shape2.ConformsWithGeometry(geometry3, out output));
			Assert.IsNull(output);

			var geometry4 = new PolylinesGeometry(new List<List<Point>> { 
				new List<Point> { new Point(-5, 2.1), new Point(20, 20), new Point(10, 5), new Point(2, 5) } });
			Assert.IsFalse(shape2.ConformsWithGeometry(geometry4, out output));
			Assert.IsNull(output);
		}

		[Test]
		public void TestConformsWithGeometry_MultiplePolylinesAndNonConsecutiveLabels()
		{
			LabelingDictionary output;

			var shape1 = new Shape(new HashSet<Connection> { new Connection(0, 4), new Connection(4, 2), new Connection(2, 0) });
			var geometry1 = new PolylinesGeometry(new List<List<Point>> { 
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) }, 
				new List<Point> { new Point(5, 10), new Point(20, 20) }, 
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) } });
			Assert.IsTrue(shape1.ConformsWithGeometry(geometry1, out output));
			Assert.AreEqual(3, output.Count);
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(-5, 2.1)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(20, 20)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(5, 10)));

			var geometry2 = new PolylinesGeometry(new List<List<Point>> {
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(19, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) } });
			Assert.IsFalse(shape1.ConformsWithGeometry(geometry2, out output));
			Assert.IsNull(output);
		}

		[Test]
		public void TestConformsWithGeometry_MultiplePolylinesInDifferentOrders1()
		{
			LabelingDictionary output;

			var shape1 = new Shape(new HashSet<Connection> { new Connection(0, 1), new Connection(1, 2), new Connection(1, 3), new Connection(2, 4) });
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 8), new Point(20, 20) },
				new List<Point> { new Point(5, 8), new Point(-5, -10) },
				new List<Point> { new Point(10.5, -10), new Point(20, 20) }
			});
			Assert.IsTrue(shape1.ConformsWithGeometry(geometry1, out output));
			Assert.AreEqual(5, output.Count);
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(-5, 2.1)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(20, 20)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(5, 8)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(10.5, -10)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(-5, -10)));
		}

		[Test]
		public void TestConformsWithGeometry_MultiplePolylinesInDifferentOrders2()
		{
			LabelingDictionary output;

			var shape1 = new Shape(new HashSet<Connection> { new Connection(2, 3), new Connection(2, 1), new Connection(0, 1), new Connection(4, 3) });
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(20, 20), new Point(-5, 2.1) },
				new List<Point> { new Point(5, 8), new Point(20, 20) },
				new List<Point> { new Point(5, 8), new Point(-5, -10) },
				new List<Point> { new Point(10.5, -10), new Point(-5, -10) }
			});
			Assert.IsTrue(shape1.ConformsWithGeometry(geometry1, out output));
			Assert.AreEqual(5, output.Count);
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(-5, 2.1)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(20, 20)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(5, 8)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(10.5, -10)));
			Assert.IsTrue(output.GetAllPoints().Contains(new Point(-5, -10)));
		}

		[Test]
		public void TestSolveLabeling_EmptyShape_OutputEmptyLabeling()
		{
			var shape0 = Shape.CreateEmptyShape();
			var geometry0 = PolylinesGeometry.CreateEmptyPolylineGeometry();
			var result0 = shape0.SolveLabeling(geometry0, null).First();
			Assert.AreEqual(0, result0.Count);
		}

		[Test]
		public void TestSolveLabeling_NullPartialSolution_ContinuousLoop_OutputCorrectLabeling_1()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> {new Point(-5, 2), new Point(20, 20)}
			});

			var result = shape1.SolveLabeling(geometry1, null);
			Assert.AreEqual(2, result.Count);

			var doesSolutionExist1 = false;
			var doesSolutionExist2 = false;
			for (int i = 0; i < 2; i++)
			{
				Assert.AreEqual(2, result[i].Count);

				if ((0 == result[i].GetLabelByPoint(new Point(-5, 2))) &&
					(4 == result[i].GetLabelByPoint(new Point(20, 20)))) 
				{
					doesSolutionExist1 = true;
				}
				if ((4 == result[i].GetLabelByPoint(new Point(-5, 2))) &&
					(0 == result[i].GetLabelByPoint(new Point(20, 20))))
				{
					doesSolutionExist2 = true;
				}
			}
			Assert.IsTrue(doesSolutionExist1);
			Assert.IsTrue(doesSolutionExist2);
		}

		[Test]
		public void TestSolveLabeling_NullPartialSolution_ContinuousLoop_OutputCorrectLabeling_2()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(2, 0)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> {new Point(-5, 2.1), new Point(20, 20), new Point(5, 10), new Point(-5, 2.1)}
			});

			var result2 = shape1.SolveLabeling(geometry1, null);
			Assert.AreEqual(6, result2.Count);

			var doesSolutionExist2 = false;
			for (int i = 0; i < 6; i++)
			{
				Assert.AreEqual(3, result2[i].Count);

				if ((0 == result2[i].GetLabelByPoint(new Point(-5, 2.1))) &&
				(2 == result2[i].GetLabelByPoint(new Point(5, 10))) &&
				(4 == result2[i].GetLabelByPoint(new Point(20, 20))))
				{
					doesSolutionExist2 = true;
				}
				Assert.IsTrue(doesSolutionExist2);
			}
		}

		[Test]
		public void TestSolveLabeling_EmptyPartialSolution_BrokenDownLoop_OutputCorrectLabeling()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(2, 0)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) }
			});

			var labeling1 = new LabelingDictionary();
			var result1 = shape1.SolveLabeling(geometry1, labeling1);

			int expectedCount1 = 6; // 024, 042, 204, 240, 420, 402
			Assert.AreEqual(expectedCount1, result1.Count); 

			var doesSolutionExist1 = false;
			for (int i = 0; i < expectedCount1; i++)
			{
				Assert.AreEqual(3, result1[i].Count); // for each valid solution, there are exactly 3 points to label

				if ((4 == result1[i].GetLabelByPoint(new Point(-5, 2.1))) && 
					(0 == result1[i].GetLabelByPoint(new Point(5, 10))) && 
					(2 == result1[i].GetLabelByPoint(new Point(20, 20))))
				{
					doesSolutionExist1 = true;
				}
			}
			Assert.IsTrue(doesSolutionExist1);
		}

		[Test]
		public void TestSolveLabeling_EmptyPartialSolution_NotLoop_OutputCorrectLabeling()
		{
			//  5 (0,50)   2/3 (100,50)
			//    |       | 
			//  0 |_______| 4  (100,0)     
			// (0,0)      |
			//            | 3/2 (100,-50)
			//
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(3, 4),
				new Connection(0, 5)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(100, -50), new Point(100, 0) },
				new List<Point> { new Point(100, 50), new Point(100, 0), new Point(0, 0) },
				new List<Point>{ new Point(0, 0), new Point(0, 50) }
			});

			var labeling1 = new LabelingDictionary();
			var result1 = shape1.SolveLabeling(geometry1, labeling1);

			int expectedCount1 = 2; 
			Assert.AreEqual(expectedCount1, result1.Count);
			var doesSolutionExist1 = false;
			var doesSolutionExist2 = false;
			for (int i = 0; i < expectedCount1; i++)
			{
				Assert.AreEqual(5, result1[i].Count);

				if ((0 == result1[i].GetLabelByPoint(new Point(0, 0))) &&
					(5 == result1[i].GetLabelByPoint(new Point(0, 50))) &&
					(4 == result1[i].GetLabelByPoint(new Point(100, 0))) &&
					(2 == result1[i].GetLabelByPoint(new Point(100, 50))) &&
					(3 == result1[i].GetLabelByPoint(new Point(100, -50)))) 
				{
					doesSolutionExist1 = true;
				}
				if ((0 == result1[i].GetLabelByPoint(new Point(0, 0))) &&
					(5 == result1[i].GetLabelByPoint(new Point(0, 50))) &&
					(4 == result1[i].GetLabelByPoint(new Point(100, 0))) &&
					(2 == result1[i].GetLabelByPoint(new Point(100, -50))) &&
					(3 == result1[i].GetLabelByPoint(new Point(100, 50))))
				{
					doesSolutionExist2 = true;
				}
			}
			Assert.IsTrue(doesSolutionExist1);
			Assert.IsTrue(doesSolutionExist2);
		}

		[Test]
		public void TestSolveLabeling_EmptyPartialSolution_GometryCannotMatchWithShape_ThrowShapeMatchFailureException()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(2, 0)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 21) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) },
			});
			Assert.Throws<ShapeMatchFailureException>(() => shape1.SolveLabeling(geometry1, null));
		}

		[Test]
		public void TestSolveLabeling_EmptyPartialSolution_DifferentConnectionsWithTheSameNumberOfPoints_LessConnectionsThanDef_ThrowShapeMatchFailureException()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(2, 0)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) }
			});

			Assert.Throws<ShapeMatchFailureException>(() => shape1.SolveLabeling(geometry1, null));
		}


		[Test]
		public void TestSolveLabeling_EmptyPartialSolution_DifferentConnectionsWithTheSameNumberOfPoints_MoreConnectionsThanDef_ThrowShapeMatchFailureException()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(1, 2),
				new Connection(1, 0)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(4, 10), new Point(-5, 2.1) },
				new List<Point> { new Point(20, 20), new Point(4, 10)}
			});

			Assert.Throws<ShapeMatchFailureException>(() => shape1.SolveLabeling(geometry1, null));
		}

		[Test]
		public void TestSolveLabeling_PerfectPartialSolution_OutputThePartialSolution()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(2, 0)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) }
			});
			var labeling1 = new LabelingDictionary();

			labeling1.Add(new Point(-5, 2.1), 0);
			labeling1.Add(new Point(5, 10), 2);
			labeling1.Add(new Point(20, 20), 4);

			var result1 = shape1.SolveLabeling(geometry1, labeling1);
			Assert.AreEqual(1, result1.Count); // only one solution
			Assert.AreEqual(3, result1.First().Count); // there are three points

			Assert.AreEqual(0, result1[0].GetLabelByPoint(new Point(-5, 2.1)));
			Assert.AreEqual(2, result1[0].GetLabelByPoint(new Point(5, 10)));
			Assert.AreEqual(4, result1[0].GetLabelByPoint(new Point(20, 20)));
		}

		[Test]
		public void TestSolveLabeling_PartialSolution_OutputCorrectLabeling()
		{
			//  5 (0,50)   2/3 (100,50)
			//    |       | 
			//  0 |_______| 4  (100,0)     
			// (0,0)      |
			//            | 3/2 (100,-50)
			//
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(3, 4),
				new Connection(0, 5)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(100, -50), new Point(100, 0) },
				new List<Point> { new Point(100, 50), new Point(100, 0), new Point(0, 0) },
				new List<Point>{ new Point(0, 0), new Point(0, 50) }
			});

			var labeling1 = new LabelingDictionary();
			labeling1.Add(new Point(0, 0), 0);
			labeling1.Add(new Point(100, 50), 2);

			var result1 = shape1.SolveLabeling(geometry1, labeling1);

			Assert.AreEqual(1, result1.Count);
			Assert.AreEqual(5, result1[0].Count);

			Assert.AreEqual(0, result1[0].GetLabelByPoint(new Point(0, 0)));
			Assert.AreEqual(5, result1[0].GetLabelByPoint(new Point(0, 50)));
			Assert.AreEqual(4, result1[0].GetLabelByPoint(new Point(100, 0)));
			Assert.AreEqual(2, result1[0].GetLabelByPoint(new Point(100, 50)));
			Assert.AreEqual(3, result1[0].GetLabelByPoint(new Point(100, -50)));
		}

		[Test]
		public void TestSolveLabeling_PartialSolutionInputWithUnusedEntries_OutputCompleteSolutionIncludingUnusedEntries()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(2, 0)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) }
			});
			var labeling1 = new LabelingDictionary();
			labeling1.Add(new Point(-5, 2.1), 0);
			labeling1.Add(new Point(5, 10), 2);
			labeling1.Add(new Point(10, 10), 5);

			var result1 = shape1.SolveLabeling(geometry1, labeling1);
			Assert.AreEqual(1, result1.Count); // only one point left to assign
			Assert.AreEqual(4, result1.First().Count); // the unused extra point is also added to the solution

			Assert.AreEqual(0, result1[0].GetLabelByPoint(new Point(-5, 2.1)));
			Assert.AreEqual(2, result1[0].GetLabelByPoint(new Point(5, 10)));
			Assert.AreEqual(4, result1[0].GetLabelByPoint(new Point(20, 20)));
			Assert.AreEqual(5, result1[0].GetLabelByPoint(new Point(10, 10)));
		}

		[Test]
		public void TestSolveLabeling_PartialSolutionLabelExistingPointWithWrongLabelInShape_ThrowShapeMatchFailureException()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(2, 5),
				new Connection(5, 0)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(0, 10) },
				new List<Point>{ new Point(-5, 2.1), new Point(0, 10) }
			});

			// This labeling is supposed to fail because 0 must be connected to 4
			var labeling1 = new LabelingDictionary();
			labeling1.Add(new Point(-5, 2.1), 0);
			labeling1.Add(new Point(5, 10), 4);

			Assert.Throws<ShapeMatchFailureException>(() => shape1.SolveLabeling(geometry1, labeling1));
		}

		[Test]
		public void TestSolveLabeling_PartialSolutionLabelsExistingPointWithWrongLabel_ThrowShapeMatchFailureException()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(2, 0)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) }
			});
			var labeling1 = new LabelingDictionary();
			labeling1.Add(new Point(-5, 2.1), 0);
			labeling1.Add(new Point(5, 10), 1);
			labeling1.Add(new Point(10, 10), 5);

			Assert.Throws<ShapeMatchFailureException>(() => shape1.SolveLabeling(geometry1, labeling1));
		}

		[Test]
		public void TestSolveLabeling_PartialSolutionLabelsNonExistantPointWithLabelInShape_ThrowShapeMatchFailureException()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(2, 0)
			});
			var geometry1 = new PolylinesGeometry(new List<List<Point>>
			{
				new List<Point> { new Point(-5, 2.1), new Point(20, 20) },
				new List<Point> { new Point(5, 10), new Point(20, 20) },
				new List<Point>{ new Point(5, 10), new Point(-5, 2.1) }
			});
			var labeling1 = new LabelingDictionary();
			labeling1.Add(new Point(-5, 2.1), 0);
			labeling1.Add(new Point(5, 10), 1);
			labeling1.Add(new Point(10, 10), 5);

			Assert.Throws<ShapeMatchFailureException>(() => shape1.SolveLabeling(geometry1, labeling1));
		}

		[Test]
		public void TestLabelsConnectedTo_LabelNotPartOfDefinition_ThrowsArgumentException()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(2, 0)
			});
			Assert.Throws<ArgumentException>(() => shape1.LabelsConnectedTo(100));
		}

		[Test]
		public void TestLabelsConnectedTo_LabelPartOfDefinition_CorrectOutput()
		{
			var shape1 = new Shape(new HashSet<Connection>
			{
				new Connection(0, 4),
				new Connection(4, 2),
				new Connection(2, 0)
			});
			var result = shape1.LabelsConnectedTo(0);
			Assert.AreEqual(2, result.Count);
			Assert.IsTrue(result.Contains(4));
			Assert.IsTrue(result.Contains(2));
		}
	}
}