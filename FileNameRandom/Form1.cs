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
            try
            {
                files = Directory.GetFiles(path).ToList();
            }
            catch
            {
                return;
            }

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
                button6.Enabled = true;
                button2.Enabled = false;
                button3.Enabled = true;
                button5.Enabled = false;
            }
            else
            {
                button6.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = false;
                button5.Enabled = true;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int counter = 1;


            if (shuffleCheckbox.Checked)
            {
                 Shuffle(files);
            }
            string t = string.Join("\n", files);



            if (String.IsNullOrEmpty(AesKey))
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
                textBox2.Text = textBox2.Text.Replace(".", "");
                if (String.IsNullOrEmpty(textBox2.Text))
                {
                    string[] arr = i.Split('.');

                    if (arr.Length > 1)
                        name = path + "\\" + counter + "." + i.Split('.').Last();
                    else
                        name = path + "\\" + counter;

                }
                else
                {
                     name = path + "\\" + counter + "."+ textBox2.Text;

                }
                counter++;

                System.IO.File.Move(i, name);
            }

            MessageBox.Show("Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            button2.Enabled = false;
            button3.Enabled = false;
            button6.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            

            int counter = 0;

            List<string> names;

            if (File.Exists(path + "\\ReplacedNames.txt"))
            {

                names = File.ReadAllLines(path + "\\ReplacedNames.txt").ToList();

            }
            else if (File.Exists(path + "\\ReplacedNames"))
            {
                if (String.IsNullOrEmpty(AesKey))
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
                names = tmp.Split('\n').ToList();


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
            button6.Enabled = false;
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
            button6.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox2.Text))
            {
                textBox2.Text = textBox2.Text.Replace(".", "");
                foreach (string i in files)
                {
                    string[] arr = i.Split('.');

                    try
                    {


                        if (arr.Length > 1&&!arr.Last().Contains("\\")&&!arr.Last().Contains("/"))
                        {
                            string tmp = i.Split('.').Last();
                            string tmp2 = i.Replace(tmp, textBox2.Text);
                            System.IO.File.Move(i, tmp2);
                        }
                        else
                        {
                            System.IO.File.Move(i, $"{i}.{textBox2.Text}");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Some file probably already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;

                    }

                }

                MessageBox.Show("Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                button2.Enabled = false;
                button3.Enabled = false;
                button6.Enabled = false;
            }
            else
            {
                MessageBox.Show("Suffix didnt entered", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }
        public void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ReplaceChars cr = new ReplaceChars(files);
            cr.Show();
            cr.charsChanged += button1_Click;
        }
    }
}
