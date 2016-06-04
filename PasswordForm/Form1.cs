using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Show();
            TopMost = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Password = NewPasswordTextBox.Text;
            string ConfirmPassword = ConfirmPasswordTextBox.Text;

            if (Password.Equals(ConfirmPassword))
            {

                string path = @"abc.dll";
                using (StreamWriter sw = File.CreateText(path))
                {
                    string encryptedstring = StringCipher.Encrypt(ConfirmPassword);
                    sw.WriteLine(encryptedstring);
                }
            }
            else
            {
                PasswordMissMatchLabel.Text = "Password Missmatch, try again...";
                return;
            }
            this.Dispose();

        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var h = (e.CloseReason);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Environment.Exit(1);
                // Do something proper to CloseButton.
            }

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
