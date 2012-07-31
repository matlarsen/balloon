using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Balloon {
    /// <summary>
    /// If a class implements this interface it will be sent a 
    /// unit vector from the center to the radius representing where
    /// x is inside the cube, on every update
    /// </summary>
    public interface AnalogueCube {
        void NotifyAnalogue(Vector3D relativePointPosition);
    }
}
