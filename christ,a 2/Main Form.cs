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
            public const int memoryCounterRefresh = 50;

            public const int memoryIncreaseRate = 50;
            public const int memoryIncrease = 1024 * 1024 * 64;
        }

        List<byte[]> meme = new List<byte[]>();

        public mainForm()
        {
            InitializeComponent();

            
            this.HandleCreated += mainForm_HandleCreated;
        }

        private void mainForm_HandleCreated(object sender, EventArgs e)
        {
            Thread memoryCounterThread = new Thread(updateMemoryCounterLoop);
            memoryCounterThread.Start();
        }

        private void updateMemoryCounterLoop()
        {
            while (true)
            {
                Invoke(new Action(() =>
                {
                    float memUsed = (float)System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024 * 1024);
                    //float memUsed = (float)GC.GetTotalMemory(true) / (1024 * 1024 * 1024);
                    memoryUsedLabel.Text = "Memory used: " + memUsed.ToString("0.00") + " GB";
                }));

                Thread.Sleep(Constants.memoryCounterRefresh);
            }
        }

        private void increaseMemoryLoop()
        {
            while (true)
            {
                byte[] tempMeme = new byte[Constants.memoryIncrease];
                for (int i = 0; i < Constants.memoryIncrease; i++)
                    tempMeme[i] = 0;

                meme.Add(tempMeme);

                Thread.Sleep(Constants.memoryIncreaseRate);
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
            Thread increaseMemoryThread = new Thread(increaseMemoryLoop);
            increaseMemoryThread.Start();
        }
    }
}
