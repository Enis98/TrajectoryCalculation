using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace TrajectoryCalculation
{
    public class TrajectoryCalculationComponent : GH_Component
    {
        
        public TrajectoryCalculationComponent()
          : base("PathPoints",
                 "PathPoints",
                 "Calculates the Path Points of the trajectory",
                 "TrajectoryCalculation",
                 "PathPoints")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Anchor points", "AnchorPts", "The anchor points", GH_ParamAccess.list);
            pManager.AddTextParameter("Anchor point IDs in order", "AnchorPtsIDs", "The anchor point IDs in order", GH_ParamAccess.list);
            pManager.AddVectorParameter("Anchor point orientations", "AnchorVecs", "The anchor point orientations", GH_ParamAccess.list);
            pManager.AddNumberParameter("Anchor point parameters", "AnchorParams", "Anchor point parameters", GH_ParamAccess.list);
            pManager.AddNumberParameter("Washer parameters", "WasherParams", "Washer parameters", GH_ParamAccess.list);
            pManager.AddBooleanParameter("StartLeftRotation", "StLeftRot", "Direction of Rotation at Start Point is left", GH_ParamAccess.item, true);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("PathPts", "PathPts", "Path Points of winding structure", GH_ParamAccess.list);
            pManager.AddVectorParameter("OriVecs", "OriVecs", "Orientation Vector of a Path Point", GH_ParamAccess.list);
            pManager.AddVectorParameter("TanVecs", "TanVecs", "Tangential Vector of a Path Point", GH_ParamAccess.list);
            pManager.AddNumberParameter("Time", "Time", "Time at a Path Point", GH_ParamAccess.list);
            pManager.AddNumberParameter("FiberLength", "FiberLength", "Fiber Length up to a Path Point", GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> anchorpts = new List<Point3d>();
            DA.GetData("AnchorPts", ref anchorpts);

            List<int> anchorids = new List<int>();           
            DA.GetData("AnchorPtsIDs", ref anchorids);

            List<Vector3d> anchorvecs = new List<Vector3d>();
            DA.GetData("AnchorVecs", ref anchorvecs);

            List<double> anchorparam = new List<double>();
            DA.GetData("AnchorParams", ref anchorparam);

            List<double> washerparam = new List<double>();
            DA.GetData("WasherParams", ref washerparam);

            Boolean leftrot = new Boolean();
            DA.GetData("StLeftRot", ref leftrot);
            // input parameter definition

            List<Point3d> pathpts = new List<Point3d>();

            List<Vector3d> orivecs = new List<Vector3d>();

            List<Vector3d> tanvecs = new List<Vector3d>();

            List<double> time = new List<double>();

            double fiberlength = 0;
            //output parameter definition



            pathpts = anchorpts;

            Vector3d orivec = new Vector3d();
            foreach(Vector3d anchorvec in anchorvecs)
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










            DA.SetData("PathPts", pathpts);

            DA.SetData("OriVecs", orivecs);

            DA.SetData("TanVecs", tanvecs);

            DA.SetData("Time", time);

            DA.SetData("FiberLength", fiberlength);
            //set output params
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("87839377-a32c-42f4-97ee-214e5dd8663a"); }
        }
    }
}
