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
			Assert.Throws<ArgumentNullException>(() => new RuleApplicationRecord(null, PolylineGroup.CreateEmptyPolylineGroup()));
			Assert.Throws<ArgumentNullException>(() => new RuleApplicationRecord(PolylineGroup.CreateEmptyPolylineGroup(), null));
			Assert.Throws<ArgumentNullException>(() => new RuleApplicationRecord(null, null));

		}
	}
}
