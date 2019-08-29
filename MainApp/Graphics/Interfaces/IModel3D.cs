namespace ArmManipulatorApp.Graphics.Interfaces
{
    using System.Collections.Generic;

    interface IModel3D
    {
        string Description { get; set; }
        ICollection<IPoint3D> Points { get; }
        ICollection<ITriangle3D> Triangles { get; }
    }
}
