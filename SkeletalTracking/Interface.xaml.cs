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
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Midi;

namespace Balloon {
    /// <summary>
    /// Interaction logic for Interface.xaml
    /// </summary>
    public partial class Interface : Window {
        public Interface() {
            InitializeComponent();

            // show some info about whats going on
            App.Engine.EngineModeChanged += new Engine.Engine.EngineModeChangedHandler(Engine_EngineModeChanged);
        }

        // change the mode text label
        public void Engine_EngineModeChanged(object sender, Engine.EngineModeChangedEventArgs e) {
            string text = "";
            switch (e.Mode) {
                case Engine.EngineMode.CreateCube: text = "Create"; break;
                default: text = ""; break;
            }
            lblEngineMode.Content = text;
        }

        // on window closing, stop the kinect, kill the interface
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            App.Terminate();
        }

        private void MenuItem_MusicCubes_Click(object sender, RoutedEventArgs e) {
            MusicCubeCreator notes = new MusicCubeCreator();
            notes.Show();
        }

        private void MenuItem_Reset_Click(object sender, RoutedEventArgs e) {
            App.Engine.RemoveAllCubes();
            App.Engine.StopTrackingAllJoints();
            App.Engine.TrackJoint(Microsoft.Kinect.JointType.HandLeft);
            App.Engine.TrackJoint(Microsoft.Kinect.JointType.HandRight);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            App.ChannelsAndInstruments[0].Instrument = Instrument.Glockenspiel;
            App.OutputDevice.SendProgramChange(App.ChannelsAndInstruments[0].Channel, App.ChannelsAndInstruments[0].Instrument);
            fun.Fun.GenerateAFuckTonneOfCubes(App.Engine, App.ChannelsAndInstruments[0], new Note("C"), 500, new Point3D(0, 1, 2), 0.9, 0.1);

            App.Engine.TrackJoint(Microsoft.Kinect.JointType.ElbowLeft);
            App.Engine.TrackJoint(Microsoft.Kinect.JointType.ElbowRight);
            App.Engine.TrackJoint(Microsoft.Kinect.JointType.KneeLeft);
            App.Engine.TrackJoint(Microsoft.Kinect.JointType.KneeRight);
        }
    }
}
