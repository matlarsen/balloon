using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Balloon.Engine;
using System.Windows.Media.Media3D;
using Midi;
using Balloon.Cubes;

namespace Balloon.fun {
    public static class Fun {

        /// <summary>
        /// Just generate a cloud of points within the given sphere
        /// </summary>
        /// <param name="engine"></param>
        public static void GenerateAFuckTonneOfCubes(Engine.Engine engine, Interface interf, Note scaleNote, int numberOfCubes, Point3D point, double radius, double cubeRadius) {
            Scale scale = new Scale(scaleNote, Scale.Major);
            Random random = new Random();
            OutputDevice device = OutputDevice.InstalledDevices[0];
            Channel channel = Channel.Channel1;
            Instrument instrument = Instrument.Pad2Warm;

            Pitch previousPitch = scale.NoteSequence[0].PitchAtOrAbove(Pitch.C4);
            device.SendProgramChange(channel, instrument);

            for (int i = 0; i < numberOfCubes; i++) {

                double x = point.X + (random.NextDouble() - 0.5) * radius;
                double y = point.Y + (random.NextDouble() - 0.5) * radius;
                double z = point.Z + (random.NextDouble() - 0.5) * radius;
                Point3D cubePoint = new Point3D(x * radius, y * radius, z * radius);
                if (i != 0)
                    previousPitch = scale.NoteSequence[i % 7].PitchAtOrAbove(previousPitch);

                SingleNoteCube a = new Cubes.SingleNoteCube(cubePoint, cubeRadius, previousPitch, instrument, device, channel);

                engine.AddCube(a);
                interf.AddCube(a);
            }
        }
    }
}
