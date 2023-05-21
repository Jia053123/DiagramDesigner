using BasicGeometries;
using ShapeGrammarEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DiagramDesignerGeometryParser;
using System.Linq;

namespace DiagramDesignerModel
{
	/// <summary>
	/// In the context of shape grammar, the collection of entities reflects the current description of the diagram. 
	/// A change in the collection reflects a change of the description but not necessarily a change of the appearance of the diagram. 
	/// </summary>
	public class DiagramDesignerModel
    {
        public ProgramRequirementsTable ProgramRequirements { get; } = new ProgramRequirementsTable();

        private List<WallEntity> wallEntities = new List<WallEntity>();
        public ReadOnlyCollection<WallEntity> WallEntities => this.wallEntities.AsReadOnly();

        public List<EnclosedProgram> Programs { get; private set; } = new List<EnclosedProgram>();

        public GrammarRulesDataTable CurrentRulesInfo => this.rulesStore.CurrentRulesInfoDataTable;

        private GrammarRulesStore rulesStore = new GrammarRulesStore();

        public event EventHandler ModelChanged;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        public void CreateNewWallEntity()
		{
            this.wallEntities.Add(new WallEntity(1));
		}

        /// <summary>
        /// Add Point at the end of the specified WallEntity to extend its polyline geometry
        /// </summary>
        /// <param name="point"> the new point to add </param>
        /// <param name="index"> the index of the WallEntity for the new point </param>
        public void AddPointToWallEntityAtIndex(Point point, int index)
		{
            EntitiesOperator.AddPointToWallEntityAtIndex(ref this.wallEntities, point, index);
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
            EntitiesOperator.DeleteSegmentsFromWallEntitiesAtIndexes(ref this.wallEntities, segmentsToDelete);
            this.OnModelChanged();
        }

        /// <summary>
        /// Explode all walls from all wall entities and put them into new wall entities, each with only two points
        /// </summary>
        public void ExplodeAllWalls()
		{
            Logger.Debug("////////////////// Explode All Walls //////////////////");

            var allSegments = new List<LineSegment>();
            foreach (WallEntity we in this.wallEntities)
            {
                var lineSegments = we.Geometry.ConvertToLineSegments();
                allSegments.AddRange(lineSegments);
            }
            var exploder = new LineSegmentsExploder(allSegments);
            var allSegmentsExploded = exploder.MergeAndExplodeSegments();

            this.RemoveAllWalls();
            foreach (LineSegment ls in allSegmentsExploded)
			{
                this.CreateNewWallEntity();
                this.wallEntities.Last().AddPointToGeometry(ls.FirstPoint);
                this.wallEntities.Last().AddPointToGeometry(ls.SecondPoint);
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
            catch (GeometryParsingFailureException e)
			{
                throw new ArgumentException("The geometires do not match the rule: " + e.Message);
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

        public void TrainModelForRule(Guid ruleId)
        {
            var rule = this.rulesStore.GetRuleById(ruleId);
            var records = rule.ApplicationRecords;

            var geometryBeforeTrainingSet = new List<PolylinesGeometry>();
            var geometryAfterTrainingSet = new List<PolylinesGeometry>();
            foreach (RuleApplicationRecord rar in records)
            {
                var geoBefore = rar.GeometryBefore;








                var geoAfter = rar.GeometryAfter;





                
                
            }

            foreach (PolylinesGeometry geo in geometryBeforeTrainingSet)
            {
                /// TODO: Get actual canvas size!!! 
                var lhsSvg = MachineLearningUtilities.PolylinesGeometryToSvgOnCanvas(geo, 500, 600, 64, 64, 2);
            }
            foreach (PolylinesGeometry geo in geometryAfterTrainingSet)
            {
                /// TODO: Get actual canvas size!!! 
                var rhsSvg = MachineLearningUtilities.PolylinesGeometryToSvgOnCanvas(geo, 500, 600, 64, 64, 2);
            }
        }

        public void RemoveAllWalls()
		{
            this.wallEntities.Clear();
            this.OnModelChanged();
		}

        public void RemoveAllWallsAndPrograms()
		{
            this.wallEntities.Clear();
            this.Programs.Clear();
            this.OnModelChanged();
		}

        public void ResolvePrograms()
		{
            Logger.Debug("////////////////// Resolve Programs //////////////////");

			// make a collection of all geometry segments
			var allSegments = new List<LineSegment>();
			foreach (WallEntity we in this.wallEntities)
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
