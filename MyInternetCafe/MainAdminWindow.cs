using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using MyInternetCafe;

namespace BackgroundWorkerSampleCSharp
{
    public partial class Administrator : Form
    {
        private static int Milliseconds = 0;
        private static int Minutes = 0;
        private static Dictionary<string, string> activityList = new Dictionary<string, string>();

        [DllImport("user32")]
        public static extern void LockWorkStation();
        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        public Administrator()
        {
            InitializeComponent();
            this.ControlBox = false;
            CreateMyTopMostForm();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            int.TryParse(MinutestextBox.Text, out Minutes);
            if (Minutes == 0)
            {
                MinutesRequiredLabel.Text = "Enter valid minutes (Required)";
                return;
            }
            else
            {
                Milliseconds = Minutes * 60000;
            }
            if (!IsCorrectPassword())
            {
                return;
            }
            // Start BackgroundWorker
            backgroundWorker1.RunWorkerAsync(Milliseconds);
            Hide();

            AdminPasswordTextBox.Text = "";
            MinutestextBox.Text = "";
            PasswordInvalidLabel.Text = "";
            MinutesRequiredLabel.Text = "";

            activityList = new Dictionary<string, string>();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (!IsCorrectPassword())
            {
                return;
            }
            try
            {
                // Cancel BackgroundWorker
                if (!backgroundWorker1.IsBusy)
                    backgroundWorker1.CancelAsync();
            }
            catch { }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker helperBW = sender as BackgroundWorker;
            int arg = (int)e.Argument;
            e.Result = BackgroundProcessLogicMethod(helperBW, arg);
            if (helperBW.CancellationPending)
            {
                e.Cancel = true;
            }

        }

        // Put all of background logic that is taking too much time
        private int BackgroundProcessLogicMethod(BackgroundWorker bw, int a)
        {
            int result = 0;
            int Millis = Milliseconds - (int)(Milliseconds * 0.3);
            int MillisCount = 0;
            int add = 40000;
            while (MillisCount < Millis)
            {

                if (Millis - MillisCount <= add)
                {
                    add = Millis - MillisCount;
                }

                MillisCount += add;
                ProcessUserData();
                Thread.Sleep(add);
            }
            //showBalloon("Time left", string.Format("You have {0} minutes left!", ((int)(Milliseconds * 0.3) / 60000)));
            
            MessageBox.Show(string.Format("You have approximately {0} minutes left! \n\n System will lock after approximately {0} minutes. \n\n YOUR WORK WON'T BE LOST, YOU JUST NEED TO ASK FOR HELP, EXTRA TIME ACTUALLY!", ((int)(Milliseconds * 0.3) / 60000)), "Time left", MessageBoxButtons.OK,MessageBoxIcon.Information,MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            Thread.Sleep((int)(Milliseconds * 0.3));
            //Open hidden window after background job completed

            return result;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) MessageBox.Show("Operation was canceled");
            else if (e.Error != null) MessageBox.Show(e.Error.Message);
            else
            {
                FinalizeUserData();
                CreateMyTopMostForm();
                LockWorkStation();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateMyTopMostForm();
        }


        private void showBalloon(string title, string body)
        {
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Visible = true;

            if (title != null)
            {
                notifyIcon.BalloonTipTitle = title;
            }
            if (body != null)
            {
                notifyIcon.BalloonTipText = body;
            }
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.ShowBalloonTip(30000);
        }

        private void CreateMyTopMostForm()
        {
            WindowState = FormWindowState.Maximized;
            Show();
            TopMost = true;
        }

        private void ProcessUserData()
        {
            foreach (Process p in Process.GetProcesses("."))
            {
                try
                {
                    if (p.MainWindowTitle.Length > 0)
                    {
                        // File.AppendAllText(@"xyz.dll", "text content" + Environment.NewLine);
                        if (!activityList.ContainsKey(p.ProcessName.ToString()))
                        {
                            activityList.Add(p.ProcessName.ToString(), p.MainWindowTitle.ToString());
                        }
                    }
                }
                catch { }
            }
        }

        private void FinalizeUserData()
        {
            foreach (var kv in activityList)
            {
                Array chars = string.Format("{0}: {1}", kv.Key, kv.Value).ToCharArray();
                string temp = "";
                foreach (char c in chars)
                {
                    temp += string.Format("{0}.", (int)c);
                }

                string path = @"xyz.dll";

                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(temp);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(temp);
                    }
                }


            }

        }


        private bool IsCorrectPassword()
        {
            string path = @"abc.dll";
            string s;
            using (StreamReader sr = File.OpenText(path))
            {
                s = sr.ReadLine();
            }

            string Password = AdminPasswordTextBox.Text;

            if (string.IsNullOrEmpty(Password))
            {
                PasswordInvalidLabel.Text = "Please enter valid administrator password...";
                return false;
            }

            string decryptedstring = StringCipher.Decrypt(s);
            if (!decryptedstring.Equals(Password))
            {
                return false;
            }
            return true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Exitbutton_Click(object sender, EventArgs e)
        {
            if (!IsCorrectPassword())
            {
                return;
            }
            Environment.Exit(1);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
