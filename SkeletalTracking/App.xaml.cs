using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;

namespace Balloon {
    // entry point to the default application
    public partial class App : Application {

        public static Balloon.Engine.Engine    Engine { get; set; }
        public static KinectChooser            KinectChooser { get; set; }

        App() {
            Engine = new Balloon.Engine.Engine();
            KinectChooser = new KinectChooser();
            KinectChooser.Show();
        }
    }
}
