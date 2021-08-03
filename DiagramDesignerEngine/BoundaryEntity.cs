using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    public class BoundaryEntity : Entity
    {
        public override PolylineGeometry Geometry { get; protected set; }

        public double CalculateEnclosedArea()
        {
            throw new NotImplementedException();
        }
    }
}
