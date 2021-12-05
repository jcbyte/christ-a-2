using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace christ_a_2
{
    public partial class mainForm : Form
    {
        public static class Constants // Global game settings
        {
            public const int memoryCounterRefresh = 50;

            public const int memoryIncreaseRate = 50;
            public const int memoryIncrease = 1024 * 1024 * 64;

            public const float playerSpeed = 0.005f;

            public static readonly Vector2 playerSize = new Vector2(0.02f, 0.06f);
            public static readonly Vector2 enemySize = new Vector2(0.02f, 0.06f);
        }

        public class Vector2 // Relative Vector2 class (0-1, 0-1)
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

        private static Vector2 toRelativeV2(System.Drawing.Point p, System.Drawing.Size formSize) // Convert from a Windows Point to relative Vector2
        {
            return new Vector2((float)p.X / formSize.Width, (float)p.Y / formSize.Height);
        }

        private static System.Drawing.Point fromRelativeV2(Vector2 v, System.Drawing.Size formSize) // Convert from a relative Vector2 to a Windows Point
        {
            return new System.Drawing.Point((int)(v.x * formSize.Width), (int)(v.y * formSize.Height));
        }

        private static System.Drawing.Size systemPointToSystemSize(System.Drawing.Point p) // Change Windows Point to Windows Scale 
        {
            return new System.Drawing.Size(p.X, p.Y);
        }

        private class Enemy // Enemy class containg visual and code objects
        {
            public Vector2 pos;
            public PictureBox pb;

            public Enemy(Vector2 pos_, System.Drawing.Size size)
            {
                pos = pos_;

                pb = new PictureBox(); // Add new picturebox (enemy) to the form
                pb.BackgroundImageLayout = ImageLayout.Stretch;
                pb.Size = size;
                pb.BackgroundImage = Properties.Resources.Cowboy_Snowman_Cropped;
            }

            public void updatePos(System.Drawing.Size formSize)
            {
                pb.Location = fromRelativeV2(pos, formSize);
            }
        }

        private List<byte[]> meme = new List<byte[]>(); // Totally useful memory
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

        private void mainForm_HandleCreated(object sender, EventArgs e) // To start memory counter after the Window has been initialised
        {
            updateMemoryCounterLoopAsync();
        }

        private async void updateMemoryCounterLoopAsync()
        {
            while (true)
            {
                float memUsed = (float)System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024 * 1024);
                //float memUsed = (float)GC.GetTotalMemory(true) / (1024 * 1024 * 1024);
                memoryUsedLabel.Text = "Memory used: " + memUsed.ToString("0.00") + " GB";

                await Task.Delay(Constants.memoryCounterRefresh);
            }
        }

        private async void increaseMemoryLoopAsync()
        {
            while (true)
            {
                byte[] tempMeme = new byte[Constants.memoryIncrease];
                //for (int i = 0; i < Constants.memoryIncrease; i++) // for non paging only
                //    tempMeme[i] = 0;

                meme.Add(tempMeme);

                await Task.Delay(Constants.memoryIncreaseRate);
            }
        }

        private void startGameButton_MouseEnter(object sender, EventArgs e) // Swap start button with memory leak button ;)
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
            /* */cutscenePanel.Visible = true;
            cutsceneMediaPlayer.URL = "D:\\Users\\joel_\\Downloads\\cutscene.mp4"; // Start the cutscene

            await Task.Delay(2000); // Wait until memory leak starts

            increaseMemoryLoopAsync(); // Start memory leak

            await Task.Delay(2000); // Waint until game starts

            cutsceneMediaPlayer.URL = ""; // Stop the cutscene
            cutscenePanel.Visible = false;//*/
            gamePanel.Visible = true;

            loadLevel(1);
        }

        private void loadLevel(int level)
        {
            System.Drawing.Bitmap floorImg;
            int enemyAmount;

            switch (level) // Change floor and enemys depending on level
            {
                case 1: 
                    floorImg = Properties.Resources.floor;
                    enemyAmount = 10;
                    break;
                default: 
                    floorImg = new System.Drawing.Bitmap(1, 1);
                    enemyAmount = 0;
                    break;
            }

            floorPictureBox.BackgroundImage = floorImg;
            floorPictureBox.Size = this.Size; // Make sure floor fills screen
            floorPictureBox.Location = new System.Drawing.Point(0, 0);
            floorPictureBox.SendToBack();

            playerPos = new Vector2(0.5f, 0.5f);
            playerPictureBox.Location = fromRelativeV2(playerPos, this.Size); // Have player start in centre // Could change depending on level?
            playerPictureBox.Size = systemPointToSystemSize(fromRelativeV2(Constants.playerSize, this.Size));

            Random rng = new Random();
            for (int i = 0; i < enemyAmount; i++) // Create the enemys in random locations // Could have specfic locations later?
            {
                enemys.Add(new Enemy(new Vector2((float)rng.NextDouble(), (float)rng.NextDouble()), systemPointToSystemSize(fromRelativeV2(Constants.enemySize, this.Size))));
                gamePanel.Controls.Add(enemys[i].pb);
                enemys[i].updatePos(this.Size);
                enemys[i].pb.BringToFront();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // Get when a key is pressed anytime in the program
        {
            Vector2 movement = new Vector2((keyData == Keys.A || keyData == Keys.Left) ? -Constants.playerSpeed : ((keyData == Keys.D || keyData == Keys.Right) ? Constants.playerSpeed : 0), (keyData == Keys.W || keyData == Keys.Up) ? -Constants.playerSpeed : ((keyData == Keys.S || keyData == Keys.Down) ? Constants.playerSpeed : 0));

            playerPos += movement; // Move the player respectively
            playerPictureBox.Location = fromRelativeV2(playerPos, this.Size);

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
