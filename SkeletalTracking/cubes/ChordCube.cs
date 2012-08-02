using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Midi;
using Balloon.Actions;

namespace Balloon.Cubes {
    /// <summary>
    /// A Cube that plays a chord
    /// </summary>
    public class ChordCube : Cube {
        public ChordCube(Point3D center, double radius,
                Chord chord, int octave, Instrument instrument, OutputDevice device, Channel channel)
            : base(center, radius, new InstrumentChordAction(device, channel, chord, octave)) {
        }
    }
}
