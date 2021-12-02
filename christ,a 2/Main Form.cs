using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace christ_a_2
{
    public partial class mainForm : Form
    {
        public static class Constants
        {
            public const int memoryRefresh = 50;
        }

        List<byte[]> meme = new List<byte[]>();

        public mainForm()
        {
            InitializeComponent();

            this.HandleCreated += mainForm_HandleCreated;
        }

        private void mainForm_HandleCreated(object sender, EventArgs e)
        {
            Thread memoryCounterThread = new Thread(updateMemoryCounter);
            memoryCounterThread.Start();
        }

        void updateMemoryCounter()
        {
            while (true)
            {
                Invoke(new Action(() =>
                {
                    memoryUsedLabel.Text = "Memory used: " + ((float)System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024 * 1024)).ToString("0.00") + " GB";
                }));

                Thread.Sleep(Constants.memoryRefresh);
            }
        }

        private void startGameButton_Click(object sender, EventArgs e)
        {
            // start game
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void memoryLeakButton_Click(object sender, EventArgs e)
        {
            meme.Add(new byte[1024 * 1024 * 1024]);
        }
    }
}
