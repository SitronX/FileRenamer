using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FileNameRandom
{
    public partial class ReplaceChars : Form
    {
        List<string> _files;
        public EventHandler charsChanged;

        public ReplaceChars(List<string> files)
        {
            InitializeComponent();
            _files = files;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {


                for (int i = 0; i < _files.Count; i++)
                {
                    string currentString = _files[i];
                    string[] arr = currentString.Split('\\');

                    arr[arr.Length-1]= arr[arr.Length-1].Replace(textBox1.Text, textBox2.Text);

                    string joined = string.Join("\\", arr);
                    System.IO.File.Move(_files[i], joined);
                }

                MessageBox.Show("Chars in files replaced", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                charsChanged.Invoke(textBox1.Text, EventArgs.Empty);

                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error!!!\n"+ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }
    }
}

