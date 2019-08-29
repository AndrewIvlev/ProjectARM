namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using ArmManipulatorApp.Common;
    using ArmManipulatorApp.Graphics3DModel.Interfaces;

    class Vector3D : Notifier, IVector3D
    {
        #region PROPS

        private double _x;
        public double X
        {
            get
            {
                return _x;
            }

            set
            {
                if (_x != value)
                {
                    _x = value;
                    NotifyWithCallerPropName();
                    Notify("Norm");
                }
            }
        }

        private double _y;
        public double Y
        {
            get
            {
                return _y;
            }

            set
            {
                if (_y != value)
                {
                    _y = value;
                    NotifyWithCallerPropName();
                    Notify("Norm");
                }
            }
        }

        private double _z;
        public double Z
        {
            get
            {
                return _z;
            }

            set
            {
                if (_z != value)
                {
                    _z = value;
                    NotifyWithCallerPropName();
                    Notify("Norm");
                }
            }
        }

        public double Norm => DotProduct(this);

        #endregion        

        #region CTOR

        public Vector3D(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = x;
        }

        #endregion

        public IVector3D Add(IVector3D vector)
        {
            return new Vector3D(X + vector.X, Y + vector.Y, Z + vector.Z);
        }

        public IVector3D Subtract(IVector3D vector)
        {
            return new Vector3D(X - vector.X, Y - vector.Y, Z - vector.Z);
        }

        public IVector3D Multiply(double factor)
        {
            return new Vector3D(X * factor, Y * factor, Z * factor);
        }

        public IVector3D CrossProduct(IVector3D vector)
        {
            return new Vector3D(Y * vector.Z - Z * vector.Y, Z * vector.X - X * vector.Z, X * vector.Y - Y * vector.X);
        }

        public double DotProduct(IVector3D vector)
        {
            return vector.X * X + vector.Y * Y + vector.Z * Z;
        }

        #region OPERATORS

        public static IVector3D operator +(Vector3D vector1, Vector3D vector2)
        {
            return vector1.Add(vector2);
        }

        public static IVector3D operator -(Vector3D vector1, Vector3D vector2)
        {
            return vector1.Subtract(vector2);
        }

        public static IVector3D operator *(Vector3D vector1, Vector3D vector2)
        {
            return vector1.CrossProduct(vector2);
        }

        public static IVector3D operator *(double factor, Vector3D vector)
        {
            return vector.Multiply(factor);
        }

        public static IVector3D operator *(Vector3D vector, double factor)
        {
            return vector.Multiply(factor);
        }

        public static double operator ^(Vector3D vector1, Vector3D vector2)
        {
            return vector1.DotProduct(vector2);
        }

        #endregion
    }
}