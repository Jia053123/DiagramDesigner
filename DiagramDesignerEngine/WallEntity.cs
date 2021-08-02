using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    public class WallEntity : Entity
    {
        public double WallThickness;

        public override PolylineGeometry Geometry => throw new NotImplementedException();

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
