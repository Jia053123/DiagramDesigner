using NUnit.Framework;
using ShapeGrammarEngine;
using System;

namespace ShapeGrammarEngineUnitTests
{
	public class ShapeTests
	{
		[Test]
		public void TestCreateShapeFromPolylines()
		{
			Assert.Throws<ArgumentNullException>(() => Shape.CreateShapeFromPolylines(null));
		}
	}
}