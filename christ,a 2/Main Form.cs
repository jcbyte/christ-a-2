using System;
using System.Collections.Generic;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Drawing;

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

            public const float playerSpeed = 0.15f;

            public static readonly Vector2 playerSize = new Vector2(0.02f, 0.06f); // Sizes are relative
            public static readonly Vector2 enemySize = new Vector2(0.02f, 0.06f);
        }

        #endregion

        #region "Relative Vector2"

        public class Vector2 // Relative Vector2 class (0-1, 0-1)
        {
            public float x;
            public float y;

            public Vector2()
            {
                x = 0;
                y = 0;
            }

            public Vector2(float _x, float _y)
            {
                x = _x;
                y = _y;
            }

            public float Magnitude()
            {
                return (float)Math.Sqrt(Math.Pow(x,2) + Math.Pow(y, 2));
            }

            public Vector2 Normalise()
            {
                float mag = this.Magnitude();
                return new Vector2(x / mag, y / mag);
            }

            static public Vector2 operator +(Vector2 lhs, Vector2 rhs)
            {
                return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y);
            }

            static public Vector2 operator -(Vector2 lhs, Vector2 rhs)
            {
                return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
            }

            static public Vector2 operator *(Vector2 lhs, float rhs)
            {
                return new Vector2(lhs.x * rhs, lhs.y * rhs);
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
            Win,
        }

        #endregion

        #region "Levels"

        private enum Levels : byte
        {
            Level1,
            Level2,
            Level3,
            BossLevel,
        }

        private struct levelOb
        {
            public System.Drawing.Bitmap floorImg;
            public int enemyAmount;

            public levelOb(System.Drawing.Bitmap _floorImg, int _enemyAmount)
            {
                floorImg = _floorImg;
                enemyAmount = _enemyAmount;
            }
        }

        #endregion

        #region "Enemy (Needs work, multiple)"

        private class Enemy // Enemy class containg visual and code objects
        {
            public Vector2 pos;
            public PictureBox pb;

            public Enemy(Vector2 _pos, System.Drawing.Size size)  // probably dont want size here
            {
                pos = _pos;

                pb = new PictureBox(); // Add new picturebox (enemy) to the form
                pb.BackgroundImageLayout = ImageLayout.Stretch;
                pb.Size = size;
                pb.BackColor = Color.Transparent;
                pb.BackgroundImage = Properties.Resources.snowmanEnemy; // different enemys?
            }

            public void UpdatePos(System.Drawing.Size formSize)
            {
                pb.Location = FromRelativeV2(pos, formSize);
            }
        }

        #endregion

        #region "Weapons"

        private enum Weapons : byte
        {
            Glock19,
            FiveSeven,
            DesertEagle,

            Galil,
            AMD65,
            AEK971,
            AK47,

            M107,
            L115A3,
            SCAR,

            UMP,
            MAC10,
            Uzi,

            M249,

            M2,

            FP6,
            M1014,

            MGL105,

            RPG7,
        }

        private enum WeaponType : byte
        {
            Semi,
            Auto,
        }

        private enum WeaponClass : byte
        {
            Pistol,
            AR,
            Marksman,
            SMG,
            LMG,
            HMG,
            Shotgun,
            GrenadeLauncher,
            RPG,
        }

        private struct WeaponClassOb
        {
            public string name;
            public WeaponType type;

            public WeaponClassOb(string _name, WeaponType _type)
            {
                name = _name;
                type = _type;
            }
        }

        private struct WeaponOb
        {
            public string name;
            public WeaponClass type;
            public string country;

            public float damage;
            public float weight;
            public int velocity;
            public int firerate;
            public int magCapacity;
            public int maxAmmo;
            public float accuracy;
            public float recoil;

            public int shotgunShots; // Shotgun only
            public float maxGrenadeDistance; // Grendade only
            public float explosionRadius; // Grenade and RPG only

            public WeaponOb(string _name, WeaponClass _type, string _country, float _damage, float _weight, int _velocity, int _firerate, int _magCapacity, int _maxAmmo, float _accuracy, float _recoil, int _shotgunShots = 0, float _maxGrenadeDistance = 0, float _explosionRadius = 0)
            {
                name = _name;
                type = _type;
                country = _country;
                damage = _damage;
                weight = _weight;
                velocity = _velocity;
                firerate = _firerate;
                magCapacity = _magCapacity;
                maxAmmo = _maxAmmo;
                accuracy = _accuracy;
                recoil = _recoil;

                shotgunShots = _shotgunShots;
                maxGrenadeDistance = _maxGrenadeDistance;
                explosionRadius = _explosionRadius;
            }
        }

        #endregion

        #region "Bullet (Needs work)"

        class Bullet
        {
            public Vector2 pos;
            public Vector2 dir;
            public float speed;
            public PictureBox pb;

            public Bullet(Vector2 _pos, Vector2 _dir, float _speed, System.Drawing.Size size) // probably dont want size here
            {
                pos = _pos;
                dir = _dir;
                speed = _speed;

                pb = new PictureBox(); // Add new picturebox (bullet) to the form
                pb.BackgroundImageLayout = ImageLayout.Stretch;
                pb.Size = size;
                pb.BackColor = Color.Transparent;
                pb.BackgroundImage = Properties.Resources.playerBullet;
            }

            public void UpdatePos(System.Drawing.Size formSize)
            {
                pos += dir * speed;
                pb.Location = FromRelativeV2(pos, formSize);
            }
        }

        #endregion

        private List<byte[]> meme = new List<byte[]>(); // Totally useful memory

        private List<Enemy> enemys = new List<Enemy>();
        private List<Bullet> bullets = new List<Bullet>();

        private Vector2 playerPos = new Vector2(0.5f, 0.5f);

        private SoundPlayer backgroundMusicPlayer = new SoundPlayer();
        private SoundPlayer soundEffectsPlayer = new SoundPlayer();

        private Dictionary<Scenes, SceneOb> scenesData;
        private Dictionary<Cutscenes, string> cutscenesData;
        private Dictionary<Levels, levelOb> levelsData;
        private Dictionary<WeaponClass, WeaponClassOb> weaponClassData;
        private Dictionary<Weapons, WeaponOb> weaponsData;

        private Scenes cScene = Scenes.Menu;
        private Levels cLevel = Levels.Level1;

        public mainForm()
        {
            InitializeComponent();

            scenesData = new Dictionary<Scenes, SceneOb> { 
                {Scenes.Menu, new SceneOb(main_menu_panel) }, 
                {Scenes.Cutscene, new SceneOb(main_cutscene_panel, CutsceneOnload) }, 
                {Scenes.Game, new SceneOb(main_game_panel, GameOnLoad) }, 
            };

            cutscenesData = new Dictionary<Cutscenes, string>
            {
                {Cutscenes.OpeningCredits, "FullResources\\Cutscenes\\openingCredits.mp4" },
                {Cutscenes.BeforeGame,     "D:\\Users\\joel_\\Downloads\\cutscenes\\cutscene.mp4" },
                {Cutscenes.BeforeBoss,     "" },
                {Cutscenes.Loss,           "FullResources\\Cutscenes\\youDied.mp4" },
                {Cutscenes.Win,            "" }
            };

            levelsData = new Dictionary<Levels, levelOb> {
                {Levels.Level1,    new levelOb(Properties.Resources.level1FloorFactory, 10) },
                {Levels.Level2,    new levelOb(Properties.Resources.level1FloorFactory, 20) },
                {Levels.Level3,    new levelOb(Properties.Resources.level1FloorFactory, 30) },
                {Levels.BossLevel, new levelOb(Properties.Resources.level1FloorFactory, 50) }
            };

            weaponClassData = new Dictionary<WeaponClass, WeaponClassOb> {
                {WeaponClass.Pistol,          new WeaponClassOb("Pistol",            WeaponType.Semi) },
                {WeaponClass.AR,              new WeaponClassOb("Assult Rifle",      WeaponType.Auto) },
                {WeaponClass.Marksman,        new WeaponClassOb("Marksman Rifle",    WeaponType.Semi) },
                {WeaponClass.SMG,             new WeaponClassOb("Sub-Machine Gun",   WeaponType.Auto) },
                {WeaponClass.LMG,             new WeaponClassOb("Light-Machine Gun", WeaponType.Auto) },
                {WeaponClass.HMG,             new WeaponClassOb("Heavy-Machine Gun", WeaponType.Auto) },
                {WeaponClass.Shotgun,         new WeaponClassOb("Shotgun",           WeaponType.Semi) },
                {WeaponClass.GrenadeLauncher, new WeaponClassOb("Grenade Launcher",  WeaponType.Semi) },
                {WeaponClass.RPG,             new WeaponClassOb("RPG",               WeaponType.Semi) },
            };

            weaponsData = new Dictionary<Weapons, WeaponOb> {
                {Weapons.Glock19,     new WeaponOb("Glock-19", WeaponClass.Pistol, "Austria", 10, 0.67f, 100, 120, 15, 45, 0.1f, 0.1f) },
                {Weapons.FiveSeven,   new WeaponOb("Five SeveN", WeaponClass.Pistol, "Belgium", 20, 0.62f, 650, 180, 20, 60, 0.08f, 0.1f) },
                {Weapons.DesertEagle, new WeaponOb("Desert Eagle", WeaponClass.Pistol, "USA", 50, 2.00f, 470, 60, 7, 14, 0.02f, 0.2f) },
                {Weapons.Galil,       new WeaponOb("Galil", WeaponClass.AR, "Israel", 20, 3.95f, 950, 80, 35, 210, 0.15f, 0.1f) },

                // http://www.military-today.com/firearms.htm
                {Weapons.AMD65,       new WeaponOb("AMD-65", WeaponClass.AR, "Hungary", ) },

                {Weapons.AEK971,      new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.AK47,        new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.M107,        new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.L115A3,      new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.SCAR,        new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.UMP,         new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.MAC10,       new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.Uzi,         new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.M249,        new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.M2,          new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.FP6,         new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.M1014,       new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.MGL105,      new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
                {Weapons.RPG7,        new WeaponOb("", WeaponClass.Pistol, "Austria", ) },
            };

            foreach (KeyValuePair<Scenes, SceneOb> s in scenesData)
                s.Value.panel.Visible = false;
            //LoadScene(Scenes.Cutscene, Cutscenes.OpeningCredits);
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

        private async void CutsceneOnload(object data)
        {
            Cutscenes cutscene = (Cutscenes)data;

            cutscene_media_windowsMediaPlayer.URL = cutscenesData[cutscene];
            cutscene_media_windowsMediaPlayer.uiMode = "none";
            
            switch(cutscene)
            {
                case Cutscenes.OpeningCredits:

                    backgroundMusicPlayer.SoundLocation = "FullResources\\Music\\menu.wav";
                    backgroundMusicPlayer.Load();
                    backgroundMusicPlayer.Play();

                    await Task.Delay(12000);

                    LoadScene(Scenes.Menu);
                    break;

                case Cutscenes.BeforeGame:

                    backgroundMusicPlayer.Stop();

                    /* * /await Task.Delay(2000); // Wait until memory leak starts
                    IncreaseMemoryLoopAsync(); // Start the memory leak
                    await Task.Delay(2000); // Wait until level1 starts */

                    LoadScene(Scenes.Game, Levels.Level1);
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

        private void GameOnLoad(object data)
        {
            Levels level = (Levels)data;
            cLevel = level;

            game_floor_pictureBox.BackgroundImage = levelsData[level].floorImg; // Set image of floor over drawing for lag purposes
            game_floor_pictureBox.Location = new Point(0, 0);
            game_floor_pictureBox.Size = this.Size;

            playerPos = new Vector2(0.5f, 0.5f);
            playerPictureBox.Location = FromRelativeV2(playerPos, this.Size); // Have player start in centre // Could change depending on level?
            playerPictureBox.Size = SystemPointToSystemSize(FromRelativeV2(Constants.playerSize, this.Size));

            Random rng = new Random(2);
            for (int i = 0; i < levelsData[level].enemyAmount; i++) // Create the enemys in random locations // Could have specfic locations later?
            {
                enemys.Add(new Enemy(new Vector2((float)rng.NextDouble(), (float)rng.NextDouble()), SystemPointToSystemSize(FromRelativeV2(Constants.enemySize, this.Size)))); // Instatiate enemys
                main_game_panel.Controls.Add(enemys[i].pb);
                enemys[i].UpdatePos(this.Size);
                enemys[i].pb.BringToFront();
            }

            switch(level) // Level specialitys
            {
                case Levels.BossLevel: 
                break;
            }
        }

        private async void GameLoopAsync()
        {
            int delay = (int)((1.0f / Constants.gameLoopFPS) * 1000);
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            float startElapsed;
            float delta = 0;

            float lastShot = 0;

            while (true)
            {
                startElapsed = sw.ElapsedMilliseconds;

                if (cScene == Scenes.Game)
                {
                    Vector2 movement = new Vector2();

                    if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up)) // Get movement varible depending on keypress
                        movement.y -= Constants.playerSpeed;
                    if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
                        movement.y += Constants.playerSpeed;
                    if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
                        movement.x -= Constants.playerSpeed;
                    if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
                        movement.x += Constants.playerSpeed;

                    playerPos += movement * delta; // Player Movement
                    playerPictureBox.Location = FromRelativeV2(playerPos, main_game_panel.Size);

                    if (MouseButtons == MouseButtons.Left) // Player tries to shoot // (needs work)
                    {
                        if (lastShot < sw.ElapsedMilliseconds - 450) // firerate
                        {
                            Vector2 bulletdir = (ToRelativeV2(MousePosition, this.Size) - playerPos).Normalise();
                            bullets.Add(new Bullet(playerPos, bulletdir, 0.04f, SystemPointToSystemSize(FromRelativeV2(new Vector2(0.01f, 0.04f), this.Size)))); // Instatiate bullet

                            int bulleti = bullets.Count - 1;
                            main_game_panel.Controls.Add(bullets[bulleti].pb);
                            bullets[bulleti].pb.BringToFront();

                            lastShot = sw.ElapsedMilliseconds;
                        }
                    }

                    for (int i = 0; i < bullets.Count; i++)
                    {
                        bullets[i].UpdatePos(this.Size);
                    }
                }

                await Task.Delay(delay);
                delta = (sw.ElapsedMilliseconds - startElapsed) / 1000; // Calculate deltatime for frame
            }
        }

        private void main_game_panel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (cScene == Scenes.Game)
            {
                g.DrawImage(levelsData[cLevel].floorImg, 0, 0, this.Size.Width, this.Size.Height); // Draw image of floor to game panel for transparency lag purposes
            }
        }

        #endregion
    }
}
