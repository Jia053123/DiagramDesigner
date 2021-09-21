using BasicGeometries;
using ListOperations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DiagramDesignerEngine
{
	class LineSegmentsTraverser
	{
		private List<LineSegment> SegmentsToTraverse;
		private List<TraversalRecord> TraversalRecords = null;

		private LineSegment StartSegment;
		private bool StartWithFirstPoint;
		private bool TurnLargerAnglesFirst;

		internal List<LineSegment> GetLastPath()
		{
			return new List<LineSegment>(this.TraversalRecords.Last().Path); // make a copy
		}
		internal List<Point> GetLastPointsAlongPath()
		{
			var tr = this.TraversalRecords.Last();
			Debug.Assert(tr.PointsAlongPath.Count == tr.Path.Count + 1);
			return new List<Point>(tr.PointsAlongPath); // make a copy
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="segmentsToTraverse"> The pool of segments to traverse without duplicate segments </param>
		internal LineSegmentsTraverser(List<LineSegment> segmentsToTraverse)
		{
			this.SegmentsToTraverse = segmentsToTraverse;
			this.TraversalRecords = new List<TraversalRecord>();
		}

		/// <summary>
		/// Finish one iteration of depth first search
		/// </summary>
		/// <param name="pathTraversedSoFar"> cannot be null or empty; will contain the full traversed path when the method returns </param>
		/// <param name="pointsTraversedSoFar"> cannot be null or empty; will contain all points in the path when the method returns </param>
		/// <returns> If the traversal ended in a loop, return the index where the loop begins; 
		/// returning 0 means the path is a perfect loop; if the traversal ended at a deadend, return -1 </returns>
		private int FinishTraversal(ref List<LineSegment> pathTraversedSoFar, ref List<Point> pointsTraversedSoFar)
		{
			if (pathTraversedSoFar is null || pathTraversedSoFar.Count == 0 || pointsTraversedSoFar is null || pointsTraversedSoFar.Count == 0)
			{
				throw new ArgumentException("invalid parameters");
			}

			var pool = new List<LineSegment>(this.SegmentsToTraverse);

			// pick it up from the last segment at the last point
			var currentSegment = pathTraversedSoFar.Last();
			LineSegment? nextSegment;
			bool isFirstPointTheOneToSearch;
			isFirstPointTheOneToSearch = pointsTraversedSoFar.Last() == currentSegment.FirstPoint ? true : false;
			
			do
			{
				if (pointsTraversedSoFar.Count >= 2)
				{
					// check for past occurance (search in the points traversed except for the last point) 
					var previousOccurence = pointsTraversedSoFar.GetRange(0, pointsTraversedSoFar.Count - 1).
						FindIndex(p => p == (isFirstPointTheOneToSearch ? currentSegment.FirstPoint : currentSegment.SecondPoint));
					if (previousOccurence != -1)
					{
						// a traversed point is reached. return with the index
						return previousOccurence;
					}
				}

				// look for next segment
				nextSegment = null;
				var searchResult = isFirstPointTheOneToSearch ?
					TraversalUtilities.FindLeftConnectedSegmentsSortedByAngle(currentSegment, pool) :
					TraversalUtilities.FindRightConnectedSegmentsSortedByAngle(currentSegment, pool);
				if (searchResult.Count > 0)
				{
					nextSegment = this.TurnLargerAnglesFirst ? searchResult.Last() : searchResult.First();

					// if first point is connected then search the free second point, vice versa
					var pointConnectedToNextSegment = isFirstPointTheOneToSearch ? currentSegment.FirstPoint : currentSegment.SecondPoint;
					isFirstPointTheOneToSearch = !(pointConnectedToNextSegment == ((LineSegment)nextSegment).FirstPoint);

					pathTraversedSoFar.Add((LineSegment)nextSegment);
					pointsTraversedSoFar.Add(isFirstPointTheOneToSearch ? 
						((LineSegment)nextSegment).FirstPoint : 
						((LineSegment)nextSegment).SecondPoint);

					currentSegment = (LineSegment)nextSegment;
				}
				
			} while (!(nextSegment is null)); // continue if not at deadend yet

			// reaching here means dead end is reached, return -1
			return -1; 
		}

		/// <summary>
		/// Tranverse a pool of segments until the end of the path connects back to a segment previously traversed or reaches a deadend
		/// </summary>
		/// <param name="startSegment"> The segment from segmentsToTraverse. The beginning of the path. </param>
		/// <param name="startWithFirstPoint"> start traversing from FirstPoint of the startSegment or not (and use SecondPoint) </param>
		/// <param name="turnLargerAngleFirst"> when the potential path branches, turn the largest angle first or not (and turn the smallest angle first) </param>
		/// <returns> If the traversal ended in a loop, return the index where the loop begins; 
		/// returning 0 means the path is a perfect loop; if the traversal ended at a deadend, return -1 </returns>
		internal Tuple<int, List<LineSegment>> TraverseSegments(LineSegment startSegment, bool startWithFirstPoint, bool turnLargerAngleFirst)
		{
			// reset and fill all fields
			this.StartSegment = startSegment;
			this.StartWithFirstPoint = startWithFirstPoint;
			this.TurnLargerAnglesFirst = turnLargerAngleFirst;

			var path = new List<LineSegment>();
			var pointsAlongPath = new List<Point>();

			path.Add(this.StartSegment);
			// the first point is the not-being-searched end of the first segment
			pointsAlongPath.Add(this.StartWithFirstPoint ? this.StartSegment.SecondPoint : this.StartSegment.FirstPoint);
			// the second point is the one being searched
			pointsAlongPath.Add(this.StartWithFirstPoint ? this.StartSegment.FirstPoint : this.StartSegment.SecondPoint);

			var result = this.FinishTraversal(ref path, ref pointsAlongPath);
			
			this.TraversalRecords.Add(new TraversalRecord(path, pointsAlongPath));
			return new Tuple<int, List<LineSegment>>(result, path); 
		}

		/// <summary>
		/// Traverse once again with the same settings, 
		/// but attempt a different route (determined by the angle setting) at the last available branching point; 
		/// a new iteration of depth first search
		/// </summary>
		/// <returns>  If the traversal ended in a loop, return the index where the loop begins; 
		/// returning 0 means the path is a perfect loop; if the traversal ended at a deadend, return -1.
		/// If all possible paths are exhausted, return null </returns>
		internal Tuple<int, List<LineSegment>> TraverseAgain()
		{
			if (this.TraversalRecords.Count == 0)
			{
				throw new InvalidOperationException("TraverseSegments not yet performed");
			}

			var lastPath = this.GetLastPath();
			var lastPoints = this.GetLastPointsAlongPath();

			// start searching from the second last point of lastPoints for an alternative branching
			int segmentIndex = lastPath.Count - 2;
			int pointIndex = lastPoints.Count - 2;
			
			LineSegment? newSegmentToProceed = null;
			Point? newPointToProceed = null;
			List<LineSegment> unvisitedBranchesAtPointToSearch;
			do
			{
				if (pointIndex < 0 || segmentIndex < 0)
				{
					// all paths from the starting segment at starting point has been traversed
					return null;
				}

				var pointToSearch = lastPoints[pointIndex];
				var segmentToSearch = lastPath[segmentIndex];

				List<LineSegment> branchesAtPointToSearch;
				if (pointToSearch == segmentToSearch.FirstPoint)
				{
					branchesAtPointToSearch = TraversalUtilities.FindLeftConnectedSegmentsSortedByAngle(segmentToSearch, this.SegmentsToTraverse);
				}
				else
				{
					Debug.Assert(pointToSearch == segmentToSearch.SecondPoint);
					branchesAtPointToSearch = TraversalUtilities.FindRightConnectedSegmentsSortedByAngle(segmentToSearch, this.SegmentsToTraverse);
				}
				Debug.Assert(branchesAtPointToSearch.Count > 0); // since we just went back by one segment, there must be at least one connected segment

				// remove segments traversed before
				unvisitedBranchesAtPointToSearch = new List<LineSegment>(branchesAtPointToSearch);
				foreach (LineSegment b in branchesAtPointToSearch)
				{
					for (int i = 0; i < this.TraversalRecords.Count; i++)
					{
						var oldPath = this.TraversalRecords[i].Path;
						var pathPreview = lastPath.GetRange(0, segmentIndex + 1);
						pathPreview.Add(b);
						if (oldPath.Count >= segmentIndex + 2)
						{
							// take old path up to this point for comparison
							oldPath = oldPath.GetRange(0, segmentIndex + 2);
						}
						if (ListUtilities.AreContentsEqualInOrder(oldPath, pathPreview))
						{
							// already gone this way before
							var r = unvisitedBranchesAtPointToSearch.Remove(b);
							Debug.Assert(r);
							break;
						}
					}
				}

				if (unvisitedBranchesAtPointToSearch.Count > 0)
				{
					newSegmentToProceed = this.TurnLargerAnglesFirst ? unvisitedBranchesAtPointToSearch.Last() : unvisitedBranchesAtPointToSearch.First();
					newPointToProceed = 
						pointToSearch == ((LineSegment)newSegmentToProceed).FirstPoint ? 
						((LineSegment)newSegmentToProceed).SecondPoint : 
						((LineSegment)newSegmentToProceed).FirstPoint;
				}
				else
				{
					// all branches at this point have been traversed, go back one more segment
					pointIndex--;
					segmentIndex--;
				}
			} while ((newSegmentToProceed is null) || (newPointToProceed is null));

			var traversedPath = lastPath.GetRange(0, segmentIndex + 1);
			traversedPath.Add((LineSegment)newSegmentToProceed);

			var traversedPoints = lastPoints.GetRange(0, pointIndex + 1);
			traversedPoints.Add((Point)newPointToProceed);

			var result = this.FinishTraversal(ref traversedPath, ref traversedPoints);

			this.TraversalRecords.Add(new TraversalRecord(traversedPath, traversedPoints));
			return new Tuple<int,List<LineSegment>>(result, traversedPath);
		}

		internal readonly struct TraversalRecord 
		{
			internal readonly List<LineSegment> Path;
			internal readonly List<Point> PointsAlongPath;
			internal TraversalRecord(List<LineSegment> path, List<Point> pointsAlongPath)
			{
				// TODO: should I check the data match? 

				if (path.Count != pointsAlongPath.Count - 1)
				{
					throw new ArgumentException("count of path and pointsAlongPath don't match");
				}
				this.Path = path;
				this.PointsAlongPath = pointsAlongPath;
			}
		}
	}
}
