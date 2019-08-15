namespace MainApp.Graphics.Interfaces
{
    using System.Collections.Generic;

    interface IModel3DSet
    {
        string Description { get; set; }
        ICollection<IModel3D> Models { get; }
    }
}
