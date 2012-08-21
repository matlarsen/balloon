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
using NAudio.Wave;

namespace Balloon {
    /// <summary>
    /// Interaction logic for StemPlayer.xaml
    /// </summary>
    public partial class StemPlayer : Window {

        IStemPlayer ThisStemPlayer;

        public StemPlayer(IStemPlayer stemPlayer) {
            InitializeComponent();
            ThisStemPlayer = stemPlayer;
            ThisStemPlayer.createCubes();
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e) {
            ThisStemPlayer.Play();
        }

        private void Pause_Button_Click(object sender, RoutedEventArgs e) {
            ThisStemPlayer.Pause();
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e) {
            ThisStemPlayer.Stop();
        }
    }
}
