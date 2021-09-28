using System;

namespace DiagramDesignerModel
{
	public class BoundaryEntity : Entity
    {
        public override PolylineEntityGeometry Geometry { get; protected set; }

        public double CalculateEnclosedArea()
        {
            throw new NotImplementedException();
        }
    }
}
