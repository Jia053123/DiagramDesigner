using NUnit.Framework;
using DiagramDesignerEngine;

namespace DiagramDesignerEngine.UnitTests
{
	public class PointTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void TestEqualities()
		{
			var p1 = new Point(1, 2);
			var p2 = new Point(1, 2);
			
			var p3 = new Point(1, 2.02);
			var p4 = new Point(0.9, 2);

			var p5 = new Point(1.0004, 1.9991);

			Point p6 = null;

			Assert.AreEqual(p1, p2);
			Assert.AreNotEqual(p3, p1);
			Assert.AreNotEqual(p1, p4);
			Assert.AreEqual(p1, p5);
			Assert.AreNotEqual(p1, p6);

			Assert.IsTrue(p1 == p2);
			Assert.IsTrue(p1 != p3);
			Assert.IsTrue(p4 != p1);
			Assert.IsTrue(p1 == p5);
			Assert.IsTrue(p1 != p6);
			Assert.IsTrue(p6 != p1);

			Assert.IsTrue(p1.Equals(p2));
			Assert.IsFalse(p1.Equals(p3));
			Assert.IsFalse(p1.Equals(p6));
		}
	}
}