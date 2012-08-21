using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using Balloon.Engine;
using Balloon.Cubes;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace Balloon {
    public class BonIverStemPlayer : IStemPlayer {
        public IWavePlayer           waveOutDevice = new DirectSoundOut(DirectSoundOut.DSDEVID_DefaultPlayback);
        public WaveMixerStream32     mixer = new WaveMixerStream32();
        public WaveFileReader[]      reader = new WaveFileReader[11];
        public WaveChannel32[]       channelStream = new WaveChannel32[11];
        public Cube[]                cubes = new Cube[11];
        public Cube[]                volumePanCubes = new Cube[11];
        public Point3D               BottomLeftOfArray = new Point3D(-0.55, 0.7, 1.2);
        public double                cubeRadius = 0.08;
        public double                cubeDistance = 0.025;
        private Engine.Engine Engine;

        // set everything up
        public BonIverStemPlayer(Engine.Engine engine) {
            mixer.AutoStop = false;
            waveOutDevice.Init(mixer);
            Engine = engine;

            // load in each of our files
            reader[0] = new WaveFileReader(@"C:\\Users\\Badger\\repos\\balloon\\SkeletalTracking\\boniver\\bangorkook.wav");
            reader[1] = new WaveFileReader(@"C:\Users\Badger\repos\balloon\SkeletalTracking\boniver\chippewa_falls.wav");
            reader[2] = new WaveFileReader(@"C:\Users\Badger\repos\balloon\SkeletalTracking\boniver\eleva.wav");
            reader[3] = new WaveFileReader(@"C:\Users\Badger\repos\balloon\SkeletalTracking\boniver\gilman.wav");
            reader[4] = new WaveFileReader(@"C:\Users\Badger\repos\balloon\SkeletalTracking\boniver\grand_rapids.wav");
            reader[5] = new WaveFileReader(@"C:\Users\Badger\repos\balloon\SkeletalTracking\boniver\heigh_on.wav");
            reader[6] = new WaveFileReader(@"C:\Users\Badger\repos\balloon\SkeletalTracking\boniver\lake_hallie.wav");
            reader[7] = new WaveFileReader(@"C:\Users\Badger\repos\balloon\SkeletalTracking\boniver\le_grange_wi.wav");
            reader[8] = new WaveFileReader(@"C:\Users\Badger\repos\balloon\SkeletalTracking\boniver\long_plain.wav");
            reader[9] = new WaveFileReader(@"C:\Users\Badger\repos\balloon\SkeletalTracking\boniver\mandolin_wa.wav");
            reader[10] = new WaveFileReader(@"C:\Users\Badger\repos\balloon\SkeletalTracking\boniver\virginia.wav");

            // set the starting positions
            // and load into the mixer
            for (int i = 0; i < 11; i++) {
                channelStream[i] = new WaveChannel32(reader[i]);
                channelStream[i].Position = 0;
                mixer.AddInputStream(channelStream[i]);
            }
        }

        // just make sure it dies a proper death
        ~BonIverStemPlayer() {
            for (int i = 0; i < 11; i++) {
                cubes[i].DeNotify();
                Engine.RemoveCube(cubes[i]);
                cubes[i] = null;
                volumePanCubes[i].DeNotify();
                Engine.RemoveCube(volumePanCubes[i]);
                volumePanCubes[i] = null;
            }

            waveOutDevice.Stop();
            waveOutDevice.Dispose();
        }

        // go through each of the channels and create cubes for them in our space
        public void createCubes() {
            for (int i = 0; i < 11; i++) {
                if (cubes[i] != null) {
                    cubes[i].DeNotify();
                    cubes[i] = null;
                    volumePanCubes[i].DeNotify();
                    Engine.RemoveCube(volumePanCubes[i]);
                    volumePanCubes[i] = null;
                }

                // mute unmute cube
                Point3D thisCenter = new Point3D(BottomLeftOfArray.X + (cubeRadius + cubeDistance) * i, BottomLeftOfArray.Y, BottomLeftOfArray.Z);
                cubes[i] = new StemMuteUnmuteCube(thisCenter, cubeRadius, channelStream[i]);
                cubes[i].SolidColorBrush.Color = Colors.Black;
                Engine.AddCube(cubes[i]);

                // volume pan cube
                Point3D thisPanVolumeCenter = new Point3D(BottomLeftOfArray.X + (cubeRadius + cubeDistance) * i, BottomLeftOfArray.Y + cubeRadius + cubeDistance, BottomLeftOfArray.Z);
                volumePanCubes[i] = new StemVolumePanCube(thisPanVolumeCenter, cubeRadius, channelStream[i]);
                volumePanCubes[i].SolidColorBrush.Color = Colors.Teal;
                Engine.AddCube(volumePanCubes[i]);
            }
        }

        public void Stop() {
            waveOutDevice.Stop();
            for (int i = 0; i < 11; i++) {
                channelStream[i].Position = 0;
            }
        }

        public void Play() {
            waveOutDevice.Play();
        }

        public void Pause() {
            waveOutDevice.Pause();
        }

    }
}
