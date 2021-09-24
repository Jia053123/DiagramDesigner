﻿using ListOperations;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ShapeGrammarEngine.UnitTests
{
	class GrammarRuleTests
	{
		[Test]
		public void TestLearnFromExample_NullInput_ThrowNullException()
		{
			var shape1 = Shape.CreateEmptyShape();
			var shape2 = Shape.CreateEmptyShape();
			var emptyRule = new GrammarRule(shape1, shape2);
			var emptyPolylineGroup = PolylineGroup.CreateEmptyPolylineGroup();

			Assert.Throws<ArgumentNullException>(() => emptyRule.LearnFromExample(null, null));
			Assert.Throws<ArgumentNullException>(() => emptyRule.LearnFromExample(emptyPolylineGroup, null));
			Assert.Throws<ArgumentNullException>(() => emptyRule.LearnFromExample(null, emptyPolylineGroup));
			Assert.DoesNotThrow(() => emptyRule.LearnFromExample(emptyPolylineGroup, emptyPolylineGroup));
		}

		[Test]
		public void TestApplyToGeometry_NullInput_ThrowNullException()
		{
			var shape1 = Shape.CreateEmptyShape();
			var shape2 = Shape.CreateEmptyShape();
			var emptyRule = new GrammarRule(shape1, shape2);
			var emptyPolylineGroup = PolylineGroup.CreateEmptyPolylineGroup();

			Assert.Throws<ArgumentNullException>(() => emptyRule.ApplyToGeometry(null));
			Assert.DoesNotThrow(() => emptyRule.ApplyToGeometry(emptyPolylineGroup));
		}

		[Test]
		public void TestApplyToGeometry_InputNotConformWithRule_ThrowArgumentException()
		{
			var shape1 = new Shape(new HashSet<Connection> { new Connection(1, 2) });
			var shape2 = new Shape(new HashSet<Connection> { new Connection(1, 2), new Connection(2, 3) });
			var rule1 = new GrammarRule(shape1, shape2);
			var polyGroup1 = new PolylineGroup(new List<List<(double, double)>> { new List<(double, double)> { (0, 0), (0, 1) } });
			Assert.Throws<ArgumentException>(() => rule1.ApplyToGeometry(polyGroup1));
		}

		[Test]
		public void TestApplyToGeometry_EmptyRuleAndInput_EmptyOutput()
		{
			var es1 = Shape.CreateEmptyShape();
			var es2 = Shape.CreateEmptyShape();
			var emptyRule = new GrammarRule(es1, es2);
			var emptyPolylineGroup = PolylineGroup.CreateEmptyPolylineGroup();
			var result = emptyRule.ApplyToGeometry(emptyPolylineGroup);
			Assert.IsTrue(result.IsEmpty());
		}

		[Test]
		public void TestApplyToGeometry_ValidInput_OutputConfromsWithRule()
		{

		}
	}
}
