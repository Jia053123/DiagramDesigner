﻿using BasicGeometries;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine.UnitTests
{
	class LabelingDictionaryTests
	{
		[Test]
		public void TestAddAndGet_EdgeCases()
		{
			var labeling = new LabelingDictionary();
			Assert.Throws<ArgumentException>(() => labeling.GetLabelByPoint(new Point(0, 0)));
			Assert.Throws<ArgumentException>(() => labeling.GetPointByLabel(0));
		}
		
		[Test]
		public void TestDoesContainPair()
		{
			var labeling = new LabelingDictionary();
			Assert.IsFalse(labeling.DoesContainPair(new Point(0, 0), 2));

			_ = labeling.Add(new Point(0, 0), 2);
			Assert.IsTrue(labeling.DoesContainPair(new Point(0, 0), 2));
		}

		[Test]
		public void TestAddAndGet_NormalCases()
		{
			var labeling = new LabelingDictionary();
			var s1 = labeling.Add(new Point(0, 0), 2);
			Assert.IsTrue(s1);
			var s2 = labeling.Add(new Point(0, 0), 1);
			Assert.IsFalse(s2);
			var s3 = labeling.Add(new Point(1, 0), 2);
			Assert.IsFalse(s3);
			var s4 = labeling.Add(new Point(0, 1), 4);
			Assert.IsTrue(s4);

			Assert.AreEqual(2, labeling.GetLabelByPoint(new Point(0, 0)));
			Assert.AreEqual(new Point(0, 0), labeling.GetPointByLabel(2));
			Assert.AreEqual(new Point(0, 1), labeling.GetPointByLabel(4));
			Assert.Throws<ArgumentException>(() => labeling.GetLabelByPoint(new Point(1, 0)));
			Assert.Throws<ArgumentException>(() => labeling.GetPointByLabel(1));
		}

		[Test]
		public void TestCopy()
		{
			var labeling1 = new LabelingDictionary();
			_ = labeling1.Add(new Point(0, 0), 2);

			var labeling2 = labeling1.Copy();
			_ = labeling2.Add(new Point(1, 0), 3);

			Assert.Throws<ArgumentException>(() => labeling1.GetPointByLabel(3));
			Assert.AreEqual(new Point(1, 0), labeling2.GetPointByLabel(3));
			Assert.AreEqual(2, labeling2.GetLabelByPoint(new Point(0, 0)));
		}
	}
}
