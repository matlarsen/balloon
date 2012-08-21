using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;

namespace Balloon.Actions {
    class StemChannelMuteAction : Action {
        private WaveChannel32 WaveChannel32;
        private bool isMute = false;
        private float beforeMuteVolume = 0f;
        private double currentOpacity = 0d;
        public Cube Cube { get; private set; }

        public StemChannelMuteAction(WaveChannel32 waveChannel32) {
            WaveChannel32 = waveChannel32;
            beforeMuteVolume = waveChannel32.Volume;
        }

        public void DoAction() {
            if (!isMute) {
                isMute = true;
                beforeMuteVolume = WaveChannel32.Volume;
                WaveChannel32.Volume = 0;
                currentOpacity = 0.5;
                Cube.SolidColorBrush.Opacity = 0.5;
            }
            else {
                isMute = false;
                WaveChannel32.Volume = beforeMuteVolume;
                currentOpacity = 1;
                Cube.SolidColorBrush.Opacity = 1;
            }
        }

        public void StopAction() {
            Cube.SolidColorBrush.Opacity = currentOpacity;
        }

        public void setCube(Cube cube) {
            Cube = cube;
            cube.SolidColorBrush.Opacity = 1;
            currentOpacity = 1;
        }
    }
}
