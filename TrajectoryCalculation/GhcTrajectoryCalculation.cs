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
            pManager.AddPointParameter("PathPts", "PathPts", "Path Points of winding structure", GH_ParamAccess.list);
            pManager.AddVectorParameter("OriVecs", "OriVecs", "Orientation Vector of a Path Point", GH_ParamAccess.list);
            pManager.AddVectorParameter("TanVecs", "TanVecs", "Tangential Vector of a Path Point", GH_ParamAccess.list);
            pManager.AddNumberParameter("Time", "Time", "Time at a Path Point", GH_ParamAccess.list);
            pManager.AddNumberParameter("FiberLength", "FiberLength", "Fiber Length up to a Path Point", GH_ParamAccess.item);
            pManager.AddCurveParameter("curves", "curves", "curves", GH_ParamAccess.list);
            pManager.AddArcParameter("arcs", "arc", "arc", GH_ParamAccess.item);
            pManager.AddSurfaceParameter("spheres", "spheres", "spheres", GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // input parameter definition
            List<int> syntax = new List<int>();
            DA.GetDataList(0, syntax);

            List<string> hooking = new List<string>();
            DA.GetDataList(1, hooking);

            List<Point3d> anchorpts = new List<Point3d>();
            DA.GetDataList(2, anchorpts);
            // neue liste die in for schleife benutzt wird machen

            List<Vector3d> anchorvecs = new List<Vector3d>();
            DA.GetDataList(3, anchorvecs);
            List<Vector3d> nanchorvecs = new List<Vector3d>();                  // normed anchorvecs

            foreach(Vector3d anchorvec in anchorvecs)
            {
                Vector3d nanchorvec = anchorvec / anchorvec.Length;             // calc of normed anchorvec ->  list: nanchorvecs
                nanchorvecs.Add(nanchorvec);
            }

            double anchorparam = 0;
            DA.GetData(4, ref anchorparam);

            double washerparam = 0;
            DA.GetData(5, ref washerparam);

            double sphereparam = 0;
            DA.GetData(6, ref sphereparam);

            Boolean leftrot = new Boolean();
            DA.GetData(7, ref leftrot);
            // input parameter definition


            // output and code parameter definition
            
            List<Point3d> pathpts = new List<Point3d>();
            List<Vector3d> orivecs = new List<Vector3d>();
            List<Vector3d> tanvecs = new List<Vector3d>();
            List<double> time = new List<double>();
            List<Sphere> spheres = new List<Sphere>();
            List<Curve> curves = new List<Curve>();
            double fiberlength = 0;
            int neg = 0;
            Point3d endpt = new Point3d();
            Vector3d vec = new Vector3d();
            Arc arc = new Arc();
            List<Point3d> ipts = new List<Point3d>();

            LineCurve line1 = new LineCurve();                                  // for last anchor
            Brep sph2 = new Brep();

            // output and code parameter definition


            // code: first anchor separately calculated

            Point3d anchorpt0 = anchorpts[syntax[0]];
            Point3d anchorpt1 = anchorpts[syntax[1]];

            Curve line0 = new LineCurve(anchorpt0, anchorpt1);
            curves.Add(line0);

            Sphere sphere0 = new Sphere(anchorpt0, sphereparam);
            spheres.Add(sphere0);

            Brep sph0 = Brep.CreateFromSphere(sphere0);
            Intersection.CurveBrep(line0, sph0, 0, out Curve[] icurves, out Point3d[] ipt0);                  // intersection point first sphere - polyline

            pathpts.Add(ipt0[0]);

            Vector3d vec0 = anchorpt1 - anchorpt0;
            Vector3d nlinevec0 = vec0 / vec0.Length;                                // norm
            Vector3d nanchorvec0 = nanchorvecs[syntax[0]];
            Vector3d crossvec0 = Vector3d.CrossProduct(nanchorvec0, nlinevec0);     // right hand system
            
            Point3d arcminpt0 = new Point3d(anchorpt0 - crossvec0 * washerparam);
            Point3d arcmaxpt0 = new Point3d(anchorpt0 + crossvec0 * washerparam);

            int StRot = 0;

            // first hooking
            if (leftrot == true)
            {
                StRot = -1;
                endpt = arcminpt0;
            }
            else
            {
                StRot = 1;
                endpt = arcmaxpt0;
            }
            pathpts.Add(endpt);
            arc = new Arc(arcminpt0, nlinevec0 * StRot, arcmaxpt0);

            // Path by Syntax
            for (int i = 1; i < syntax.Count - 1; i++)
            {
                anchorpt0 = anchorpts[syntax[i - 1]];                         // hooking position can differ from anchor mid point
                anchorpt1 = anchorpts[syntax[i]];
                Point3d anchorpt2 = anchorpts[syntax[i + 1]];
                
                Point3d pt0 = anchorpt0;
                Point3d pt1 = anchorpt1;
                Point3d pt2 = anchorpt2;

                sphere0 = new Sphere(anchorpt0, sphereparam);
                sph0 = Brep.CreateFromSphere(sphere0);
                Sphere sphere1 = new Sphere(anchorpt1, sphereparam);
                Brep sph1 = Brep.CreateFromSphere(sphere1);
                Sphere sphere2 = new Sphere(anchorpt2, sphereparam);
                sph2 = Brep.CreateFromSphere(sphere2);

                spheres.Add(sphere1);

                line0 = new LineCurve(pt0, pt1);
                line1 = new LineCurve(pt1, pt2);

                curves.Add(line1);

                Intersection.CurveBrep(line0, sph0, 0, out Curve[] curve0, out ipt0);
                Intersection.CurveBrep(line0, sph1, 0, out Curve[] curve1, out Point3d[] ipt1);
                pathpts.Add(ipt1[0]);
                Intersection.CurveBrep(line1, sph1, 0, out Curve[] curve2, out Point3d[] ipt2);
                pathpts.Add(ipt2[0]);


                vec0 = ipt0[0] - pt0;                                                   // vectors between intersection points and anchorpoints
                Vector3d nvec0 = vec0 / vec0.Length;
                Vector3d vec1 = ipt1[0] - pt1;                                          
                Vector3d nvec1 = vec1 / vec1.Length;
                Vector3d vec2 = ipt2[0] - pt1;                                         
                Vector3d nvec2 = vec2 / vec2.Length;

                Vector3d veclast = endpt - pt0;
                Vector3d nveclast = veclast / veclast.Length;

                Point3d lpt0 = ipt0[0] + anchorparam * nveclast;
                pathpts.Add(lpt0);

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
                Vector3d nA2 = A2 / A2.Length;
                Vector3d B1 = vecb1;
                Vector3d B2 = vecb2;
                Vector3d nB2 = B2 / B2.Length;

                string hook = hooking[syntax[i]];

                if(hook == "Y")
                {
                    neg = 1;
                }
                else if(hook == "U" || hook == "X")
                {
                    neg = -1;
                }

                Point3d lpt1 = ipt1[0] + neg * nB2 * anchorparam;
                pathpts.Add(lpt1);

                Point3d startpt = pt1 + neg * nB2 * washerparam;
                pathpts.Add(startpt);
                endpt = pt1 + neg * nA2 * washerparam;
                pathpts.Add(endpt);

                if(hook == "X")
                {
                    // weitere umwicklung
                }


            }

            Intersection.CurveBrep(line1, sph2, 0, out Curve[] curve3, out Point3d[] ipt3);
            pathpts.Add(ipt3[0]);

            // code

            // test of output parameter
            Vector3d orivec = new Vector3d();                   
            foreach (Vector3d nanchorvec in nanchorvecs)
            {
                orivec = nanchorvec * 2.0;
                orivecs.Add(orivec);
                tanvecs.Add(orivec);
            }
               
               
            if (leftrot == true)
                time.Add(1);
            else
                time.Add(5);

            fiberlength = 5;                                    
            // test of output parameter

            // set output parameter
            DA.SetDataList(0, pathpts);
            DA.SetDataList(1, orivecs);
            DA.SetDataList(2, tanvecs);
            DA.SetDataList(3, time);
            DA.SetData(4, fiberlength);
            DA.SetDataList(5, curves);
            DA.SetData(6, arc);
            DA.SetDataList(7, spheres);
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