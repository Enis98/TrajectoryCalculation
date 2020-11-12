using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rhino.Geometry;

namespace TrajectoryCalculation
{
    class AnchorGeometry
    {
        public Point3d Origin;
        public Vector3d NormalVec;
        public double Height1;
        public double Radius1;
        public double Height2;
        public double Radius2;

        public AnchorGeometry(Point3d origin, Vector3d normalvec, double height1, double radius1, double height2, double radius2)
        {
            Origin = origin;
            NormalVec = normalvec;
            Height1 = height1;
            Radius1 = radius1;
            Height2 = height2;
            Radius2 = radius2;
        }

        public List<Brep> ComputeAnchorGeometry()
        {
            Vector3d NormNormalVec = NormalVec / NormalVec.Length;
            Point3d A = Origin + NormNormalVec * Height1 / 2;
            Point3d B = Origin - NormNormalVec * Height1 / 2;
            Point3d C = Origin - NormNormalVec * (Height1 / 2 + Height2);

            List<Brep> displayGeometry = new List<Brep>();

            Rhino.Geometry.Plane planea = new Rhino.Geometry.Plane(A, NormNormalVec);
            Rhino.Geometry.Plane planeb = new Rhino.Geometry.Plane(B, NormNormalVec);
            Rhino.Geometry.Plane planec = new Rhino.Geometry.Plane(C, NormNormalVec);

            Rhino.Geometry.Circle circlea = new Rhino.Geometry.Circle(planea, Radius2);
            Rhino.Geometry.Circle circleb = new Rhino.Geometry.Circle(planeb, Radius1);
            Rhino.Geometry.Circle circlec = new Rhino.Geometry.Circle(planec, Radius2);

            Rhino.Geometry.Cylinder cylindera = new Rhino.Geometry.Cylinder(circlea, Height2);
            Rhino.Geometry.Brep brepa = cylindera.ToBrep(true, true);
            displayGeometry.Add(brepa);

            Rhino.Geometry.Cylinder cylinderb = new Rhino.Geometry.Cylinder(circleb, Height1);
            Rhino.Geometry.Brep brepb = cylinderb.ToBrep(true, true);
            displayGeometry.Add(brepb);
            
            Rhino.Geometry.Cylinder cylinderc = new Rhino.Geometry.Cylinder(circlec, Height2);
            Rhino.Geometry.Brep brepc = cylinderc.ToBrep(true, true);
            displayGeometry.Add(brepc);

            return displayGeometry;
        }
    }
}
