using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using DiagramDesignerEngine;

namespace DiagramDesignerEngine.UnitTests
{
	class LineSegmentTests
	{
		[Test]
		public void TestConstructor()
		{
			var p1 = new Point(1, -2);
			var p2 = new Point(1, -2);
			var p3 = new Point(2, -1);
			Assert.Throws<ArgumentException>(() => new LineSegment(null, p1));
			Assert.Throws<ArgumentException>(() => new LineSegment(p1, p2));
			Assert.DoesNotThrow(() => new LineSegment(p1, p3));
		}
	}
}
