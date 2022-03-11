using System;
using System.Collections.Generic;
using System.Text;
using DiagramDesignerModel;
using NUnit.Framework;
using MyPoint = BasicGeometries.Point;

namespace DiagramDesignerModel.UnitTests
{
	class EntitiesOperatorTests
	{
		List<WallEntity> wallEntities1;

		[SetUp] 
		public void SetUp()
		{
			var we1 = new WallEntity(2);
			we1.Geometry.PathsDefinedByPoints.AddRange(new List<MyPoint> { new MyPoint(1, 1), new MyPoint(2, 2), new MyPoint(3, 3) });
			wallEntities1.Add(we1);

			var we2 = new WallEntity(2);
			we2.Geometry.PathsDefinedByPoints.AddRange(new List<MyPoint> { new MyPoint(2, 2), new MyPoint(2, 3), new MyPoint(2, 4), new MyPoint(2, 5) });
			wallEntities1.Add(we2);

			var we3 = new WallEntity(2);
			we3.Geometry.PathsDefinedByPoints.AddRange(new List<MyPoint> { new MyPoint(1, 0), new MyPoint(2, 0) });
			wallEntities1.Add(we3);
		}

		[TearDown]
		public void TearDown()
		{
			wallEntities1 = null;
		}

		[Test]
		public void TestAddPointToWallEntityAtIndex_IndexOutOfBound_ThrowException()
		{
		}
	}
}
