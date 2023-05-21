﻿using NUnit.Framework;
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
		[Test]
		public void TestGenerateVariations_RunWithoutError()
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
			var variations = MachineLearningUtilities.GenerateVariations(geo1L, geo1R, 300, 250, 20);
			Assert.IsTrue(variations.variationsForGeometryBefore.Count > 0);
			Assert.IsTrue(variations.variationsForGeometryAfter.Count > 0);
			// TODO: better tests? 
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

			string testResultsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GraphicsTestResults\\");
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

			string testResultsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GraphicsTestResults\\");
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

			string testResultsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GraphicsTestResults\\");
			_ = System.IO.Directory.CreateDirectory(testResultsDir);

			string fileName = nameof(this.TestPolylinesGeometryToSvgPadded) + ".bmp";
			string testResultPath = System.IO.Path.Combine(testResultsDir, fileName);
			bm.Save(testResultPath);
			Console.WriteLine("Test result saved to: " + testResultPath);
		}
	}
}
