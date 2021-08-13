using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileNameRandom
{
    public partial class KeyInput : Form
    {
        public EventHandler keyEntered;
        public KeyInput()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("No key detected","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            keyEntered.Invoke(textBox1.Text, EventArgs.Empty);
            this.Close();
        }
    }
}
