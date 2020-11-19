using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace TrajectoryCalculation
{
    class BezierSpline
    {
        public Point3d pt1;
        public Point3d pt4;
        public Vector3d vec1;
        public Vector3d vec4;

        public BezierSpline(Point3d Pt1, Point3d Pt4, Vector3d Vec1, Vector3d Vec4)
        {
            pt1 = Pt1;
            pt4 = Pt4;
            vec1 = Vec1;
            vec4 = Vec4;
        }

        public List<Point3d> BezierSplinePoints()
        {
            Point3d pt2 = pt1 + vec1;
            Point3d pt3 = pt4 + vec4;

            List<Point3d> pts = new List<Point3d>();

            for (double t = 0; t < 1.0; t = t + 0.05)
            {
                Point3d pt = (1 - t) * (1 - t) * (1 - t) * pt1 + 3 * t * (1 - t) * (1 - t) * pt2 + 3 * t * t * (1 - t) * pt3 + t * t * t * pt4;
                pts.Add(pt);
            }

            return pts;
        }

        public Curve BezierSplineCurve()
        {
            Point3d pt2 = pt1 + vec1;
            Point3d pt3 = pt4 + vec4;

            List<Point3d> pts = new List<Point3d>();

            for (double t = 0; t < 1.0; t = t + 0.1)
            {
                Point3d pt = (1 - t) * (1 - t) * (1 - t) * pt1 + 3 * t * (1 - t) * (1 - t) * pt2 + 3 * t * t * (1 - t) * pt3 + t * t * t * pt4;
                pts.Add(pt);
            }

            Curve curve = new PolylineCurve(pts);

            return curve;
        }
    }
}
