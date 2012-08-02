using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.Windows.Controls;
using Balloon.Cubes;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;

namespace Balloon.Engine {

    // the various states that we are allowing the engine to be in
    public enum EngineMode { NormalInteractive, NoInteractive, CreateCube, MoveCube, DeleteCube }

    public class Engine {

        // a utility property, for getting a null origin thingy
        public static Point3D            Origin { get { return new Point3D(); } }

        // properties of the engine
        public KinectSensor              Kinect { get; private set; }
        public TransformSmoothParameters SmoothParameters { get; set; }
        public List<JointType>           JointsToTrack { get; private set; }
        public Skeleton[]                Skeletons { get; private set; }
        public HelixViewport3D           Viewport3D { get; private set; }
        public World                     World { get; private set; }
        public EngineMode                Mode { get; set; }

        // internal workings of the engine
        private List<Cube>                                           Cubes { get; set; }
        private Dictionary<int, Dictionary<JointType, List<Cube>>>   DenotifyQueue { get; set; }
        private Dictionary<int, Dictionary<JointType, CubeVisual3D>> Joint3DGeometry { get; set; }
        
        // floor and angle utility 
        private double[] floorStabilisationAngles = new double[_Constants.FloorStabilisationSamples];
        private int currentFloorStabilisationAngleIndex = 0;

        // create mode
        private Cube createCube;
        private bool handsTogether = false;
        private long stableTime;
        private Point3D previousPoint;

