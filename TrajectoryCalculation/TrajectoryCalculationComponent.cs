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
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("PathPoints", "PathPoints", "Path Points of winding structure", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distances", "Distances", "Distances", GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.Get

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
