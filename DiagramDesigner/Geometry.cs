using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesigner
{
    abstract class Geometry
    {
        public Double translationX = 0;
        public Double translationY = 0;
        public Double rotation = 0;
        abstract public void Draw();
    }
}
