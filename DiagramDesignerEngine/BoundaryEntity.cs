using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    class BoundaryEntity : Entity
    {
        public override PolylineGeometry Geometry => throw new NotImplementedException();

        public double CalculateEnclosedArea()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
