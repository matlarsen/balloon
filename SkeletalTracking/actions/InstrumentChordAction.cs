using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Midi;

namespace Balloon.Actions {
    public class InstrumentChordAction : Action {

        Chord Chord { get; set; }
        int Octave { get; set; }
        OutputDevice internalDevice;
        OutputDevice Device { get { return internalDevice; } set { internalDevice = StopNotesBeforeChange(value); } }
        Channel internalChannel;
        Channel Channel { get { return internalChannel; } set { internalChannel = StopNotesBeforeChange(value); } }
        bool isPlaying = false;

        public InstrumentChordAction(OutputDevice device, Channel channel, Chord chord, int octave)
            : base() {
            internalDevice = device;
            internalChannel = channel;
            Chord = chord;
            Octave = octave;
        }

        dynamic StopNotesBeforeChange(dynamic value) {
            Pitch lastPitch = Pitch.A0;
            foreach (Note note in Chord.NoteSequence) {
                Pitch pitch;
                if (lastPitch == Pitch.A0)
                    pitch = note.PitchInOctave(Octave);
                else
                    pitch = note.PitchAtOrAbove(lastPitch);

                Device.SendNoteOff(Channel, pitch, 64);
                lastPitch = pitch;
            }
            return value;
        }

        public void DoAction() {
            // play the chord
            if (!isPlaying) {

                Pitch lastPitch = Pitch.A0;
                foreach (Note note in Chord.NoteSequence) {
                    Pitch pitch;
                    if (lastPitch == Pitch.A0)
                        pitch = note.PitchInOctave(Octave);
                    else
                        pitch = note.PitchAtOrAbove(lastPitch);

                    Device.SendNoteOn(Channel, pitch, 64);
                    lastPitch = pitch;
                }
                isPlaying = true;
            }
        }
        public void StopAction() {
            // stop playing the chord
            StopNotesBeforeChange(null);
            isPlaying = false;
        }
    }
}
