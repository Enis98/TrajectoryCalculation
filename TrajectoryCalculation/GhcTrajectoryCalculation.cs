using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry.Intersect;
using Rhino.Geometry;

namespace TrajectoryCalculation
{
    public class GhcTrajectoryCalculation : GH_Component
    {
        public GhcTrajectoryCalculation()
          : base("GhcTrajectoryCalculation",
                 "GhcTrajCalc",
                 "calculates the trajectory",
                 "CorelessWinding",
                 "Trajectory")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Fiber polylines", "Polylines", "Fiber syntaxes as polylines", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Anchor point IDs", "AnchorIDs", "Anchor point IDs", GH_ParamAccess.list);
            pManager.AddTextParameter("Hooking type", "Hooking", "Anchor point hooking type", GH_ParamAccess.list);
            pManager.AddPointParameter("Anchor points", "AnchorPts", "The anchor points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Anchor point orientations", "AnchorVecs", "The anchor point orientations", GH_ParamAccess.list);
            pManager.AddNumberParameter("Anchor point parameters", "AnchorParams", "Anchor point parameters", GH_ParamAccess.item);
            pManager.AddNumberParameter("Washer parameters", "WasherParams", "Washer parameters", GH_ParamAccess.item);
            pManager.AddNumberParameter("Sphere parameters", "SphereParams", "Sphere parameters", GH_ParamAccess.item);
            pManager.AddBooleanParameter("StartLeftRotation", "StLeftRot", "Direction of Rotation at Start Point is left", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("CheckPts", "CheckPts", "CheckPoints for curve generation", GH_ParamAccess.list);
            pManager.AddPointParameter("PathPts", "PathPts", "Path Points of winding structure", GH_ParamAccess.list);
            pManager.AddVectorParameter("OriVecs", "OriVecs", "Orientation Vector of a Path Point", GH_ParamAccess.list);
            pManager.AddVectorParameter("TanVecs", "TanVecs", "Tangential Vector of a Path Point", GH_ParamAccess.list);
            pManager.AddNumberParameter("Time", "Time", "Time at a Path Point", GH_ParamAccess.list);
            pManager.AddNumberParameter("FiberLength", "FiberLength", "Fiber Length up to a Path Point", GH_ParamAccess.item);
            pManager.AddCurveParameter("curves", "curves", "curves", GH_ParamAccess.list);
            pManager.AddArcParameter("arcs", "arc", "arc", GH_ParamAccess.list);

        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // input parameter definition
            List<Curve> polylines = new List<Curve>();
            DA.GetDataList(0, polylines);

            List<int> syntax = new List<int>();
            DA.GetDataList(1, syntax);

            List<string> hooking = new List<string>();
            DA.GetDataList(2, hooking);

            List<Point3d> anchorpts = new List<Point3d>();
            DA.GetDataList(3, anchorpts);
            // neue liste die in for schleife benutzt wird machen

            List<Vector3d> anchorvecs = new List<Vector3d>();
            DA.GetDataList(4, anchorvecs);
            List<Vector3d> nanchorvecs = new List<Vector3d>();                  // normed anchorvecs

            foreach(Vector3d anchorvec in anchorvecs)
            {
                Vector3d nanchorvec = anchorvec / anchorvec.Length;             // calc of normed anchorvec ->  list: nanchorvecs
                nanchorvecs.Add(nanchorvec);
            }

            double anchorparam = 0;
            DA.GetData(5, ref anchorparam);

            double washerparam = 0;
            DA.GetData(6, ref washerparam);

            double sphereparam = 0;
            DA.GetData(7, ref sphereparam);

            Boolean leftrot = new Boolean();
            DA.GetData(8, ref leftrot);
            // input parameter definition


            // output and code parameter definition : global variables
            List<Point3d> checkpts = new List<Point3d>();
            List<Point3d> pathpts = new List<Point3d>();
            List<Vector3d> orivecs = new List<Vector3d>();
            List<Vector3d> tanvecs = new List<Vector3d>();
            List<double> time = new List<double>();
            List<Sphere> spheres = new List<Sphere>();
            List<Curve> curves = new List<Curve>();

            double fiberlength = 0;
            int neg = 0;
            int h = 0;

            Point3d endpt = new Point3d();
            Point3d startpt = new Point3d();
            Vector3d nA2 = new Vector3d();
            Vector3d nB2 = new Vector3d();
            Point3d ipt2 = new Point3d();
            Vector3d vec2 = new Vector3d();
            Curve polyline1 = polylines[0];
            List<Arc> arc = new List<Arc>();
            Brep sph2 = new Brep();
            // output and code parameter definition


            // code: first anchor separately calculated

            Point3d anchorpt0 = anchorpts[syntax[0]];
            Point3d anchorpt1 = anchorpts[syntax[1]];

            Vector3d vec0 = anchorpt1 - anchorpt0;
            Vector3d nlinevec0 = vec0 / vec0.Length;                                // norm
            Vector3d nanchorvec0 = nanchorvecs[syntax[0]];
            Vector3d crossvec0 = Vector3d.CrossProduct(nanchorvec0, nlinevec0);     // right hand system
            
            Point3d arcminpt0 = new Point3d(anchorpt0 - crossvec0 * washerparam);
            Point3d arcmaxpt0 = new Point3d(anchorpt0 + crossvec0 * washerparam);

            // first hooking
            if (leftrot == true)
            {
                startpt = arcmaxpt0;
                endpt = arcminpt0;
            }
            else
            {
                startpt = arcminpt0;
                endpt = arcmaxpt0;
            }
            checkpts.Add(endpt);
            Arc arc0 = new Arc(startpt, - nlinevec0, endpt);
            arc.Add(arc0);
            
            // Path by Syntax
            for (int i = 1; i < syntax.Count - 1; i++)
            {
                anchorpt0 = anchorpts[syntax[i - 1]];                         // hooking position can differ from anchor mid point
                anchorpt1 = anchorpts[syntax[i]];
                
                Point3d pt0 = anchorpt0;
                Point3d pt1 = anchorpt1;

                Sphere sphere0 = new Sphere(anchorpt0, sphereparam);
                Brep sph0 = Brep.CreateFromSphere(sphere0);
                Sphere sphere1 = new Sphere(anchorpt1, sphereparam);
                Brep sph1 = Brep.CreateFromSphere(sphere1);

                Curve polyline0 = polylines[i - 1];
                polyline1 = polylines[i];

                Intersection.CurveBrep(polyline0, sph0, 0, out Curve[] curve1, out Point3d[] ipt);
                Point3d ipt0 = ipt[0];
                checkpts.Add(ipt0);
                Intersection.CurveBrep(polyline0, sph1, 0, out curve1, out ipt);
                Point3d ipt1 = ipt[0];
                checkpts.Add(ipt1);
                Intersection.CurveBrep(polyline1, sph1, 0, out curve1, out ipt);
                ipt2 = ipt[0];


                vec0 = ipt0 - pt0;   // vectors between intersection points and anchorpoints
                Vector3d nvec0 = vec0 / vec0.Length;
                Vector3d vec1 = ipt1 - pt1;                                          
                Vector3d nvec1 = vec1 / vec1.Length;
                vec2 = ipt2 - pt1;                                         
                Vector3d nvec2 = vec2 / vec2.Length;

                Vector3d veclast = endpt - pt0;
                Vector3d nveclast = veclast / veclast.Length;

                Vector3d nvecb = vec2 / vec2.Length;                                    // calculate projected vectors for hooking types
                Vector3d nveca = vec1 / vec1.Length;
                
                double a1 = Vector3d.Multiply(vec1, nvecb);
                double b1 = Vector3d.Multiply(vec2, nveca);
                
                Vector3d veca1 = a1 * nvecb;
                Vector3d vecb1 = b1 * nveca;

                Vector3d veca2 = vec1 - veca1;
                Vector3d vecb2 = vec2 - vecb1;

                Vector3d A1 = veca1;
                Vector3d A2 = veca2;
                nA2 = A2 / A2.Length;
                Vector3d B1 = vecb1;
                Vector3d B2 = vecb2;
                nB2 = B2 / B2.Length;

                ProjVector vecproj = new ProjVector(vec1, vec2);
                Vector3d vecp0 = vecp0.n

                string hook = hooking[syntax[i]];

                if(hook == "Y")
                {
                    neg = 1;
                    h = 1;
                }
                else if(hook == "X")
                {
                    neg = -1;
                    h = 1;   
                }
                else if(hook == "U")
                {
                    neg = -1;
                    h = 0;
                }                                                                       // Generierung der Kurven ab hier -> Unterteilung in Pfadpunkte
                                                                                        // Vektoren + Zeiten
                Point3d lpt0 = ipt0 + anchorparam * nveclast;                           
                checkpts.Add(lpt0);

                Point3d lpt1 = ipt1 + neg * nB2 * anchorparam;
                checkpts.Add(lpt1);

                Point3d stpt = pt1 + neg * nB2 * washerparam;
                checkpts.Add(stpt);

                BezierSpline spline0 = new BezierSpline(endpt, lpt0, vec0/2, -vec0/2);
                List<Point3d> sp0 = spline0.BezierSplinePoints();
                pathpts.AddRange(sp0);

                BezierSpline spline1 = new BezierSpline(lpt1, stpt, -vec1 / 2, vec1 / 2);
                List<Point3d> sp1 = spline1.BezierSplinePoints();
                pathpts.AddRange(sp1);
        
                endpt = pt1 + neg * nA2 * washerparam + nanchorvecs[i] * 0.3 * anchorparam * h;
                checkpts.Add(endpt);

                Arc arc1 = new Arc(stpt, - vec1, endpt);                                  // arc segment
                arc.Add(arc1);
            }

            Point3d anchorpt2 = anchorpts[syntax[syntax.Count - 1]];

            checkpts.Add(ipt2);
            Point3d lpt2 = ipt2 + neg * nA2 * anchorparam;
            checkpts.Add(lpt2);
            Sphere sphere2 = new Sphere(anchorpt2, sphereparam);
            sph2 = Brep.CreateFromSphere(sphere2);

            Intersection.CurveBrep(polyline1, sph2, 0, out Curve[] curve3, out Point3d[] inpt3);
            Point3d ipt3 = inpt3[0];
            checkpts.Add(ipt3);

            string lasthook = hooking[syntax.Count - 1];

            if (lasthook == "Y")
            {
                neg = 1;
                h = 1;
            }
            else if (lasthook == "X")
            {
                neg = -1;
                h = 1;
            }
            else if (lasthook == "U")
            {
                neg = -1;
                h = 0;
            }
            
            Point3d lpt3 = ipt3 + neg * nA2 * anchorparam;
            checkpts.Add(lpt3);

            Point3d pt2 = anchorpt2;
            Point3d lastpoint = pt2 + neg * nA2 * washerparam;
            checkpts.Add(lastpoint);

            Vector3d vec3 = ipt3 - pt2;


            BezierSpline spline2 = new BezierSpline(endpt, lpt2, vec2 / 2, -vec2 / 2);
            List<Point3d> sp2 = spline2.BezierSplinePoints();
            pathpts.AddRange(sp2);

            BezierSpline spline3 = new BezierSpline(lpt3, lastpoint, -vec3 / 2, vec3 / 2);
            List<Point3d> sp3 = spline3.BezierSplinePoints();
            pathpts.AddRange(sp3);
            // code

            // set output parameter
            DA.SetDataList(0, checkpts);
            DA.SetDataList(1, pathpts);
            DA.SetDataList(2, orivecs);
            DA.SetDataList(3, tanvecs);
            DA.SetDataList(4, time);
            DA.SetData(5, fiberlength);
            DA.SetDataList(6, curves);
            DA.SetDataList(7, arc);
            // set output parameter
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("9fe50ecf-20d6-461e-84e7-8659560d6e53"); }
        }
    }
}