using ShapeGrammarEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MyPoint = BasicGeometries.Point;

[assembly: InternalsVisibleToAttribute("DiagramDesignerModel.UnitTests")]

namespace DiagramDesignerModel
{
	static class EntitiesOperator
	{
        /// <summary>
        /// Add Point at the end of the specified WallEntity to extend its polyline geometry
        /// </summary>
        /// <param name="wallEntities"> the WallEntities list to modify </param>
        /// <param name="point"> the new point to add </param>
        /// <param name="index"> the index of the WallEntity for the new point </param>
        static internal void AddPointToWallEntityAtIndex(ref List<WallEntity> wallEntities, MyPoint point, int index)
        {
            // TODO: check for overlapping lines and throw exception when found
            wallEntities[index].AddPointToGeometry(point);
        }

        /// <summary>
        /// Delete multiple segments from WallEntities in Model. 
        /// </summary>
        /// <param name="wallEntities"> the WallEntities list to modify </param>
        /// <param name="segmentsToDelete"> The segments to be deleted; 
        /// each Tuple represents a single segment with the index of the containing WallEntity within all WallEntities 
        /// and the two ascending consecutive indexes indicating the line segment within the WallEntity. </param>
        static internal void DeleteSegmentsFromWallEntitiesAtIndexes(ref List<WallEntity> wallEntities, List<Tuple<int, int, int>> segmentsToDelete)
        {
            // sort the segments so that deleting one does not change the subsequent indexes
            var wallsToHighlightAsContextInDescendingOrder = new List<Tuple<int, int, int>>(segmentsToDelete).Distinct().ToList();
            wallsToHighlightAsContextInDescendingOrder.Sort(delegate (Tuple<int, int, int> w1, Tuple<int, int, int> w2)
            {
                if (w1.Item1 != w2.Item1) { return -1 * (w1.Item1 - w2.Item1); }
                else { return -1 * (w1.Item2 - w2.Item2); }
            });
            foreach (Tuple<int, int, int> segment in wallsToHighlightAsContextInDescendingOrder)
            {
                EntitiesOperator.DeleteSegmentFromWallEntityAtIndex(ref wallEntities, segment.Item2, segment.Item3, segment.Item1);
            }
        }

        /// <summary>
        /// Remove a single segment from the specified WallEntity. 
        /// After the removal, if the WallEntity is no longer continuous, it is deleted and two WallEntities containing the two sides are inserted at the original index;
        /// After the removal, if the WallEntity contains less than two points, it is deleted. 
        /// </summary>
        /// <param name="wallEntities"> the WallEntities list to modify </param>
        /// <param name="firstEndPointIndex"> the index of the first end point of the segment to remove; it must be equal to secondEndPointIndex - 1 </param>
        /// <param name="secondEndPointIndex"> the index of the second end point of the segment to remove; it must be equal to firstEndPointIndex + 1 </param>
        /// <param name="wallEntityIndex"> index of the WallEntity to operate upon </param>
        static private void DeleteSegmentFromWallEntityAtIndex(ref List<WallEntity> wallEntities, int firstEndPointIndex, int secondEndPointIndex, int wallEntityIndex)
        {
            var we = wallEntities[wallEntityIndex];
            if (firstEndPointIndex != secondEndPointIndex - 1)
            {
                throw new ArgumentException("firstEndPointIndex is not 1 less than secondEndPointIndex");
            }
            if (firstEndPointIndex < 0 || we.Geometry.PathsDefinedByPoints.Count < (secondEndPointIndex + 1))
			{
                throw new ArgumentException("firstEndPointIndex or secondEndPointIndex out of bound");
			}

            if (firstEndPointIndex == 0)
            {
                // simply remove the first point
                we.Geometry.PathsDefinedByPoints.RemoveAt(0);
                if (we.Geometry.PathsDefinedByPoints.Count < 2)
                {
                    wallEntities.RemoveAt(wallEntityIndex);
                }
            }
            else if (secondEndPointIndex == we.Geometry.PathsDefinedByPoints.Count - 1)
            {
                // simply remove the last point
                we.Geometry.PathsDefinedByPoints.RemoveAt(we.Geometry.PathsDefinedByPoints.Count - 1);
                if (we.Geometry.PathsDefinedByPoints.Count < 2)
                {
                    wallEntities.RemoveAt(wallEntityIndex);
                }
            }
            else
            {
                // the WallEntity is to be split into two 0 1 2 3
                var newWe1 = new WallEntity(we.WallThickness);
                var newWe2 = new WallEntity(we.WallThickness);
                newWe1.Geometry.PathsDefinedByPoints = we.Geometry.PathsDefinedByPoints.GetRange(0, firstEndPointIndex + 1);
                int lastIndex = we.Geometry.PathsDefinedByPoints.Count - 1;
                newWe2.Geometry.PathsDefinedByPoints = we.Geometry.PathsDefinedByPoints.GetRange(secondEndPointIndex, lastIndex - secondEndPointIndex + 1);

                wallEntities.RemoveAt(wallEntityIndex);
                wallEntities.Insert(wallEntityIndex, newWe2);
                wallEntities.Insert(wallEntityIndex, newWe1);
            }
        }

        static internal void ApplyRuleGivenLeftHandGeometry(ref List<WallEntity> wallEntities, List<Tuple<int, int, int>> leftHandGeometry, GrammarRule ruleToApply)
		{
            // TODO: stub
		}
    }
}
