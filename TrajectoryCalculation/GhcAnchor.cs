using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace TrajectoryCalculation
{
    public class GhcAnchor : GH_Component
    {
        public GhcAnchor()
          : base("GhcAnchor",
                 "GhcAnchor",
                 "Builds the Anchors as Brep",
                 "CorelessWinding",
                 "Trajectory")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Anchor points", "AnchorPts", "The anchor points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Anchor point orientations", "AnchorVecs", "The anchor point orientations", GH_ParamAccess.list);
            pManager.AddNumberParameter("Anchor point parameters", "AnchorParams", "Anchor point parameters", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddBrepParameter("AnchorGeometry", "AnchorGeometry", "3D Geometry of the Anchors", GH_ParamAccess.tree);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> anchorpts = new List<Point3d>();
            DA.GetDataList(0, anchorpts);

            List<Vector3d> anchorvecs = new List<Vector3d>();
            DA.GetDataList(1, anchorvecs);

            double dia1 = 0;
            DA.GetData(2, ref dia1);

            DataTree<Brep> displayGeo = new DataTree<Brep>();

            for (int i=0; i < anchorpts.Count; i++)
            { 
                AnchorGeometry all = new AnchorGeometry(anchorpts[i], anchorvecs[i], 40, dia1/2, 5, 22.5);
                List<Brep> displayGeometry = all.ComputeAnchorGeometry();
                displayGeo.AddRange(displayGeometry);
            }


            DA.SetDataTree(0, displayGeo);
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