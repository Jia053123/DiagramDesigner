using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ShapeGrammarEngine;
using ListOperations;

namespace ShapeGrammarEngineUnitTests
{
	class UtilitiesTests
	{
		[Test]
		public void TestGenerateAllPermutations_EdgeCases()
		{
			Assert.Throws<ArgumentException>(() => Utilities.GenerateAllPermutations(-1, 2));

			var result1 = Utilities.GenerateAllPermutations(0, 0);
			Assert.AreEqual(1, result1.Count);
			Assert.IsTrue(ListUtilities.DoesContainList(result1, new List<int> { 0 }));
		}

		[Test]
		public void TestGenerateAllPermutations_NormalCases()
		{
			var result2 = Utilities.GenerateAllPermutations(0, 2);
			Assert.AreEqual(6, result2.Count);
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 0, 1, 2 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 0, 2, 1 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 1, 0, 2 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 1, 2, 0 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 2, 0, 1 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result2, new List<int> { 2, 1, 0 }));

			var result3 = Utilities.GenerateAllPermutations(3, 1);
			Assert.AreEqual(6, result3.Count);
			Assert.IsTrue(ListUtilities.DoesContainList(result3, new List<int> { 1, 2, 3 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result3, new List<int> { 1, 3, 2 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result3, new List<int> { 2, 1, 3 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result3, new List<int> { 2, 3, 1 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result3, new List<int> { 3, 1, 2 }));
			Assert.IsTrue(ListUtilities.DoesContainList(result3, new List<int> { 3, 2, 1 }));
		}
	}
}
