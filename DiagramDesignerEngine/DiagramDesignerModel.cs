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
        public List<BoundaryEntity> BoundaryEntities { get; } = new List<BoundaryEntity>();
        public List<WallEntity> WallEntities { get; } = new List<WallEntity>();
        public List<EnclosedProgram> EnclosedProgramEntities { get; } = new List<EnclosedProgram>();

        public event EventHandler ModelChanged;

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
    }
}
