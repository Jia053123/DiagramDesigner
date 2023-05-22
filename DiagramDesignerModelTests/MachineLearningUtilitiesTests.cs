using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Svg;
using Svg.Pathing;
using System.Drawing;
using MyPoint = BasicGeometries.Point;

namespace ShapeGrammarEngine.UnitTests
{
    class MachineLearningUtilitiesTests
    {
		string testResultsBaseDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GraphicsTestResults");

		[Test]
		public void TestGenerateVariations()
        {
			//  10        100        10         100
			//    _________            __________
			//10 |                    |          
			//   |              =>    |          
			//   |                    |__________
			//100
			//

			var geo1L = new PolylinesGeometry(new List<List<MyPoint>> {
				new List<MyPoint>{new MyPoint(10,10), new MyPoint(100,10)},
				new List<MyPoint>{new MyPoint(10,10), new MyPoint(10,100) }});
			var geo1R = new PolylinesGeometry(new List<List<MyPoint>> {
				new List<MyPoint>{new MyPoint(10,10), new MyPoint(100,10)},
				new List<MyPoint>{new MyPoint(10,10), new MyPoint(10,100)},
				new List<MyPoint>{new MyPoint(10,100), new MyPoint(100,100) } });

			const int numOfVariations = 20;
			const int canvasWidth = 300;
			const int canvasHeight = 250;
			var variations = MachineLearningUtilities.GenerateVariations(geo1L, geo1R, canvasWidth, canvasHeight, numOfVariations);
			Console.WriteLine(variations.variationsForGeometryBefore.Count);
			Console.WriteLine(variations.variationsForGeometryAfter.Count);

			var firstResult = variations.variationsForGeometryBefore[0];
			var svgDoc = MachineLearningUtilities.PolylinesGeometryToSvgOnCanvas(firstResult, canvasWidth, canvasHeight, 100, 100, 2);
			Console.WriteLine(((SvgPath)svgDoc.Children[0]).PathData.ToString());

			string testResultsDir1 = System.IO.Path.Combine(testResultsBaseDir, "TestGenerateVariations_Before\\");
			MachineLearningUtilities.BatchRenderToSvgAndWriteToDirectory(variations.variationsForGeometryBefore, canvasWidth, canvasHeight, 100, 100, 2, testResultsDir1);
			string testResultsDir2 = System.IO.Path.Combine(testResultsBaseDir, "TestGenerateVariations_After\\");
			MachineLearningUtilities.BatchRenderToSvgAndWriteToDirectory(variations.variationsForGeometryAfter, canvasWidth, canvasHeight, 100, 100, 2, testResultsDir2);
		}

		[Test]
		public void TestBatchRenderToSvgAndWriteToDirectory()
        {
			//  10        100        10         100
			//    _________            __________
			//10 |                    |          
			//   |                    |          
			//   |                    |__________
			//100
			//
			var geo1 = new PolylinesGeometry(new List<List<MyPoint>> {
				new List<MyPoint>{new MyPoint(10,10), new MyPoint(100,10)},
				new List<MyPoint>{new MyPoint(10,10), new MyPoint(10,100) }});
			var geo2 = new PolylinesGeometry(new List<List<MyPoint>> {
				new List<MyPoint>{new MyPoint(10,10), new MyPoint(100,10)},
				new List<MyPoint>{new MyPoint(10,10), new MyPoint(10,100)},
				new List<MyPoint>{new MyPoint(10,100), new MyPoint(100,100) } });

			string testResultsDir = System.IO.Path.Combine(testResultsBaseDir, nameof(this.TestBatchRenderToSvgAndWriteToDirectory) + "\\");
			MachineLearningUtilities.BatchRenderToSvgAndWriteToDirectory(new List<PolylinesGeometry> { geo1, geo2 }, 300, 250, 100, 100, 2, testResultsDir);
			Console.WriteLine("Test result saved to: " + testResultsDir);
		}
		

