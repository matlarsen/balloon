using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace Balloon {
    public abstract class Cube {
        public Point3D          Center { get; private set; }
        public double           Radius { get; private set; }
        public Action           Action { get; set; }
        public Material         Material { get; private set; }
        public ModelVisual3D    ModelVisual3D { get; private set; }
        public bool             Notified { get; private set; }
        public SolidColorBrush  SolidColorBrush { get; set; }

        public Cube(Point3D center, double radius, Action action) {
            Center = center;
            Radius = radius;
            Action = action;
            Notified = false;
            SolidColorBrush = new SolidColorBrush();
            SolidColorBrush.Opacity = _Constants.BaseOpacity;
            SolidColorBrush.Color = _Constants.DefaultCubeColour;
            Material = new DiffuseMaterial(SolidColorBrush);
            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddBox(center, radius, radius, radius);

            ModelVisual3D = Engine._3DUtil.WrapMeshAndMaterialIntoModelVisual3D(meshBuilder.ToMesh(), Material);
        }

        /// <summary>
        /// Move the center of this point to here
        /// </summary>
        /// <param name="point"></param>
        public void MoveTo(Point3D point) {
            Center = point;
            GeometryModel3D g = (GeometryModel3D)ModelVisual3D.Content;
            MeshGeometry3D m = (MeshGeometry3D)g.Geometry;

            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddBox(Center, Radius, Radius, Radius);
            m.Positions = meshBuilder.Positions;
        }

        /// <summary>
        /// Resizes the cube
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="resizeGeometryAsWell">By default it won't resize the 3D geometry, this forces it to</param>
        public void Resize(double radius, bool resizeGeometryAsWell = false) {
            Radius = radius;
            if (resizeGeometryAsWell)
                MoveTo(Center);
        }

        // when the point goes in bounds do something
        public void Notify() {
            Notified = true;
            SolidColorBrush.Opacity = _Constants.InteractedOpacity;
            Action.DoAction();
        }
        // and when it goes out, stop it
        public void DeNotify() {
            Notified = false;
            SolidColorBrush.Opacity = _Constants.BaseOpacity;
            Action.StopAction();
        }
    }
}
