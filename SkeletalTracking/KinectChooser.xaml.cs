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
using Microsoft.Kinect;

namespace Balloon {
    public partial class KinectChooser : Window {
        public KinectChooser() {
            InitializeComponent();
        }

        // on window load, bind when a kinect gets detected / changed
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            kinectSensorChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(KinectSensorChanged);
        }

        // on window closing, stop the kinect
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            App.Engine.StopKinect();
        }

        // what to do when the state of the kinect changes
        // just give the new kinect object to the engine
        private void KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e) {
            KinectSensor kinect = (KinectSensor)e.NewValue;
            if (kinect != null)
                App.Engine.RegisterKinect(kinect);
        }

    }
}
