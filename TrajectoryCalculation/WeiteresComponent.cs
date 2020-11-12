using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace TrajectoryCalculation
{
    public class WeiteresComponent : GH_Component
    {
        public WeiteresComponent()
          : base("MyComponent1",
                 "Nickname",
                 "Description",
                 "CorelessWinding",
                 "Trajectory")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Anchor points", "AnchorPts", "The anchor points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Anchor point orientations", "AnchorVecs", "The anchor point orientations", GH_ParamAccess.list);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddBrepParameter("AnchorGeometry", "AnchorGeometry", "3D Geometry of the Anchors", GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> anchorpts = new List<Point3d>();
            DA.GetDataList(0, anchorpts);

            List<Vector3d> anchorvecs = new List<Vector3d>();
            DA.GetDataList(1, anchorvecs);

            AnchorGeometry all = new AnchorGeometry(anchorpts[1], anchorvecs[1], 20, 20, 20, 20);
            List<Brep> displayGeometry = all.ComputeAnchorGeometry();

            DA.SetDataList(0, displayGeometry);
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
            get { return new Guid("9b0c3a03-eb03-4078-a045-7299666e3aa6"); }
        }
    }
}