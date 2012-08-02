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

namespace Balloon {
    /// <summary>
    /// Interaction logic for Interface.xaml
    /// </summary>
    public partial class Interface : Window {
        public Interface() {
            InitializeComponent();
        }

        // on window closing, stop the kinect, kill the interface
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            App.Terminate();
        }

        private void MenuItem_MusicCubes_Click(object sender, RoutedEventArgs e) {
            MusicCubeCreator notes = new MusicCubeCreator();
            notes.Show();
        }
    }
}
