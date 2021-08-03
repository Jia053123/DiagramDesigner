using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    public class WallEntity : Entity
    {
        public double WallThickness { get; internal set; }
        public override PolylineGeometry Geometry { get; internal set; }

        public WallEntity(double thickness)
		{
            this.WallThickness = thickness;
		}
    }
}