		[Test]
		public void TestPolylinesGeometryToSvgOnCanvas()
		{
			//  10       /|
			//  100     / /
			//  200     v    
			//
			//         10   100       

			var testPolyline = new List<List<MyPoint>> {
				new List<MyPoint> { new MyPoint(10, 200), new MyPoint(100, 10), new MyPoint(100, 100), new MyPoint(10, 200) } };
			var geo = new PolylinesGeometry(testPolyline);

			SvgDocument polyGeoDoc = MachineLearningUtilities.PolylinesGeometryToSvgOnCanvas(geo, 500, 600, 128, 128, 2);
			Assert.AreEqual("M3 43 L26 2 L26 21 L3 43", ((SvgPath)polyGeoDoc.Children[0]).PathData.ToString());

			Bitmap bm = polyGeoDoc.Draw();
			Assert.AreEqual(128, bm.Width);
			Assert.AreEqual(128, bm.Height);

			string testResultsDir = testResultsBaseDir;
			_ = System.IO.Directory.CreateDirectory(testResultsDir);
			string fileName = nameof(this.TestPolylinesGeometryToSvgOnCanvas) + ".bmp";
			string testResultPath = System.IO.Path.Combine(testResultsDir, fileName);
			bm.Save(testResultPath);
			Console.WriteLine("Test result saved to: " + testResultPath);
		}
		[Test]
		public void TestPolylinesGeometryToSvg()
		{
			//  1        /|
			//  0       / /
			// -1       v    
			//
			//         0   1       

			var testPolyline = new List<List<MyPoint>> {
				new List<MyPoint> { new MyPoint(0, -1), new MyPoint(1, 1), new MyPoint(1, 0), new MyPoint(0, -1) } };
			var geo = new PolylinesGeometry(testPolyline);

			SvgDocument polyGeoDoc = MachineLearningUtilities.PolylinesGeometryToSvg(geo, 128, 128, 2);
			Assert.AreEqual("M0 128 L128 0 L128 64 L0 128", ((SvgPath)polyGeoDoc.Children[0]).PathData.ToString());

			Bitmap bm = polyGeoDoc.Draw();
			Assert.AreEqual(128, bm.Width);
			Assert.AreEqual(128, bm.Height);

			string testResultsDir = testResultsBaseDir;
			_ = System.IO.Directory.CreateDirectory(testResultsDir);
			string fileName = nameof(this.TestPolylinesGeometryToSvg) + ".bmp";
			string testResultPath = System.IO.Path.Combine(testResultsDir, fileName);
			bm.Save(testResultPath);
			Console.WriteLine("Test result saved to: " + testResultPath);
		}

		[Test]
		public void TestPolylinesGeometryToSvgPadded()
		{
			//  1        /|
			//  0       / /
			// -1       v    
			//
			//         0   1       

			var testPolyline = new List<List<MyPoint>> {
				new List<MyPoint> { new MyPoint(0, -1), new MyPoint(1, 1), new MyPoint(1, 0), new MyPoint(0, -1) } };
			var geo = new PolylinesGeometry(testPolyline);

			SvgDocument polyGeoDoc = MachineLearningUtilities.PolylinesGeometryToSvgPadded(geo, 128, 128, 2);
			Assert.AreEqual("M1 127 L127 1 L127 64 L1 127", ((SvgPath)polyGeoDoc.Children[0]).PathData.ToString());

			Bitmap bm = polyGeoDoc.Draw();
			Assert.AreEqual(128, bm.Width);
			Assert.AreEqual(128, bm.Height);

			string testResultsDir = testResultsBaseDir;
			_ = System.IO.Directory.CreateDirectory(testResultsDir);

			string fileName = nameof(this.TestPolylinesGeometryToSvgPadded) + ".bmp";
			string testResultPath = System.IO.Path.Combine(testResultsDir, fileName);
			bm.Save(testResultPath);
			Console.WriteLine("Test result saved to: " + testResultPath);
		}
	}
}
