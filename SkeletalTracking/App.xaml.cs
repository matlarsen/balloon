using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using Midi;
using System.Windows.Media.Media3D;

namespace Balloon {
    // entry point to the default application
    public partial class App : Application {

        public static Balloon.Engine.Engine    Engine { get; set; }
        public static KinectChooser            KinectChooser { get; set; }

        App() {
            // create a balloon engine and get it going
            
            Interface Interface = new Interface();
            //Point3D cameraPosition = new Point3D(0, 10, -20);
            //PerspectiveCamera camera = new PerspectiveCamera() {
            //    Position = cameraPosition,
            //    LookDirection = _3DUtil.UnitDirectionVectorFromPointAToB(cameraPosition, new Point3D(0, 0, 0))
            //};
            Engine = new Balloon.Engine.Engine(Interface.Orthographic);
            Interface.Show();

            // so we can tell what's going on
            // debug
            KinectChooser = new KinectChooser();
            KinectChooser.Show();

            // debug
            // so now lets play a bit!

            // track hands for now
            Engine.TrackJoint(Microsoft.Kinect.JointType.HandLeft);
            Engine.TrackJoint(Microsoft.Kinect.JointType.HandRight);

            // setup a midi device
            OutputDevice device = OutputDevice.InstalledDevices[0];
            device.Open();
            Channel channel = Channel.Channel1;
            Instrument instrument = Instrument.AltoSax;
            device.SendProgramChange(channel, instrument);

            //SingleNoteCube a = new SingleNoteCube(new BaloonCoordinate(0.0f, 0.0f, 2.5f), 0.25f, new Note("C"), 4, instrument, device, channel);
            
            ////mainNotifier.Shapes.Add(a);

            // create our test cubes
            List<Cube> cubes = new List<Cube>();

            //cubes.Add(new Cubes.SingleNoteCube(new System.Windows.Media.Media3D.Point3D(-0.5, 0, 2), 0.2, Pitch.C4, instrument, device, channel));
            cubes.Add(new Cubes.YPitchBendCube(new System.Windows.Media.Media3D.Point3D(0, 0, 2), 0.4, Pitch.G4, instrument, device, channel));


            foreach (Cube cube in cubes) {
                Engine.AddCube(cube);
                Interface.AddCube(cube);
            }

            //Interface.camMain.

        }
    }
}
