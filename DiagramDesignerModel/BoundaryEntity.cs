using System;

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
