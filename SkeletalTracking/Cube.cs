using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Balloon {
    public abstract class Cube {
        public Point3D  Center { get; private set; }
        public float    Radius { get; private set; }
        public Color    Color { get; set; }
        public Action   Action { get; set; }
        public Rect3D   Rect3D { get; private set; }
        public bool     Notified { get; private set; }

        public Cube(Point3D center, float radius, Action action) {
            Center = center;
            Radius = radius;
            Action = action;
            Color = Colors.Green;   // default colour
            Notified = false;
            Rect3D = new Rect3D(center, new Size3D(radius * 2, radius * 2, radius * 2));
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
