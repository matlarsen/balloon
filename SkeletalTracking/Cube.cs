using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Balloon {
    public abstract class Cube {
        public Point3D          Center { get; private set; }
        public double           Radius { get; private set; }
        public Double           Opacity { get; set; }
        public SolidColorBrush  Color { get; set; }
        public Action           Action { get; set; }
        public Rect3D           Rect3D { get; private set; }
        public Material         Material { get; set; }
        public ModelVisual3D    ModelVisual3D { get; private set; }
        public bool             Notified { get; private set; }

        public Cube(Point3D center, double radius, Action action) {
            Center = center;
            Radius = radius;
            Action = action;
            Color = new SolidColorBrush(Colors.Green);  // default colour
            Opacity = 0.5;
            Notified = false;

            // create our geometry for wpf rendering
            SolidColorBrush scb = Color;
            scb.Opacity = Opacity;
            Material = new DiffuseMaterial(scb);
            ModelVisual3D = _3DUtil.WrapMeshAndMaterialIntoModelVisual3D(
                _3DUtil.GetMeshGeometry3DFromCenterAndRadius(center, radius),
                Material);
            Rect3D = ModelVisual3D.Content.Bounds;
        }

        /// <summary>
        /// Move the center of this point to here
        /// </summary>
        /// <param name="point"></param>
        public void MoveTo(Point3D point) {
            Center = point;
            GeometryModel3D g = (GeometryModel3D)ModelVisual3D.Content;
            MeshGeometry3D m = (MeshGeometry3D)g.Geometry;
            m.Positions = _3DUtil.GetPoint3DCollectionFromCenterAndRadius(point, Radius);
        }

        // when the point goes in bounds do something
        public void Notify() {
            Action.DoAction();
            Notified = true;
        }
        // and when it goes out, stop it
        public void DeNotify() {
            Action.StopAction();
            Notified = false;
        }
    }
}
