﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	class LineSegmentsTraverser
	{
		private List<LineSegment> SegmentsToTraverse;
		private List<LineSegment> Path = null;

		internal List<LineSegment> GetPath()
		{
			return new List<LineSegment>(this.Path); // make a copy
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="segmentsToTraverse"> The pool of segments to traverse without duplicate segments </param>
		internal LineSegmentsTraverser(List<LineSegment> segmentsToTraverse)
		{
			this.SegmentsToTraverse = segmentsToTraverse;
		}

		/// <summary>
		/// Tranverse a pool of segments until the end of the path connects back to a segment previously traversed or reaches a deadend
		/// </summary>
		/// <param name="startSegment"> The segment from segmentsToTraverse. The beginning of the path. </param>
		/// <param name="startWithFirstPoint"> start traversing from FirstPoint of the startSegment or not (and use SecondPoint) </param>
		/// <param name="turnLargestAngle"> when the potential path branches, turn the largest angle or not (and turn the smallest angle) </param>
		/// <returns> If the traversal ended in a loop, return the index where the loop begins; 
		/// returning 0 means the path is a perfect loop; if the traversal ended at a deadend, return -1 </returns>
		internal int TraverseSegments(LineSegment startSegment, bool startWithFirstPoint, bool turnLargestAngle)
		{
			var pool = new List<LineSegment>(this.SegmentsToTraverse);
			this.Path = new List<LineSegment>();
			var pointsOnPath = new List<Point>(); // remembers the order the points are traversed for loop detection

			var currentSegment = startSegment;
			LineSegment? nextSegment;
			bool isFirstPointTheOneToSearch = startWithFirstPoint;

			// the first point is the not-being-searched end of the first segment
			pointsOnPath.Add(startWithFirstPoint ? currentSegment.SecondPoint : currentSegment.FirstPoint); 

			do
			{
				this.Path.Add(currentSegment);
				nextSegment = null;

				var previousOccurence = pointsOnPath.FindIndex(p => p == (isFirstPointTheOneToSearch ? currentSegment.FirstPoint : currentSegment.SecondPoint));
				if (previousOccurence != -1)
				{
					// a traversed point is reached. return with the index
					return (previousOccurence);
				}

				// add the new point being searched
				pointsOnPath.Add(isFirstPointTheOneToSearch ? currentSegment.FirstPoint : currentSegment.SecondPoint);

				// look for next segment
				var searchResult = isFirstPointTheOneToSearch ?
					SegmentsUtilities.FindLeftConnectedSegmentsSortedByAngle(currentSegment, pool) :
					SegmentsUtilities.FindRightConnectedSegmentsSortedByAngle(currentSegment, pool);

				if (searchResult.Count > 0)
				{
					nextSegment = turnLargestAngle ? searchResult.Last() : searchResult.First();
					
					// if first point is connected then search the free second point, vice versa
					var pointConnectedToNextSegment = isFirstPointTheOneToSearch ? currentSegment.FirstPoint : currentSegment.SecondPoint;
					isFirstPointTheOneToSearch = !(pointConnectedToNextSegment == ((LineSegment)nextSegment).FirstPoint); 

					currentSegment = (LineSegment)nextSegment;
				}
			} while (!(nextSegment is null)); // continue if not at deadend yet

			return -1; // reaching here means dead end is reached, return -1
		}
	}
}