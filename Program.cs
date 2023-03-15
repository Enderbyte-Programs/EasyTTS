using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Reflection.Emit;

namespace EasyTTS
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Utils.f = new Form1();
            Application.Run(Utils.f);
        }
    }
    public static class Utils
    {
        public static Form1 f;
        public static SpeechSynthesizer synth = new SpeechSynthesizer();
        public static void SpeakText(string text)
        {
            f.SetStatusLabelText("Speaking");
            try
            {
                synth.Speak(text);
            } catch (System.OperationCanceledException)
            {
                //Cleared
            }
            f.EnableDisablePlayOnlyButtons(false);
            f.SetStatusLabelText("Ready");
        }
        public static void Refreshsynth()
        {
            synth.Dispose();
            synth = new SpeechSynthesizer();
            
        }
        public static void WttsFile(string filename,string text)
        {
            f.SetStatusLabelText("Writing...");
            synth.SetOutputToWaveFile(filename);
            synth.Speak(text);
            synth.SetOutputToDefaultAudioDevice();
            f.SetStatusLabelText("Ready");
        }
    }
}
