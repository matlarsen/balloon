using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Midi;
using Balloon.Cubes;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Balloon {
    /// <summary>
    /// Interaction logic for MusicCubeCreator.xaml
    /// </summary>
    public partial class MusicCubeCreator : Window {

        Instrument  CurrentInstrument { get; set; }
        Channel     CurrentChannel { get; set; }
        Type        CurrentCubeType { get; set; }

        int currentScaleNoteIndex = 0;

        /// <summary>
        /// Enumerate all our shizzle
        /// </summary>
        public MusicCubeCreator() {
            InitializeComponent();

            // populate instrument list
            PopulateInstrumentList();

            // set cube maker defaults
            CurrentInstrument = App.ChannelsAndInstruments[0].Instrument;
            CurrentChannel = App.ChannelsAndInstruments[0].Channel;
            CurrentCubeType = typeof(SingleNoteCube);

            // get the list of channels and the instruments on em
            foreach (ChannelAndInstrument ci in App.ChannelsAndInstruments) {
                lstActiveInstruments.Items.Add(
                    new ListBoxItem() {
                        Content = String.Format("{0}: {1}", ci.Channel, ci.Instrument)
                    });
            }

            // we want to recreate a cube when the mode changes
            App.Engine.EngineCubeCreated += new Engine.Engine.EngineCubeCreatedHandler(Engine_EngineCubeCreated);

            // UI tidy up
            ((ComboBoxItem)cmbInstruments.Items[0]).IsSelected = true;
            ((ListBoxItem)lstActiveInstruments.Items[0]).IsSelected = true;
        }

        private void PopulateInstrumentList() {
            Array instruments = Enum.GetValues(typeof(Instrument));
            foreach(Instrument instrument in instruments ) {
                cmbInstruments.Items.Add(
                    new ComboBoxItem() {
                        Content = Enum.GetName(typeof(Instrument), instrument).Trim()
                    });
            }
        }

        private ChannelAndInstrument GetChannelAndInstrument() {
            string channelString = (string)((ListBoxItem)lstActiveInstruments.SelectedItem).Content;
            channelString = Regex.Match(channelString, @"Channel\d*").Value;
            Channel channel = (Channel)Enum.Parse(typeof(Channel), channelString);

            foreach (ChannelAndInstrument ci in App.ChannelsAndInstruments) {
                if (ci.Channel == channel)
                    return ci;
            }

            throw new KeyNotFoundException();
        }

        private void btnSetInstrument_Click(object sender, RoutedEventArgs e) {
            // set this instrument to the channel, update our defaults

            // get the tuple associated with the selected channel
            ChannelAndInstrument ci = GetChannelAndInstrument();
            CurrentChannel = ci.Channel;

            // work out the instrument enum from what is selected
            string selectedInstrument = (string)((ComboBoxItem)cmbInstruments.SelectedItem).Content;
            Instrument instrument = (Instrument)Enum.Parse(typeof(Instrument), selectedInstrument);

            // set the instruments
            CurrentInstrument = instrument;
            ci.Instrument = instrument;

            // set the midi channel
            App.OutputDevice.SilenceAllNotes();
            App.OutputDevice.SendProgramChange(CurrentChannel, CurrentInstrument);

            // update the channel and instrument list
            ((ListBoxItem)lstActiveInstruments.SelectedItem).Content = String.Format("{0}: {1}", ci.Channel, ci.Instrument);
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e) {
            // scale mode?
            if (!(bool)chkScaleMode.IsChecked)
                currentScaleNoteIndex = 0;

            // otherwise, create a cube that matches the specifications we just set
            Cube cube = new NullCube(Engine.Engine.Origin, 0);

            // yank values from comboboxes n shit
            Note note = new Note((string)((ComboBoxItem)cmbNote.SelectedItem).Content);
            int octave = int.Parse((string)((ComboBoxItem)cmbOctave.SelectedItem).Content);
            
            ChordPattern chordPattern = Chord.Major;
            ScalePattern scalePattern = Scale.Major;

            switch ((string)((ComboBoxItem)cmbScale.SelectedItem).Content) {
                case "Major": scalePattern = Scale.Major; break;
                case "HarmonicMinor": scalePattern = Scale.HarmonicMinor; break;
                case "MelodicMinorAscending": scalePattern = Scale.MelodicMinorAscending; break;
                case "MelodicMinorDescending": scalePattern = Scale.MelodicMinorDescending; break;
                case "NaturalMinor": scalePattern = Scale.NaturalMinor; break;
            }
            
            switch ((string)((ComboBoxItem)cmbChordType.SelectedItem).Content) {
                case "Major": chordPattern = Chord.Major; break;
                case "Minor": chordPattern = Chord.Minor; break;
                case "Seventh": chordPattern = Chord.Seventh; break;
                case "Augmented": chordPattern = Chord.Augmented; break;
                case "Diminished": chordPattern = Chord.Diminished; break;
            }
            Scale scale = new Scale(note, scalePattern);

            // scalemode translation
            note = scale.NoteSequence[currentScaleNoteIndex++ % 7];

            Pitch pitch = note.PitchInOctave(octave);

            int chordInversion = int.Parse((string)((ComboBoxItem)cmbInversion.SelectedItem).Content);
            Chord chord = new Chord(note, chordPattern, chordInversion);

            // single note cube
            if ((bool)Note.IsChecked) {
                cube = new SingleNoteCube(Engine.Engine.Origin, _Constants.CreateHandDistance / 2.0, pitch, CurrentInstrument, App.OutputDevice, CurrentChannel);
            }

            if ((bool)Chord2.IsChecked) {
                cube = new ChordCube(Engine.Engine.Origin, _Constants.CreateHandDistance / 2.0, chord, octave, CurrentInstrument, App.OutputDevice, CurrentChannel);
            }

            
            // give the cube a random colour
            Random randomGen = new Random();
            KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            KnownColor randomColorName = names[randomGen.Next(names.Length)];
            System.Drawing.Color tempColor = System.Drawing.Color.FromKnownColor(randomColorName);
            System.Windows.Media.Color randomColor = System.Windows.Media.Color.FromArgb(tempColor.A, tempColor.R, tempColor.G, tempColor.B);
            cube.SolidColorBrush.Color = randomColor;

            // now set the engine to create mode, and assign this as the cube to be created
            App.Engine.SetCreateCube(cube);
        }

        // when the engine creates a new cube, reproduce another one
        void Engine_EngineCubeCreated(object sender, Engine.EngineCubeCreatedEventArgs e) {
            btnCreate_Click(null, null);
        }
    }
}
