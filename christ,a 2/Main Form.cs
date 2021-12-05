using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace christ_a_2
{
    public partial class mainForm : Form
    {
        #region "Constants"

        public static class Constants // Global game settings
        {
            public const int memoryCounterRefresh = 50;

            public const int memoryIncreaseRate = 50;
            public const int memoryIncrease = 1024 * 1024 * 64;

            public const int gameLoopFPS = 60;

            public const float playerSpeed = 0.005f;

            public static readonly Vector2 playerSize = new Vector2(0.02f, 0.06f); // Sizes are relative
            public static readonly Vector2 enemySize = new Vector2(0.02f, 0.06f);
        }

        #endregion

        #region "Relative Vector2"

        public class Vector2 // Relative Vector2 class (0-1, 0-1)
        {
            public float x;
            public float y;

            public Vector2(float _x, float _y)
            {
                x = _x;
                y = _y;
            }

            static public Vector2 operator +(Vector2 lhs, Vector2 rhs)
            {
                lhs.x += rhs.x;
                lhs.y += rhs.y;
                return lhs;
            }
        }

        private static Vector2 ToRelativeV2(System.Drawing.Point p, System.Drawing.Size formSize) // Convert from a Windows Point to relative Vector2
        {
            return new Vector2((float)p.X / formSize.Width, (float)p.Y / formSize.Height);
        }

        private static System.Drawing.Point FromRelativeV2(Vector2 v, System.Drawing.Size formSize) // Convert from a relative Vector2 to a Windows Point
        {
            return new System.Drawing.Point((int)(v.x * formSize.Width), (int)(v.y * formSize.Height));
        }

        private static System.Drawing.Size SystemPointToSystemSize(System.Drawing.Point p) // Change Windows Point to Windows Scale 
        {
            return new System.Drawing.Size(p.X, p.Y);
        }

        #endregion

        #region "Scenes"

        private enum Scenes : byte
        {
            Menu,
            Cutscene,
            Game,
        }

        private struct SceneOb
        {
            public Panel panel;
            public Action<object> onLoad;

            public SceneOb(Panel _panel, Action<object> _onLoad = null)
            {
                panel = _panel;
                onLoad = _onLoad;
            }
        }

        private void LoadScene(Scenes s, object data = null) // Close current scene and initialise new scene with optional data
        {
            scenesData[cScene].panel.Visible = false;
            cScene = s;
            scenesData[cScene].panel.Visible = true;
            if (scenesData[cScene].onLoad != null) scenesData[cScene].onLoad(data);
        }

        #endregion

        #region "Cutscenes"

        private enum Cutscenes : byte
        {
            OpeningCredits,
            BeforeGame,
            BeforeBoss,
            Loss,
            Win
        }

        #endregion

        #region "Enemy (needs work)"

        private class Enemy // Enemy class containg visual and code objects
        {
            public Vector2 pos;
            public PictureBox pb;

            public Enemy(Vector2 _pos, System.Drawing.Size size)
            {
                pos = _pos;

                pb = new PictureBox(); // Add new picturebox (enemy) to the form
                pb.BackgroundImageLayout = ImageLayout.Stretch;
                pb.Size = size;
                pb.BackgroundImage = Properties.Resources.Cowboy_Snowman_Cropped;
            }

            public void UpdatePos(System.Drawing.Size formSize)
            {
                pb.Location = FromRelativeV2(pos, formSize);
            }
        }

        #endregion

        private List<byte[]> meme = new List<byte[]>(); // Totally useful memory

        private List<Enemy> enemys = new List<Enemy>();
        private Vector2 playerPos = new Vector2(0.5f, 0.5f);

        private Dictionary<Scenes, SceneOb> scenesData;
        private Dictionary<Cutscenes, string> cutscenesData;
        private Scenes cScene = Scenes.Menu;

        public mainForm()
        {
            InitializeComponent();

            scenesData = new Dictionary<Scenes, SceneOb> { 
                {Scenes.Menu, new SceneOb(main_menu_panel) }, 
                {Scenes.Cutscene, new SceneOb(main_cutscene_panel, CutsceneOnload) }, 
                {Scenes.Game, new SceneOb(main_game_panel) }, 
            };

            cutscenesData = new Dictionary<Cutscenes, string>
            {
                {Cutscenes.OpeningCredits, "" },
                {Cutscenes.BeforeGame, "D:\\Users\\joel_\\Downloads\\cutscenes\\cutscene.mp4" },
                {Cutscenes.BeforeBoss, "" },
                {Cutscenes.Loss, "" },
                {Cutscenes.Win, "" }
            };

            foreach (KeyValuePair<Scenes, SceneOb> s in scenesData)
                s.Value.panel.Visible = false;
            LoadScene(Scenes.Menu);
            
            this.HandleCreated += mainForm_HandleCreated;
        }

        #region "Async"

        private void mainForm_HandleCreated(object sender, EventArgs e) // To start memory counter after the Window has been initialised
        {
            UpdateMemoryCounterLoopAsync();
            GameLoopAsync();
        }

        private async void UpdateMemoryCounterLoopAsync()
        {
            while (true)
            {
                float memUsed = (float)System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024 * 1024);
                //float memUsed = (float)GC.GetTotalMemory(true) / (1024 * 1024 * 1024);
                main_memoryCounter_label.Text = "Memory used: " + memUsed.ToString("0.00") + " GB";

                await Task.Delay(Constants.memoryCounterRefresh);
            }
        }

        private async void IncreaseMemoryLoopAsync()
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

        #endregion

        #region "Menu"

        private void menu_startGame_button_MouseEnter(object sender, EventArgs e) // Swap start button with memory leak button ;)
        {
            System.Drawing.Point tempLocation = menu_startMemoryLeak_button.Location;
            menu_startMemoryLeak_button.Location = menu_startGame_button.Location;
            menu_startGame_button.Location = tempLocation;
        }

        private void menu_exit_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menu_startMemoryLeak_button_Click(object sender, EventArgs e)
        {
            LoadScene(Scenes.Cutscene, Cutscenes.BeforeGame);
        }

        #endregion

        #region "Cutscene"

        private async void CutsceneOnload(object cs)
        {
            Cutscenes cutscene = (Cutscenes)cs;

            cutscene_media_windowsMediaPlayer.URL = cutscenesData[cutscene];
            
            switch(cutscene)
            {
                case Cutscenes.OpeningCredits:

                    LoadScene(Scenes.Menu);
                    break;

                case Cutscenes.BeforeGame:

                    await Task.Delay(2000); // Wait until memory leak starts
                    IncreaseMemoryLoopAsync(); // Start the memory leak
                    await Task.Delay(2000); // Wait until level1 starts

                    LoadScene(Scenes.Game);
                    break;

                case Cutscenes.BeforeBoss:
                    break;

                case Cutscenes.Loss:
                    break;

                case Cutscenes.Win:
                    break;
            }

            cutscene_media_windowsMediaPlayer.URL = "";
        }

        #endregion

        #region "Game (needs work)"

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
                main_game_panel.Controls.Add(enemys[i].pb);
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

        private async void gameLoopAsync()
        {
            int delay = (int)((1.0f / Constants.gameLoopFPS) * 1000);

            while (true)
            {
                if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.F))
                {
                    Console.WriteLine("loamo");
                }

                await Task.Delay(delay);
            }
        }

        #endregion
    }
}
