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

            public const int playerSpeed = 50;
        }

        private class Vector2i
        {
            public int x;
            public int y;

            public Vector2i(int x_, int y_)
            {
                x = x_;
                y = y_;
            }

            public Vector2i(System.Drawing.Point p)
            {
                x = p.X;
                y = p.Y;
            }

            static public Vector2i operator +(Vector2i lhs, Vector2i rhs)
            {
                lhs.x += rhs.x;
                lhs.y += rhs.y;
                return lhs;
            }

            public System.Drawing.Point toPoint()
            {
                return new System.Drawing.Point(x, y);
            }
        }

        private class Enemy
        {
            public Vector2i pos;
            public PictureBox pb;

            public Enemy(Vector2i pos_, System.Drawing.Size size)
            {
                pos = pos_;

                pb = new PictureBox();
                pb.BackgroundImageLayout = ImageLayout.Stretch;
                pb.Size = size;
                pb.BackgroundImage = Properties.Resources.Cowboy_Snowman_Cropped;
                
                updatePos();
            }

            public void updatePos()
            {
                pb.Location = pos.toPoint();
            }
        }

        private List<byte[]> meme = new List<byte[]>();
        private List<Enemy> enemys = new List<Enemy>();

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
                //for (int i = 0; i < Constants.memoryIncrease; i++) // for non paging only
                //    tempMeme[i] = 0;

                meme.Add(tempMeme);

                Thread.Sleep(Constants.memoryIncreaseRate);
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void memoryLeakButton_Click(object sender, EventArgs e)
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

            Level1();
        }

        private void Level1()
        {
            Random rng = new Random();
            for (int i = 0; i < 10; i++)
            { 
                enemys.Add(new Enemy(new Vector2i(rng.Next(0, 1000), rng.Next(0, 1000)), new System.Drawing.Size(100, 100)));
                gamePanel.Controls.Add(enemys[i].pb);
                enemys[i].pb.BringToFront();
            }
        }

        private void startGameButton_MouseEnter(object sender, EventArgs e)
        {
            System.Drawing.Point tempLocation = memoryLeakButton.Location;
            memoryLeakButton.Location = startGameButton.Location;
            startGameButton.Location = tempLocation;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Vector2i movement = new Vector2i((keyData == Keys.A || keyData == Keys.Left) ? Constants.playerSpeed : ((keyData == Keys.D || keyData == Keys.Right) ? -Constants.playerSpeed : 0), (keyData == Keys.W || keyData == Keys.Up) ? Constants.playerSpeed : ((keyData == Keys.S || keyData == Keys.Down) ? -Constants.playerSpeed : 0));
            
            Vector2i floorPos = new Vector2i(floorPictureBox.Location);
            floorPos += movement;
            floorPictureBox.Location = floorPos.toPoint();

            for (int i = 0; i < enemys.Count; i++)
            {
                enemys[i].pos += movement;
                enemys[i].updatePos();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
