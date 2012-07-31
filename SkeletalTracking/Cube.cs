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
        public Rect3D           Rect3D { get; private set; }
        public Material         Material { get; private set; }
        public ModelVisual3D    ModelVisual3D { get; private set; }
        public bool             Notified { get; private set; }

        private Color           color = _Constants.DefaultCubeColour;
        private SolidColorBrush SolidColorBrush { get; set; }
        public Color            Color {
            get {
                return color;
            }

            set {
                color = value;
                SolidColorBrush.Color = color;
            }
        }

        public Cube(Point3D center, double radius, Action action) {
            Center = center;
            Radius = radius;
            Action = action;
            Notified = false;
            SolidColorBrush = new SolidColorBrush();
            SolidColorBrush.Opacity = _Constants.BaseOpacity;
            Material = new DiffuseMaterial(SolidColorBrush);
            Color = _Constants.DefaultCubeColour;

            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddBox(center, radius, radius, radius);

            ModelVisual3D = Engine._3DUtil.WrapMeshAndMaterialIntoModelVisual3D(meshBuilder.ToMesh(), Material);
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
            m.Positions = Engine._3DUtil.GetPoint3DCollectionFromCenterAndRadius(point, Radius);
        }

        // when the point goes in bounds do something
        public void Notify() {
            Action.DoAction();
            Notified = true;
            SolidColorBrush.Opacity = _Constants.InteractedOpacity;
        }
        // and when it goes out, stop it
        public void DeNotify() {
            Action.StopAction();
            Notified = false;
            SolidColorBrush.Opacity = _Constants.BaseOpacity;
        }
    }
}
