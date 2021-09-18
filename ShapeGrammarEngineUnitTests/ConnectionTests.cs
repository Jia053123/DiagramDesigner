using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ShapeGrammarEngine;

namespace ShapeGrammarEngineUnitTests
{
	public class ConnectionTests
	{
		[Test]
		public void TestConstructor()
		{
			Assert.Throws<ArgumentException>(() => new Connection(1, -2));
			Assert.Throws<ArgumentException>(() => new Connection(-1, -2));
			Assert.Throws<ArgumentException>(() => new Connection(-1, -1));
			Assert.Throws<ArgumentException>(() => new Connection(1, 1));

			var c1 = new Connection(0, 2);
			Assert.AreEqual(0, c1.LabelOfFirstNode);
			Assert.AreEqual(2, c1.LabelOfSecondNode);

			var c2 = new Connection(2, 1);
			Assert.AreEqual(1, c2.LabelOfFirstNode);
			Assert.AreEqual(2, c2.LabelOfSecondNode);
		}

		[Test]
		public void TestEquality()
		{
			var c1 = new Connection(0, 1);
			var c2 = new Connection(0, 1);
			Assert.IsTrue(c1 == c2);

			var c3 = new Connection(1, 2);
			var c4 = new Connection(2, 1);
			Assert.IsTrue(c3 == c4);
		}
	}
}
