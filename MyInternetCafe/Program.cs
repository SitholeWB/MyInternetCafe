using MyInternetCafe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace BackgroundWorkerSampleCSharp
{
    static class Program
    {
        private static Process checker;
        private static Process main;
        private static int mainProcessID;

        [DllImport("user32")]
        public static extern void LockWorkStation();
        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            string path = @"abc.dll";
            if (!CheckInstallation(path))
            {
                return;
            }
            else
            {

                while (!PasswordChanged(path))
                {
                    //return; 
                }
            }


            //Main App Process.
            if (args.Length == 0)
            {
                //Saves current process info to pass on command line.
                main = Process.GetCurrentProcess();
                mainProcessID = main.Id;

                //Initializes the helper process
                checker = new Process();
                checker.StartInfo.FileName = main.MainModule.FileName;
                checker.StartInfo.Arguments = mainProcessID.ToString();

                checker.EnableRaisingEvents = true;
                checker.Exited += new EventHandler(checker_Exited);

                //Launch the helper process.
                checker.Start();

                Application.Run(new Administrator());
            }
            else //On the helper Process
            {
                main = Process.GetProcessById(int.Parse(args[0]));

                main.EnableRaisingEvents = true;
                main.Exited += new EventHandler(main_Exited);

                while (!main.HasExited)
                {
                    Thread.Sleep(1000); //Wait 1 second. 
                }

                //Provide some time to process the main_Exited event. 
                Thread.Sleep(2000);
            }

        }

        private static bool PasswordChanged(string path)
        {
            string s;
            using (StreamReader sr = File.OpenText(path))
            {
                s = sr.ReadLine();
            }
            string decryptedstring = StringCipher.Decrypt(s);
            if (decryptedstring.Equals("_password_"))
            {
                PasswordForm.Program.Main();
                return false; // Password not changed
            }
            return true; // Password changed            
        }

        private static bool CheckInstallation(string path)
        {

            if (!File.Exists(path))
            {
                MessageBox.Show("Error", "One of the installation file is corrupted, please re-install the application again.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string s;
            using (StreamReader sr = File.OpenText(path))
            {
                s = sr.ReadLine();
            }
            if (string.IsNullOrEmpty(s))
            {
                MessageBox.Show("Error", "One of the installation file is corrupted, please re-install the application again.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; //
            }
            return true;
        }


        //Double checks the user not closing the checker process.
        static void checker_Exited(object sender, EventArgs e)
        {
            //This only checks for the task manager process running. 
            //It does not make sure that the app has been closed by it. But close enough.
            //If you can think of a better way please let me know.
            if (Process.GetProcessesByName("taskmgr").Length != 0)
            {
                // MessageBox.Show("Task Manager killed helper process.");

                //If you like you could kill the main app here to. 
                //main.Kill();
            }
        }

        //Only gets to run on the checker process. The other one should be dead. 
        //If it isn't then run Forrest, run!!
        static void main_Exited(object sender, EventArgs e)
        {
            //This only checks for the task manager process running. 
            //It does not make sure that the app has been closed by it. But close enough.
            //If you can think of a better way please let me know.
            if (Process.GetProcessesByName("taskmgr").Length != 0)
            {
                LockWorkStation();
                MessageBox.Show("Task Manager killed my my app.");

            }
        }

    }
}
