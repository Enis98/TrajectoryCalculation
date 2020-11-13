using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
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
            pManager.AddPointParameter("Anchor points", "AnchorPts", "The anchor points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Anchor point orientations", "AnchorVecs", "The anchor point orientations", GH_ParamAccess.list);
            pManager.AddNumberParameter("Anchor point parameters", "AnchorParams", "Anchor point parameters", GH_ParamAccess.item);
            pManager.AddNumberParameter("Washer parameters", "WasherParams", "Washer parameters", GH_ParamAccess.item);
            pManager.AddBooleanParameter("StartLeftRotation", "StLeftRot", "Direction of Rotation at Start Point is left", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("PathPts", "PathPts", "Path Points of winding structure", GH_ParamAccess.list);
            pManager.AddVectorParameter("OriVecs", "OriVecs", "Orientation Vector of a Path Point", GH_ParamAccess.list);
            pManager.AddVectorParameter("TanVecs", "TanVecs", "Tangential Vector of a Path Point", GH_ParamAccess.list);
            pManager.AddNumberParameter("Time", "Time", "Time at a Path Point", GH_ParamAccess.list);
            pManager.AddNumberParameter("FiberLength", "FiberLength", "Fiber Length up to a Path Point", GH_ParamAccess.item);
            pManager.AddLineParameter("Line", "Line", "Line", GH_ParamAccess.list);
            pManager.AddArcParameter("arc", "arc", "arc", GH_ParamAccess.item);
            
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // input parameter definition
            List<int> gfsyntax = new List<int>();
            DA.GetDataList(0, gfsyntax);

            List<Point3d> anchorpts = new List<Point3d>();
            DA.GetDataList(1, anchorpts);

            List<Vector3d> anchorvecs = new List<Vector3d>();
            DA.GetDataList(2, anchorvecs);
            List<Vector3d> nanchorvecs = new List<Vector3d>();                  // normed anchorvec

            foreach(Vector3d anchorvec in anchorvecs)
            {
                Vector3d nanchorvec = anchorvec / anchorvec.Length;             // calc of normed anchorvec ->  list: nanchorvecs
                nanchorvecs.Add(nanchorvec);
            }

            double anchorparam = 0;
            DA.GetData(3, ref anchorparam);

            double washerparam = 0;
            DA.GetData(4, ref washerparam);

            Boolean leftrot = new Boolean();
            DA.GetData(5, ref leftrot);
            // input parameter definition


            // output parameter definition
            List<Point3d> pathpts = new List<Point3d>();

            List<Vector3d> orivecs = new List<Vector3d>();

            List<Vector3d> tanvecs = new List<Vector3d>();

            List<double> time = new List<double>();

            double fiberlength = 0;

            List<LineCurve> displayLines = new List<LineCurve>();

            Vector3d linevec = new Vector3d();
            // output and code parameter definition


            // code
            Point3d anchorpt0 = anchorpts[gfsyntax[0]];
            Point3d anchorpt1 = anchorpts[gfsyntax[1]];

            Vector3d linevec0 = anchorpt1 - anchorpt0;
            Vector3d nlinevec0 = linevec0 / linevec0.Length;                        // norm
            Vector3d nanchorvec0 = nanchorvecs[gfsyntax[0]];
            Vector3d crossvec0 = Vector3d.CrossProduct(nanchorvec0, nlinevec0);     // right hand system
            Point3d arcminpt0 = new Point3d(anchorpt0 - crossvec0 * washerparam);
            Point3d arcmaxpt0 = new Point3d(anchorpt0 + crossvec0 * washerparam);
            Arc arc = new Arc(arcmaxpt0, nlinevec0 * -1, arcminpt0);




            for (int i = 0; i < gfsyntax.Count - 1; i++)
            {
                Point3d pt1 = anchorpts[gfsyntax[i]];                               // pos of current anchor
                Point3d pt2 = anchorpts[gfsyntax[i + 1]];                           // pos of next anchor
                linevec = pt2 - pt1;                                                // vector that equals line between current and next anchor with direction onto next anchor
                Vector3d nlinevec = linevec / linevec.Length;                       // normed linevector
                
                displayLines.Add(new LineCurve(pt1, pt2));                          // lines between anchors that follows syntax
                pathpts.Add(pt1);
            }

            // code

            
            //pathpts = anchorpts;

            Vector3d orivec = new Vector3d();
            foreach (Vector3d anchorvec in anchorvecs)
            {
                orivec = anchorvec * 2.0;
                orivecs.Add(orivec);
                tanvecs.Add(orivec);
            }
               
               
            if (leftrot == true)
                time.Add(1);
            else
                time.Add(5);

            fiberlength = 5;                            //Beispielhafte Rechnungen zum testen der Ausgabeparaeter
            


            // set output parameter
            DA.SetDataList(0, pathpts);

            DA.SetDataList(1, orivecs);

            DA.SetDataList(2, tanvecs);

            DA.SetDataList(3, time);

            DA.SetData(4, fiberlength);

            DA.SetDataList(5, displayLines);

            DA.SetData(6, arc);
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