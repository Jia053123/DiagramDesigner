using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    /// <summary>
    /// In the context of shape grammar, the collection of entities reflects the current description of the diagram. 
    /// A change in the collection reflects a change of the description but not necessarily a change of the appearance of the diagram. 
    /// </summary>
    class DiagramDesignerModel
    {
        private List<BoundaryEntity> BoundaryEntities;
        private List<WallEntity> WallEntities;
        private List<EnclosedProgram> EnclosedProgramEntities;

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