        /// <summary>
        /// Constructs an engine that tracks the first skeletons' arms with the default transform settings
        /// </summary>
        public Engine() {
            

            // Initialise objects
            DenotifyQueue = new Dictionary<int, Dictionary<JointType, List<Cube>>>();
            JointsToTrack = new List<JointType>();
            Cubes = new List<Cube>();
            Joint3DGeometry = new Dictionary<int, Dictionary<JointType, CubeVisual3D>>();
            Mode = EngineMode.NormalInteractive;

            // setup our viewport
            Viewport3D = new HelixViewport3D() {
                ClipToBounds = true,
                Orthographic = true,
                ShowCameraInfo = true,
                ShowCoordinateSystem = true,
                ShowFieldOfView = true,
                ShowFrameRate = true,
                CameraRotationMode = CameraRotationMode.Trackball,
                MinHeight = 450,
                MinWidth = 800,
                TouchMode = TouchMode.Rotating,
                RotateAroundMouseDownPoint = true,
                Camera = new OrthographicCamera() {
                    //Position = new Point3D(11, 8, -5),
                    //LookDirection = new Vector3D(-6.7, -4.7, 5)
                    Position = new Point3D(18.5, 13.5, -12.2),
                    LookDirection = new Vector3D(-14.9, -10.4, 11.1),
                    Width = 5.5
                },
            };
            Viewport3D.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0.5, -0.5, 0.5)));

            // register the camera move property on our viewer, stop us from doing lots of repetitive crap
            Viewport3D.CameraChanged += new RoutedEventHandler(Viewport3D_CameraChanged);

            // setup our world
            World = new World();
            Viewport3D.Children.Add(World.WorldObjects);
            Viewport3D.Children.Add(World.JointObjects);
            Viewport3D.Children.Add(World.Cubes);
            Viewport3D.Children.Add(World.Kinect);

            // set the default kinect settings
            SmoothParameters = _Constants.SmoothParameters;
        }

        /// <summary>
        /// Callback function for when the camera in the viewport changes,
        /// just need to rejig a few things so it renders correctly
        /// </summary>
        /// <param name="o"></param>
        /// <param name="a"></param>
        private void Viewport3D_CameraChanged(Object o, RoutedEventArgs a) {
            // sort all the elements by transparency so it looks ok
            ElementSortingHelper.SortModel(Viewport3D.Camera.Position, Viewport3D.Children);
        }


        /// <summary>
        /// Tick the engine. Usually gets called by the kinect when AllFramesReady fires
        /// </summary>
        public void Tick(object sender, SkeletonFrameReadyEventArgs e) {


            // first thing we want to do is check if the kinect and all our objects
            // are valid

            // if there's no kinect, we need to forcibly denotify everything
            // and throw an exception
            if (Kinect == null || !Kinect.IsRunning) {
                DenotifyEverything();
                throw new NoKinectException();
            }

            SkeletonFrame skeletonFrame = e.OpenSkeletonFrame();

            // now is where we put any pre-conditions of the objects
            if (
                skeletonFrame == null       // no skeleton data 
             || JointsToTrack == null       // nothing has been specified to track
             || JointsToTrack.Count == 0
                )
                return;                     // get the fuck outta here


            // bring in our skeletons
            Skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
            skeletonFrame.CopySkeletonDataTo(Skeletons);


            // re-arrange our world according to parameters from the kinect
            // such as where it thinks the floor is
            //if (skeletonFrame.FloorClipPlane.Item4 != 0)
            //    World.SetFloorHeight(skeletonFrame.FloorClipPlane.Item4);
            // and the angle the kinect is lookint at

            // stabilise our angles by picking the median from a set of angle samples
            floorStabilisationAngles[currentFloorStabilisationAngleIndex++] = Kinect.ElevationAngle;
            if (currentFloorStabilisationAngleIndex == _Constants.FloorStabilisationSamples) {
                World.SetKinectAngle((float)_Math.GetMedian(floorStabilisationAngles));
                currentFloorStabilisationAngleIndex = 0;
            }


            // denotify any cubes that are associated with skeletons that no longer exist
            List<int> skeletonsToNuke = new List<int>();
            foreach (int skeletonTrackingId in DenotifyQueue.Keys) {
                // if it exists in this new list of skeletons, fine
                bool exists = false;
                foreach (Skeleton skeleton in Skeletons)
                    if (skeleton.TrackingId == skeletonTrackingId)
                        exists = true;

                // if it doesnt, denotify its arse off
                // and remove the joint geometry too
                if (!exists) {
                    foreach (JointType jointType in DenotifyQueue[skeletonTrackingId].Keys) {
                        foreach (Cube cube in DenotifyQueue[skeletonTrackingId][jointType])
                            cube.DeNotify();
                        World.JointObjects.Children.Remove(Joint3DGeometry[skeletonTrackingId][jointType]);
                    }
                    skeletonsToNuke.Add(skeletonTrackingId);
                }
            }
            foreach (int skeletonToNuke in skeletonsToNuke)
                DenotifyQueue.Remove(skeletonToNuke);



            // ok at this point we should be all stable and ready to do operations
            if (Mode != EngineMode.NoInteractive) {

                // first thing we need to do is register our skeletons & joints
                // and sort out their reference 3d geometry
                // for each skeleton
                foreach (Skeleton skeleton in Skeletons) {

                    // only track full skeletons that we can find (will be 2 - all the API supports)
                    if (skeleton != null && skeleton.TrackingState == SkeletonTrackingState.Tracked) {

                        // initialise this skeleton if we need to
                        if (!DenotifyQueue.ContainsKey(skeleton.TrackingId))
                            DenotifyQueue.Add(skeleton.TrackingId, new Dictionary<JointType, List<Cube>>()); // initialise it

                        // and the joint geometry if we need to
                        if (!Joint3DGeometry.ContainsKey(skeleton.TrackingId))
                            Joint3DGeometry.Add(skeleton.TrackingId, new Dictionary<JointType, CubeVisual3D>());

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
                                    CubeVisual3D cube = new CubeVisual3D() {
                                        SideLength = _Constants.JointRadius,
                                        Center = jointLocation,
                                        Material = MaterialHelper.CreateMaterial(_Constants.JointBrush, _Constants.JointBrush)
                                    };
                                    Joint3DGeometry[skeleton.TrackingId].Add(jointType, cube);
                                    World.JointObjects.Children.Add(cube);
                                }

                                // Move the joint representation geometry
                                Joint3DGeometry[skeleton.TrackingId][jointType].Center = jointLocation;

                                // adjust the joint geometry because of the world transform
                                jointLocation.Y += World.FloorHeight;


                                // now go through our notification queue
                                // only do this if we are in normal interaction mode
                                if (Mode == EngineMode.NormalInteractive) {

                                    // if the point of this joint is in a cube
                                    foreach (Cube cube in Cubes) {
                                        if (cube.ModelVisual3D.Content.Bounds.Contains(jointLocation)) {

                                            // and the cube is not already notified (if someone hasnt already stuck their limb in there)
                                            if (!cube.Notified) {

                                                // notify it, and add it to the list of things to denotify for this skeleton and joint type
                                                DenotifyQueue[skeleton.TrackingId][jointType].Add(cube);
                                                cube.Notify();
                                            }
                                            else {  // delays detection by a frame, but makes the program more efficient
                                                AnalogueCube analogueCube = cube as AnalogueCube;
                                                if (analogueCube != null) {
                                                    // if its an analogue cube we have to always notify with the
                                                    // reletave x position
                                                    ((AnalogueCube)cube).NotifyAnalogue(_3DUtil.UnitDirectionVectorFromPointAToB(cube.ModelVisual3D.Content.Bounds.Location, jointLocation));
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
                            else {  // this joint has been marked as not tracked. So we need to denotify all its cubes
                                foreach (Cube cube in DenotifyQueue[skeleton.TrackingId][jointType])
                                    cube.DeNotify();
                                // kill from our view
                                World.JointObjects.Children.Remove(Joint3DGeometry[skeleton.TrackingId][jointType]);
                            }
                        }

                        // at this point all our joints are guaranteed to have been registered / moved
                        // so now we need to do our CRMD operations
                        // notification of cubes was already disabled if we are in one of these modes (not the interactivenormal mode)
                        // first of all, make sure our pre-requisites are in place
                        if (JointsToTrack.Contains(JointType.HandLeft) && JointsToTrack.Contains(JointType.HandRight)
                            && Skeletons.Length > 0) {

                            // what mode are we in
                            switch (Mode) {

                                // Create mode - we are making a coooob!
                                case EngineMode.CreateCube:
                                    Point3D leftHandPoint = Joint3DGeometry[skeleton.TrackingId][JointType.HandLeft].Center;
                                    Point3D rightHandPoint = Joint3DGeometry[skeleton.TrackingId][JointType.HandRight].Center;

                                    // ok well this depends on what stage of the process we are in

                                    // step 1
                                    // wait until the hands are within distance of each other
                                    if (!handsTogether) {
                                        if (_3DUtil.DistanceBetween(leftHandPoint, rightHandPoint) <= _Constants.CreateHandDistance) {
                                            handsTogether = true;
                                            Debug.WriteLine("Hands are together");
                                        }
                                    }
                                    else {
                                        // step 2
                                        // move and resize the cube we are creating relative to the hands
                                        Point3D cubeCenter = _3DUtil.Midpoint(leftHandPoint, rightHandPoint);
                                        cubeCenter.Y += World.FloorHeight;
                                        createCube.Resize(_3DUtil.DistanceBetween(leftHandPoint, rightHandPoint) / 2.0);
                                        createCube.MoveTo(cubeCenter);

                                        // step 3
                                        // we need to materialise the cube and get out of this process
                                        // we do this through a timeout 
                                        // or TODO: A signal
                                        
                                        // reset the counter if we fall outside the deadzone
                                        if (previousPoint == null || _3DUtil.DistanceBetween(previousPoint, cubeCenter) > _Constants.CreateStableDeadzone) {
                                            Debug.WriteLine("resetting create counter");
                                            previousPoint = cubeCenter;
                                            stableTime = DateTime.Now.Ticks;
                                        }
                                        // otherwise if our time has elapsed, material the arsehole and leave create mode!
                                        else {
                                            long currentTime = DateTime.Now.Ticks;
                                            if ((currentTime - stableTime) / TimeSpan.TicksPerMillisecond >= _Constants.CreateStableTimout) {
                                                Debug.WriteLine("timeout, cube is materialised");
                                                Mode = EngineMode.NormalInteractive;
                                            }
                                        }
                                    }
                                    break;
                                case EngineMode.DeleteCube:

                                    break;
                                case EngineMode.MoveCube:

                                    break;
                            }

                        }


                    }
                }
            }

            // drawing of the cubes is handled already
            // we're a done done!
            skeletonFrame.Dispose();
        }


        /// <summary>
        /// Sets the engine to create mode for the specified cube
        /// </summary>
        /// <param name="cubeToCreate"></param>
        public void EnterCreateCubeMode(Cube cubeToCreate) {
            // put the engine in create mode
            Mode = EngineMode.CreateCube;
            handsTogether = false;

            // assign the local create cube object
            createCube = cubeToCreate;

            // add to our world; make sure it has the same transform as joints though
            //createCube.ModelVisual3D.Transform = World.FloorTransform;
            AddCube(createCube);
            
            Debug.WriteLine("entered create mode");
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
            // add to our list of cubes for notification
            Cubes.Add(cube);

            // add it to our world
            World.Cubes.Children.Add(cube.ModelVisual3D);
        }
        /// <summary>
        /// Remove a cube
        /// </summary>
        /// <param name="cube"></param>
        public void RemoveCube(Cube cube) {
            // remove from the denotify queue

            // remove from the cube list
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


    /// <summary>
    /// Internal class representing the world of this engine
    /// </summary>
    public class World {
        public double                   FloorHeight { get { return FloorTransform.OffsetY; } }
        public GridLinesVisual3D        GridLines { get; set; }
        public CoordinateSystemVisual3D CoordinateSystem { get; set; }
        public CubeVisual3D             Kinect { get; set; }
        public ModelVisual3D            WorldObjects { get; set; }
        public ModelVisual3D            Cubes { get; set; }
        public ModelVisual3D            JointObjects { get; set; }
        public Transform3DGroup         WorldTransforms { get; set; }

        public TranslateTransform3D     FloorTransform { get; private set; }
        public RotateTransform3D        AngleRotationTransform { get; private set; }

        public World() {
            // instantiate our world
            WorldObjects = new ModelVisual3D();
            Cubes = new ModelVisual3D();
            JointObjects = new ModelVisual3D();

            // create something to represent where the kinect is
            Kinect = new CubeVisual3D() {
                SideLength = 0.1,
                Center = new Point3D(0, 0, 0),
                Material = MaterialHelper.CreateMaterial(_Constants.KinectBrush, _Constants.KinectBrush)
            };

            // something to represent the floor
            RectangleVisual3D Floor = new RectangleVisual3D() {
                LengthDirection = new Vector3D(0, 0, 1),
                Normal = new Vector3D(0, 1, 0),
                Origin = new Point3D(0, -0.01, 0),
                Length = 20,
                Width = 20,
                Fill = Brushes.White,
                Material = MaterialHelper.CreateMaterial(Brushes.White, Brushes.White)
            };
            WorldObjects.Children.Add(Floor);

            // grid lines to layer on top of the floor
            GridLines = new GridLinesVisual3D() {
                LengthDirection = new Vector3D(0, 0, 1),
                Normal = new Vector3D(0, 1, 0),
                Transform = new ScaleTransform3D(0.025, 0.025, 0.025),
                Center = new Point3D(0, 0, 100),
            };
            WorldObjects.Children.Add(GridLines);

            // and a reference co-ordinate system
            CoordinateSystem = new CoordinateSystemVisual3D() {
                ArrowLengths = 0.3
            };
            WorldObjects.Children.Add(CoordinateSystem);

            // setup our world transformations
            FloorTransform = new TranslateTransform3D();
            AngleRotationTransform = new RotateTransform3D(Rotation3D.Identity, Kinect.Center);
            WorldTransforms = new Transform3DGroup();
            WorldTransforms.Children.Add(FloorTransform);
            WorldTransforms.Children.Add(AngleRotationTransform);
            Kinect.Transform = WorldTransforms;
            JointObjects.Transform = FloorTransform;
        }

        /// <summary>
        /// Moves our kinect to wherever it is above the floor. This only
        /// affects the geometry of the kinect cube, not any of the interception calculations
        /// </summary>
        /// <param name="plane"></param>
        public void SetKinectHeight(float height) {
            FloorTransform.OffsetY = height;
        }


        /// <summary>
        /// Sets the view angle of the kinect. This transform is used in the interception
        /// calculations to make everything relative to the floor
        /// </summary>
        /// <param name="degrees"></param>
        public void SetKinectAngle(float degrees) {
            AngleRotationTransform.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), -degrees);
        }
    }

    public class BalloonEngineException : Exception {}
    public class NoKinectException : BalloonEngineException {}
}
