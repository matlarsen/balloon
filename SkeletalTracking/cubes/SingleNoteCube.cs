using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Text;
using Midi;
using Balloon.Actions;

namespace Balloon.Cubes {
    /// <summary>
    /// A Cube that plays a single note
    /// </summary>
    public class SingleNoteCube : Cube {
        public SingleNoteCube(Point3D center, double radius,
                Pitch pitch, Instrument instrument, OutputDevice device, Channel channel)
            : base(center, radius, new InstrumentNoteAction(device, channel, pitch)) {
        }
    }
}
