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

            public const float playerSpeed = 0.15f;
            public const int weaponSwitchCooldown = 100;

            public static readonly Vector2 playerSize = new Vector2(0.04f, 0.06f); // Scaled relative Vector2

            public const int enemyHealthBarThickness = 6; // (px)
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

            public Vector2(float a)
            {
                x = a;
                y = a;
            }

            public Vector2(float _x, float _y)
            {
                x = _x;
                y = _y;
            }

            public float Angle(bool deg = false)
            {
                float angle = (float)Math.Atan(y / x);
                if (!deg) return angle;
                else return angle * (180 / (float)Math.PI);
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

        private static Vector2 ToRelativeV2(Point p, Size formSize) // Convert from a Windows Point to relative Vector2
        {
            return new Vector2((float)p.X / formSize.Width, (float)p.Y / formSize.Height);
        }

        private static Point FromRelativeV2(Vector2 v, Size formSize) // Convert from a relative Vector2 to a Windows Point
        {
            return new Point((int)(v.x * formSize.Width), (int)(v.y * formSize.Height));
        }

        private static Point FromRelativeV2Center(Vector2 v, Size obSize, Size formSize)
        {
            return new Point((int)(v.x * formSize.Width) - (obSize.Width / 2), (int)(v.y * formSize.Height) - (obSize.Height / 2));
        }

        private static Size SystemPointToSystemSize(Point p) // Change Windows Point to Windows Scale 
        {
            return new Size(p.X, p.Y);
        }

        private static Vector2 FromScaledRelativeV2ToRealtiveV2(Vector2 c, Size formSize) // A scaled relative Vector2 so that the proportions are 1:1, used for scaling
        {
            if (formSize.Width > formSize.Height)
                return new Vector2(c.x * ((float)formSize.Height / formSize.Width), c.y);
            else
                return new Vector2(c.x, c.y * ((float)formSize.Width / formSize.Height));
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
            public Image floorImg;
            public Dictionary<Enemys, int> enemyAmount;

            public levelOb(Image _floorImg, Dictionary<Enemys, int> _enemyAmount)
            {
                floorImg = _floorImg;
                enemyAmount = _enemyAmount;
            }
        }

        #endregion

        #region "Enemy"

        private class Enemy // Enemy class containg visual and code objects
        {
            public PictureBox pb;

            public Panel healthPanel;
            public PictureBox healthPb;

            public Vector2 pos;
            public Weapons weapon;
            public int health;
            public int maxHealth;
            public float speed;

            public Enemy(Vector2 _pos, Image img, Size size, Weapons _weapon, int _health, float _speed) 
            {
                pb = new PictureBox(); // Add new picturebox (enemy) to the form
                pb.BackgroundImageLayout = ImageLayout.Stretch;
                pb.Size = size;
                pb.BackColor = Color.Transparent;
                pb.BackgroundImage = img;

                healthPanel = new Panel();
                healthPanel.Size = new Size(pb.Size.Width, Constants.enemyHealthBarThickness);
                healthPanel.Location = new Point(pb.Location.X, pb.Location.Y - Constants.enemyHealthBarThickness - 2);

                healthPb = new PictureBox();
                Bitmap healthImg = new Bitmap(1, 1);
                healthImg.SetPixel(0, 0, Color.FromArgb(255, 0, 0));
                healthPb.BackgroundImageLayout = ImageLayout.Stretch;
                healthPb.BackgroundImage = healthImg;
                healthPanel.Controls.Add(healthPb);
                healthPb.Location = new Point(0, 0);
                healthPb.BringToFront();

                pos = _pos;
                weapon = _weapon;
                health = _health;
                maxHealth = health;
                speed = _speed;
            }

            public void UpdatePos(Size formSize)
            {
                pb.Location = FromRelativeV2Center(pos, pb.Size, formSize);
                healthPanel.Location = new Point(pb.Location.X, pb.Location.Y - Constants.enemyHealthBarThickness);
            }

            public void UpdateHealth()
            {
                healthPb.Size = SystemPointToSystemSize(FromRelativeV2(new Vector2((float)health / maxHealth, 1), healthPanel.Size));
            }
        }

        private enum Enemys : byte
        {
            Regular,
            Tank,
            Scout,
            Sniper,
            Rowland,
            Boss,
        }

        private struct EnemyOb
        {
            public Image img;
            public Vector2 size; // Relative scaled vector2
            public int health;
            public float speed;
            public Weapons weapon;

            public EnemyOb(Image _img, Vector2 _size, int _health, float _speed, Weapons _weapon)
            {
                img = _img;
                size = _size;
                health = _health;
                speed = _speed;
                weapon = _weapon;
            }
        }

        #endregion

        #region "Weapons"

        private enum Weapons : byte
        {
            None,

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
            public Image img;
            public Image bulletImg;
            public float bulletSize; // Scaled relative
            public string country;

            public int damage;
            public float weight;
            public float velocity;
            public float firerate; // rate of fire (rps)
            public int reload; // Time to reload (ms)
            public int magCapacity;
            public int maxAmmoMultiplier; // Amount of extra mags (maxAmmo = magCapacity * maxAmmoMultiplier)
            public float accuracy; // Relative value for amount of deviation between bullet target and mouse
            public float recoil; // Relative value for amount of movement in mouse when shot

            public float pushBack; // Relative value for how much player is pushed back when shoting - Only for large weapons
            public int shotgunShots; // Shotgun only
            public float maxGrenadeDistance; // Relative - Grendade only
            public float explosionRadius; // Relative - Grenade and RPG only

            public WeaponOb(string _name, WeaponClass _type, Image _img, Image _bulletImg, float _bulletSize, string _country, int _damage, float _weight, float _velocity, float _firerate, int _reload, int _magCapacity, int _maxAmmoMultiplier, float _accuracy, float _recoil, float _pushBack = 0, int _shotgunShots = 0, float _maxGrenadeDistance = 0, float _explosionRadius = 0)
            {
                name = _name;
                type = _type;
                img = _img;
                bulletImg = _bulletImg;
                bulletSize = _bulletSize;
                country = _country;

                damage = _damage;
                weight = _weight;
                velocity = _velocity;
                firerate = _firerate;
                reload = _reload;
                magCapacity = _magCapacity;
                maxAmmoMultiplier = _maxAmmoMultiplier;
                accuracy = _accuracy;
                recoil = _recoil;

                pushBack = _pushBack;
                shotgunShots = _shotgunShots;
                maxGrenadeDistance = _maxGrenadeDistance;
                explosionRadius = _explosionRadius;
            }
        }

        #endregion

        #region "Bullet"

        class Bullet
        {
            public Vector2 pos;
            public Vector2 dir;
            public float speed;
            public int damage;
            public PictureBox pb;

            public Bullet(Vector2 _pos, Vector2 _dir, bool flip, float _speed, int _damage, Image img, Size size)
            {
                pos = _pos;
                dir = _dir;
                speed = _speed;
                damage = _damage;

                Bitmap bmp = new Bitmap(img.Width, img.Height); // Rotate bullet
                Graphics gfx = Graphics.FromImage(bmp);
                gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
                gfx.RotateTransform(dir.Angle(true));
                gfx.ScaleTransform((flip ? -1 : 1), 1);
                gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
                gfx.DrawImage(img, new Point(0, 0));
                gfx.Dispose();

                pb = new PictureBox(); // Add new picturebox (bullet) to the form
                pb.BackgroundImageLayout = ImageLayout.Stretch;
                pb.Size = size;
                pb.BackColor = Color.Transparent;
                pb.BackgroundImage = bmp;
            }

            public void UpdatePos(float delta, Size formSize)
            {
                pos += dir * speed * delta;
                pb.Location = FromRelativeV2Center(pos, pb.Size, formSize);
            }
        }

        #endregion

        #region "Inventory"

        private struct inventoryOb
        {
            public Weapons weapon;
            public int magBullets;
            public int reserveBullets;

            public inventoryOb(Weapons _weapon = Weapons.None, int _magBullets = 0, int _reserveBullets = 0)
            {
                weapon = _weapon;
                magBullets = _magBullets;
                reserveBullets = _reserveBullets;
            }
        }

        #endregion

        #region "Global vars"

        private List<byte[]> meme = new List<byte[]>(); // Totally useful memory
        private Random rng = new Random();

        private List<Enemy> enemys = new List<Enemy>();
        private List<Bullet> bullets = new List<Bullet>();

        private Vector2 playerPos = new Vector2(0.5f, 0.5f);
        private inventoryOb[] inventory = new inventoryOb[3] { new inventoryOb(Weapons.Glock19, 14, 30), new inventoryOb(Weapons.AK47, 1000, 0), new inventoryOb(Weapons.RPG7, 1, 2) };

        private SoundPlayer backgroundMusicPlayer = new SoundPlayer();
        private SoundPlayer soundEffectsPlayer = new SoundPlayer();

        private Dictionary<Scenes, SceneOb> scenesData;
        private Dictionary<Cutscenes, string> cutscenesData;
        private Dictionary<Levels, levelOb> levelsData;
        private Dictionary<WeaponClass, WeaponClassOb> weaponClassData;
        private Dictionary<Weapons, WeaponOb> weaponsData;
        private Dictionary<Enemys, EnemyOb> enemysData;

        private Scenes cScene = Scenes.Menu;
        private Levels cLevel = Levels.Level1;

        #endregion

        public mainForm()
        {
            InitializeComponent();

            #region "Data"

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
                {Levels.Level1,    new levelOb(Properties.Resources.level_1Factory, new Dictionary<Enemys, int> { { Enemys.Regular, 6 }, { Enemys.Scout, 2 }, { Enemys.Rowland, 2 } }) },
                {Levels.Level2,    new levelOb(Properties.Resources.level_1Factory, new Dictionary<Enemys, int> { }) },
                {Levels.Level3,    new levelOb(Properties.Resources.level_1Factory, new Dictionary<Enemys, int> { }) },
                {Levels.BossLevel, new levelOb(Properties.Resources.level_1Factory, new Dictionary<Enemys, int> { }) }
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

            // http://www.military-today.com/firearms.htm
            weaponsData = new Dictionary<Weapons, WeaponOb> {
                {Weapons.None, new WeaponOb("None", WeaponClass.Pistol, Properties.Resources.weapon_none, Properties.Resources.bullet_other, 0, "None", 0, 0, 0, 0, 0, 0, 0, 0, 0) },
            //  Weapon,                            Name,           Type,                        Img,                                   BulletImg,                           BulletSize, Country,          Damage, Weight, Velocity, Firerate, Reload, MagCapacity, MaxAmmoMultiplier, Accuracy, Recoil, PushBack, ShotgunShots, maxGrenadeDistance, ExplosionRadius
                {Weapons.Glock19,     new WeaponOb("Glock-19",     WeaponClass.Pistol,          Properties.Resources.weapon_glock19,   Properties.Resources.bullet_pistol,  0.01f,      "Austria",        0,      1.00f,  0.20f,    0.50f,    800,    15,          3,                 0.00f,    0.00f) },
                {Weapons.FiveSeven,   new WeaponOb("Five SeveN",   WeaponClass.Pistol,          Properties.Resources.weapon_fiveseven, Properties.Resources.bullet_pistol,  0.01f,      "Belgium",        0,      1.00f,  0.50f,    1.00f,    500,    20,          3,                 0.00f,    0.00f) },
                {Weapons.DesertEagle, new WeaponOb("Desert Eagle", WeaponClass.Pistol,          Properties.Resources.weapon_deagle,    Properties.Resources.bullet_pistol,  0.01f,      "USA",            0,      1.00f,  0.50f,    1.00f,    500,    7,           2,                 0.00f,    0.00f) },
                {Weapons.Galil,       new WeaponOb("Galil",        WeaponClass.AR,              Properties.Resources.weapon_galil,     Properties.Resources.bullet_other,   0.01f,      "Israel",         0,      1.00f,  0.50f,    1.00f,    500,    35,          6,                 0.00f,    0.00f) },
                {Weapons.AMD65,       new WeaponOb("AMD-65",       WeaponClass.AR,              Properties.Resources.weapon_amd65,     Properties.Resources.bullet_other,   0.01f,      "Hungary",        0,      1.00f,  0.50f,    1.00f,    500,    30,          6,                 0.00f,    0.00f) },
                {Weapons.AEK971,      new WeaponOb("AEK-971",      WeaponClass.AR,              Properties.Resources.weapon_aek971,    Properties.Resources.bullet_other,   0.01f,      "Russia",         0,      1.00f,  0.50f,    1.00f,    500,    30,          6,                 0.00f,    0.00f) },
                {Weapons.AK47,        new WeaponOb("AK-47",        WeaponClass.AR,              Properties.Resources.weapon_ak47,      Properties.Resources.bullet_other,   0.02f,      "Russia",         0,      1.00f,  0.50f,    1.00f,    500,    30,          6,                 0.05f,    0.02f) },
                {Weapons.M107,        new WeaponOb("M107",         WeaponClass.Marksman,        Properties.Resources.weapon_m107,      Properties.Resources.bullet_other,   0.01f,      "USA",            0,      1.00f,  0.50f,    1.00f,    500,    1,           10,                0.00f,    0.00f) },
                {Weapons.L115A3,      new WeaponOb("L115A3",       WeaponClass.Marksman,        Properties.Resources.weapon_l115a3,    Properties.Resources.bullet_other,   0.01f,      "United Kingdom", 0,      1.00f,  0.50f,    1.00f,    500,    2,           5,                 0.00f,    0.00f) },
                {Weapons.SCAR,        new WeaponOb("SCAR SSR",     WeaponClass.Marksman,        Properties.Resources.weapon_scar,      Properties.Resources.bullet_other,   0.01f,      "Belgium",        0,      1.00f,  0.50f,    1.00f,    500,    10,          2,                 0.00f,    0.00f) },
                {Weapons.UMP,         new WeaponOb("UMP",          WeaponClass.SMG,             Properties.Resources.weapon_ump,       Properties.Resources.bullet_other,   0.01f,      "Germany",        0,      1.00f,  0.50f,    1.00f,    500,    25,          10,                0.00f,    0.00f) },
                {Weapons.MAC10,       new WeaponOb("MAC-10",       WeaponClass.SMG,             Properties.Resources.weapon_mac10,     Properties.Resources.bullet_other,   0.01f,      "USA",            0,      1.00f,  0.50f,    1.00f,    500,    30,          10,                0.00f,    0.00f) },
                {Weapons.Uzi,         new WeaponOb("Uzi",          WeaponClass.SMG,             Properties.Resources.weapon_uzi,       Properties.Resources.bullet_other,   0.01f,      "Israel",         0,      1.00f,  0.50f,    1.00f,    500,    20,          10,                0.00f,    0.00f) },
                {Weapons.M249,        new WeaponOb("M249",         WeaponClass.LMG,             Properties.Resources.weapon_m249,      Properties.Resources.bullet_other,   0.01f,      "USA",            0,      1.00f,  0.50f,    1.00f,    500,    30,          15,                0.00f,    0.00f) },
                {Weapons.M2,          new WeaponOb("M2",           WeaponClass.HMG,             Properties.Resources.weapon_m2,        Properties.Resources.bullet_other,   0.01f,      "USA",            0,      1.00f,  0.50f,    1.00f,    500,    100,         4,                 0.00f,    0.00f) },
                {Weapons.FP6,         new WeaponOb("FP6",          WeaponClass.Shotgun,         Properties.Resources.weapon_fp6,       Properties.Resources.bullet_shotgun, 0.01f,      "Germany",        0,      1.00f,  0.50f,    1.00f,    500,    6,           2,                 0.00f,    0.00f,  0.00f,    3) },
                {Weapons.M1014,       new WeaponOb("M1014",        WeaponClass.Shotgun,         Properties.Resources.weapon_m1014,     Properties.Resources.bullet_shotgun, 0.01f,      "Italy",          0,      1.00f,  0.50f,    1.00f,    500,    8,           3,                 0.00f,    0.00f,  0.00f,    3) },
                {Weapons.MGL105,      new WeaponOb("MGL-105",      WeaponClass.GrenadeLauncher, Properties.Resources.weapon_mgl105,    Properties.Resources.bullet_grenade, 0.01f,      "South Africa",   0,      1.00f,  0.50f,    1.00f,    500,    6,           1,                 0.00f,    0.00f,  0.00f,    0,            0.50f,              0.05f) },
                {Weapons.RPG7,        new WeaponOb("RPG-7",        WeaponClass.RPG,             Properties.Resources.weapon_rpg7,      Properties.Resources.bullet_rpg,     0.01f,      "Russia",         0,      10.00f, 0.50f,    1.00f,    500,    1,           4,                 0.00f,    0.00f,  0.00f,    0,            0.00f,              0.05f) },
            };

            enemysData = new Dictionary<Enemys, EnemyOb> {
            //   Enemy                       Img,                                Size,                      Health, Speed, Weapon
                {Enemys.Regular, new EnemyOb(Properties.Resources.enemy_regular, new Vector2(0.04f, 0.06f), 100,    0.15f, Weapons.Glock19 ) },
                {Enemys.Tank,    new EnemyOb(Properties.Resources.enemy_tank,    new Vector2(0.04f, 0.06f), 100,    0.15f, Weapons.Glock19 ) },
                {Enemys.Scout,   new EnemyOb(Properties.Resources.enemy_scout,   new Vector2(0.04f, 0.06f), 100,    0.15f, Weapons.Glock19 ) },
                {Enemys.Sniper,  new EnemyOb(Properties.Resources.enemy_sniper,  new Vector2(0.04f, 0.06f), 100,    0.15f, Weapons.Glock19 ) },
                {Enemys.Rowland, new EnemyOb(Properties.Resources.enemy_rowland, new Vector2(0.04f, 0.06f), 100,    0.15f, Weapons.Glock19 ) },
                {Enemys.Boss,    new EnemyOb(Properties.Resources.enemy_boss,    new Vector2(0.04f, 0.06f), 100,    0.15f, Weapons.Glock19 ) },
            };

            #endregion

            foreach (KeyValuePair<Scenes, SceneOb> s in scenesData)
                s.Value.panel.Visible = false;
            //LoadScene(Scenes.Cutscene, Cutscenes.OpeningCredits);
            LoadScene(Scenes.Menu);

            main_game_panel.Cursor = System.Windows.Forms.Cursors.Cross;

            this.HandleCreated += mainForm_HandleCreated;
            Application.Idle += GameLoop;
        }

        #region "Misc"

        Point getCenter(PictureBox pb)
        {
            return new Point(pb.Location.X - (pb.Size.Width / 2), pb.Location.Y - (pb.Height / 2));
        }

        float GetFloatRng(float min = 0, float max = 1)
        {
            return (float)rng.NextDouble() * (max - min) + min;
        }

        #endregion

        #region "Async"

        private void mainForm_HandleCreated(object sender, EventArgs e) // To start memory counter after the Window has been initialised
        {
            UpdateMemoryCounterLoopAsync();
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

            game_inventory_reloading_label.Visible = false;
            UpdateInventoryGraphics(); // Update inventory graphics - useful on first load

            playerPos = new Vector2(0.5f, 0.5f);
            game_player_pictureBox.Location = FromRelativeV2(playerPos, this.Size); // Have player start in centre // Could change depending on level?
            game_player_pictureBox.Size = SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(Constants.playerSize, this.Size), this.Size));
            game_player_pictureBox.BringToFront();

            foreach (KeyValuePair<Enemys, int> enemyType in levelsData[level].enemyAmount) // Create the enemys for the level
            {
                for (int i = 0; i < enemyType.Value; i++)
                {
                    enemys.Add(new Enemy( // Instatiate enemys
                        new Vector2(GetFloatRng(0.1f, 0.9f), GetFloatRng(0.1f, 0.9f)),
                        enemysData[enemyType.Key].img,
                        SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(enemysData[enemyType.Key].size, this.Size), this.Size)),
                        enemysData[enemyType.Key].weapon,
                        enemysData[enemyType.Key].health,
                        enemysData[enemyType.Key].speed
                    ));

                    int j = enemys.Count - 1;
                    main_game_panel.Controls.Add(enemys[j].pb);
                    enemys[j].UpdatePos(this.Size);
                    enemys[j].pb.BringToFront();
                    main_game_panel.Controls.Add(enemys[j].healthPanel);
                    enemys[j].UpdateHealth();
                    enemys[j].healthPanel.BringToFront();
                }
            }

            switch(level) // Level specialitys
            {
                case Levels.BossLevel: 
                break;
            }
        }

        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

        float startFrame = 0;
        float delta = 0;

        float lastShot = 0;
        bool lastClick = false;
        bool reloading = false;

        void GameLoop(object sender, EventArgs e)
        {
            if (cScene == Scenes.Game)
            {
                Vector2 movement = new Vector2();

                if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up)) // Get movement varible depending on keypress
                    movement.y -= 1;
                if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
                    movement.y += 1;
                if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
                    movement.x -= 1;
                if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
                    movement.x += 1;

                float speedWeightModifier = (float)1 / weaponsData[inventory[0].weapon].weight;
                playerPos += movement * Constants.playerSpeed * speedWeightModifier * delta; // Player Movement
                game_player_pictureBox.Location = FromRelativeV2Center(playerPos, game_player_pictureBox.Size, main_game_panel.Size);

                bool click = MouseButtons == MouseButtons.Left;
                if (click) // Player clicks to try and shoot
                {
                    if (lastRotate < sw.ElapsedMilliseconds - Constants.weaponSwitchCooldown) // Can only shoot after weapon switch cooldown after changing weapon
                    {
                        if (weaponClassData[weaponsData[inventory[0].weapon].type].type == WeaponType.Auto || (weaponClassData[weaponsData[inventory[0].weapon].type].type == WeaponType.Semi && !lastClick)) // If weapon is semi make sure its a new click
                        {
                            if (lastShot < sw.ElapsedMilliseconds - (1000 / weaponsData[inventory[0].weapon].firerate)) // Only shoot at firerate (1 / (rps / 1000))
                            {
                                if (inventory[0].magBullets > 0) // Make sure there are bullets in the mag
                                {
                                    if (!reloading) // Cant shoot while reloading
                                    {
                                        inventory[0].magBullets--; // Remove bullet from mag
                                        UpdateInventoryAmmo();

                                        float accuracyVal = weaponsData[inventory[0].weapon].accuracy / 2;
                                        Vector2 aimPos = ToRelativeV2(MousePosition, this.Size) + FromScaledRelativeV2ToRealtiveV2(new Vector2(GetFloatRng(-accuracyVal, accuracyVal), GetFloatRng(-accuracyVal, accuracyVal)), this.Size); // Affected by accuracy
                                        bullets.Add(new Bullet( // Instatiate bullet
                                            playerPos,
                                            (aimPos - playerPos).Normalise(),
                                            (playerPos.x > aimPos.x), // For weird negative angle flipping
                                            (float)weaponsData[inventory[0].weapon].velocity,
                                            weaponsData[inventory[0].weapon].damage,
                                            weaponsData[inventory[0].weapon].bulletImg,
                                            SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(new Vector2(weaponsData[inventory[0].weapon].bulletSize), this.Size), this.Size))
                                        ));
                                        int bulleti = bullets.Count - 1;
                                        main_game_panel.Controls.Add(bullets[bulleti].pb);
                                        bullets[bulleti].pb.BringToFront();

                                        float recoilVal = weaponsData[inventory[0].weapon].recoil / 2;
                                        Vector2 recoil = FromScaledRelativeV2ToRealtiveV2(new Vector2(GetFloatRng(-recoilVal, recoilVal), GetFloatRng(-recoilVal, recoilVal)), this.Size);
                                        Vector2 newMousePos = ToRelativeV2(System.Windows.Forms.Cursor.Position, this.Size) + recoil; // Change mouse position depending on recoil
                                        System.Windows.Forms.Cursor.Position = FromRelativeV2(newMousePos, this.Size);

                                        lastShot = sw.ElapsedMilliseconds;
                                    }
                                }
                            }
                        }
                    }
                }
                lastClick = click;

                if (Keyboard.IsKeyDown(Key.R)) // Reload
                {
                    if (!reloading)
                    {
                        ReloadWeapon();
                    }
                }

                for (int i = 0; i < bullets.Count; i++) // loop through every bullet
                {
                    bullets[i].UpdatePos(delta, this.Size); // Move bullet in direction
                    
                    // Check if enemy hit
                    
                    // destory bullet if off game area
                }

                for (int i = 0; i < enemys.Count; i++) //  Loop through every enemy
                {
                    // Enemy AI
                }
            }

            delta = (sw.ElapsedMilliseconds - startFrame) / 1000; // Calculate deltatime for frame
            startFrame = sw.ElapsedMilliseconds;
        }

        private async void ReloadWeapon()
        {
            if (inventory[0].reserveBullets > 0) // If there are bullets in reserve
            {
                reloading = true;
                game_inventory_reloading_label.Visible = true;

                await Task.Delay(weaponsData[inventory[0].weapon].reload); // Reload time

                int bulletsToAdd = weaponsData[inventory[0].weapon].magCapacity - inventory[0].magBullets;
                if (bulletsToAdd <= inventory[0].reserveBullets)
                {
                    inventory[0].magBullets += bulletsToAdd;
                    inventory[0].reserveBullets -= bulletsToAdd;
                }
                else
                {
                    inventory[0].magBullets += inventory[0].reserveBullets;
                    inventory[0].reserveBullets = 0;
                }
                UpdateInventoryAmmo();

                game_inventory_reloading_label.Visible = false;
                reloading = false;
            }
        }

        private void UpdateInventoryAmmo() // Update only current weapon ammo graphics in inventory
        {
            game_inventory_currentWeaponAmmo_Label.Text = inventory[0].magBullets.ToString() + "/" + weaponsData[inventory[0].weapon].magCapacity.ToString();
            game_inventory_currentWeaponAmmoReserve_Label.Text = inventory[0].reserveBullets.ToString() + "/" + (weaponsData[inventory[0].weapon].magCapacity * weaponsData[inventory[0].weapon].maxAmmoMultiplier).ToString();
        }

        private void UpdateInventoryGraphics() // Update all inventory graphics
        {
            game_inventory_currentWeapon_PictureBox.BackgroundImage = weaponsData[inventory[0].weapon].img;
            game_inventory_currentWeaponName_label.Text = weaponsData[inventory[0].weapon].name;
            
            game_inventory_nextWeapon_PictureBox.BackgroundImage = weaponsData[inventory[1].weapon].img;
            game_inventory_prevWeapon_PictureBox.BackgroundImage = weaponsData[inventory[2].weapon].img;

            UpdateInventoryAmmo();
        }

        private void RotateWeapons(bool next) // Shift inventory either forwards or backwards
        {
            inventoryOb[] tempInventory = new inventoryOb[inventory.Length];
            Array.Copy(inventory, tempInventory, inventory.Length);

            if (next)
            {
                inventoryOb first = inventory[0];
                Array.Copy(tempInventory, 1, inventory, 0, inventory.Length - 1);
                inventory[inventory.Length - 1] = first;
            }
            else
            {
                inventoryOb last = inventory[inventory.Length - 1];
                Array.Copy(tempInventory, 0, inventory, 1, inventory.Length - 1);
                inventory[0] = last;
            }

            if (inventory[0].weapon == Weapons.None)
                RotateWeapons(next);
            else
                UpdateInventoryGraphics();
        }

        float lastRotate = 0;

        private void main_game_panel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) // Change weapon on scroll
        {
            if (!reloading && lastRotate < sw.ElapsedMilliseconds - Constants.weaponSwitchCooldown) // Can only rotate if not reloading and after weapon switch cooldown
            {
                RotateWeapons(e.Delta > 0);
                lastRotate = sw.ElapsedMilliseconds;
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
