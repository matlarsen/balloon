// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using Midi; 

namespace SkeletalTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool closing = false; // util so we can shutdown kinect safe

        // our smooth parameters
        public TransformSmoothParameters parameters = new TransformSmoothParameters {
            Smoothing = 0.3f,
            Correction = 0.0f,
            Prediction = 0.0f,
            JitterRadius = 1.0f,
            MaxDeviationRadius = 0.5f
        };

        Notifier mainNotifier = new Notifier();

        // window init shit
        public MainWindow() { InitializeComponent(); }
        private void Window_Loaded(object sender, RoutedEventArgs e) {

            // make it so we can have a kinect
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);

            // create default cubes
            OutputDevice device = OutputDevice.InstalledDevices[0];
            device.Open();
            /*Channel channel = Channel.Channel1;
            Instrument instrument = Instrument.AltoSax;
            SingleNoteCube a = new SingleNoteCube(new BaloonCoordinate(0.0f, 0.0f, 2.5f), 0.25f, new Note("C"), 4, instrument, device, channel);
            device.SendProgramChange(channel, instrument);
            //mainNotifier.Shapes.Add(a);

            Instrument instrumentb = Instrument.OverdrivenGuitar;
            Channel channelb = Channel.Channel2;
            SingleNoteSphere b = new SingleNoteSphere(new BaloonCoordinate(0.75f, 0.75f, 1.75f), 0.5f, new Note("G"), 4, instrumentb, device, channelb);
            device.SendProgramChange(channelb, instrumentb);*/
            //mainNotifier.Shapes.Add(b);

            
            /* Fun tests :) */
            //createAFuckTonneOfCubes(new Note("C"), 64, 1f);
            MidiSettings settings = new MidiSettings() {
                device = OutputDevice.InstalledDevices[0],
                channel = Channel.Channel1,
                instrument = Instrument.StringEnsemble1
            };
            MidiSettings settingsb = new MidiSettings() {
                device = OutputDevice.InstalledDevices[0],
                channel = Channel.Channel2,
                instrument = Instrument.StringEnsemble2
            };
            MidiSettings settingsc = new MidiSettings() {
                device = OutputDevice.InstalledDevices[0],
                channel = Channel.Channel3,
                instrument = Instrument.SynthStrings1
            };
            MidiSettings settingsd = new MidiSettings() {
                device = OutputDevice.InstalledDevices[0],
                channel = Channel.Channel4,
                instrument = Instrument.SynthStrings2
            };
            device.SendProgramChange(settings.channel, settings.instrument);
            device.SendProgramChange(settingsb.channel, settingsb.instrument);
            device.SendProgramChange(settingsc.channel, settingsc.instrument);
            device.SendProgramChange(settingsd.channel, settingsd.instrument);

            createArray(7, new BaloonCoordinate(0f, 0f, 1.5f), 0.55f, settings);
            createArray(7, new BaloonCoordinate(0f, 0f, 1.65f), 0.55f, settingsb);
            createArray(7, new BaloonCoordinate(0f, 0f, 1.80f), 0.55f, settingsc);
            createArray(7, new BaloonCoordinate(0f, 0f, 1.95f), 0.55f, settingsd);

        }

        struct MidiSettings {
            public OutputDevice device;
            public Channel channel;
            public Instrument instrument;
        }

        void createArray(int arraySize, BaloonCoordinate center, float radius, MidiSettings midiSettings) {
            int rows = arraySize;
            int columns = arraySize;
            float cubeDiameter = radius * 2f / (float)arraySize;
            float cubeRadius = cubeDiameter / 2f;

            BaloonCoordinate bottomLeftPoint = new BaloonCoordinate(
                center.x - radius + cubeRadius,
                center.y - radius + cubeRadius,
                center.z
            );

            Scale scale = new Scale(new Note("C"), Scale.Major);
            List<Pitch> scalePitches = new List<Pitch>(arraySize * arraySize);
            for (int i = 0; i < arraySize * arraySize; i++) {
                if (i == 0)
                    scalePitches.Add(scale.NoteSequence[i % 7].PitchInOctave(2));
                else
                    scalePitches.Add(scale.NoteSequence[i % 7].PitchAtOrAbove(scalePitches[i - 1]));
            }

            // for each one
            int x = 0;
            for (int column = 0; column < columns; column++) {
                for (int row = 0; row < rows; row++) {
                    BaloonCoordinate thisCubeCenter = new BaloonCoordinate(
                        bottomLeftPoint.x + (float)row * cubeDiameter,
                        bottomLeftPoint.y + (float)column * cubeDiameter,
                        bottomLeftPoint.z
                    );
                    SingleNoteCube cube = new SingleNoteCube(thisCubeCenter, cubeRadius, scalePitches[x++], 4, 
                        midiSettings.instrument, midiSettings.device, midiSettings.channel);
                    mainNotifier.Shapes.Add(cube);
                }
            }
        }

        /*void createAFuckTonneOfCubes(Note scaleNote, int noCubes, float cubeRadius) {
            Scale scale = new Scale(scaleNote, Scale.Major);
            Random random = new Random();
            OutputDevice device = OutputDevice.InstalledDevices[0];
            Channel channel = Channel.Channel1;
            Instrument instrument = Instrument.Pad2Warm;

            for (int i = 0; i < noCubes; i++) {

                float x = (float)(random.NextDouble() - 0.5) * 2;
                float y = (float)(random.NextDouble() - 0.5) * 2;
                float z = 2.5f;//; + (float)(random.NextDouble() - random.NextDouble());

                SingleNoteCube a = new SingleNoteCube(new BaloonCoordinate(x, y, z), 0.2f, scale.NoteSequence[i % 7], 4, instrument, device, channel);
                device.SendProgramChange(channel, instrument);
                mainNotifier.Shapes.Add(a);
            }
        }*/

        // Event that gets fired when this kinect sensor changer .. changes
        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e) {

            KinectSensor old = (KinectSensor)e.OldValue;
            StopKinect(old);
            KinectSensor sensor = (KinectSensor)e.NewValue;

            if (sensor == null)
                return;

            sensor.SkeletonStream.Enable(parameters);
            sensor.SkeletonStream.Enable();

            sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
            sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30); 
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            // lets goooooooooooooooooooooo
            try {
                sensor.Start();
            }
            catch (System.IO.IOException) {
                kinectSensorChooser1.AppConflictOccurred();
            }
        }

        // called when we have a new frame
        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e) {
            if (closing) return;

            DepthImageFrame depthImageFrame = e.OpenDepthImageFrame();
            if (depthImageFrame == null || kinectSensorChooser1.Kinect == null) return;

            // for all the skeletons
            Skeleton[] skeletons = new Skeleton[e.OpenSkeletonFrame().SkeletonArrayLength];
            e.OpenSkeletonFrame().CopySkeletonDataTo(skeletons);
            foreach (Skeleton s in skeletons) {

                // if there actually bloody is someone there
                if (s != null && s.TrackingState == SkeletonTrackingState.Tracked) {

                    // get depth points for their hands
                    List<BaloonCoordinate> pointsToCheck = new List<BaloonCoordinate>();

                    // the points we get back from a joint are in skeletal space
                    // it doesn't matter where the camera is pointed, we get back a sensible value. woo.
                    // skeletal space has the x/y origin at 
                    Joint leftHandJoint = s.Joints[JointType.HandLeft];
                    Joint rightHandJoint = s.Joints[JointType.HandRight];

                    //pointsToCheck.Add(GetBaloonCoordinateFromJoint(s.Joints[JointType.HandLeft]));
                    //pointsToCheck.Add(GetBaloonCoordinateFromJoint(s.Joints[JointType.HandRight]));
                    //mainNotifier.Joints.Clear();
                    //mainNotifier.Joints.Add(leftHandJoint);
                    //mainNotifier.Joints.Add(rightHandJoint);
                    mainNotifier.addJoint(JointType.HandLeft);
                    mainNotifier.addJoint(JointType.HandRight);
                    mainNotifier.addJoint(JointType.Head);


                    /* debug */
                    lblX.Content = String.Format("x: {0}", rightHandJoint.Position.X);
                    lblY.Content = String.Format("y: {0}", rightHandJoint.Position.Y);
                    lblZ.Content = String.Format("z: {0}", rightHandJoint.Position.Z);

                    // and run them through the notifier
                    // expand to have multiple notifier queues 
                    // (for instance, respond to different joints or skeletons)
                    //foreach (BaloonCoordinate point in pointsToCheck) {
                        mainNotifier.Notify(s);
                    //}
                }
            }

        }

        BaloonCoordinate GetBaloonCoordinateFromJoint(Joint joint) {
            return new BaloonCoordinate(joint.Position.X, joint.Position.Y, joint.Position.Z);
        }

        void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
        {

            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null ||
                    kinectSensorChooser1.Kinect == null)
                {
                    return;
                }
                

                //Map a joint location to a point on the depth map
                //head
                DepthImagePoint headDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.Head].Position);
                //left hand
                DepthImagePoint leftDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandLeft].Position);
                //right hand
                DepthImagePoint rightDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);


                //Map a depth point to a point on the color image
                //head
                ColorImagePoint headColorPoint =
                    depth.MapToColorImagePoint(headDepthPoint.X, headDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //left hand
                ColorImagePoint leftColorPoint =
                    depth.MapToColorImagePoint(leftDepthPoint.X, leftDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //right hand
                ColorImagePoint rightColorPoint =
                    depth.MapToColorImagePoint(rightDepthPoint.X, rightDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
            }        
        }

        private void StopKinect(KinectSensor sensor) {
            if (sensor != null && sensor.IsRunning)
            {
                sensor.Stop();
                if (sensor.AudioSource != null)
                        sensor.AudioSource.Stop();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            closing = true;
            StopKinect(kinectSensorChooser1.Kinect);
        }
    }
}
