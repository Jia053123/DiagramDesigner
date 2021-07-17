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
        public Double translationX = 0;
        public Double translationY = 0;
        public Double rotation = 0;
        public abstract Geometry Geometry { get; }
        public abstract void Draw();
    }
}
