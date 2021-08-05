using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    /// <summary>
    /// In the context of shape grammar, the collection of entities reflects the current description of the diagram. 
    /// A change in the collection reflects a change of the description but not necessarily a change of the appearance of the diagram. 
    /// </summary>
    public class DiagramDesignerModel
    {
        public ProgramRequirementsTable ProgramRequirements { get; } = new ProgramRequirementsTable();

        //public List<BoundaryEntity> BoundaryEntities { get; private set; } = new List<BoundaryEntity>();
        public List<WallEntity> WallEntities { get; private set; } = new List<WallEntity>();
        private List<LineSegment> CollapsedWallSegments = null;
        private Stack<List<LineSegment>> SegmentsToResolve;

        public List<EnclosedProgram> Programs { get; private set; } = new List<EnclosedProgram>();

        public event EventHandler ModelChanged;

        public void CreateNewWallEntity()
		{
            this.WallEntities.Add(new WallEntity(1));
		}

        public void AddPointToWallEntityAtIndex(Point point, int index)
		{
            if (index >= this.WallEntities.Count)
			{
                return;
			}
            this.WallEntities[index].AddPointToGeometry(point);

            this.OnEntitiesChanged();
		}

        public void ResolvePrograms()
		{
            // make a collection of all geometry segments
            var allSegments = new List<LineSegment>();
            foreach (WallEntity we in this.WallEntities)
			{
                allSegments.AddRange(we.Geometry.ConvertToLineSegments());
			}

            // find intersections
            List<List<Point>> pointsToSplitForEachLine = new List<List<Point>>();
            for (int i = 0; i < allSegments.Count; i++)
			{
                pointsToSplitForEachLine.Add(new List<Point>());
			}
            for (int i = 0; i < allSegments.Count; i++)
			{
                for (int j = i+1; j < allSegments.Count; j++)
				{
                    // If intersection is found, remember which segment should be split at what point
                    Point? pointToSplit = allSegments[i].FindIntersection(allSegments[j]);
					if (pointToSplit != null)
					{
						pointsToSplitForEachLine[i].Add((Point)pointToSplit);
                        pointsToSplitForEachLine[j].Add((Point)pointToSplit);
			    	}
		    	}
			}

            // split segments at points identified
            this.CollapsedWallSegments = new List<LineSegment>();
            for (int i = 0; i < pointsToSplitForEachLine.Count; i++)
			{
                this.CollapsedWallSegments.AddRange(allSegments[i].SplitAtPoints(pointsToSplitForEachLine[i]));
			}
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

        private void OnEntitiesChanged()
		{
            if (this.ModelChanged != null)
            {
                this.ModelChanged(this, null);
            }
        }
    }
}
