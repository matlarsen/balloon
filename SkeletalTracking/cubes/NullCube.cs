using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Text;
using Midi;
using Balloon.Actions;
using System.Windows.Media;

namespace Balloon.Cubes {
    /// <summary>
    /// A Cube that plays a single note
    /// </summary>
    public class NullCube : Cube {
        public NullCube(Point3D center, double radius)
            : base(center, radius, null) {
                //System.Windows.MessageBox.Show("created nullcube");
                Opacity = 1;
                Color = new SolidColorBrush(Colors.Gray);
        }
    }
}
