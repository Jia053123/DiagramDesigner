using BasicGeometries;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ListOperations.UnitTests
{
	class ListUtilitiesTest
	{
		[Test]
		public void TestDoesContainList()
		{
			var contained1 = new List<int> { 1, 2, 3 };
			var container1 = new List<List<int>>();
			Assert.IsFalse(ListUtilities.DoesContainList(container1, contained1));

			var container2 = new List<List<int>> { new List<int> { 2, 3, 4 } };
			Assert.IsFalse(ListUtilities.DoesContainList(container2, contained1));

			var container3 = new List<List<int>> { new List<int> { 1, 3, 2 } };
			Assert.IsFalse(ListUtilities.DoesContainList(container3, contained1));

			var container4 = new List<List<int>> { new List<int> { 2, 3, 4 }, new List<int> { 1, 2, 3 } };
			Assert.IsTrue(ListUtilities.DoesContainList(container4, contained1));
		}

		[Test]
		public void TestAreContentsEqual()
		{
			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, 0));
			var ls4 = new LineSegment(new Point(1, 0), new Point(1, -2));

			var list1 = new List<LineSegment> { ls1, ls3, ls2 };
			var list2 = new List<LineSegment> { ls3, ls2, ls1 };
			Assert.IsTrue(ListUtilities.AreContentsEqual(list1, list2));

			var list3 = new List<LineSegment> { ls3, ls2, ls1, ls4 };
			Assert.IsFalse(ListUtilities.AreContentsEqual(list1, list3));

			var list4 = new List<LineSegment>();
			var list5 = new List<LineSegment>();
			Assert.IsTrue(ListUtilities.AreContentsEqual(list4, list5));
		}

		[Test]
		public void TestAreContentsEqualInOrder()
		{
			var ls1 = new LineSegment(new Point(-1, -2), new Point(-1, 2));
			var ls2 = new LineSegment(new Point(-1, 2), new Point(1, 2));
			var ls3 = new LineSegment(new Point(1, 2), new Point(1, 0));
			var ls4 = new LineSegment(new Point(1, 0), new Point(1, -2));

			var list1 = new List<LineSegment> { ls1, ls3, ls2 };
			var list2 = new List<LineSegment> { ls3, ls2, ls1 };
			Assert.IsFalse(ListUtilities.AreContentsEqualInOrder(list1, list2));

			var list3 = new List<LineSegment> { ls1, ls3, ls2 };
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(list1, list3));

			var list4 = new List<LineSegment>();
			var list5 = new List<LineSegment>();
			Assert.IsTrue(ListUtilities.AreContentsEqualInOrder(list4, list5));
		}
	}
}
