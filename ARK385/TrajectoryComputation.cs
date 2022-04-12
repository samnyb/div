using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace ARK385
{
    public class TrajectoryComputation : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public TrajectoryComputation() : base("Trajectory computation", "TrajComp", "Calculates the trajectory of a launched object.", "ARK385", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "Pt", "The initial location of the object.", GH_ParamAccess.item, new Point3d(0, 0, 0));
            pManager.AddNumberParameter("Velocity", "v", "Initial velocity of object in m/s (double)", GH_ParamAccess.list); // Accessar list ifall man vill ge m�nga olika hastigheter och s�dant
            pManager.AddVectorParameter("Vector", "V", "Initial direction vector of object (vector)", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Iterations", "n", "Number of displayed points along trajectory (1/second)", GH_ParamAccess.item, 20);
            // �verv�g att addera typ frekvens, tid osv
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "Pts", "Calculated points along trajectory", GH_ParamAccess.list);
            pManager.AddCurveParameter("Trajectory", "T", "Approximated trajectory of object", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Variables for saving our initial values in
            Point3d pt = new Point3d();
            List<double> velocity = new List<double>();
            List<Vector3d> vectors = new List<Vector3d>();
            int iterations = new int();

            // Variables for later use
            Point2d tempPoint = new Point2d();
            Vector2d tempVel = new Vector2d();

            // Variables to output
            List<Point2d> location = new List<Point2d>();
            var trajectory = new Polyline();


            // Get indata, validate
            if (!DA.GetData(0, ref pt)) return;
            if (pt == null)
            {
                pt.X = 0;                                       // If there is no point, start from world origo.
                pt.Y = 0;
                pt.Z = 0;
            }

            if (!DA.GetDataList(1, velocity)) return;
            if (velocity.Count == 0)
            {
                velocity.Add(0);                                // If the list is empty, add a velocity of zero.
            }

            if (!DA.GetDataList(2, vectors)) return;
            if (vectors.Count == 0)
            {
                vectors.Add(new Vector3d(1, 5, 0));                // If the list is empty, add a basic initial vector.
            }


            if (!DA.GetData(3, ref iterations)) return;
            if (iterations < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Iterations must be a positive integer");
                return;
            }

            // Actual runtime

            // IF vectors and velocities aren't long enough, pad in some way? Maybe test first though

            foreach (Vector3d v in vectors)
            {
                v.Unitize();
            }

            tempPoint.X = pt.X;
            tempPoint.Y = pt.Y;

            tempVel.X = vectors[0].X * velocity[0];
            tempVel.Y = vectors[0].Y * velocity[0]; 

            for (double i = 0; i < iterations; i+= 0.1)
            {
                tempVel.Y = tempVel.Y - 9.82 * i;
                tempPoint.X = /*tempPoint.X +*/ tempVel.X * i; //Ska den verkligen vara d�r? Det blir ju bara snabbare och snabbare..!!
                tempPoint.Y = tempPoint.Y + tempVel.Y * i; // Den h�r �r ev. inte heller r�tt, det borde v�l vara delat med tv� n�gonstans? unders�k. Vore najs att sl� ihop det med "initiella" v�rden p� n�got s�tt ocks�.
                if (tempPoint.Y < 0)
                {
                    tempPoint.Y = Math.Abs(tempPoint.Y);
                    tempVel.Y = Math.Abs(tempVel.Y);            // Det h�r funkar inte, �terkom till stutsen.
                }
                location.Add(tempPoint);
            }

            foreach (Point2d node in location)
            {
                trajectory.Add(new Point3d(node.X, node.Y, 0));
            }

            DA.SetDataList(0, location);
            DA.SetData(1, trajectory);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("79832768-B7B3-4D00-9195-85A6D608AADE");
    }
}