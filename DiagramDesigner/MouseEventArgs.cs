﻿using System;

namespace DiagramDesigner
{
	class MouseEventArgs : EventArgs
    {
        public readonly double LocationX;
        public readonly double LocationY;
        //public readonly bool IsButtonPressed;
        public MouseEventArgs(double locationX, double locationY)
        {
            this.LocationX = locationX;
            this.LocationY = locationY;
        }
    }
}
