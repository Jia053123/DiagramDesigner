using BasicGeometries;
using ListOperations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

			Assert.Throws<ArgumentNullException>(() => emptyRule.LearnFromExample(null, null, out _));
			Assert.Throws<ArgumentNullException>(() => emptyRule.LearnFromExample(emptyPolylineGeo, null, out _));
			Assert.Throws<ArgumentNullException>(() => emptyRule.LearnFromExample(null, emptyPolylineGeo, out _));
			Assert.DoesNotThrow(() => emptyRule.LearnFromExample(emptyPolylineGeo, emptyPolylineGeo, out _));
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
		public void TestAssignAngleAndAssignLength__LabelsInNewGeometryLabelingNotInLeftOrRightHandShape_Throws()
		{
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			var geo1R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1)},
				new List<Point>{new Point(0,-1), new Point(1,-1) } });
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(geo1L, geo1R, out var oldLabeling);
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//    
			var newGeoL = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			rule.LeftHandShape.ConformsWithGeometry(newGeoL, out var newLabeling);
			newLabeling.Add(new Point(90, 90), 100);
			Assert.Throws<ArgumentException>(() => rule.AssignAngle(
				newLabeling, 
				oldLabeling.GetLabelByPoint(new Point(0, -1)),
				oldLabeling.GetLabelByPoint(new Point(1, -1))));
			Assert.Throws<ArgumentException>(() => rule.AssignLength(
				newLabeling,
				oldLabeling.GetLabelByPoint(new Point(0, -1)),
				oldLabeling.GetLabelByPoint(new Point(1, -1))));
		}

		[Test]
		public void TestAssignAngleAndAssignLength__LabelForExistingPointNotInGeometry_Throws()
		{
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			var geo1R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1)},
				new List<Point>{new Point(0,-1), new Point(1,-1) } });
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(geo1L, geo1R, out var oldLabeling);
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//    
			var newGeoL = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			rule.LeftHandShape.ConformsWithGeometry(newGeoL, out var newLabeling);
			var labelsList = oldLabeling.GetAllLabels().ToList();
			labelsList.Sort();
			var labelNotInNewLabeling = labelsList.Last() + 1;
			Assert.Throws<ArgumentException>(() => rule.AssignAngle(
				newLabeling, 
				labelNotInNewLabeling, 
				oldLabeling.GetLabelByPoint(new Point(1, -1))));
			Assert.Throws<ArgumentException>(() => rule.AssignLength(
				newLabeling,
				labelNotInNewLabeling,
				oldLabeling.GetLabelByPoint(new Point(1, -1))));
		}

		[Test]
		public void TestAssignAngleAndLength__LabelForPointToAssignNotInGeometry_Throws()
		{
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			var geo1R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1)},
				new List<Point>{new Point(0,-1), new Point(1,-1) } });
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(geo1L, geo1R, out var oldLabeling);
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//    
			var newGeoL = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			rule.LeftHandShape.ConformsWithGeometry(newGeoL, out var newLabeling);
			var labelsList = oldLabeling.GetAllLabels().ToList();
			labelsList.Sort();
			var labelNotInOldLabeling = labelsList.Last() + 1;
			Assert.Throws<ArgumentException>(() => rule.AssignAngle(
				oldLabeling, 
				oldLabeling.GetLabelByPoint(new Point(0, -1)), 
				labelNotInOldLabeling));
			Assert.Throws<ArgumentException>(() => rule.AssignLength(
			oldLabeling,
			oldLabeling.GetLabelByPoint(new Point(0, -1)),
			labelNotInOldLabeling));
		}

		[Test]
		public void TestAssignAngle__RepeatAssignment()
		{
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			var geo1R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1)},
				new List<Point>{new Point(0,-1), new Point(1,-1) } });
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(geo1L, geo1R, out var labeling);

			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//    
			var newGeoL = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			rule.LeftHandShape.ConformsWithGeometry(newGeoL, out var newLabeling);

			Assert.AreEqual(0, rule.AssignAngle(
				newLabeling, 
				labeling.GetLabelByPoint(new Point(0, -1)), 
				labeling.GetLabelByPoint(new Point(1, -1))
				));
		}

		[Test]
		public void TestAssignAngle_HandleDifferentProportions()
		{
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			var geo1R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1)},
				new List<Point>{new Point(0,-1), new Point(1,-1) } });
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(geo1L, geo1R, out var oldLabeling);

			//  ______________            _______________
			// |                         |          
			// |                   =>    |          
			// |                         |_______________
			//    
			var newGeoL = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(2,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			rule.LeftHandShape.ConformsWithGeometry(newGeoL, out var newLabeling);

			Assert.AreEqual(0, rule.AssignAngle(
				newLabeling,
				oldLabeling.GetLabelByPoint(new Point(0, -1)),
				oldLabeling.GetLabelByPoint(new Point(1, -1))));
		}

		[Test]
		public void TestAssignAngle_HandleDifferentProportionsAndOrders()
		{
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			var geo1R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,-1), new Point(0,0)},
				new List<Point>{new Point(1,-1), new Point(0,-1) } });
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(geo1L, geo1R, out var oldLabeling1);

			//  ______________            _______________
			// |                         |          
			// |                   =>    |          
			// |                         |_________
			//   
			var geo2L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(2,0), new Point(0,0)},
				new List<Point>{new Point(0,-1), new Point(0,0) }});
			var geo2R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(2,0), new Point(0,0)},
				new List<Point>{new Point(0,0), new Point(0,-1)},
				new List<Point>{new Point(1,-1), new Point(0,-1) } });
			rule.LearnFromExample(geo2L, geo2R, out var oldLabeling2);

			//  _________            __________
			// |                    |          |
			// |              =>    |          |
			// |                    |          |
			// |                    |
			// |                    |
			//     
			var newGeoL = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,-2), new Point(0,0), new Point(1,0) }});
			rule.LeftHandShape.ConformsWithGeometry(newGeoL, out var newLabeling);

			var tolerance = 0.00001;
			var result = rule.AssignAngle(
				newLabeling,
				oldLabeling1.GetLabelByPoint(new Point(0, -1)),
				oldLabeling1.GetLabelByPoint(new Point(1, -1)));
			Assert.IsTrue(Math.Abs(-0.5*Math.PI - result) < tolerance);
		}

		[Test]
		public void TestAssignAngle_HandleDifferentProportionsAndOrders_VaryingAssignedAngle()
		{
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |
			//                       \
			//                         \
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			var geo1R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,-1), new Point(0,0)},
				new List<Point>{new Point(0.5,-1.5), new Point(0,-1) } });
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(geo1L, geo1R, out var oldLabeling1);

			//  ______________            _______________
			// |                         |          
			// |                   =>    |          
			// |                         |_________
			//   
			var geo2L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(2,0), new Point(0,0)},
				new List<Point>{new Point(0,-1), new Point(0,0) }});
			var geo2R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(2,0), new Point(0,0)},
				new List<Point>{new Point(0,0), new Point(0,-1)},
				new List<Point>{new Point(1,-1), new Point(0,-1) } });
			rule.LearnFromExample(geo2L, geo2R, out var oldLabeling2);

			//  _________            _________
			//           |         /|         |         
			//           |        / |         |         
			//           |   =>  /  |         |         
			//           |                    |
			//           |                    |
			//     
			var newGeoL = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(2,-2), new Point(2,0), new Point(0,0) }});
			rule.LeftHandShape.ConformsWithGeometry(newGeoL, out var newLabeling);

			for (int i = 0; i < 20; i++)
			{
				var result1 = rule.AssignAngle(
					newLabeling,
					oldLabeling2.GetLabelByPoint(new Point(0, -1)),
					oldLabeling2.GetLabelByPoint(new Point(1, -1)));
				Assert.IsTrue(Math.PI * -0.75 < result1);
				Assert.IsTrue(Math.PI * -0.5 > result1);
			}
		}

		[Test]
		public void TestAssignAngle_HandleDifferentProportionsAndOrders_ChooseTheBestReference()
		{
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			var geo1R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,-1), new Point(0,0)},
				new List<Point>{new Point(1,-1), new Point(0,-1) } });
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(geo1L, geo1R, out var oldLabeling1);

			//            
			// |\                        |\
			// |  \                =>    |  \         
			// |                         |
			// |                         |_________
			//
			var geo2L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(1,-1), new Point(0,0)},
				new List<Point>{new Point(0,-2), new Point(0,0) }});
			var geo2R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(1,-1), new Point(0,0)},
				new List<Point>{new Point(0,0), new Point(0,-2)},
				new List<Point>{new Point(2,-2), new Point(0,-2) } });
			rule.LearnFromExample(geo2L, geo2R, out var oldLabeling2);

			// \                    \
			//  \                    \         
			//   \              =>    \            |
			//    \                    \           |
			//     \_________           \__________|
			//
			var newGeoL = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(-2,-2), new Point(0,0), new Point(1,0) }});
			rule.LeftHandShape.ConformsWithGeometry(newGeoL, out var newLabeling);

			var tolerance = 0.00001;
			var result = rule.AssignAngle(
				newLabeling,
				oldLabeling1.GetLabelByPoint(new Point(0, -1)),
				oldLabeling1.GetLabelByPoint(new Point(1, -1)));
			Assert.IsTrue(Math.Abs(0.5 * Math.PI - result) < tolerance);
		}

		[Test]
		public void TestAssignLegnth_RepeatAssignment()
		{
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			var geo1R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1)},
				new List<Point>{new Point(0,-1), new Point(1,-1) } });
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(geo1L, geo1R, out var labeling);

			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//    
			var newGeoL = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			rule.LeftHandShape.ConformsWithGeometry(newGeoL, out var newLabeling);

			Assert.AreEqual(1, rule.AssignLength(
				newLabeling,
				labeling.GetLabelByPoint(new Point(0, -1)),
				labeling.GetLabelByPoint(new Point(1, -1))
				));
		}

		[Test]
		public void TestAssignLength_HandleDifferentProportionsAndOrder_VaryingAssignedLength()
		{
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			var geo1R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,-1), new Point(0,0)},
				new List<Point>{new Point(1,-1), new Point(0,-1) } });
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(geo1L, geo1R, out var oldLabeling1);

			//  _________              _________
			// |                      |          
			// |                =>    |          
			// |                      |_____
			//   
			var geo2L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(1,0), new Point(0,0)},
				new List<Point>{new Point(0,-1), new Point(0,0) }});
			var geo2R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(1,0), new Point(0,0)},
				new List<Point>{new Point(0,0), new Point(0,-1)},
				new List<Point>{new Point(0.5,-1), new Point(0,-1) } });
			rule.LearnFromExample(geo2L, geo2R, out var oldLabeling2);

			//  _________            _________
			//           |          |         |         
			//           |          |         |         
			//           |    =>    |         |         
			//           |          |         |
			//           |                    |
			//     
			var newGeoL = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(2,-2), new Point(2,0), new Point(0,0) }});
			rule.LeftHandShape.ConformsWithGeometry(newGeoL, out var newLabeling);

			for (int i = 0; i < 20; i++)
			{
				var result1 = rule.AssignLength(
				newLabeling,
				oldLabeling2.GetLabelByPoint(new Point(0, -1)),
				oldLabeling2.GetLabelByPoint(new Point(0.5, -1)));
				Assert.IsTrue(1 < result1);
				Assert.IsTrue(2 > result1);
			}
		}

		[Test]
		public void TestAssignLength_HandleDifferentProportionsAndOrders_ChooseTheBestReference()
		{
			//  _________            __________
			// |                    |          
			// |              =>    |          
			// |                    |__________
			//     
			var geo1L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,0), new Point(0,-1) }});
			var geo1R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(0,0), new Point(1,0)},
				new List<Point>{new Point(0,-1), new Point(0,0)},
				new List<Point>{new Point(1,-1), new Point(0,-1) } });
			var rule = GrammarRule.CreateGrammarRuleFromOneExample(geo1L, geo1R, out var oldLabeling1);

			//  ______________            _______________
			// |                         |          
			// |                   =>    |          
			// |                         |_________
			//   
			var geo2L = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(2,0), new Point(0,0)},
				new List<Point>{new Point(0,-2), new Point(0,0) }});
			var geo2R = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(2,0), new Point(0,0)},
				new List<Point>{new Point(0,0), new Point(0,-2)},
				new List<Point>{new Point(1,-2), new Point(0,-2) } });
			rule.LearnFromExample(geo2L, geo2R, out var oldLabeling2);

			// \                    \
			//  \                    \         
			//   \              =>    \            |
			//    \                    \           |
			//     \_________           \__________|
			//
			var newGeoL = new PolylineGeometry(new List<List<Point>> {
				new List<Point>{new Point(-2,-2), new Point(0,0), new Point(1,0) }});
			rule.LeftHandShape.ConformsWithGeometry(newGeoL, out var newLabeling);

			var tolerance = 0.00001;
			var result = rule.AssignAngle(
				newLabeling,
				oldLabeling1.GetLabelByPoint(new Point(0, -1)),
				oldLabeling1.GetLabelByPoint(new Point(1, -1)));
			Assert.IsTrue(Math.Abs(0.5 * Math.PI - result) < tolerance);
		}

		[Test]
		public void TestCalculateScoreForOneConnectionByDifference_NoDifference_ReturnZero()
		{
			var pastData1 = new List<(double, double)> { (1, 1), (3, 3), (5, 5) };
			Assert.AreEqual(0, GrammarRule.CalculateScoreForOneConnectionByDifference(pastData1));
		}

		[Test]
		public void TestCalculateScoreForOneConnectionByDifference_ConstantDifference_ReturnZero()
		{
			var pastData1 = new List<(double, double)> { (1, 3), (8, 10), (36, 38) };
			Assert.AreEqual(0, GrammarRule.CalculateScoreForOneConnectionByDifference(pastData1));
		}

		[Test]
		public void TestCalculateScoreForOneConnectionByDifference_VariableDifference()
		{
			var pastData1 = new List<(double, double)> { (2, 2), (5, 7), (20, 24) };
			Assert.AreEqual(-1 * 8 / 3.0, GrammarRule.CalculateScoreForOneConnectionByDifference(pastData1));
		}

		[Test]
		public void TestCalculateScoreForOneConnectionByRatio_OneToOneRatio_ReturnZero()
		{
			var pastData1 = new List<(double, double)> { (1, 1), (3, 3), (5, 5) };
			Assert.AreEqual(0, GrammarRule.CalculateScoreForOneConnectionByRatio(pastData1));
		}

		[Test]
		public void TestCalculateScoreForOneConnectionByRatio_ConsistantRatio_ReturnZero()
		{
			var pastData1 = new List<(double, double)> { (1, 2), (5, 10), (20, 40) };
			Assert.AreEqual(0, GrammarRule.CalculateScoreForOneConnectionByRatio(pastData1));
		}

		[Test]
		public void TestCalculateScoreForOneConnectionByRatio_VariableRatio()
		{
			var pastData1 = new List<(double, double)> { (2, 2), (5, 10), (20, 30) };
			Assert.AreEqual(-1 * 0.5/3, GrammarRule.CalculateScoreForOneConnectionByRatio(pastData1));
		}

		[Test]
		public void TestCalculateScoreForOneConnectionByRatio_ReferenceValueIsZero_Throws()
		{
			var pastData1 = new List<(double, double)> { (1, 2), (0, 2) };
			Assert.Throws<ArgumentException>(() => GrammarRule.CalculateScoreForOneConnectionByRatio(pastData1));
		}

		[Test]
		public void TestAssignValueBasedOnPastOccurancesByDifference_NoDifference()
		{
			var pastData1 = new List<(double, double)> { (1, 1), (2, 2), (100, 100) };
			Assert.AreEqual(50, GrammarRule.AssignValueBasedOnPastOccurancesByDifference(50, pastData1));
		}

		[Test]
		public void TestAssignValueBasedOnPastOccurancesByDifference_ConstantDifference()
		{
			var pastData1 = new List<(double, double)> { (5, 1), (27, 23), (104, 100) };
			Assert.AreEqual(46, GrammarRule.AssignValueBasedOnPastOccurancesByDifference(50, pastData1));
		}

		[Test]
		public void TestAssignValueBasedOnPastOccurancesByDifference_VariableDifference()
		{
			var pastData1 = new List<(double, double)> { (4, 3), (2, 3), (10, 13), (5, 3) };
			for (int i = 0; i < 20; i++) // since randomness is involved, repeat many times
			{
				var assignedValue = GrammarRule.AssignValueBasedOnPastOccurancesByDifference(100, pastData1);
				Assert.IsTrue(assignedValue > 98 && assignedValue < 103);
			}
		}

		[Test]
		public void TestAssignValueBasedOnPastOccurancesByRatio_OneToOneRatio()
		{
			var pastData1 = new List<(double, double)> { (1, 1), (2, 2), (100, 100) };
			Assert.AreEqual(50, GrammarRule.AssignValueBasedOnPastOccurancesByRatio(50, pastData1));
		}

		[Test]
		public void TestAssignValueBasedOnPastOccurancesByRatio_ConsistantRatio()
		{
			var pastData1 = new List<(double, double)> { (1, 2), (2, 4), (100, 200) };
			Assert.AreEqual(100, GrammarRule.AssignValueBasedOnPastOccurancesByRatio(50, pastData1));
		}

		[Test]
		public void TestAssignValueBasedOnPastOccurancesByRatio_VariableRatio()
		{
			var pastData1 = new List<(double, double)> { (1, 2), (2, 3), (10, 9), (5, 6) };
			for (int i = 0; i < 20; i++) // since randomness is involved, repeat many times
			{
				var assignedValue = GrammarRule.AssignValueBasedOnPastOccurancesByRatio(100, pastData1);
				Assert.IsTrue(assignedValue > 90 && assignedValue < 200);
			}
		}

		[Test]
		public void TestAssignValueBasedOnPastOccurancesByRatio_CasesInvolvingZero()
		{
			var pastData1 = new List<(double, double)> { (1, 2), (0, 2) };
			Assert.Throws<ArgumentException>(() => GrammarRule.AssignValueBasedOnPastOccurancesByRatio(50, pastData1));
		}
	}
}
