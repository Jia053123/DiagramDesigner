using NUnit.Framework;
using System;

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
	}
}
