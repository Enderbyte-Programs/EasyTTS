using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyTTS
{
    public partial class Form1 : Form
    {
        private bool ispaused = false;
        public int volume = 100;
        public int speed = 0;
        public VoiceInfo voice;
        public int MODE = 0;//0: Speak, 1: File
        public Form1()
        {
            InitializeComponent();
            button3.Enabled = false;//Only allow pausing when speaking
            button5.Enabled = false;
            textBox1.Enabled = false;
            
        }
        public void SetStatusLabelText(string newText)
        {
            label3.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                label3.Text = newText;
                label3.Update();
                Application.DoEvents();
            });

        }
        public void EnableDisablePlayOnlyButtons(bool enable)
        {
            button3.Invoke((MethodInvoker)delegate
            {
                button3.Enabled=enable;
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label3.Text = "Loading...";
            label3.Update();
            Application.DoEvents();
            if (richTextBox1.Text.Replace(" ","").Length == 0)
            {
                MessageBox.Show("Please type some text in the text box\nBefore pressing Speak","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                label3.Text = "Ready";
                label3.Update();
                Application.DoEvents();
            } else
            {


                string etext = richTextBox1.Text;
                if (MODE == 0)
                {
                    button3.Enabled = true;
                    new Thread(new ParameterizedThreadStart(x => Utils.SpeakText(etext))).Start();
                } else
                {
                    if (File.Exists(textBox1.Text))
                    {
                        if (MessageBox.Show("The selected output file already exists.\nAre you sure you want to overwrite it?","File warning",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.No)
                        {
                            label3.Text = "Ready";
                            label3.Update();
                            Application.DoEvents();
                            return;
                        }
                    }
                    try
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(textBox1.Text)))
                        {
                            MessageBox.Show("Provided path cannot be written to", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            label3.Text = "Ready";
                            label3.Update();
                            Application.DoEvents();
                            return;
                        }
                    } catch (Exception ex)
                    {
                        MessageBox.Show("Provided path cannot be written to", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        label3.Text = "Ready";
                        label3.Update();
                        Application.DoEvents();
                        return;
                    }
                    string efm = textBox1.Text;
                    new Thread(new ParameterizedThreadStart(x => Utils.WttsFile(efm,etext))).Start();
                    
                }
                //Utils.SpeakText(richTextBox1.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label3.Text = "Stopping...";
            label3.Update();
            Application.DoEvents();
            Application.Exit();
            Environment.Exit(0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MODE == 1)
            {
                MessageBox.Show("You can't do this in file mode.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if (!ispaused)
            {
                label3.Text = "Paused";
                label3.Update();
                Application.DoEvents();
                Utils.synth.Pause();
                ispaused = true;
            } else
            {
                label3.Text = "Speaking";
                label3.Update();
                Application.DoEvents();
                Utils.synth.Resume();
                ispaused = false;
                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the speech buffer?","Question",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //richTextBox1.Text = String.Empty;
                Utils.Refreshsynth();
                UpdateSynthConfig();
                ispaused = false;
                label3.Text = "Ready";
                label3.Update();
                Application.DoEvents();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach (InstalledVoice x in Utils.synth.GetInstalledVoices())
            {
                comboBox1.Items.Add(x.VoiceInfo.Name);
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }
        private void UpdateSynthConfig()
        {
            Utils.synth.Volume = volume;
            Utils.synth.Rate = speed;
            Utils.synth.SelectVoice(voice.Name);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            volume = int.Parse(numericUpDown1.Value.ToString());
            UpdateSynthConfig();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            speed = int.Parse(numericUpDown2.Value.ToString()) - 10;
            UpdateSynthConfig();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            voice = Utils.synth.GetInstalledVoices()[comboBox1.SelectedIndex].VoiceInfo;
            UpdateSynthConfig();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            MODE = comboBox2.SelectedIndex;
            if (MODE == 1)
            {
                textBox1.Enabled = true;
                button5.Enabled = true;
            } else
            {
                textBox1.Enabled = false;
                button5.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "WAV Audio Files|*.wav";
            saveFileDialog.Title = "Please choose save file";
            DialogResult d = saveFileDialog.ShowDialog();
            if (d == DialogResult.OK)
            {
                textBox1.Text = saveFileDialog.FileName;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            label10.Text = richTextBox1.Text.Split(' ').Length.ToString() + " Words";
            label11.Text = richTextBox1.Text.Length.ToString() + " Characters";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void ntt(Control control,string text) {
            ToolTip t = new ToolTip();
            t.SetToolTip(control, text);
        }

        private void textBox1_MouseHover(object sender, EventArgs e)
        {
            ntt(this.textBox1, "The file to save the speech to (WAV audio)");
        }

        private void button5_MouseHover(object sender, EventArgs e)
        {
            ntt(button5, "Open a file dialogue to select a file");
        }

        private void comboBox1_MouseHover(object sender, EventArgs e)
        {
            ntt(comboBox1, "Select a voice to be used");
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            ntt(button1, "Go!");
        }

        private void button3_MouseHover(object sender, EventArgs e)
        {
            ntt(button3, "Pause or resume speech playback");
        }

        private void button4_MouseHover(object sender, EventArgs e)
        {
            ntt(button4, "Clear speech buffer. Use this if you want to stop audio");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Title = "Please choose a file to open";
            //d.Filter = "*|All Files|*.txt|Text Files";
            d.Multiselect = false;
            //d.FilterIndex = 0;
            DialogResult dx = d.ShowDialog();
            if (dx == DialogResult.OK)
            {
                byte[] xdata = File.ReadAllBytes(d.FileName);
                string rttf = Encoding.UTF8.GetString(xdata);
                if (checkBox1.Checked)
                {
                    richTextBox1.Text += rttf;
                } else
                {
                    richTextBox1.Text = rttf;
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.ShowDialog(this);
            if (f.exitedwell)
            {
                if (checkBox1.Checked)
                {
                    richTextBox1.Text += f.GetSelectedURL();
                } else
                {
                    richTextBox1.Text = f.GetSelectedURL();
                }
            }
            
        }
    }
}
