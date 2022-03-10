using BasicGeometries;
using ShapeGrammarEngine;
using System;
using System.Collections.Generic;

namespace DiagramDesignerModel
{
	/// <summary>
	/// In the context of shape grammar, the collection of entities reflects the current description of the diagram. 
	/// A change in the collection reflects a change of the description but not necessarily a change of the appearance of the diagram. 
	/// </summary>
	public class DiagramDesignerModel
    {
        public ProgramRequirementsTable ProgramRequirements { get; } = new ProgramRequirementsTable();

        public List<WallEntity> WallEntities { get; private set; } = new List<WallEntity>();

        public List<EnclosedProgram> Programs { get; private set; } = new List<EnclosedProgram>();

        public GrammarRulesDataTable CurrentRulesInfo => this.rulesStore.CurrentRulesInfoDataTable;

        private GrammarRulesStore rulesStore = new GrammarRulesStore();

        public event EventHandler ModelChanged;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        public void CreateNewWallEntity()
		{
            this.WallEntities.Add(new WallEntity(1));
		}

        public void AddPointToWallEntityAtIndex(Point point, int index)
		{
            // TODO: check for overlapping lines and throw exception when found

            if (index >= this.WallEntities.Count)
			{
                return; // TODO: why do I need this check? 
			}
            this.WallEntities[index].AddPointToGeometry(point);

            this.OnModelChanged();
		}

        /// <summary>
        /// Delete multiple segments from WallEntities in Model. 
        /// </summary>
        /// <param name="segmentsToDelete"> The segments to be deleted; 
        /// each Tuple represents a single segment with the index of the containing WallEntity within all WallEntities 
        /// and the two ascending consecutive indexes indicating the line segment within the WallEntity. </param>
        public void DeleteSegmentsFromWallEntitiesAtIndexes(List<Tuple<int, int, int>> segmentsToDelete)
		{
            // sort the segments so that deleting one does not change the subsequent indexes
            var wallsToHighlightAsContextInDescendingOrder = new List<Tuple<int, int, int>>(segmentsToDelete);
            wallsToHighlightAsContextInDescendingOrder.Sort(delegate (Tuple<int, int, int> w1, Tuple<int, int, int> w2)
            {
                if (w1.Item1 != w2.Item1) { return -1 * (w1.Item1 - w2.Item1); }
                else { return -1 * (w1.Item2 - w2.Item2); }
            });
            foreach (Tuple<int, int, int> segment in wallsToHighlightAsContextInDescendingOrder)
            {
                this.DeleteSegmentFromWallEntityAtIndex(segment.Item2, segment.Item3, segment.Item1);
            }
        }

        /// <summary>
        /// Remove a single segment from the specified WallEntity. 
        /// After the removal, if the WallEntity is no longer continuous, it is deleted and two WallEntities containing the two sides are inserted at the original index;
        /// After the removal, if the WallEntity contains less than two points, it is deleted. 
        /// </summary>
        /// <param name="firstEndPointIndex"> the index of the first end point of the segment to remove; it must be equal to secondEndPointIndex - 1 </param>
        /// <param name="secondEndPointIndex"> the index of the second end point of the segment to remove; it must be equal to firstEndPointIndex + 1 </param>
        /// <param name="wallEntityIndex"> index of the WallEntity to operate upon </param>
        public void DeleteSegmentFromWallEntityAtIndex(int firstEndPointIndex, int secondEndPointIndex, int wallEntityIndex)
		{
            if (firstEndPointIndex != secondEndPointIndex - 1)
			{
                throw new ArgumentException("firstEndPointIndex is not 1 less than secondEndPointIndex");
			}
            var we = this.WallEntities[wallEntityIndex];

            if (firstEndPointIndex == 0)
			{
                // simply remove the first point
                we.Geometry.PathsDefinedByPoints.RemoveAt(0);
                if (we.Geometry.PathsDefinedByPoints.Count < 2)
				{
                    this.WallEntities.RemoveAt(wallEntityIndex);
				}
			}
            else if (secondEndPointIndex == we.Geometry.PathsDefinedByPoints.Count - 1)
			{
                // simply remove the last point
                we.Geometry.PathsDefinedByPoints.RemoveAt(we.Geometry.PathsDefinedByPoints.Count - 1);
                if (we.Geometry.PathsDefinedByPoints.Count < 2)
                {
                    this.WallEntities.RemoveAt(wallEntityIndex);
                }
            }
            else
			{ 
                // the WallEntity is to be split into two
                var newWe1 = new WallEntity(we.WallThickness);
                var newWe2 = new WallEntity(we.WallThickness);
                newWe1.Geometry.PathsDefinedByPoints = we.Geometry.PathsDefinedByPoints.GetRange(0, firstEndPointIndex + 1);
                newWe2.Geometry.PathsDefinedByPoints = we.Geometry.PathsDefinedByPoints.GetRange(firstEndPointIndex + 1, secondEndPointIndex - firstEndPointIndex);

                this.WallEntities.RemoveAt(wallEntityIndex);
                this.WallEntities.Insert(wallEntityIndex, newWe2);
                this.WallEntities.Insert(wallEntityIndex, newWe1);
			}

            this.OnModelChanged();
		}

		public void CreateNewRuleFromExample(PolylinesGeometry leftHandGeometry, PolylinesGeometry rightHandGeometry)
		{
            this.rulesStore.CreateNewRuleFromExample(leftHandGeometry, rightHandGeometry);
		}

        public void LearnFromExampleForRule(PolylinesGeometry leftHandGeometry, PolylinesGeometry rightHandGeometry, Guid ruleId)
		{
            var rule = this.rulesStore.GetRuleById(ruleId);
            try
			{
                rule.LearnFromExample(leftHandGeometry, rightHandGeometry, out _);
			}
            catch (GeometryParsingFailureException)
			{
                throw new ArgumentException("the geometires do not match the rule");
			}
            this.rulesStore.RuleUpdated(ruleId);
        }

        public PolylinesGeometry ApplyRuleGivenLeftHandGeometry(PolylinesGeometry leftHandGeometry, Guid ruleId)
		{
            var rule = this.rulesStore.GetRuleById(ruleId);
            try
			{
                var newGeo = rule.ApplyToGeometry(leftHandGeometry);
				return newGeo;
			}
			catch (GeometryParsingFailureException e)
            {
                throw new ArgumentException(e.Message);
			}
            catch (RuleApplicationFailureException e)
			{
                throw e;
			}
        }

        public void RemoveAllWallsAndPrograms()
		{
            this.WallEntities.Clear();
            this.Programs.Clear();
            this.OnModelChanged();
		}

        public void ResolvePrograms()
		{
            Logger.Debug("////////////////// Resolve Programs //////////////////");

			// make a collection of all geometry segments
			var allSegments = new List<LineSegment>();
			foreach (WallEntity we in this.WallEntities)
			{
                var lineSegments = we.Geometry.ConvertToLineSegments();
                foreach (LineSegment ls in lineSegments)
				{
                    Logger.Debug(ls.ToString());
                }

                allSegments.AddRange(lineSegments);
			}

			this.Programs = (new ProgramsFinder(allSegments, this.ProgramRequirements)).FindPrograms();
            this.OnModelChanged();
		}

		public double TotalEnclosedArea()
        {
            // TODO: stub
            return 0;
        }

        public double TotalPerimeterLength()
        {
            // TODO: stub
            return 0;
        }

        private void OnModelChanged()
		{
            if (this.ModelChanged != null)
            {
                this.ModelChanged(this, null);
            }
        }
    }
}
