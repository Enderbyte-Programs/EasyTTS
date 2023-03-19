using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyTTS
{
    public partial class Form2 : Form
    {
        private string data = "";
        public bool exitedwell = false;
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!data.Equals(""))
            {
                exitedwell = true;
                this.Close();
            } else
            {
                try
                {
                    label2.Text = "Getting";
                    label2.ForeColor = Color.Black;
                    label2.Invalidate();
                    label2.Refresh();
                    Application.DoEvents();
                    HttpClient httpClient = new HttpClient();
                    Task<byte[]> t = httpClient.GetByteArrayAsync(textBox1.Text);
                    t.Wait();
                    if (t.IsFaulted)
                    {
                        throw t.Exception.GetBaseException();
                    }
                    string fi = Encoding.UTF8.GetString(t.Result);//Got to here, all is well
                    label3.Text = fi.Length.ToString() + " bytes";
                    data = fi;
                    exitedwell = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    label3.Text = "? Bytes";
                    label2.ForeColor = Color.Red;
                    label2.Text = "Invalid, try again";
                    MessageBox.Show($"URL is invalid. Please try again.\n\nMessage: {ex.Message}\nType: {ex.GetType().FullName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
            }
        }
        public string GetSelectedURL()
        {
            return data;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                label2.Text = "Checking";
                label2.ForeColor = Color.Black;
                label2.Invalidate();
                label2.Refresh();
                Application.DoEvents();
                HttpClient httpClient = new HttpClient();
                Task<byte[]> t = httpClient.GetByteArrayAsync(textBox1.Text);
                t.Wait();
                if (t.IsFaulted)
                {
                    throw t.Exception.GetBaseException();
                }
                string fi = Encoding.UTF8.GetString(t.Result);//Got to here, all is well
                label2.ForeColor = Color.Green;
                label2.Text = "Good! (press GO to submit)";
                label3.Text = fi.Length.ToString() + " bytes";
                data = fi;
            } catch (Exception ex)
            {
                label3.Text = "? Bytes";
                label2.ForeColor= Color.Red;
                label2.Text = "Invalid, try again";
                MessageBox.Show($"URL is invalid. Please try again.\n\nMessage: {ex.Message}\nType: {ex.GetType().FullName}","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
    }
}
