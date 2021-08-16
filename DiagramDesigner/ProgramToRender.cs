using System;
using System.Collections.Generic;
using System.Text;
using WinPoint = System.Windows.Point;

namespace DiagramDesigner
{
    internal readonly struct ProgramToRender
    {
        internal readonly List<WinPoint> Perimeter;
        internal readonly List<List<WinPoint>> InnerPerimeters;
        internal readonly String Name;
        internal readonly double Area;

        internal ProgramToRender(List<WinPoint> perimeter, List<List<WinPoint>> innerPerimeters, String name, double area)
        {
            this.Perimeter = perimeter;
            this.InnerPerimeters = innerPerimeters;
            this.Name = name;
            this.Area = area;
        }
    }
}
