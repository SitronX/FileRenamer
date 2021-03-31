using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileNameRandom
{
    public partial class Form1 : Form
    {
        string[] files;
        string path = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    files = Directory.GetFiles(fbd.SelectedPath);
                    Array.Sort(files);
                    textBox1.Text = fbd.SelectedPath;
                    path = fbd.SelectedPath;

                    if (File.Exists(path + "\\ReplacedNames.txt"))
                    {
                        button2.Enabled = false;
                        button3.Enabled = true;
                    }
                    else
                    {
                        button2.Enabled = true;
                        button3.Enabled = false;
                    }


                        
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int counter = 1;

            string t = string.Join("\n", files);
            File.WriteAllText(path + "\\ReplacedNames.txt", t);

            foreach(string i in files)
            {
                string name =path+"\\"+ counter + "." + i.Split('.').Last();
                counter++;

                System.IO.File.Move(i, name);
            }

            MessageBox.Show("Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            button2.Enabled = false;
            button3.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(!File.Exists(path + "\\ReplacedNames.txt"))
            {
                MessageBox.Show("FileWithNamesDoesntExist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            int counter = 0;

            string[] names = File.ReadAllLines(path + "\\ReplacedNames.txt");

            for(int i=0;i<files.Length-1;i++)
            { 
                

                System.IO.File.Move(files[i], names[counter]);
                counter++;
            }

            File.Delete(path + "\\ReplacedNames.txt");

            MessageBox.Show("Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            button2.Enabled = false;
            button3.Enabled = false;
        }
    }
}
