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

        public static Balloon.Engine.Engine Engine { get; set; }
        public static KinectChooser         KinectChooser { get; set; }
        public static Interface             Interface { get; set; }

        App() {
            // create a balloon engine and get it going
            
            Interface = new Interface();
            Engine = new Balloon.Engine.Engine();
            Interface.viewbox.Child = Engine.Viewport3D;
            
            Interface.Show();

            // setup our camera
            //Interface.Orthographic.CameraController.CameraTarget = new Point3D(0, 0, 3);
            //Interface.Orthographic.CameraController.Zoom(0.3);

            // so we can tell what's going on
            // debug
            KinectChooser = new KinectChooser();
            KinectChooser.Show();

            // debug
            // so now lets play a bit!

            // track hands for now
            Engine.TrackJoint(Microsoft.Kinect.JointType.HandLeft);
            Engine.TrackJoint(Microsoft.Kinect.JointType.HandRight);
            Engine.TrackJoint(Microsoft.Kinect.JointType.ElbowLeft);
            Engine.TrackJoint(Microsoft.Kinect.JointType.ElbowRight);

            // setup a midi device
            OutputDevice device = OutputDevice.InstalledDevices[0];
            device.Open();
            Channel channel = Channel.Channel1;
            Instrument instrument = Instrument.AltoSax;
            device.SendProgramChange(channel, instrument);

            // create our test cubes
            List<Cube> cubes = new List<Cube>();

            //cubes.Add(new Cubes.SingleNoteCube(new System.Windows.Media.Media3D.Point3D(-0.5, 0, 2), 0.2, Pitch.C4, instrument, device, channel));
            Cube PitchBendTest = new Cubes.YPitchBendCube(new System.Windows.Media.Media3D.Point3D(0.5, 0, 1.2), 0.4, Pitch.G4, instrument, device, channel) {
                //Color = System.Windows.Media.Colors.Gold
            };
            PitchBendTest.Color = System.Windows.Media.Colors.Gold;
            cubes.Add(PitchBendTest);

            fun.Fun.GenerateAFuckTonneOfCubes(Engine, new Note("C"), 200, new Point3D(0.5, 0.5, 1.5), 1.1, 0.2);
            fun.Fun.GenerateAFuckTonneOfCubes(Engine, new Note("F"), 200, new Point3D(-0.5, 0.5, 1.5), 1.1, 0.2);

            foreach (Cube cube in cubes)
                Engine.AddCube(cube);

            // set the floor
            Engine.World.SetKinectHeight(0.9f);
        }
    }
}
