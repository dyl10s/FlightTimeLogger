using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlightTimeLogger
{
    public partial class frmMain : Form
    {
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        Stopwatch simTimer = new Stopwatch();
        Process sim;

        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.Wheat;
            this.TransparencyKey = Color.Wheat;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;

            int curStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, curStyle | 0x80000 | 0x20);

            Rectangle workingArea = Screen.AllScreens.OrderBy(x => x.Bounds.X).Last().Bounds;
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);

            this.Hide();

            // Start Disclaimer
            Process disc = Process.Start("notepad.exe");
            disc.WaitForExit();

            this.Show();

            // Start Sim
            sim = Process.Start("cmd.exe");
            timer1.Start();
            simTimer.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = $"Time: {simTimer.Elapsed.Hours}h {simTimer.Elapsed.Minutes}m {simTimer.Elapsed.Seconds}s";
            if(sim != null && sim.HasExited)
            {
                timer1.Enabled = false;
                simTimer.Stop();
                var frmSummary = new frmSummary(simTimer.Elapsed);
                this.Hide();
                frmSummary.StartPosition = FormStartPosition.CenterScreen;
                frmSummary.ShowDialog();
                this.Close();
            }
        }
    }
}
