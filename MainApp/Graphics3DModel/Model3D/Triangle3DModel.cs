using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MainApp.Graphics3DModel.Model3D
{
    public static class MeshGeometry3DHelper
    {
        #region Triangle

        // Add a triangle to the indicated mesh.
        // Do not reuse points so triangles don't share normals.
        public static void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            var index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }

        #endregion

        #region Parallelepiped
        
        // If extend is true, extend the segment by half the
        // thickness so segments with the same end points meet nicely.
        public static void AddParallelepiped(
            MeshGeometry3D mesh,
            Point3D point1,
            Point3D point2,
            Vector3D up,
            double thickness,
            bool extend = false)
        {
            // Get the segment's vector.
            var v = point2 - point1;

            if (extend)
            {
                // Increase the segment's length on both ends by thickness / 2.
                var n = Vector3D.Multiply(v, thickness / 2.0);
                point1 -= n;
                point2 += n;
            }

            // Get the scaled up vector.
            var n1 = Vector3D.Multiply(up, thickness / 2.0);

            // Get another scaled perpendicular vector.
            var n2 = Vector3D.CrossProduct(v, n1);
            n2 = Vector3D.Multiply(n2, thickness / 2.0);

            // Make a skinny box.
            // p1pm means point1 PLUS n1 MINUS n2.
            var p1pp = point1 + n1 + n2;
            var p1mp = point1 - n1 + n2;
            var p1pm = point1 + n1 - n2;
            var p1mm = point1 - n1 - n2;
            var p2pp = point2 + n1 + n2;
            var p2mp = point2 - n1 + n2;
            var p2pm = point2 + n1 - n2;
            var p2mm = point2 - n1 - n2;

            // Sides.
            AddTriangle(mesh, p1pp, p1mp, p2mp);
            AddTriangle(mesh, p1pp, p2mp, p2pp);

            AddTriangle(mesh, p1pp, p2pp, p2pm);
            AddTriangle(mesh, p1pp, p2pm, p1pm);

            AddTriangle(mesh, p1pm, p2pm, p2mm);
            AddTriangle(mesh, p1pm, p2mm, p1mm);

            AddTriangle(mesh, p1mm, p2mm, p2mp);
            AddTriangle(mesh, p1mm, p2mp, p1mp);

            // Ends.
            AddTriangle(mesh, p1pp, p1pm, p1mm);
            AddTriangle(mesh, p1pp, p1mm, p1mp);

            AddTriangle(mesh, p2pp, p2mp, p2mm);
            AddTriangle(mesh, p2pp, p2mm, p2pm);
        }

        #endregion

        #region Sphere

        public static void AddSphere(MeshGeometry3D mesh, Point3D center, double radius, int num_phi, int num_theta)
        {
            double phi0, theta0;
            var dphi = Math.PI / num_phi;
            var dtheta = 2 * Math.PI / num_theta;

            phi0 = 0;
            var y0 = radius * Math.Cos(phi0);
            var r0 = radius * Math.Sin(phi0);
            for (var i = 0; i < num_phi; i++)
            {
                var phi1 = phi0 + dphi;
                var y1 = radius * Math.Cos(phi1);
                var r1 = radius * Math.Sin(phi1);

                // Point ptAB has phi value A and theta value B.
                // For example, pt01 has phi = phi0 and theta = theta1.
                // Find the points with theta = theta0.
                theta0 = 0;
                var pt00 = new Point3D(
                    center.X + r0 * Math.Cos(theta0),
                    center.Y + y0,
                    center.Z + r0 * Math.Sin(theta0));
                var pt10 = new Point3D(
                    center.X + r1 * Math.Cos(theta0),
                    center.Y + y1,
                    center.Z + r1 * Math.Sin(theta0));
                for (var j = 0; j < num_theta; j++)
                {
                    // Find the points with theta = theta1.
                    var theta1 = theta0 + dtheta;
                    var pt01 = new Point3D(
                        center.X + r0 * Math.Cos(theta1),
                        center.Y + y0,
                        center.Z + r0 * Math.Sin(theta1));
                    var pt11 = new Point3D(
                        center.X + r1 * Math.Cos(theta1),
                        center.Y + y1,
                        center.Z + r1 * Math.Sin(theta1));

                    // Create the triangles.
                    AddTriangle(mesh, pt00, pt11, pt10);
                    AddTriangle(mesh, pt00, pt01, pt11);

                    // Move to the next value of theta.
                    theta0 = theta1;
                    pt00 = pt01;
                    pt10 = pt11;
                }

                // Move to the next value of phi.
                phi0 = phi1;
                y0 = y1;
                r0 = r1;
            }
        }

        #endregion
    }
}
