using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Text;
using Midi;
using Balloon.Actions;
using System.Windows.Media;

namespace Balloon.Cubes {
    /// <summary>
    /// A Null Cube that does nothing
    /// </summary>
    public class NullCube : Cube {
        public NullCube(Point3D center, double radius)
            : base(center, radius, null) {
                Color = Colors.Gray;
        }
    }
}
