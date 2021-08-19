using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileNameRandom
{
    public partial class Form1 : Form
    {
        List<string> files=new List<string>();
        string path;

        string AesKey;
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text !="")
            {
                if (Directory.Exists(textBox1.Text))
                {
                    path = textBox1.Text;
                    textBox1.Text = path;

                }
                else
                {
                    MessageBox.Show("PathDoesntExist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        path = fbd.SelectedPath;
                        textBox1.Text = path;
                    }
                }
            }

            files = Directory.GetFiles(path).ToList();

            if (files.Contains(path + "\\ReplacedNames.txt"))
            {
                files.Remove(path + "\\ReplacedNames.txt");
            }
            else if (files.Contains(path + "\\ReplacedNames"))
            {
                files.Remove(path + "\\ReplacedNames");
            }


            files.Sort(new Comparer());     //Windows compare equivalent





                        
                        

            if (File.Exists(path + "\\ReplacedNames.txt") || File.Exists(path + "\\ReplacedNames"))
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

        private void button2_Click(object sender, EventArgs e)
        {
            int counter = 1;

            string t = string.Join("\n", files);

            if(String.IsNullOrEmpty(AesKey))
            {
                File.WriteAllText(path + "\\ReplacedNames.txt", t);
            }
            else
            {
                Aes a = Aes.Create();

                using (SHA256 mySHA256 = SHA256.Create())
                {
                                               
                    a.Key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(AesKey));
                    a.IV = new byte[16];
                                               
                }

                byte[] arr = AES.EncryptStringToBytes_Aes(t, a.Key, a.IV);

                File.WriteAllBytes(path + "\\ReplacedNames", arr);

            }






            foreach (string i in files)
            {
                string name;

                if(String.IsNullOrEmpty(textBox2.Text))
                {
                     name = path + "\\" + counter + "." + i.Split('.').Last();

                }
                else
                {
                     name = path + "\\" + counter + textBox2.Text;

                }
                counter++;

                System.IO.File.Move(i, name);
            }

            MessageBox.Show("Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            button2.Enabled = false;
            button3.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            

            int counter = 0;

            string[] names;

            if(File.Exists(path + "\\ReplacedNames.txt"))
            {
                
                names = File.ReadAllLines(path + "\\ReplacedNames.txt");
                
            }
            else if(File.Exists(path + "\\ReplacedNames"))
            {
                if(String.IsNullOrEmpty(AesKey))
                {
                    MessageBox.Show("PasswordNotEntered", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }

                Aes a = Aes.Create();

                using (SHA256 mySHA256 = SHA256.Create())
                {

                    a.Key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(AesKey));
                    a.IV = new byte[16];

                }

                string tmp = AES.DecryptStringFromBytes_Aes(File.ReadAllBytes(path + "\\ReplacedNames"), a.Key, a.IV);
                names = tmp.Split('\n');
                

            }
            else
            {
                MessageBox.Show("NameFileDoesntExist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            


            for (int i=0;i<files.Count;i++)
            { 
                

                System.IO.File.Move(files[i], names[counter]);
                counter++;
            }

            File.Delete(path + "\\ReplacedNames.txt");
            File.Delete(path + "\\ReplacedNames");

            MessageBox.Show("Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            button2.Enabled = false;
            button3.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            KeyInput tmp=new KeyInput();
            tmp.keyEntered += GreyKeyOnEnter;
            tmp.Show();

        }
        private void GreyKeyOnEnter(Object obj, EventArgs args)
        {
            button4.Enabled = false;
            AesKey = (string)obj;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            path = textBox1.Text;
            button2.Enabled = false;
            button3.Enabled = false;
        }
    }
}
