using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace Balloon.Engine {

    public class Engine {

        public KinectSensor                             Kinect { get; private set; }
        public TransformSmoothParameters                SmoothParameters { get; set; }
        public List<JointType>                          JointsToTrack { get; private set; }
        public Skeleton[]                               Skeletons { get; private set; }

        private List<Cube>                              Cubes { get; set; }
        private Dictionary<int, Dictionary<JointType, List<Cube>>>  DenotifyQueue { get; set; }

        /// <summary>
        /// Constructs an engine that tracks the first skeletons' arms with the default transform settings
        /// </summary>
        public Engine() {
            // smoothing parameters for the kinect skeleton tracker
            SmoothParameters = new TransformSmoothParameters {
                Smoothing = 0.3f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
            };
        }


        /// <summary>
        /// Tick the engine. Usually gets called by the kinect when AllFramesReady fires
        /// </summary>
        public void Tick(object sender, SkeletonFrameReadyEventArgs e) {
            // if there's no kinect, we need to forcibly denotify everything
            // and throw an exception
            if (Kinect == null) {
                DenotifyEverything();
                throw new NoKinectException();
            }

            // get the skeleton array from this frame
            e.OpenSkeletonFrame().CopySkeletonDataTo(Skeletons);

            // if the denotify queue has a skeleton ID in it that is not in our skeletons array
            foreach (int skeletonTrackingId in DenotifyQueue.Keys) {
                foreach (Skeleton skeleton in Skeletons)
                    if (skeleton.TrackingId == skeletonTrackingId)
                        continue;

                // denotify everything in the respective denotify queue
                foreach (JointType jointType in DenotifyQueue[skeletonTrackingId].Keys)
                    foreach (Cube cube in DenotifyQueue[skeletonTrackingId][jointType])
                        cube.DeNotify();

                // and remove the skeleton from the denotify queue
                DenotifyQueue.Remove(skeletonTrackingId);
            }

            // for each skeleton
            foreach (Skeleton skeleton in Skeletons) {
                
                // if the skeleton doesnt exist in our denotify queue
                if (!DenotifyQueue.ContainsKey(skeleton.TrackingId))
                    DenotifyQueue.Add(skeleton.TrackingId, new Dictionary<JointType,List<Cube>>()); // initialise it

                // for each jointType
                foreach (JointType jointType in JointsToTrack) {

                    // if the jointType doesnt exist in our denotify queue for this skeleton
                    if (!DenotifyQueue[skeleton.TrackingId].ContainsKey(jointType))
                        DenotifyQueue[skeleton.TrackingId].Add(jointType, new List<Cube>());    // initialise it

                    // if the point of this joint is in a cube
                    foreach (Cube cube in Cubes) {
                        if (cube.Rect3D.Contains(new Point3D(skeleton.Joints[jointType].Position.X, skeleton.Joints[jointType].Position.Y, skeleton.Joints[jointType].Position.Z))) {

                            // and the cube is not already notified (if someone hasnt already stuck their limb in there)
                            if (!cube.Notified) {

                                // notify it, and add it to the list of things to denotify for this skeleton and joint type
                                DenotifyQueue[skeleton.TrackingId][jointType].Add(cube);
                                cube.Notify();
                            }
                        }

                        // otherwise if it exists in the denotify queue for this skeleton and joint, denotify and remove it
                        else {
                            if (DenotifyQueue[skeleton.TrackingId][jointType].Contains(cube)) {
                                cube.DeNotify();
                                DenotifyQueue[skeleton.TrackingId][jointType].Remove(cube);
                            }
                        }
                    }
                }
            }

            // drawing of the cubes is handled already
            // we're a done done!
        }


        /// <summary>
        /// Track this joint across all skeletons
        /// </summary>
        /// <param name="jointType"></param>
        public void TrackJoint(JointType jointType) {
            // dont add twice
            if (!JointsToTrack.Contains(jointType)) {
                JointsToTrack.Add(jointType);
            }
        }
        /// <summary>
        /// Track these joints across all skeletons
        /// </summary>
        /// <param name="jointTypes"></param>
        public void TrackJoints(List<JointType> jointTypes) {
            foreach (JointType jointType in jointTypes)
                TrackJoint(jointType);
        }
        /// <summary>
        /// Stops tracking this joint across all skeletons
        /// </summary>
        /// <param name="jointType"></param>
        public void StopTrackingJoint(JointType jointType) {
            // we need to denotify any cube registered to this jointType

            // and then we need to deinitialise that jointType array

            throw new NotImplementedException();
        }


        /// <summary>
        /// Denotify absolutely everything - reset
        /// </summary>
        private void DenotifyEverything() {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Registers a kinect to the engine.
        /// </summary>
        /// <param name="kinect"></param>
        public void RegisterKinect(KinectSensor kinect) {
            // dont bother if we werent given anything
            if (kinect == null)
                throw new ArgumentNullException();

            StopKinect();   // safely stop the existing object
            kinect.SkeletonStream.Enable(SmoothParameters);
            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(Tick);

            // try and start it
            try {
                kinect.Start();
                Kinect = kinect;
            }
            catch (System.IO.IOException) { // if we cant start, some other buggering program is using it
                App.KinectChooser.kinectSensorChooser.AppConflictOccurred();
            }
        }


        /// <summary>
        /// Safely stops and shuts down the kinect
        /// </summary>
        public void StopKinect() {
            if (Kinect != null && Kinect.IsRunning) {
                Kinect.Stop();
                if (Kinect.AudioSource != null)
                    Kinect.AudioSource.Stop();
            }
        }
    }


    public class BalloonEngineException : Exception {}
    public class NoKinectException : BalloonEngineException {}
}
