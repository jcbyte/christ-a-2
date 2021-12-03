using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

            mainMenuPanel.Visible = true;
            gamePanel.Visible = false;
            cutscenePanel.Visible = false;
            
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

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        async private void memoryLeakButton_Click(object sender, EventArgs e)
        {
            mainMenuPanel.Visible = false;
            cutscenePanel.Visible = true;
            cutsceneMediaPlayer.URL = "D:\\Users\\joel_\\Downloads\\cutscene.mp4";

            await Task.Delay(5000); // until memory leak starts

            Thread increaseMemoryThread = new Thread(increaseMemoryLoop);
            increaseMemoryThread.Start();

            await Task.Delay(5000); // until game starts

            cutsceneMediaPlayer.URL = "";
            cutscenePanel.Visible = false;
            gamePanel.Visible = true;
        }

        private void startGameButton_MouseEnter(object sender, EventArgs e)
        {
            System.Drawing.Point tempLocation = memoryLeakButton.Location;
            memoryLeakButton.Location = startGameButton.Location;
            startGameButton.Location = tempLocation;
        }
    }
}
