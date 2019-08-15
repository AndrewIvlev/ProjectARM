namespace MainApp.Graphics.Model3D
{
    using MainApp.Common;
    using MainApp.Graphics.Interfaces;

    class Triangle3D : Notifier, ITriangle3D
    {

        #region PROPS

        public IModel3D Model3D { get; }

        private IPoint3D _point1;
        public IPoint3D Point1
        {
            get { return _point1; }

            set
            {
                if (_point1 != value)
                {
                    _point1 = value;
                    NotifyWithCallerPropName();
                }
            }
        }

        private IPoint3D _point2;
        public IPoint3D Point2
        {
            get { return _point2; }

            set
            {
                if (_point2 != value)
                {
                    _point2 = value;
                    NotifyWithCallerPropName();
                }
            }
        }

        private IPoint3D _point3;
        public IPoint3D Point3
        {
            get { return _point3; }

            set
            {
                if (_point3 != value)
                {
                    _point3 = value;
                    NotifyWithCallerPropName();
                }
            }
        }

        #endregion

        #region CTOR

        public Triangle3D(IModel3D model, IPoint3D point1, IPoint3D point2, IPoint3D point3)
        {
            Model3D = model;
            Point1 = point1;
            Point2 = point2;
            Point3 = point3;
        }

        #endregion
    }
}