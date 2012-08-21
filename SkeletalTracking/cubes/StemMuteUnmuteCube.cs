using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.Windows.Media.Media3D;
using Balloon.Actions;

namespace Balloon.Cubes {
    class StemMuteUnmuteCube : Cube {
        public StemMuteUnmuteCube(Point3D center, double radius, WaveChannel32 waveChannel32)
            : base(center, radius, new StemChannelMuteAction(waveChannel32)) {
                ((StemChannelMuteAction)this.Action).setCube(this);
        }
    }
}
