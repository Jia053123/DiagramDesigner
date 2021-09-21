using NUnit.Framework;
using System.Collections.Generic;

namespace ShapeGrammarEngine.UnitTests
{
	class PolylineGroupTests
	{
		[Test]
		public void TestDoesIntersectOrOverlapWithItself_Intersect()
		{
			Assert.IsTrue(new PolylineGroup(new List<List<(double, double)>> { new List<(double, double)> { (0, -1), (0, 1), (1, 0), (-1, 0) } }).DoesIntersectOrOverlapWithItself());
			Assert.IsTrue(new PolylineGroup(new List<List<(double, double)>> { new List<(double, double)> { (1, 0), (1, 2) }, new List<(double, double)> { (0, 1), (2, 1) } }).DoesIntersectOrOverlapWithItself());
		}

		[Test]
		public void TestDoesIntersectOrOverlapWithItself_Overlap()
		{
			Assert.IsTrue(new PolylineGroup(new List<List<(double, double)>> { new List<(double, double)> { (0, -1), (0, 1), (0, 0), (0, 0.5) } }).DoesIntersectOrOverlapWithItself());
		}

		[Test]
		public void TestConvertToConnections()
		{
			var geometry1 = new PolylineGroup(new List<List<(double X, double Y)>> {
				new List<(double X, double Y)> { (-5, 2.1), (20, 20) },
				new List<(double X, double Y)> { (5, 10), (20, 20) },
				new List<(double X, double Y)>{ (5, 10), (-5, 2.1), (-6, -6) } });

			var dic = new Dictionary<(double X, double Y), int>();
			dic.Add((-5, 2.1), 1);
			dic.Add((20, 20), 2);
			dic.Add((5, 10), 3);
			dic.Add((-6, -6), 4);

			var cs = geometry1.ConvertToConnections(dic);
			Assert.AreEqual(4, cs.Count);
			Assert.IsTrue(cs.Contains(new Connection(1, 2)));
			Assert.IsTrue(cs.Contains(new Connection(3, 2)));
			Assert.IsTrue(cs.Contains(new Connection(3, 1)));
			Assert.IsTrue(cs.Contains(new Connection(1, 4)));
		}
	}
}
