using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace ARK385
{
    public class ARK385Info : GH_AssemblyInfo
    {
        public override string Name => "ARK385";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("F629D65E-6CDE-455C-B067-36ED43983207");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}