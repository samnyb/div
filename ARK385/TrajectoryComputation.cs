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
            pManager.AddNumberParameter("Velocity", "v", "Initial velocity of object in m/s (double)", GH_ParamAccess.list); // Accessar list ifall man vill ge många olika hastigheter och sådant
            pManager.AddVectorParameter("Vector", "V", "Initial direction vector of object (vector)", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Iterations", "n", "Number of displayed points along trajectory (1/second)", GH_ParamAccess.item, 20);
            // Överväg att addera typ frekvens, tid osv

            // TODO: det är inte sant att iterationen är per sekund för hastigheten ges nu i m/s, men är helt opåverkad av iterationen. Samstäm.
            // ELLER???
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
            Point2d prevPoint = new Point2d();
            Vector2d tempVel = new Vector2d();
            Vector2d prevVel = new Vector2d();

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

            foreach (Vector3d v in vectors)
            {
                v.Unitize();
            }

            // Utgångsvärden
            prevPoint.X = pt.X;
            prevPoint.Y = pt.Y;

            prevVel.X = vectors[0].X * velocity[0];
            prevVel.Y = vectors[0].Y * velocity[0];

            int bounceReset = 0;

            for (int t = 0; t < iterations*10; t++)
            {
                tempVel.X = prevVel.X;
                tempVel.Y = prevVel.Y - 9.82 * t/10;

                tempPoint.X = prevPoint.X + tempVel.X;
                tempPoint.Y = prevPoint.Y + tempVel.Y;

                bounceReset++;

                if (tempPoint.Y <= 0)
                {
                    break;
                    /*tempPoint.Y = Math.Abs(tempPoint.Y);
                    tempVel.Y = Math.Sqrt(0.1 * Math.Pow(tempVel.Y, 2));            // Math.Abs(tempVel.Y) * 0.1;
                    bounceReset = 0;*/
                }

                location.Add(tempPoint);
                prevVel = tempVel;
                prevPoint = tempPoint;
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