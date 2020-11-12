using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rhino.Geometry;

namespace TrajectoryCalculation
{
    class Anchor
    {
        public int Index { get; }
        public Point3d Position { get; }
        public Vector3d AnchorVec { get; }
        public double Length = 1.0;
        public double Width = 1.0;
        public double Height = 1.0;


        public Anchor(double length, double height, double width)
        {
            Length = length;
            Width = width;
            Height = height;
        }


        //public Anchor(Plane basePlane, double length, double height, double width)
        //{
            //AnchorVec = basePlane;
            //Length = length;
            //Width = width;
            //Height = height;
        //}

        //public List<LineCurve> ComputeDisplayLines(){}
        
            
        
    }
}
