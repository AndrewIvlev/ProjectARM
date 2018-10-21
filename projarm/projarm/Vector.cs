using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projarm
{
    class Vector
    {
        double[] v;

        public Vector()
        {
            /*
            v = new double[4];
            v[3] = 1;
            */
            v = new double[3];
            v[2] = 1;
        }   //2D - (0,0,1); 3D - (0,0,0,1)
        public Vector(Vector W)
        {
            v = new double[3];
            v[0] = W.v[0];
            v[1] = W.v[1];
            v[2] = W.v[2];

        }
        public Vector(double x1, double x2) : this()
        {
            v[0] = x1;
            v[1] = x2;
        }
        public Vector(double x1, double x2, double x3) : this()
        {
            v[0] = x1;
            v[1] = x2;
            v[2] = x3;
        }
        public static Vector operator *(Vector V, double s)
        {
            V.v[3] /= s;
            return V;
        }
        public static Vector operator +(Vector V, Vector W)
        {
            Vector R = new Vector(V);
            for(int i = 0; i < 2; i++)
                R.v[i] = V.v[i] + W.v[i];
            R.v[2] = 1;
            return R;
        }
        /*public static Vector operator *(Vector V, Vector W)
        {
            Vector R = new Vector(V);
            for (int i = 0; i < 2; i++)
                R.v[i] = V.v[i] + W.v[i];
            R.v[2] = 1;
            return R;
        }*/
        public static double op_ScalarMultiply(Vector V, Vector W)
        {
            return (V.v[0] * W.v[0] + V.v[1] * W.v[1] + V.v[2] * W.v[2]) / (V.v[3] * W.v[3]);
        }
        public double GetX()
        {
            return v[0] / v[2];
        }
        public double GetY()
        {
            return v[1] / v[2];
        }
        /*public double GetZ()
        {
            return v[2] / v[3];
        }*/
    }
}
