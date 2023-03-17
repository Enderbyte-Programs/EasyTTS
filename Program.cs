using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

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
        public static string stext = "";
        public static bool registered = false;
        private static int ecall = 0;
        public static void SpeakText(string text)
        {
            if (!registered)
            {
                synth.SpeakProgress += Synth_SpeakProgress;
                registered = true;
            }
            stext = text;
            //synth.SpeakProgress += Synth_SpeakProgress;
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

        private static void Synth_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            ecall++;
            f.SetStatusLabelText($"Speaking... ({Math.Round((double)(e.CharacterCount + e.CharacterPosition) / (double)stext.Length * 100D,0)}%)");
        }

        public static void Refreshsynth()
        {
            synth.Dispose();
            synth = new SpeechSynthesizer();
            registered = false;
            if (!registered)
            {
                synth.SpeakProgress += Synth_SpeakProgress;
                registered = true;
            }

        }
        public static void WttsFile(string filename,string text)
        {
            DateTime start = DateTime.Now;
            if (!registered)
            {
                synth.SpeakProgress += Synth_SpeakProgress;
                registered = true;
            }
            stext = text;
            f.SetStatusLabelText("Writing...");
            synth.SetOutputToWaveFile(filename);
            synth.Speak(text);
            synth.SetOutputToDefaultAudioDevice();
            f.SetStatusLabelText("Ready");
            DateTime end = DateTime.Now;
            double diff = (end - start).TotalMilliseconds;
            MessageBox.Show($"Wrote {ParseSize(new FileInfo(filename).Length)} bytes in {Math.Round(diff / 1000, 3)} seconds ({ParseSize((int)(new FileInfo(filename).Length / (diff / 1000)))} bytes per second)", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        public static string ParseSize(long size)
        {
            bool isneg = size < 0;
            if (isneg)
            {
                size = -size;
            }
            if (size > 2000000000)
            {
                return Math.Round((double)((double)size/1000000000D),2).ToString() + " GB";
            } else if (size > 2000000)
            {
                return Math.Round((double)((double)size / 1000000D), 2).ToString() + " MB";
            } else if (size > 2000)
            {
                return Math.Round((double)((double)size / 1000D), 2).ToString() + " KB";
            } else
            {
                return size.ToString() + " bytes";
            }

        }
    }
}
