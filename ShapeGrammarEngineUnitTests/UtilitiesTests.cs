using NUnit.Framework;
using System;
using System.Collections.Generic;
using ShapeGrammarEngine;
using ListOperations;
using System.Linq;
using Svg;
using System.Drawing;
using MyPoint = BasicGeometries.Point;

namespace ShapeGrammarEngineUnitTests
{
	class UtilitiesTests
	{
		[Test]
		public void TestGenerateAllPermutations_EdgeCases()
		{
			Assert.Throws<ArgumentException>(() => Utilities.GenerateAllPermutations(new List<int>()));

			var result1 = Utilities.GenerateAllPermutations(new List<int> { 1 });
			Assert.AreEqual(1, result1.Count);
			Assert.IsTrue(ListUtilities.DoesContainList(result1, new List<int> { 1 }));

			var result2 = Utilities.GenerateAllPermutations(new List<int> { 1, 1 });
			Assert.AreEqual(1, result2.Count);
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 1, 1 }));
		}

		[Test]
		public void TestGenerateAllPermutations_NormalCases()
		{
			var result2 = Utilities.GenerateAllPermutations(Enumerable.Range(0, 3).ToList());
			Assert.AreEqual(6, result2.Count);
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 0, 1, 2 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 0, 2, 1 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 1, 0, 2 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 1, 2, 0 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 2, 0, 1 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 2, 1, 0 }));

			var result3 = Utilities.GenerateAllPermutations(new List<int> { 1, 3, 4 });
			Assert.AreEqual(6, result3.Count);
			Assert.IsTrue(ListUtilities.DoesContainList(result3, new List<int> { 1, 4, 3 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result3, new List<int> { 1, 3, 4 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result3, new List<int> { 4, 1, 3 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result3, new List<int> { 4, 3, 1 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result3, new List<int> { 3, 1, 4 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result3, new List<int> { 3, 4, 1 }));
		}

		[Test]
		public void TestGenerateAllPermutations_HasDuplicateEntries_OutputUniquePermutationsOnly()
		{
			var result1 = Utilities.GenerateAllPermutations(new List<int> { 2, 4, 2 });
			Assert.AreEqual(3, result1.Count);
			Assert.IsTrue(ListUtilities.DoesContainList(result1, new List<int> { 2, 2, 4 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result1, new List<int> { 2, 4, 2 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result1, new List<int> { 4, 2, 2 }));
		}

		[Test]
		public void TestCalculateVariance()
		{
			var data1 = new List<double> { 0, 0, 0, 0, 0 };
			Assert.AreEqual(0, Utilities.CalculateVariance(data1));

			var data2 = new List<double> { 1, 2, 3, 4, 5 };
			Assert.AreEqual(2, Utilities.CalculateVariance(data2));
		}

		[Test]
		public void TestPolylinesGeometryToSvg()
        {
			//  1      |\
			//  0      | >
			// -1      |/   
			//
			//         0  1       

			var testPolyline = new List<List<MyPoint>> {
				new List<MyPoint> { new MyPoint(0, -1), new MyPoint(0, 1), new MyPoint(1, 0), new MyPoint(0, -1) } };
			var geo = new PolylinesGeometry(testPolyline);

			SvgDocument shapeDoc = Utilities.PolylinesGeometryToSvg(geo, 128, 128);
			Bitmap bm = shapeDoc.Draw();
            string testResultsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GraphicsTestResults\\");
			_ = System.IO.Directory.CreateDirectory(testResultsDir);

			string fileName = nameof(this.TestPolylinesGeometryToSvg) + ".bmp";
			string testResultPath = System.IO.Path.Combine(testResultsDir, fileName);
			bm.Save(testResultPath);
			Console.WriteLine("Test result saved to: " + testResultPath);
		}
	}
}
