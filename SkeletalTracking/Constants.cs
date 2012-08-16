using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Microsoft.Kinect;

namespace Balloon {
    public static class _Constants {
        // Cubes
        public const double     BaseOpacity = 0.3;  // opacity when a cube is not notified
        public const double     InteractedOpacity = 1;  // opacity of a cube when it is notified
        public static Color     DefaultCubeColour = Colors.Green;   // default colour to make a cube

        // Joints
        public static Brush     JointBrush = Brushes.LightGreen;    // colour of joint representation
        public const double     JointRadius = 0.025;    // size of joint representation

        // World
        public static Brush     KinectBrush = Brushes.SkyBlue;  // colour of the kinect
        
        // Engine
        public const int        FloorStabilisationSamples = 50; // how many samples to be used when determining the floor / angle
        public static TransformSmoothParameters SmoothParameters = new TransformSmoothParameters {  // smoothing parameters for the kinect
            Smoothing = 0.2f,
            Correction = 0.8f,
            Prediction = 1.0f,
            JitterRadius = 0.5f,
            MaxDeviationRadius = 0.2f
        };
        public const double     CreateHandDistance = 0.04; // how far apart do the hands have to be for create mode to start creating
        public const int        CreateStableTimout = 2000;  // how long do the hands have to be stable before create mode activates / materialises the cube
        public const double     CreateStableDeadzone = 0.05;    // deadzone (tolerance) movement of the hands for stabilisation
    }
}
