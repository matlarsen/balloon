using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace Balloon.Engine {
    public static class _3DUtil {

        /// <summary>
        /// Get a collection of points that represents a cube around this point
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Point3DCollection GetPoint3DCollectionFromCenterAndRadius(Point3D center, double radius) {
            Point3DCollection point3dCollection = new Point3DCollection(8);
            point3dCollection.Add(new Point3D(center.X - radius, center.Y - radius, center.Z + radius)); //0
            point3dCollection.Add(new Point3D(center.X - radius, center.Y + radius, center.Z + radius)); //1
            point3dCollection.Add(new Point3D(center.X + radius, center.Y - radius, center.Z + radius)); //2
            point3dCollection.Add(new Point3D(center.X + radius, center.Y + radius, center.Z + radius)); //3
            point3dCollection.Add(new Point3D(center.X - radius, center.Y - radius, center.Z - radius)); //4
            point3dCollection.Add(new Point3D(center.X - radius, center.Y + radius, center.Z - radius)); //5
            point3dCollection.Add(new Point3D(center.X + radius, center.Y - radius, center.Z - radius)); //6
            point3dCollection.Add(new Point3D(center.X + radius, center.Y + radius, center.Z - radius)); //7
            return point3dCollection;
        }

        /// <summary>
        /// Combine a mesh and material into a ModelVisual3D
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public static ModelVisual3D WrapMeshAndMaterialIntoModelVisual3D(MeshGeometry3D mesh, Material material) {
            GeometryModel3D geometryModel3d = new GeometryModel3D(mesh, material);
            return new ModelVisual3D() { Content = geometryModel3d };
        }

        /// <summary>
        /// Calculate the Unit Direction vector from one point to another
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Vector3D UnitDirectionVectorFromPointAToB(Point3D to, Point3D from) {
            Point3D direction = new Point3D();
            direction.X = from.X - to.X;
            direction.Y = from.Y - to.Y;
            direction.Z = from.Z - to.Z;

            double length = Math.Sqrt(direction.X * direction.X +
                                      direction.Y * direction.Y +
                                      direction.Z * direction.Z);

            direction.X /= length;
            direction.Y /= length;
            direction.Z /= length;

            return new Vector3D(direction.X, direction.Y, direction.Z);
        }


        /// <summary>
        /// Calculate the distance between these 2 points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double DistanceBetween(Point3D a, Point3D b) {
            Point3D direction = new Point3D();
            direction.X = b.X - a.X;
            direction.Y = b.Y - a.Y;
            direction.Z = b.Z - a.Z;

            return Math.Sqrt(direction.X * direction.X +
                             direction.Y * direction.Y +
                             direction.Z * direction.Z);
        }


        public static Vector3D VectorFromPointAToB(Point3D to, Point3D from) {
            Vector3D direction = new Vector3D();
            direction.X = from.X - to.X;
            direction.Y = from.Y - to.Y;
            direction.Z = from.Z - to.Z;
            return direction;
        }


        public static Point3D Midpoint(Point3D a, Point3D b) {
            return new Point3D((b.X + a.X) / 2.0, (b.Y + a.Y) / 2.0, (b.Z + a.Z) / 2.0);
        }
    }
}
