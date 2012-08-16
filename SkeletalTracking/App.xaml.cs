using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using Midi;
using System.Windows.Media.Media3D;
using System.Windows.Controls;

namespace Balloon {
    // entry point to the default application
    public partial class App : Application {

        public static Balloon.Engine.Engine             Engine { get; set; }
        public static KinectChooser                     KinectChooser { get; set; }
        public static Interface                         Interface { get; set; }

        public static OutputDevice                      OutputDevice { get; set; }
        public static List<ChannelAndInstrument>        ChannelsAndInstruments { get; set; }   // channel for each instrument

        App() {
            // create a balloon engine and get it going
            Engine = new Balloon.Engine.Engine();
            Interface = new Interface();
            Interface.viewbox.Child = Engine.Viewport3D;

            // set the floor
            Engine.World.SetKinectHeight(0.9f);
            
            Interface.Show();

            // so we can tell what's going on
            // debug
            KinectChooser = new KinectChooser();
            KinectChooser.Show();

            // track hands for now
            Engine.TrackJoint(Microsoft.Kinect.JointType.HandLeft);
            Engine.TrackJoint(Microsoft.Kinect.JointType.HandRight);
            Engine.TrackJoint(Microsoft.Kinect.JointType.FootLeft);

            // ok, initialise our midi stuff
            OutputDevice = OutputDevice.InstalledDevices[0];
            OutputDevice.Open();
            ChannelsAndInstruments = new List<ChannelAndInstrument>();

            Array channels = Enum.GetValues(typeof(Channel));
            foreach (Channel channel in channels) {
                ChannelsAndInstruments.Add(new ChannelAndInstrument() { Channel = channel, Instrument = Instrument.AcousticGrandPiano });
                OutputDevice.SendProgramChange(channel, ChannelsAndInstruments[0].Instrument);
            }


            // create our test cube
            Cube PitchBendTest = new Cubes.YPitchBendCube(new System.Windows.Media.Media3D.Point3D(0, 0, 0), 0, Pitch.G4, ChannelsAndInstruments[0].Instrument, App.OutputDevice, ChannelsAndInstruments[0].Channel);
            Engine.SetCreateCube(PitchBendTest);
        }

        /// <summary>
        /// Safely terminates the application
        /// </summary>
        public static void Terminate() {
            App.Engine.StopKinect();
            System.Environment.Exit(0);
        }
    }

    // utility class for MIDI
    public class ChannelAndInstrument {
        public Channel Channel { get; set; }
        public Instrument Instrument { get; set; }
    }
}
