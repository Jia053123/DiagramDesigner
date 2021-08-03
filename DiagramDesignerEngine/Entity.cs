using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    /// <summary>
    /// An thing in the diagram that is not an annotation for such diagram. 
    /// </summary>
    public abstract class Entity {
        public bool Isfixed;
        public Double translationX { get; internal set; } = 0;
        public Double translationY { get; internal set; } = 0;
        public Double rotation { get; internal set; } = 0;
        public abstract PolylineGeometry Geometry { get; internal set; }
    }
}
