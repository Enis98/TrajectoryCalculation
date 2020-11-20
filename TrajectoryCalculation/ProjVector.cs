using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;


namespace TrajectoryCalculation
{
    class ProjVector
    {
        public Vector3d veca;
        public Vector3d vecb;

        public ProjVector(Vector3d Veca, Vector3d Vecb)
        {
            veca = Veca;
            vecb = Vecb;
        }

        public Vector3d nA1()
        {
            Vector3d nvecb = vecb / vecb.Length;                                    // calculate projected vectors for hooking types
            Vector3d nveca = veca / veca.Length;

            double a1 = Vector3d.Multiply(veca, nvecb);
            double b1 = Vector3d.Multiply(vecb, nveca);

            Vector3d veca1 = a1 * nvecb;
            Vector3d vecb1 = b1 * nveca;

            Vector3d veca2 = veca - veca1;
            Vector3d vecb2 = vecb - vecb1;

            Vector3d A1 = veca1;
            Vector3d nA1 = A1 / A1.Length;
            Vector3d A2 = veca2;
            Vector3d nA2 = A2 / A2.Length;
            Vector3d B1 = vecb1;
            Vector3d nB1 = B1 / B1.Length;
            Vector3d B2 = vecb2;
            Vector3d nB2 = B2 / B2.Length;

            return nA1;
        }

        public Vector3d nA2()
        {
            Vector3d nvecb = vecb / vecb.Length;                                    // calculate projected vectors for hooking types
            Vector3d nveca = veca / veca.Length;

            double a1 = Vector3d.Multiply(veca, nvecb);
            double b1 = Vector3d.Multiply(vecb, nveca);

            Vector3d veca1 = a1 * nvecb;
            Vector3d vecb1 = b1 * nveca;

            Vector3d veca2 = veca - veca1;
            Vector3d vecb2 = vecb - vecb1;

            Vector3d A1 = veca1;
            Vector3d nA1 = A1 / A1.Length;
            Vector3d A2 = veca2;
            Vector3d nA2 = A2 / A2.Length;
            Vector3d B1 = vecb1;
            Vector3d nB1 = B1 / B1.Length;
            Vector3d B2 = vecb2;
            Vector3d nB2 = B2 / B2.Length;

            return nA2;
        }

        public Vector3d nB1()
        {
            Vector3d nvecb = vecb / vecb.Length;                                    // calculate projected vectors for hooking types
            Vector3d nveca = veca / veca.Length;

            double a1 = Vector3d.Multiply(veca, nvecb);
            double b1 = Vector3d.Multiply(vecb, nveca);

            Vector3d veca1 = a1 * nvecb;
            Vector3d vecb1 = b1 * nveca;

            Vector3d veca2 = veca - veca1;
            Vector3d vecb2 = vecb - vecb1;

            Vector3d A1 = veca1;
            Vector3d nA1 = A1 / A1.Length;
            Vector3d A2 = veca2;
            Vector3d nA2 = A2 / A2.Length;
            Vector3d B1 = vecb1;
            Vector3d nB1 = B1 / B1.Length;
            Vector3d B2 = vecb2;
            Vector3d nB2 = B2 / B2.Length;

            return nB1;
        }

        public Vector3d nB2()
        {
            Vector3d nvecb = vecb / vecb.Length;                                    // calculate projected vectors for hooking types
            Vector3d nveca = veca / veca.Length;

            double a1 = Vector3d.Multiply(veca, nvecb);
            double b1 = Vector3d.Multiply(vecb, nveca);

            Vector3d veca1 = a1 * nvecb;
            Vector3d vecb1 = b1 * nveca;

            Vector3d veca2 = veca - veca1;
            Vector3d vecb2 = vecb - vecb1;

            Vector3d A1 = veca1;
            Vector3d nA1 = A1 / A1.Length;
            Vector3d A2 = veca2;
            Vector3d nA2 = A2 / A2.Length;
            Vector3d B1 = vecb1;
            Vector3d nB1 = B1 / B1.Length;
            Vector3d B2 = vecb2;
            Vector3d nB2 = B2 / B2.Length;

            return nB2;
        }
    }
}
