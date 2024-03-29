﻿using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Text;
using Midi;
using Balloon.Actions;

namespace Balloon.Cubes {
    /// <summary>
    /// Pitch bends along the y axis
    /// </summary>
    public class YPitchBendCube : Cube, AnalogueCube {

        private OutputDevice outputDevice;
        private Channel channel;

        public YPitchBendCube(Point3D center, double radius, 
                Pitch pitch, Instrument instrument, OutputDevice device, Channel channel)
            : base(center, radius, new InstrumentNoteAction(device, channel, pitch)) {

                outputDevice = device;
                this.channel = channel;
        }

        // ok so what we want to do here is a mini notify type of thing
        // we want to notify all the subcubes
        public void NotifyAnalogue(Vector3D vector) {
            // convert the y relative into a bend
            outputDevice.SendPitchBend(channel, 8192 + (int)(vector.Y * 8192d));
            System.Diagnostics.Debug.WriteLine("{0}, {1}", vector.Y, 8192 + (int)(vector.Y * 8192));
        }
    }
}
