using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerModel
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
