using BasicGeometries;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine.UnitTests
{
	class RuleApplicationRecordTests
	{
		[Test]
		public void TestConstructor_NullParameters_ThrowNullArgumentException()
		{
			Assert.Throws<ArgumentNullException>(() => new RuleApplicationRecord(null, PolylinesGeometry.CreateEmptyPolylineGeometry(), new LabelingDictionary()));
			Assert.Throws<ArgumentNullException>(() => new RuleApplicationRecord(PolylinesGeometry.CreateEmptyPolylineGeometry(), null, new LabelingDictionary()));
			Assert.Throws<ArgumentNullException>(() => new RuleApplicationRecord(PolylinesGeometry.CreateEmptyPolylineGeometry(), PolylinesGeometry.CreateEmptyPolylineGeometry(), null));
			Assert.Throws<ArgumentNullException>(() => new RuleApplicationRecord(null, null, null));
		}
	}
}
