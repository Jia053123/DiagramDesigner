﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine.UnitTests
{
	class TestUtilitiesTest
	{
		[Test]
		public void TestAreContentsEqual()
		{
			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, 0));
			var ls4 = new LineSegment(new Point(1, 0), new Point(1, -2));

			var ls5 = new LineSegment(new Point(1, -2), new Point(-1, -2));
			var ls6 = new LineSegment(new Point(1, 0), new Point(0.5, 0.5));
			var ls7 = new LineSegment(new Point(1, 0), new Point(0.5, -0.5));
			var ls8 = new LineSegment(new Point(0.5, -0.5), new Point(0.5, 0.5));

			var list1 = new List<LineSegment> { ls1, ls3, ls2 };
			var list2 = new List<LineSegment> { ls3, ls2, ls1 };
			Assert.IsTrue(TestUtilities.AreContentsEqual(list1, list2));

			var list3 = new List<LineSegment> { ls3, ls2, ls1, ls4 };
			Assert.IsFalse(TestUtilities.AreContentsEqual(list1, list3));

			var list4 = new List<LineSegment>();
			var list5 = new List<LineSegment>();
			Assert.IsTrue(TestUtilities.AreContentsEqual(list4, list5));
		}
	}
}
