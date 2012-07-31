using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Microsoft.Kinect;

namespace Balloon {
    public static class _Constants {
        // Cubes
        public const double     BaseOpacity = 0.3;
        public const double     InteractedOpacity = 1;
        public static Color     DefaultCubeColour = Colors.Green;

        // Joints
        public static Brush     JointBrush = Brushes.LightGreen;
        public const double     JointRadius = 0.025;

        // World
        public static Brush     KinectBrush = Brushes.SkyBlue;
        
        // Engine
        public const int        FloorStabilisationSamples = 50;
        public static TransformSmoothParameters SmoothParameters = new TransformSmoothParameters {
            Smoothing = 0.5f,
            Correction = 0.5f,
            Prediction = 0.5f,
            JitterRadius = 0.05f,
            MaxDeviationRadius = 0.04f
        };
    }
}
