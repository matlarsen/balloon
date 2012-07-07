using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows.Media;
using Midi;


namespace SkeletalTracking {

    // because things are retarded and I cant write to the
    // depth property
    public class BaloonCoordinate {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public BaloonCoordinate(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    // A cube that plays a single note
    public class SingleNoteCube : Cube {
        public SingleNoteCube(BaloonCoordinate center, float radius,/*Canvas canvas*/
                Pitch pitch, int octave, Instrument instrument, OutputDevice device, Channel channel) 
            : base(center, radius, new InstrumentNoteAction(device, channel, pitch, octave)) {
        }
    }

    // A sphere that plays a single note
    public class SingleNoteSphere : Sphere {
        public SingleNoteSphere(BaloonCoordinate center, float radius,/*Canvas canvas*/
                Pitch pitch, int octave, Instrument instrument, OutputDevice device, Channel channel)
            : base(center, radius, new InstrumentNoteAction(device, channel, pitch, octave)) {
        }
    }

    /*public class MultiNoteCube : Cube {
        // collection of 56 SingleNoteCubes
    }

    public class SoundFileCube : Cube {

    }*/

    // general definition of an action
    public interface Action {
        void DoAction();
        void StopAction();
    }

    public class InstrumentNoteAction : Action {
        Pitch internalPitch;
        Pitch           InternalPitch { get { return internalPitch; } set { internalPitch = StopNoteBeforeChange(value); } }
        OutputDevice internalDevice;
        OutputDevice    Device { get { return internalDevice; } set { internalDevice = StopNoteBeforeChange(value); } }
        Channel internalChannel;
        Channel         Channel { get { return internalChannel; } set { internalChannel = StopNoteBeforeChange(value); } }
        int internalOctave;
        int             Octave { get { return internalOctave; } set { internalOctave = StopNoteBeforeChange(value); } }
        bool            isPlaying = false;

        public InstrumentNoteAction(OutputDevice device, Channel channel, Pitch pitch, int octave)
            : base() {
            internalDevice = device;
            internalChannel = channel;
            internalPitch = pitch;
            internalOctave = octave;
        }

        dynamic StopNoteBeforeChange(dynamic value) {
            Device.SendNoteOff(Channel, internalPitch, 64);
            return value;
        }

        public void DoAction() {
            // start playing the note
            if (!isPlaying) {
                Device.SendNoteOn(Channel, internalPitch, 64);
                isPlaying = true;
            }
        }
        public void StopAction() {
            // stop playing the note
            StopNoteBeforeChange(null);
            isPlaying = false;
        }
    }

    public class Cube : Shape {

        public Cube(BaloonCoordinate center, float radius, Action action) : base(center, radius, action) { }

        public override bool IsThisPointInBounds(BaloonCoordinate point) {
            // for now just check against a plain cube, sod transformations and stuff
            bool xValid = false;
            bool yValid = false;
            bool zValid = false;

            Func<float, float, float, bool> CheckBounds = (pointCheck, pointMin, pointMax) => {
                return (pointCheck > pointMin) && (pointCheck < pointMax);
            };

            xValid = CheckBounds(point.x, Center.x - Radius, Center.x + Radius);
            yValid = CheckBounds(point.y, Center.y - Radius, Center.y + Radius);
            zValid = CheckBounds(point.z, Center.z - Radius, Center.z + Radius);

            return xValid && yValid && zValid;
        }
    }

    public class Sphere : Shape {
        
        public Sphere(BaloonCoordinate center, float radius, Action action) : base(center, radius, action) { }

        // Inside= (sqrt ( (px-xc)2 + (py-yc)2 + (pz-zc)2 ) < radius
        public override bool IsThisPointInBounds(BaloonCoordinate point) {
            return Math.Sqrt(
                (point.x - Center.x) * (point.x - Center.x) +
                (point.y - Center.y) * (point.y - Center.y) +
                (point.z - Center.z) * (point.z - Center.z)
                ) < Radius;
        }
    }

    public abstract class Shape {
        public BaloonCoordinate    Center { get; set; }
        public float               Radius { get; set; }
        public Color               Color { get; set; }
        public Action              Action { get; set; }

        public Shape(BaloonCoordinate center, float radius, Action action /*Canvas canvas*/) {
            Center = center;
            Radius = radius;
            Action = action;
            Color = Colors.Green;   // default colour
        }

        // gets called when we need to update for whatever reason
        // TODO

        // needs to be overridden. By default,yeah, its in the fucking shape somewhere
        public virtual bool IsThisPointInBounds(BaloonCoordinate point) {
            return true;
        }

        // when the point goes in bounds do something
        public void PointInBounds() {
            Action.DoAction();
        }
        // and when it goes out, stop it
        public void PointLeftBounds() {
            Action.StopAction();
        }
    }

    public class Notifier {
        //List<Shape> deNotifyShapes = new List<Shape>();
        Dictionary<JointType, List<Shape>> jointTypesAndShapesToDenotify = new Dictionary<JointType, List<Shape>>();

        public List<Shape> Shapes { get; set; }
        //public List<Joint> Joints { get; set; }
        public List<JointType> jointTypes = new List<JointType>(); //Enum.GetValues(typeof(JointType)).Cast<JointType>().ToList();

        public Notifier() {
            Shapes = new List<Shape>();
            //Joints = new List<Joint>();
        }

        // when we add a joint we also need to create an array thing for it
        public void addJoint(JointType joint) {
            // dont add twice
            if (!jointTypes.Contains(joint)) {
                jointTypes.Add(joint);
                jointTypesAndShapesToDenotify.Add(joint, new List<Shape>());
            }
        }

        // notify function runs through all our cubes and
        // all our joints to check if the point of the joint
        // falls in the cube bounds. If so we notify that cube, 
        // and store it in the collection of things to denotify
        // for that joint.
        public void Notify(Skeleton s) {

            // notify queue
            foreach (Shape c in Shapes) {
                foreach (JointType j in jointTypes) {
                    Joint joint = s.Joints[j];
                    BaloonCoordinate thisJointPoint = new BaloonCoordinate(joint.Position.X, joint.Position.Y, joint.Position.Z);
                    if (c.IsThisPointInBounds(thisJointPoint)) { // if the point is inside this cube
                        c.PointInBounds(); // notify the cube the point is in bounds
                        jointTypesAndShapesToDenotify[j].Add(c); // and add the cube to our denotify list
                    }
                    else // if its not in bounds
                        if (jointTypesAndShapesToDenotify[j].Contains(c)) { // and is on our de notify list
                            c.PointLeftBounds(); // denotify it
                            jointTypesAndShapesToDenotify[j].Remove(c); // and remove from our denotify list
                        }
                }
            }

            // now we need to nuke the joints

            // first of all, denotify
            /*List<Cube> notDenotifiedCubes = new List<Cube>();
            foreach (Cube c in deNotifyCubes)
                if (c.IsThisPointInBounds(point))
                    notDenotifiedCubes.Add(c);  // if its still in bounds we want to check again
                else
                    c.PointLeftBounds();    // otherwise call its left bounds function

            deNotifyCubes = notDenotifiedCubes;   // swap the lists
            */
            // denotify queue
            // (we want a cube thats been entered to check if it goes out)
            
            
        }
    }
}