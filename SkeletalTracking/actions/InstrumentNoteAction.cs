using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Midi;

namespace Balloon.Actions {
    public class InstrumentNoteAction : Action {
        Pitch internalPitch;
        Pitch InternalPitch { get { return internalPitch; } set { internalPitch = StopNoteBeforeChange(value); } }
        OutputDevice internalDevice;
        OutputDevice Device { get { return internalDevice; } set { internalDevice = StopNoteBeforeChange(value); } }
        Channel internalChannel;
        Channel Channel { get { return internalChannel; } set { internalChannel = StopNoteBeforeChange(value); } }
        bool isPlaying = false;

        public InstrumentNoteAction(OutputDevice device, Channel channel, Pitch pitch)
            : base() {
            internalDevice = device;
            internalChannel = channel;
            internalPitch = pitch;
        }

        dynamic StopNoteBeforeChange(dynamic value) {
            Device.SendNoteOff(Channel, internalPitch, 64);
            return value;
        }

        public void DoAction() {
            // start playing the note
            if (!isPlaying) {
                if (!internalPitch.IsInMidiRange())
                    internalPitch = Pitch.C4;
                Device.SendNoteOn(Channel, internalPitch, 64 );
                isPlaying = true;
            }
        }
        public void StopAction() {
            // stop playing the note
            StopNoteBeforeChange(null);
            isPlaying = false;
        }
    }
}
