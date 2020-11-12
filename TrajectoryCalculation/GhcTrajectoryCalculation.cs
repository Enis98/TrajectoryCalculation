using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace TrajectoryCalculation
{
    public class GhcTrajectoryCalculation : GH_Component
    {
        public GhcTrajectoryCalculation()
          : base("TrajectoryCalc",
                 "TrajectoryCalc",
                 "calculates the trajectory",
                 "CorelessWinding",
                 "Trajectory")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
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
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> anchorpts = new List<Point3d>();
            DA.GetDataList(0, anchorpts);

            List<Vector3d> anchorvecs = new List<Vector3d>();
            DA.GetDataList(1, anchorvecs);

            double anchorparam = 0;
            DA.GetData(2, ref anchorparam);

            double washerparam = 0;
            DA.GetData(3, ref washerparam);

            Boolean leftrot = new Boolean();
            DA.GetData(4, ref leftrot);
            // input parameter definition

            List<Point3d> pathpts = new List<Point3d>();

            List<Vector3d> orivecs = new List<Vector3d>();

            List<Vector3d> tanvecs = new List<Vector3d>();

            List<double> time = new List<double>();

            double fiberlength = 5;
            // output parameter definition




            pathpts = anchorpts;

            Vector3d orivec = new Vector3d();
            foreach (Vector3d anchorvec in anchorvecs)
                orivec = anchorvec * 2.0;
            orivecs.Add(orivec);

            Vector3d tanvec = new Vector3d();
            foreach (Vector3d anchorvec in anchorvecs)
                tanvec = anchorvec * 2.0;
            tanvecs.Add(tanvec);

            if (leftrot == true)
                time.Add(1);
            else
                time.Add(5);

            fiberlength = 5;



            DA.SetDataList(0, pathpts);

            DA.SetDataList(1, orivecs);

            DA.SetDataList(2, tanvecs);

            DA.SetDataList(3, time);

            DA.SetData(4, fiberlength);
            // set output params
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