using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
		/// Tranverse a pool of segments until the end of the path connects back to a segment previously traversed or reaches a deadend
		/// </summary>
		/// <param name="startSegment"> The segment from segmentsToTraverse. The beginning of the path. </param>
		/// <param name="startWithFirstPoint"> start traversing from FirstPoint of the startSegment or not (and use SecondPoint) </param>
		/// <param name="turnLargerAnglesFirst"> when the potential path branches, turn the largest angle first or not (and turn the smallest angle first) </param>
		/// <returns> If the traversal ended in a loop, return the index where the loop begins; 
		/// returning 0 means the path is a perfect loop; if the traversal ended at a deadend, return -1 </returns>
		internal Tuple<int, List<LineSegment>> TraverseSegments(LineSegment startSegment, bool startWithFirstPoint, bool turnLargerAnglesFirst)
		{
			// reset and fill all fields
			this.StartSegment = startSegment;
			this.StartWithFirstPoint = startWithFirstPoint;
			this.TurnLargerAnglesFirst = turnLargerAnglesFirst;

			
			var pool = new List<LineSegment>(this.SegmentsToTraverse);
			var path = new List<LineSegment>();
			var pointsAlongPath = new List<Point>();
			var currentSegment = this.StartSegment;
			LineSegment? nextSegment;
			bool isFirstPointTheOneToSearch = this.StartWithFirstPoint;

			// the first point is the not-being-searched end of the first segment
			pointsAlongPath.Add(this.StartWithFirstPoint ? currentSegment.SecondPoint : currentSegment.FirstPoint);

			do
			{
				path.Add(currentSegment);
				nextSegment = null;

				// check for past occurance before adding the new point itself to the list
				var previousOccurence = pointsAlongPath.FindIndex(p => p == (isFirstPointTheOneToSearch ? currentSegment.FirstPoint : currentSegment.SecondPoint));
				pointsAlongPath.Add(isFirstPointTheOneToSearch ? currentSegment.FirstPoint : currentSegment.SecondPoint);

				if (previousOccurence != -1)
				{
					// a traversed point is reached. return with the index
					this.TraversalRecords.Add(new TraversalRecord(path, pointsAlongPath));
					return new Tuple<int, List<LineSegment>>(previousOccurence, this.GetLastPath());
				}

				// look for next segment
				var searchResult = isFirstPointTheOneToSearch ?
					TraversalUtilities.FindLeftConnectedSegmentsSortedByAngle(currentSegment, pool) :
					TraversalUtilities.FindRightConnectedSegmentsSortedByAngle(currentSegment, pool);

				if (searchResult.Count > 0)
				{
					nextSegment = this.TurnLargerAnglesFirst ? searchResult.Last() : searchResult.First();

					// if first point is connected then search the free second point, vice versa
					var pointConnectedToNextSegment = isFirstPointTheOneToSearch ? currentSegment.FirstPoint : currentSegment.SecondPoint;
					isFirstPointTheOneToSearch = !(pointConnectedToNextSegment == ((LineSegment)nextSegment).FirstPoint);

					currentSegment = (LineSegment)nextSegment;
				}
			} while (!(nextSegment is null)); // continue if not at deadend yet

			this.TraversalRecords.Add(new TraversalRecord(path, pointsAlongPath));
			return new Tuple<int, List<LineSegment>>(-1, this.GetLastPath()); // reaching here means dead end is reached, return -1
		}

		/// <summary>
		/// Traverse once again with the same settings, 
		/// but attempt a different route (determined by the angle setting) at the last available branching point; 
		/// a new iteration of depth first search
		/// </summary>
		/// <returns>  If the traversal ended in a loop, return the index where the loop begins; 
		/// returning 0 means the path is a perfect loop; if the traversal ended at a deadend, return -1 </returns>
		internal new Tuple<int, List<LineSegment>> TraverseAgain()
		{
			if (this.TraversalRecords.Count == 0)
			{
				throw new InvalidOperationException("TraverseSegments not yet performed");
			}

			//var traversedPaths = this.GetLastPath;
			var lastPath = this.GetLastPath();
			var lastPoints = this.GetLastPointsAlongPath();

			// start searching from the second last point of lastPoints
			int pointIndex = lastPoints.Count - 1;
			int segmentIndex = pointIndex - 1;
			bool foundNewSegmentToProceed = false;
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
					//foreach (List<LineSegment> tp in traversedPaths)
					{
						var tp = this.TraversalRecords[i].Path;
						if (tp.Contains(b))
						{
							unvisitedBranchesAtPointToSearch.Remove(b);
							break;
						}
					}
				}
				if (unvisitedBranchesAtPointToSearch.Count > 0)
				{
					foundNewSegmentToProceed = true;
				}
				else
				{
					// all branches at this point have been traversed, go back one more segment
					foundNewSegmentToProceed = false;
					pointIndex--;
					segmentIndex--;
				}
			} while (!foundNewSegmentToProceed);

			var newSegmentToProceed = this.TurnLargerAnglesFirst ? unvisitedBranchesAtPointToSearch.Last() : unvisitedBranchesAtPointToSearch.First();

			return null; //stub
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
