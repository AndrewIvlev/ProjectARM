namespace ArmManipulatorApp.Graphics.Model
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    using ArmManipulatorApp.Common;
    using ArmManipulatorApp.Graphics.Interfaces;

    class Model3D : Notifier, IModel3D
    {

        static int InstanceCount = 1;

        #region PROPS

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyWithCallerPropName();
                }
            }
        }

        public ICollection<IPoint3D> Points { get; }

        public ICollection<ITriangle3D> Triangles { get; }

        #endregion

        #region CTOR

        public Model3D(string description = null, IEnumerable<IPoint3D> points = null, IEnumerable<ITriangle3D> triangles = null)
        {
            Description = description ?? "MODEL_" + InstanceCount++;
            Points = new ObservableCollection<IPoint3D>();
            Triangles = new ObservableCollection<ITriangle3D>();

            PassNotificationThrough(Points);
            if (points != null)
            {
                foreach (var point in points)
                    Points.Add(point);
            }

            PassNotificationThrough(Triangles);
            if (triangles != null)
            {
                foreach (var triangle in triangles)
                    Triangles.Add(triangle);
            }
        }

        #endregion

        private void PassNotificationThrough(object sender, PropertyChangedEventArgs e)
        {
            Notify("Internal");
        }

        private void PassNotificationThrough(object arg)
        {
            var collectionNotify = arg as INotifyCollectionChanged;
            if (collectionNotify != null)
            {
                collectionNotify.CollectionChanged += (o, e) => {
                        if (e.Action == NotifyCollectionChangedAction.Add)
                        {
                            foreach (var item in e.NewItems)
                            {
                                var notify = item as Notifier;
                                if (notify != null)
                                    notify.PropertyChanged += PassNotificationThrough;
                            }
                        }

                        if (e.Action == NotifyCollectionChangedAction.Remove)
                        {
                            foreach (var item in e.OldItems)
                            {
                                var notify = item as Notifier;
                                if (notify != null)
                                    notify.PropertyChanged -= PassNotificationThrough;
                            }
                        }
                    };
            }
        }

    }
}
