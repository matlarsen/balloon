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
        public static void GenerateAFuckTonneOfCubes(Engine.Engine engine, ChannelAndInstrument ci, Note scaleNote, int numberOfCubes, Point3D point, double radius, double cubeRadius) {
            Scale scale = new Scale(scaleNote, Scale.Major);
            Random random = new Random();
            OutputDevice device = OutputDevice.InstalledDevices[0];
            Channel channel = ci.Channel;
            Instrument instrument = ci.Instrument;
            int rootOctave = 3;

            int thisOctave = rootOctave;
            for (int i = 0; i < numberOfCubes; i++) {

                double x = point.X + (random.NextDouble() - 0.5) * radius;
                double y = point.Y + (random.NextDouble() - 0.5) * radius;
                double z = point.Z + (random.NextDouble() - 0.5) * radius;
                Point3D cubePoint = new Point3D(x * radius, y * radius, z * radius);
                Pitch thisPitch;
                
                // limit it to 3 octaves
                if (i % 21 == 0)
                    thisOctave = rootOctave;

                thisPitch = scale.NoteSequence[i % 7].PitchInOctave(thisOctave);

                // increment the octave if we have run out of notes
                if (i % 7 == 0)
                    thisOctave++;

                SingleNoteCube a = new Cubes.SingleNoteCube(cubePoint, cubeRadius, thisPitch, instrument, App.OutputDevice, channel);

                engine.AddCube(a);
            }
        }
    }
}
