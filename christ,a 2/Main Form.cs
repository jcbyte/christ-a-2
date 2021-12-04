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

            public const float playerSpeed = 0.005f;

            public static readonly Vector2 playerSize = new Vector2(0.02f, 0.06f);
            public static readonly Vector2 enemySize = new Vector2(0.02f, 0.06f);
        }

        public class Vector2
        {
            public float x;
            public float y;

            public Vector2(float x_, float y_)
            {
                x = x_;
                y = y_;
            }

            static public Vector2 operator +(Vector2 lhs, Vector2 rhs)
            {
                lhs.x += rhs.x;
                lhs.y += rhs.y;
                return lhs;
            }
        }

        private static Vector2 toRelativeV2(System.Drawing.Point p, System.Drawing.Size formSize)
        {
            return new Vector2((float)p.X / formSize.Width, (float)p.Y / formSize.Height);
        }

        private static System.Drawing.Point fromRelativeV2(Vector2 v, System.Drawing.Size formSize)
        {
            return new System.Drawing.Point((int)(v.x * formSize.Width), (int)(v.y * formSize.Height));
        }

        private static System.Drawing.Size systemPointToSystemSize(System.Drawing.Point p)
        {
            return new System.Drawing.Size(p.X, p.Y);
        }

        private class Enemy
        {
            public Vector2 pos;
            public PictureBox pb;

            public Enemy(Vector2 pos_, System.Drawing.Size size)
            {
                pos = pos_;

                pb = new PictureBox();
                pb.BackgroundImageLayout = ImageLayout.Stretch;
                pb.Size = size;
                pb.BackgroundImage = Properties.Resources.Cowboy_Snowman_Cropped;
            }

            public void updatePos(System.Drawing.Size formSize)
            {
                pb.Location = fromRelativeV2(pos, formSize);
            }
        }

        private List<byte[]> meme = new List<byte[]>();
        private List<Enemy> enemys = new List<Enemy>();
        private Vector2 playerPos = new Vector2(0.5f, 0.5f);

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

        private void startGameButton_MouseEnter(object sender, EventArgs e)
        {
            System.Drawing.Point tempLocation = memoryLeakButton.Location;
            memoryLeakButton.Location = startGameButton.Location;
            startGameButton.Location = tempLocation;
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

            loadLevel(1);
        }

        private void loadLevel(int level)
        {
            System.Drawing.Bitmap floorImg;
            switch (level)
            {
                case 1: floorImg = Properties.Resources.floor; break;
                default: floorImg = new System.Drawing.Bitmap(1, 1); break;
            }
            floorPictureBox.BackgroundImage = floorImg;

            floorPictureBox.Size = this.Size;
            floorPictureBox.Location = new System.Drawing.Point(0, 0);

            playerPos = new Vector2(0.5f, 0.5f);
            playerPictureBox.Location = fromRelativeV2(playerPos, this.Size);
            playerPictureBox.Size = systemPointToSystemSize(fromRelativeV2(Constants.playerSize, this.Size));

            Random rng = new Random(); // lvevl specfic
            for (int i = 0; i < 10; i++)
            {
                enemys.Add(new Enemy(new Vector2((float)rng.NextDouble(), (float)rng.NextDouble()), systemPointToSystemSize(fromRelativeV2(Constants.enemySize, this.Size))));
                gamePanel.Controls.Add(enemys[i].pb);
                enemys[i].updatePos(this.Size);
                enemys[i].pb.BringToFront();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Vector2 movement = new Vector2((keyData == Keys.A || keyData == Keys.Left) ? -Constants.playerSpeed : ((keyData == Keys.D || keyData == Keys.Right) ? Constants.playerSpeed : 0), (keyData == Keys.W || keyData == Keys.Up) ? -Constants.playerSpeed : ((keyData == Keys.S || keyData == Keys.Down) ? Constants.playerSpeed : 0));

            playerPos += movement;
            playerPictureBox.Location = fromRelativeV2(playerPos, this.Size);

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
