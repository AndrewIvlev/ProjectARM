namespace ArmManipulatorApp.Graphics.Model3D
{
    using System.Windows.Media.Media3D;

    using ArmManipulatorApp.Common;
    using ArmManipulatorApp.Graphics.Interfaces;

    class Point3D : Notifier, IPoint3D
    {

        #region PROPS

        private double _x;
        public double X
        {
            get
            {
                return this._x;
            }

            set
            {
                if (this._x != value)
                {
                    this._x = value;
                    this.NotifyWithCallerPropName();
                    this.Notify("Coordinates");
                }
            }
        }

        private double _y;
        public double Y
        {
            get
            {
                return this._y;
            }

            set
            {
                if (this._y != value)
                {
                    this._y = value;
                    this.NotifyWithCallerPropName();
                    this.Notify("Coordinates");
                }
            }
        }

        private double _z;
        public double Z
        {
            get
            {
                return this._z;
            }

            set
            {
                if (this._z != value)
                {
                    this._z = value;
                    this.NotifyWithCallerPropName();
                    this.Notify("Coordinates");
                }
            }
        }

        public string Coordinates => $"({this.X},{this.Y},{this.Z})";

        #endregion

        #region CTOR

        public Point3D(double x = 0, double y = 0, double z = 0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        #endregion

        public IVector3D DistanceTo(IPoint3D endPoint)
        {
            return new Vector3D(endPoint.X - this.X, endPoint.Y - this.Y, endPoint.Z - this.Z);
        }

        #region OPERATORS

        public static IVector3D operator -(Point3D endPoint, Point3D startPoint)
        {
            return startPoint.DistanceTo(endPoint);
        }

        #endregion
    }
}
