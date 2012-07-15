using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace Balloon {
    public static class _3DUtil {


        public static MeshGeometry3D CubeMeshFromPoints(Point3D[] points) {
            MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
            if (points.Length < 8) return null;
            for (int i = 0; i < 8; i++) {
                meshGeometry3D.Positions.Add(points[i]);

                foreach (int index in new int[] { 2, 3, 1, 2, 1, 0, 7, 1, 3, 7, 5, 1, 6, 5, 7, 6, 4, 5, 6, 2, 0, 2, 0, 4, 2, 7, 3, 2, 6, 7, 0, 1, 5, 0, 5, 4 })
                    meshGeometry3D.TriangleIndices.Add(index);
            }
            return meshGeometry3D;
        }

        public static MeshGeometry3D GetMeshGeometry3DFromCenterAndRadius(Point3D center, double radius) {
            MeshGeometry3D newMeshGeometry3D = new MeshGeometry3D();
            newMeshGeometry3D.Positions = GetPoint3DCollectionFromCenterAndRadius(center, radius);
            foreach (int index in new int[] { 2, 3, 1, 2, 1, 0, 7, 1, 3, 7, 5, 1, 6, 5, 7, 6, 4, 5, 6, 2, 0, 2, 0, 4, 2, 7, 3, 2, 6, 7, 0, 1, 5, 0, 5, 4 })
                newMeshGeometry3D.TriangleIndices.Add(index);
            return newMeshGeometry3D;
        }

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

        public static ModelVisual3D WrapMeshAndMaterialIntoModelVisual3D(MeshGeometry3D mesh, Material material) {
            GeometryModel3D geometryModel3d = new GeometryModel3D(mesh, material);
            return new ModelVisual3D() { Content = geometryModel3d };
        }

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
    }
}
