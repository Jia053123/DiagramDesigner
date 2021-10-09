using BasicGeometries;
using ListOperations;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ShapeGrammarEngine.UnitTests
{
	class GrammarRuleTests
	{
		[Test]
		public void TestCreateGrammarRuleFromOneExample_OnePolyline_1()
		{
			var polyGeo1 = new PolylineGeometry(new List<List<Point>> { 
				new List<Point> { new Point(0, 0), new Point(0, 1) } });
			var polyGeo2 = new PolylineGeometry(new List<List<Point>> {
				new List<Point> { new Point(0, 0),  new Point(0, 2), new Point(0, 3) } });

			LabelingDictionary labeling;
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(polyGeo1, polyGeo2, out labeling);

			Assert.IsTrue(labeling.GetAllPoints().Contains(new Point(0, 0)));
			int label0 = labeling.GetLabelByPoint(new Point(0, 0));

			Assert.IsTrue(labeling.GetAllPoints().Contains(new Point(0, 1)));
			int label1 = labeling.GetLabelByPoint(new Point(0, 1));

			Assert.IsTrue(labeling.GetAllPoints().Contains(new Point(0, 2)));
			int label2 = labeling.GetLabelByPoint(new Point(0, 2));

			Assert.IsTrue(labeling.GetAllPoints().Contains(new Point(0, 3)));
			int label3 = labeling.GetLabelByPoint(new Point(0, 3));

			Assert.IsTrue(rule.LeftHandShape.ConformsWithGeometry(polyGeo1, out _));
			Assert.IsTrue(rule.RightHandShape.ConformsWithGeometry(polyGeo2, out _));

			Assert.IsTrue(rule.LeftHandShape.DefiningConnections.Contains(new Connection(label0, label1)));
			Assert.IsTrue(rule.RightHandShape.DefiningConnections.Contains(new Connection(label0, label2)));
			Assert.IsTrue(rule.RightHandShape.DefiningConnections.Contains(new Connection(label2, label3)));

			Assert.AreEqual(1, rule.ApplicationRecords.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(polyGeo1.PolylinesCopy, rule.ApplicationRecords[0].GeometryBefore.PolylinesCopy));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(polyGeo2.PolylinesCopy, rule.ApplicationRecords[0].GeometryAfter.PolylinesCopy));
		}

		[Test]
		public void TestCreateGrammarRuleFromOneExample_MultiplePolylines_1()
		{
			var polyGeo1 = new PolylineGeometry(new List<List<Point>> {
				new List<Point> { new Point(0, 0), new Point(1, 1) }, 
				new List<Point> { new Point(0, 0), new Point(0, 2) } });
			var polyGeo2 = new PolylineGeometry(new List<List<Point>> {
				new List<Point> { new Point(0, 0),  new Point(0, 2), new Point(0, 3) } });

			LabelingDictionary labeling;
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(polyGeo1, polyGeo2, out labeling);

			Assert.IsTrue(labeling.GetAllPoints().Contains(new Point(0, 0)));
			int label00 = labeling.GetLabelByPoint(new Point(0, 0));

			Assert.IsTrue(labeling.GetAllPoints().Contains(new Point(1, 1)));
			int label11 = labeling.GetLabelByPoint(new Point(1, 1));

			Assert.IsTrue(labeling.GetAllPoints().Contains(new Point(0, 2)));
			int label02 = labeling.GetLabelByPoint(new Point(0, 2));

			Assert.IsTrue(labeling.GetAllPoints().Contains(new Point(0, 3)));
			int label03 = labeling.GetLabelByPoint(new Point(0, 3));

			Assert.IsTrue(rule.LeftHandShape.ConformsWithGeometry(polyGeo1, out _));
			Assert.IsTrue(rule.RightHandShape.ConformsWithGeometry(polyGeo2, out _));

			Assert.IsTrue(rule.LeftHandShape.DefiningConnections.Contains(new Connection(label00, label11)));
			Assert.IsTrue(rule.LeftHandShape.DefiningConnections.Contains(new Connection(label00, label02)));
			Assert.IsTrue(rule.RightHandShape.DefiningConnections.Contains(new Connection(label00, label02)));
			Assert.IsTrue(rule.RightHandShape.DefiningConnections.Contains(new Connection(label02, label03)));

			Assert.AreEqual(1, rule.ApplicationRecords.Count);
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(polyGeo1.PolylinesCopy, rule.ApplicationRecords[0].GeometryBefore.PolylinesCopy));
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(polyGeo2.PolylinesCopy, rule.ApplicationRecords[0].GeometryAfter.PolylinesCopy));
		}

		[Test]
		public void TestLearnFromExample_NullInput_ThrowNullException()
		{
			var shape1 = Shape.CreateEmptyShape();
			var shape2 = Shape.CreateEmptyShape();
			var emptyRule = new GrammarRule(shape1, shape2);
			var emptyPolylineGeo = PolylineGeometry.CreateEmptyPolylineGeometry();

			Assert.Throws<ArgumentNullException>(() => emptyRule.LearnFromExample(null, null));
			Assert.Throws<ArgumentNullException>(() => emptyRule.LearnFromExample(emptyPolylineGeo, null));
			Assert.Throws<ArgumentNullException>(() => emptyRule.LearnFromExample(null, emptyPolylineGeo));
			Assert.DoesNotThrow(() => emptyRule.LearnFromExample(emptyPolylineGeo, emptyPolylineGeo));
		}

		[Test]
		public void TestApplyToGeometry_NullInput_ThrowNullException()
		{
			var shape1 = Shape.CreateEmptyShape();
			var shape2 = Shape.CreateEmptyShape();
			var emptyRule = new GrammarRule(shape1, shape2);
			var emptyPolylineGeo = PolylineGeometry.CreateEmptyPolylineGeometry();

			Assert.Throws<ArgumentNullException>(() => emptyRule.ApplyToGeometry(null));
			Assert.DoesNotThrow(() => emptyRule.ApplyToGeometry(emptyPolylineGeo));
		}

		[Test]
		public void TestApplyToGeometry_InputNotConformWithRule_ThrowArgumentException()
		{
			var shape1 = new Shape(new HashSet<Connection> { new Connection(1, 2) });
			var shape2 = new Shape(new HashSet<Connection> { new Connection(1, 2), new Connection(2, 3) });
			var rule1 = new GrammarRule(shape1, shape2);
			var polyGeo1 = new PolylineGeometry(new List<List<Point>> { new List<Point> { new Point(0, 0), new Point(0, 1), new Point(0, 2) } });
			Assert.Throws<ArgumentException>(() => rule1.ApplyToGeometry(polyGeo1));
		}

		[Test]
		public void TestApplyToGeometry_EmptyRuleAndInput_EmptyOutput()
		{
			var es1 = Shape.CreateEmptyShape();
			var es2 = Shape.CreateEmptyShape();
			var emptyRule = new GrammarRule(es1, es2);
			var emptyPolylineGeo = PolylineGeometry.CreateEmptyPolylineGeometry();
			var result = emptyRule.ApplyToGeometry(emptyPolylineGeo);
			Assert.IsTrue(result.IsEmpty());
		}

		[Test]
		public void TestApplyToGeometry_ValidInput_OutputConfromsWithRule_1()
		{
			var shape1 = new Shape(new HashSet<Connection> { new Connection(1, 2) });
			var shape2 = new Shape(new HashSet<Connection> { new Connection(1, 2), new Connection(2, 3) });
			var rule1 = new GrammarRule(shape1, shape2);
			var polyGeo1 = new PolylineGeometry(new List<List<Point>> { new List<Point> { new Point(0, 0), new Point(0, 1) } });
			var result1 = rule1.ApplyToGeometry(polyGeo1);
			Assert.IsTrue(shape2.ConformsWithGeometry(result1, out _));
		}

		[Test]
		public void TestAssignAngle__FaultyInput()
		{
			var pastLeftHandGeos = new List<PolylineGeometry>();
			var pastExistingPs = new List<Point>();
			var pastAssignedPs = new List<Point>();
		  
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			pastLeftHandGeos.Add(geo1L);
			pastExistingPs.Add(new Point(0, -100)); // not in the geometry
			pastAssignedPs.Add(new Point(1, -1));
			Assert.Throws<ArgumentException>(() => GrammarRule.AssignAngle(new Point(0, -5), pastLeftHandGeos, pastExistingPs, pastAssignedPs));
		}

		[Test]
		public void TestAssignAngle__SingleExample()
		{
			var pastLeftHandGeos = new List<PolylineGeometry>();
			var pastExistingPs = new List<Point>();
			var pastAssignedPs = new List<Point>();
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			pastLeftHandGeos.Add(geo1L);
			pastExistingPs.Add(new Point(0, -1));
			pastAssignedPs.Add(new Point(1, -1));

			Assert.AreEqual(0, GrammarRule.AssignAngle(new Point(0, -5), pastLeftHandGeos, pastExistingPs, pastAssignedPs));
		}

		[Test]
		public void TestAssignAngle_MultipleExamples_HandleDifferentProportions()
		{
			var pastLeftHandGeos = new List<PolylineGeometry>();
			var pastExistingPs = new List<Point>();
			var pastAssignedPs = new List<Point>();
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			pastLeftHandGeos.Add(geo1L);
			pastExistingPs.Add(new Point(0, -1));
			pastAssignedPs.Add(new Point(1, -1));

			//  ______________            _______________
			// |                         |          
			// |                   =>    |          
			// |                         |_______________
			//    
			var geo2L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(2,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			pastLeftHandGeos.Add(geo2L);
			pastExistingPs.Add(new Point(0, -1));
			pastAssignedPs.Add(new Point(2, -1));

			Assert.AreEqual(0, GrammarRule.AssignAngle(new Point(1, -4), pastLeftHandGeos, pastExistingPs, pastAssignedPs));
		}

		[Test]
		public void TestAssignAngle_MultipleExamples_HandleDifferentProportionsAndOrders()
		{
			var pastLeftHandGeos = new List<PolylineGeometry>();
			var pastExistingPs = new List<Point>();
			var pastAssignedPs = new List<Point>();
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			pastLeftHandGeos.Add(geo1L);
			pastExistingPs.Add(new Point(0, -1));
			pastAssignedPs.Add(new Point(1, -1));

			//  ______________            _______________
			// |                         |          
			// |                   =>    |          
			// |                         |_________
			//   
			var geo2L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(2,0), new Point(0,0)},
				new List<Point>{new Point(0,-2), new Point(0,0) }});
			pastLeftHandGeos.Add(geo2L);
			pastExistingPs.Add(new Point(0, -1));
			pastAssignedPs.Add(new Point(1, -1));

			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |
			// |                    |
			// |                    |__________________
			//     
			var geo3L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,-2), new Point(0,0), new Point(1,0) }});
			pastLeftHandGeos.Add(geo3L);
			pastExistingPs.Add(new Point(0, -2));
			pastAssignedPs.Add(new Point(2, -2));

			Assert.AreEqual(0, GrammarRule.AssignAngle(new Point(-1, -4), pastLeftHandGeos, pastExistingPs, pastAssignedPs));
		}

		[Test]
		public void TestAssignAngle_MultipleExamples_HandleDifferentProportionsAndOrders_NonOrthogonal()
		{
			Assert.Fail();
		}

		[Test]
		public void TestAssignLegnth()
		{
			Assert.Fail();
		}
	}
}
