﻿using System;
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
		List<WallEntity> wallEntities2;
		List<WallEntity> wallEntities3;

		[SetUp] 
		public void SetUp()
		{
			wallEntities1 = new List<WallEntity>();
			var we11 = new WallEntity(3);
			wallEntities1.Add(we11);
			var we12 = new WallEntity(3);
			we12.Geometry.PathsDefinedByPoints.AddRange(new List<MyPoint> { new MyPoint(1, 1), new MyPoint(2, 2)});
			wallEntities1.Add(we12);

			wallEntities2 = new List<WallEntity>();
			var we21 = new WallEntity(2);
			we21.Geometry.PathsDefinedByPoints.AddRange(new List<MyPoint> { new MyPoint(1, 1), new MyPoint(2, 2), new MyPoint(3, 3) });
			wallEntities2.Add(we21);
			var we22 = new WallEntity(2);
			we22.Geometry.PathsDefinedByPoints.AddRange(new List<MyPoint> { new MyPoint(2, 2), new MyPoint(2, 3), new MyPoint(2, 4), new MyPoint(2, 5) });
			wallEntities2.Add(we22);
			var we23 = new WallEntity(2);
			we23.Geometry.PathsDefinedByPoints.AddRange(new List<MyPoint> { new MyPoint(1, 0), new MyPoint(2, 0) });
			wallEntities2.Add(we23);
		}

		[TearDown]
		public void TearDown()
		{
			wallEntities2 = null;
		}

		[Test]
		public void TestAddPointToWallEntityAtIndex_IndexOutOfBound_ThrowArgumentOutOfRangeException()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => EntitiesOperator.AddPointToWallEntityAtIndex(ref this.wallEntities2, new MyPoint(0, 0), 3));
		}

		[Test]
		public void TestAddPointToWallEntityAtIndex_TheEntityGeometryIsEmpty_AddTheFirstPointToTheCorrectEntity()
		{
			EntitiesOperator.AddPointToWallEntityAtIndex(ref this.wallEntities1, new MyPoint(100, 100), 0);
			Assert.AreEqual(2, this.wallEntities1.Count);

			Assert.AreEqual(1, this.wallEntities1[0].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(100, 100), this.wallEntities1[0].Geometry.PathsDefinedByPoints[0]);

			Assert.AreEqual(2, this.wallEntities1[1].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(1, 1), this.wallEntities1[1].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities1[1].Geometry.PathsDefinedByPoints[1]);
		}

		[Test]
		public void TestAddPointToWallEntityAtIndex_TheEntityGeometryAlreadyHasPoints_AddThePointToTheEndOfTheCorrectEntity()
		{
			EntitiesOperator.AddPointToWallEntityAtIndex(ref this.wallEntities2, new MyPoint(100, 100), 1);
			Assert.AreEqual(3, this.wallEntities2.Count);
			
			Assert.AreEqual(3, this.wallEntities2[0].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(1, 1), this.wallEntities2[0].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[0].Geometry.PathsDefinedByPoints[1]);
			Assert.AreEqual(new MyPoint(3, 3), this.wallEntities2[0].Geometry.PathsDefinedByPoints[2]);

			Assert.AreEqual(5, this.wallEntities2[1].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[1].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 3), this.wallEntities2[1].Geometry.PathsDefinedByPoints[1]);
			Assert.AreEqual(new MyPoint(2, 4), this.wallEntities2[1].Geometry.PathsDefinedByPoints[2]);
			Assert.AreEqual(new MyPoint(2, 5), this.wallEntities2[1].Geometry.PathsDefinedByPoints[3]);
			Assert.AreEqual(new MyPoint(100, 100), this.wallEntities2[1].Geometry.PathsDefinedByPoints[4]);

			Assert.AreEqual(2, this.wallEntities2[2].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(1, 0), this.wallEntities2[2].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 0), this.wallEntities2[2].Geometry.PathsDefinedByPoints[1]);
		}

		[Test]
		public void TestDeleteSegmentsFromWallEntitiesAtIndexes_EntityIndexesOutOfRange_ThrowArgumentOutOfRangeException()
		{
			var segmentsToDelete = new List<Tuple<int, int, int>>();
			segmentsToDelete.Add(new Tuple<int, int, int>(2, 0, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => EntitiesOperator.DeleteSegmentsFromWallEntitiesAtIndexes(ref this.wallEntities1, segmentsToDelete));
		}

		[Test]
		public void TestDeleteSegmentsFromWallEntitiesAtIndexes_SegmentIndexesOutOfRange_ThrowArgumentException()
		{
			var segmentsToDelete = new List<Tuple<int, int, int>>();
			segmentsToDelete.Add(new Tuple<int, int, int>(1, 1, 2));
			Assert.Throws<ArgumentException>(() => EntitiesOperator.DeleteSegmentsFromWallEntitiesAtIndexes(ref this.wallEntities1, segmentsToDelete));
		}

		[Test]
		public void TestDeleteSegmentsFromWallEntitiesAtIndexes_SegmentIndexesNotConsecutive_ThrowArgumentException()
		{
			var segmentsToDelete = new List<Tuple<int, int, int>>();
			segmentsToDelete.Add(new Tuple<int, int, int>(0, 0, 2));
			Assert.Throws<ArgumentException>(() => EntitiesOperator.DeleteSegmentsFromWallEntitiesAtIndexes(ref this.wallEntities2, segmentsToDelete));
		}

		[Test]
		public void TestDeleteSegmentsFromWallEntitiesAtIndexes_SegmentIndexesNotAscending_ThrowArgumentException()
		{
			var segmentsToDelete = new List<Tuple<int, int, int>>();
			segmentsToDelete.Add(new Tuple<int, int, int>(0, 1, 0));
			Assert.Throws<ArgumentException>(() => EntitiesOperator.DeleteSegmentsFromWallEntitiesAtIndexes(ref this.wallEntities2, segmentsToDelete));
		}

		[Test]
		public void TestDeleteSegmentsFromWallEntitiesAtIndexes_RemoveOneSegmentAtEndWithSegmentsRemaining_RemoveSpecifiedSegment() 
		{
			var segmentsToDelete = new List<Tuple<int, int, int>>();
			segmentsToDelete.Add(new Tuple<int, int, int>(0, 1, 2));
			EntitiesOperator.DeleteSegmentsFromWallEntitiesAtIndexes(ref this.wallEntities2, segmentsToDelete);

			Assert.AreEqual(3, this.wallEntities2.Count);

			Assert.AreEqual(2, this.wallEntities2[0].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(1, 1), this.wallEntities2[0].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[0].Geometry.PathsDefinedByPoints[1]);

			Assert.AreEqual(4, this.wallEntities2[1].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[1].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 3), this.wallEntities2[1].Geometry.PathsDefinedByPoints[1]);
			Assert.AreEqual(new MyPoint(2, 4), this.wallEntities2[1].Geometry.PathsDefinedByPoints[2]);
			Assert.AreEqual(new MyPoint(2, 5), this.wallEntities2[1].Geometry.PathsDefinedByPoints[3]);

			Assert.AreEqual(2, this.wallEntities2[2].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(1, 0), this.wallEntities2[2].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 0), this.wallEntities2[2].Geometry.PathsDefinedByPoints[1]);
		}

		[Test]
		public void TestDeleteSegmentsFromWallEntitiesAtIndexes_RemoveOneSegmentAtBeginningWithSegmentsRemaining_RemoveSpecifiedSegment()
		{
			var segmentsToDelete = new List<Tuple<int, int, int>>();
			segmentsToDelete.Add(new Tuple<int, int, int>(0, 0, 1));
			EntitiesOperator.DeleteSegmentsFromWallEntitiesAtIndexes(ref this.wallEntities2, segmentsToDelete);

			Assert.AreEqual(3, this.wallEntities2.Count);

			Assert.AreEqual(2, this.wallEntities2[0].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[0].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(3, 3), this.wallEntities2[0].Geometry.PathsDefinedByPoints[1]);

			Assert.AreEqual(4, this.wallEntities2[1].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[1].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 3), this.wallEntities2[1].Geometry.PathsDefinedByPoints[1]);
			Assert.AreEqual(new MyPoint(2, 4), this.wallEntities2[1].Geometry.PathsDefinedByPoints[2]);
			Assert.AreEqual(new MyPoint(2, 5), this.wallEntities2[1].Geometry.PathsDefinedByPoints[3]);

			Assert.AreEqual(2, this.wallEntities2[2].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(1, 0), this.wallEntities2[2].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 0), this.wallEntities2[2].Geometry.PathsDefinedByPoints[1]);
		}

		[Test]
		public void TestDeleteSegmentsFromWallEntitiesAtIndexes_RemoveTheLastSegment_RemoveTheEntity()
		{
			var segmentsToDelete = new List<Tuple<int, int, int>>();
			segmentsToDelete.Add(new Tuple<int, int, int>(2, 0, 1));
			EntitiesOperator.DeleteSegmentsFromWallEntitiesAtIndexes(ref this.wallEntities2, segmentsToDelete);

			Assert.AreEqual(2, this.wallEntities2.Count);

			Assert.AreEqual(3, this.wallEntities2[0].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(1, 1), this.wallEntities2[0].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[0].Geometry.PathsDefinedByPoints[1]);
			Assert.AreEqual(new MyPoint(3, 3), this.wallEntities2[0].Geometry.PathsDefinedByPoints[2]);

			Assert.AreEqual(4, this.wallEntities2[1].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[1].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 3), this.wallEntities2[1].Geometry.PathsDefinedByPoints[1]);
			Assert.AreEqual(new MyPoint(2, 4), this.wallEntities2[1].Geometry.PathsDefinedByPoints[2]);
			Assert.AreEqual(new MyPoint(2, 5), this.wallEntities2[1].Geometry.PathsDefinedByPoints[3]);
		}

		[Test]
		public void TestDeleteSegmentsFromWallEntitiesAtIndexes_RemoveOneSegmentInTheMiddle_RemoveTheSegmentAndSplitTheEntityIntoTwo()
		{
			var segmentsToDelete = new List<Tuple<int, int, int>>();
			segmentsToDelete.Add(new Tuple<int, int, int>(1, 1, 2));
			EntitiesOperator.DeleteSegmentsFromWallEntitiesAtIndexes(ref this.wallEntities2, segmentsToDelete);

			Assert.AreEqual(4, this.wallEntities2.Count);

			Assert.AreEqual(3, this.wallEntities2[0].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(1, 1), this.wallEntities2[0].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[0].Geometry.PathsDefinedByPoints[1]);
			Assert.AreEqual(new MyPoint(3, 3), this.wallEntities2[0].Geometry.PathsDefinedByPoints[2]);

			Assert.AreEqual(2, this.wallEntities2[1].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[1].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 3), this.wallEntities2[1].Geometry.PathsDefinedByPoints[1]);

			Assert.AreEqual(2, this.wallEntities2[2].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(2, 4), this.wallEntities2[2].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 5), this.wallEntities2[2].Geometry.PathsDefinedByPoints[1]);

			Assert.AreEqual(2, this.wallEntities2[3].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(1, 0), this.wallEntities2[3].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 0), this.wallEntities2[3].Geometry.PathsDefinedByPoints[1]);
		}

		[Test]
		public void TestDeleteSegmentsFromWallEntitiesAtIndexes_RemoveRepeatedSegments_DeleteOnlyOnce()
		{
			var segmentsToDelete = new List<Tuple<int, int, int>>();
			segmentsToDelete.Add(new Tuple<int, int, int>(1, 0, 1));
			segmentsToDelete.Add(new Tuple<int, int, int>(1, 0, 1));
			segmentsToDelete.Add(new Tuple<int, int, int>(1, 0, 1));
			EntitiesOperator.DeleteSegmentsFromWallEntitiesAtIndexes(ref this.wallEntities2, segmentsToDelete);

			Assert.AreEqual(3, this.wallEntities2.Count);

			Assert.AreEqual(3, this.wallEntities2[0].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(1, 1), this.wallEntities2[0].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[0].Geometry.PathsDefinedByPoints[1]);
			Assert.AreEqual(new MyPoint(3, 3), this.wallEntities2[0].Geometry.PathsDefinedByPoints[2]);

			Assert.AreEqual(3, this.wallEntities2[1].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(2, 3), this.wallEntities2[1].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 4), this.wallEntities2[1].Geometry.PathsDefinedByPoints[1]);
			Assert.AreEqual(new MyPoint(2, 5), this.wallEntities2[1].Geometry.PathsDefinedByPoints[2]);

			Assert.AreEqual(2, this.wallEntities2[2].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(1, 0), this.wallEntities2[2].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 0), this.wallEntities2[2].Geometry.PathsDefinedByPoints[1]);
		}

		[Test]
		public void TestDeleteSegmentsFromWallEntitiesAtIndexes_RemoveMultipleSegments()
		{
			var segmentsToDelete = new List<Tuple<int, int, int>>();
			segmentsToDelete.Add(new Tuple<int, int, int>(2, 0, 1));
			segmentsToDelete.Add(new Tuple<int, int, int>(0, 1, 2));
			segmentsToDelete.Add(new Tuple<int, int, int>(1, 1, 2));
			segmentsToDelete.Add(new Tuple<int, int, int>(0, 1, 2));
			EntitiesOperator.DeleteSegmentsFromWallEntitiesAtIndexes(ref this.wallEntities2, segmentsToDelete);

			Assert.AreEqual(3, this.wallEntities2.Count);

			Assert.AreEqual(2, this.wallEntities2[0].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(1, 1), this.wallEntities2[0].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[0].Geometry.PathsDefinedByPoints[1]);

			Assert.AreEqual(2, this.wallEntities2[1].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(2, 2), this.wallEntities2[1].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 3), this.wallEntities2[1].Geometry.PathsDefinedByPoints[1]);

			Assert.AreEqual(2, this.wallEntities2[2].Geometry.PathsDefinedByPoints.Count);
			Assert.AreEqual(new MyPoint(2, 4), this.wallEntities2[2].Geometry.PathsDefinedByPoints[0]);
			Assert.AreEqual(new MyPoint(2, 5), this.wallEntities2[2].Geometry.PathsDefinedByPoints[1]);
		}
	}
}
