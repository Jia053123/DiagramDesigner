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
			Assert.Throws<ArgumentNullException>(() => new RuleApplicationRecord(null, PolylineGeometry.CreateEmptyPolylineGeometry(), new Dictionary<Point, int>()));
			Assert.Throws<ArgumentNullException>(() => new RuleApplicationRecord(PolylineGeometry.CreateEmptyPolylineGeometry(), null, new Dictionary<Point, int>()));
			Assert.Throws<ArgumentNullException>(() => new RuleApplicationRecord(PolylineGeometry.CreateEmptyPolylineGeometry(), PolylineGeometry.CreateEmptyPolylineGeometry(), null));
			Assert.Throws<ArgumentNullException>(() => new RuleApplicationRecord(null, null, null));

		}
	}
}
