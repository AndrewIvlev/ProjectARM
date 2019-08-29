
namespace ArmManipulatorApp.Graphics.Model3D
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using ArmManipulatorApp.Common;
    using ArmManipulatorApp.Graphics.Interfaces;

    class Model3DSet : Notifier, IModel3DSet
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

        public ICollection<IModel3D> Models { get; }

        #endregion

        #region CTOR

        public Model3DSet(string description = null, IEnumerable<IModel3D> models = null)
        {
            Description = description ?? "MODEL_SET_" + InstanceCount++;
            Models = models == null ? new ObservableCollection<IModel3D>() : new ObservableCollection<IModel3D>(models);
        }

        #endregion
    }
}