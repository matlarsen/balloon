using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using NAudio.Wave;
using Balloon.Actions;

namespace Balloon.Cubes {
    class StemVolumePanCube : Cube, AnalogueCube {

        private WaveChannel32 WaveChannel32;

        public StemVolumePanCube(Point3D center, double radius, WaveChannel32 waveChannel32)
            : base(center, radius, new NullAction()) {
                WaveChannel32 = waveChannel32;
        }

        // ok so what we want to do here is use the x for pan, and y for volume
        public void NotifyAnalogue(Vector3D vector) {
            WaveChannel32.Pan = (float)vector.X;
            WaveChannel32.Volume = ((float)vector.Y + 1) / 2f;
        }

    }
}
