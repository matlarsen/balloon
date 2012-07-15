using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.Windows.Controls;
using Balloon.Cubes;

namespace Balloon.Engine {

    public class Engine {

        public KinectSensor                                         Kinect { get; private set; }
        public TransformSmoothParameters                            SmoothParameters { get; set; }
        public List<JointType>                                      JointsToTrack { get; private set; }
        public Skeleton[]                                           Skeletons { get; private set; }
        public Viewport3D                                           Viewport3D { get; set; }

        private List<Cube>                                              Cubes { get; set; }
        private Dictionary<int, Dictionary<JointType, List<Cube>>>      DenotifyQueue { get; set; }
        private Dictionary<int, Dictionary<JointType, Cube>>            Joint3DGeometry { get; set; }

        /// <summary>
        /// Constructs an engine that tracks the first skeletons' arms with the default transform settings
        /// </summary>
        public Engine(Viewport3D viewport) {
            // smoothing parameters for the kinect skeleton tracker
            SmoothParameters = new TransformSmoothParameters {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            };
            Viewport3D = viewport;

            // Initialise
            DenotifyQueue = new Dictionary<int, Dictionary<JointType, List<Cube>>>();
            JointsToTrack = new List<JointType>();
            Cubes = new List<Cube>();
            Joint3DGeometry = new Dictionary<int, Dictionary<JointType, Cube>>();

            // setup our hook into the WPF 3D system
            ModelVisual3D visual3dObject = new ModelVisual3D();
            Viewport3D.Children.Add(visual3dObject);
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

            SkeletonFrame skeletonFrame = e.OpenSkeletonFrame();
            
            // debug
            // so I can fuck around with the camera
            //OrthographicCamera camera = (OrthographicCamera)Viewport3D.Camera;

            // now is where we put any pre-conditions of the objects
            if (
                skeletonFrame == null       // no skeleton data 
             || JointsToTrack == null       // nothing has been specified to track
             || JointsToTrack.Count == 0
                )
                return;                     // get the fuck outta here
            Skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
            skeletonFrame.CopySkeletonDataTo(Skeletons);
            





            // get the skeleton array from this frame
            //SkeletonFrame skeletonFrame = e.OpenSkeletonFrame();
            //Skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
            //e.OpenSkeletonFrame().CopySkeletonDataTo(Skeletons);

            // if the denotify queue has a skeleton ID in it that is not in our skeletons array
            /*foreach (int skeletonTrackingId in DenotifyQueue.Keys.ToList()) {
                foreach (Skeleton skeleton in Skeletons)
                    if (skeleton.TrackingId == skeletonTrackingId)
                        continue;

                // denotify everything in the respective denotify queue
                foreach (JointType jointType in DenotifyQueue[skeletonTrackingId].Keys)
                    foreach (Cube cube in DenotifyQueue[skeletonTrackingId][jointType])
                        cube.DeNotify();

                // and remove the skeleton from the denotify queue
                DenotifyQueue.Remove(skeletonTrackingId);
            }*/


            //foreach (Skeleton s in skeletons) {

                


            // for each skeleton
            foreach (Skeleton skeleton in Skeletons) {

                // if there actually bloody is someone there
                if (skeleton != null && skeleton.TrackingState == SkeletonTrackingState.Tracked) {

                    // initialise this skeleton if we need to
                    if (!DenotifyQueue.ContainsKey(skeleton.TrackingId))
                        DenotifyQueue.Add(skeleton.TrackingId, new Dictionary<JointType, List<Cube>>()); // initialise it
                    
                    if (!Joint3DGeometry.ContainsKey(skeleton.TrackingId))
                        Joint3DGeometry.Add(skeleton.TrackingId, new Dictionary<JointType, Cube>());
                    

                    // for each jointType
                    foreach (JointType jointType in JointsToTrack) {

                        // check its being tracked by the skeleton tracker
                        if (skeleton.Joints[jointType].TrackingState != JointTrackingState.NotTracked) {

                            // transform the location of this joint to something more useful
                            Point3D jointLocation = new Point3D(skeleton.Joints[jointType].Position.X, skeleton.Joints[jointType].Position.Y, skeleton.Joints[jointType].Position.Z);
                            jointLocation.X = -jointLocation.X;

                            // initialise in our denotify queue
                            if (!DenotifyQueue[skeleton.TrackingId].ContainsKey(jointType))
                                DenotifyQueue[skeleton.TrackingId].Add(jointType, new List<Cube>());    // initialise it
                            
                            // create some geometry for the joint if necessary (to visualise)
                            if (!Joint3DGeometry[skeleton.TrackingId].ContainsKey(jointType)) {
                                Cube jointRepresentation = new NullCube(jointLocation, 0.03);
                                Joint3DGeometry[skeleton.TrackingId].Add(jointType, jointRepresentation);
                                Viewport3D.Children.Add(jointRepresentation.ModelVisual3D);
                            }

                            // Move the joint representation geometry
                            Joint3DGeometry[skeleton.TrackingId][jointType].MoveTo(jointLocation);


                            // if the point of this joint is in a cube
                            foreach (Cube cube in Cubes) {
                                if (cube.Rect3D.Contains(jointLocation)) {

                                    // and the cube is not already notified (if someone hasnt already stuck their limb in there)
                                    if (!cube.Notified) {

                                        // notify it, and add it to the list of things to denotify for this skeleton and joint type
                                        DenotifyQueue[skeleton.TrackingId][jointType].Add(cube);
                                        cube.Notify();
                                    }
                                    else {
                                        AnalogueCube analogueCube = cube as AnalogueCube;
                                        if (analogueCube != null) {
                                            // if its an analogue cube we have to always notify with the
                                            // reletave x position
                                            ((AnalogueCube)cube).NotifyAnalogue(_3DUtil.UnitDirectionVectorFromPointAToB(cube.Center, jointLocation));
                                        }
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
                }
                else {
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
        /// Add a cube
        /// </summary>
        /// <param name="cube"></param>
        public void AddCube(Cube cube) {
            Cubes.Add(cube);
        }
        /// <summary>
        /// Remove a cube
        /// </summary>
        /// <param name="cube"></param>
        public void RemoveCube(Cube cube) {
            // remove from the denotify queue

            // remove from the cube list
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
            kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
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
                Kinect.SkeletonStream.Disable();
                Kinect.DepthStream.Disable();
                Kinect.ColorStream.Disable();
                Kinect.Stop();
                if (Kinect.AudioSource != null)
                    Kinect.AudioSource.Stop();
            }
        }
    }


    public class BalloonEngineException : Exception {}
    public class NoKinectException : BalloonEngineException {}
}
