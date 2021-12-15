#region "Using"

using System;
using System.Collections.Generic;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace christ_a_2
{
    public partial class mainForm : Form
    {
        #region "Constants"

        public static class Constants // Global game settings
        {
            public const int memoryCounterRefresh = 50;

            public const int memoryIncreaseRate = 50;
            public const int memoryIncrease = 1024 * 1024 * 1;

            public const float playerSpeed = 0.15f;
            public const int maxPlayerHealth = 100;
            public static readonly Vector2 playerSize = new Vector2(0.04f, 0.06f); // Scaled relative Vector2

            public const int weaponSwitchCooldown = 100;
            public const int tryShootCooldown = 500;
            public const int tryReloadCooldown = 500;

            public const int enemyHealthBarThickness = 6; // (px)

            public const float ammoDropChance = 0.50f; // Ammo chance + health chance = 1 
            public const float pickupBoxRange = 0.02f; // Relative
            public const int healthPickupHealth = 20;
            public static readonly Vector2 weaponDropSize = new Vector2(0.08f, 0.06f);
            public const int weaponPickupDelay = 200; // (ms)

            public const float explosionSpeed = 0.6f;

            public const int concurrentSoundEffects = 10;

            public const int loadCutsceneCheck = 50;
        }

        public static class EnemyConstants
        {
            public const float closeEnough = 0.01f;

            public const float regularPlayerRange = 0.4f;
            public const float tankPlayerRange = 0.3f;
            public const float scoutTooFarRange = 0.2f;
            public const float sniperRunRange = 0.2f;
            public const float rowlandTooFarRange = 0.2f;
        }

        #endregion

        #region "EnumClasses"

        #region "EnumClasses.RelativeVector2"

        #region "EnumClasses.RelativeVector2.Class"

        public class Vector2 // Relative Vector2 class (0-1, 0-1)
        {
            public float x;
            public float y;

            public Vector2()
            {
                x = 0;
                y = 0;
            }

            public Vector2(float both)
            {
                x = both;
                y = both;
            }

            public Vector2(float _x, float _y)
            {
                x = _x;
                y = _y;
            }

            public Vector2(float a, bool deg)
            {
                float angle = a;
                if (deg) angle *= ((float)Math.PI / 180);

                x = (float)Math.Cos(angle);
                y = (float)Math.Sin(angle);
            }

            public float Angle(bool deg = false)
            {
                float angle = (float)Math.Atan2(y, x);
                if (!deg) return angle;
                else return angle * (180 / (float)Math.PI);
            }

            public float Magnitude()
            {
                return (float)Math.Sqrt(Math.Pow(x,2) + Math.Pow(y, 2));
            }

            public float ScaledMagnitude(Size formSize)
            {
                Vector2 scaled = FromRelativeV2ToScaledRealtiveV2(this, formSize);
                return scaled.Magnitude();
            }

            public Vector2 Normalise()
            {
                float mag = this.Magnitude();
                return new Vector2(x / mag, y / mag);
            }

            public override string ToString()
            {
                return "(" + x.ToString() + ", " + y.ToString() + ")";
            }

            static public Vector2 operator +(Vector2 lhs, Vector2 rhs)
            {
                return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y);
            }

            static public Vector2 operator -(Vector2 lhs, Vector2 rhs)
            {
                return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
            }

            static public Vector2 operator -(Vector2 rhs)
            {
                return new Vector2(-rhs.x, -rhs.y);
            }
            static public Vector2 operator *(Vector2 lhs, float rhs)
            {
                return new Vector2(lhs.x * rhs, lhs.y * rhs);
            }
        }

        #endregion

        #region "EnumClasses.RelativeVector2.Functions"

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

        private static Point SystemSizeToSystemPoint(Size s) // Change Windows Size to Windows Point 
        {
            return new Point(s.Width, s.Height);
        }

        private static Vector2 FromScaledRelativeV2ToRealtiveV2(Vector2 c, Size formSize) // A scaled relative Vector2 so that the proportions are 1:1, used for scaling
        {
            if (formSize.Width > formSize.Height)
                return new Vector2(c.x * ((float)formSize.Height / formSize.Width), c.y);
            else
                return new Vector2(c.x, c.y * ((float)formSize.Width / formSize.Height));
        }

        private static Vector2 FromRelativeV2ToScaledRealtiveV2(Vector2 c, Size formSize) // To a scaled relative Vector2 so that the proportions are 1:1
        {
            if (formSize.Width > formSize.Height)
                return new Vector2(c.x * ((float)formSize.Width / formSize.Height), c.y);
            else
                return new Vector2(c.x, c.y * ((float)formSize.Height / formSize.Width));
        }

        private static Vector2 AddV2WithBounds(Vector2 original, Vector2 add, Vector2 boundsMin, Vector2 boundsMax)
        {
            float Clamp(float x, float min, float max)
            {
                if (x > max) return max;
                if (x < min) return min;
                return x;
            }

            return new Vector2(Clamp(original.x + add.x, boundsMin.x, boundsMax.x), Clamp(original.y + add.y, boundsMin.y, boundsMax.y));
        }

        #endregion

        #endregion

        #region "EnumClasses.Scenes"

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

        #endregion

        #region "EnumClasses.Cutscenes"

        private enum Cutscenes : byte
        {
            OpeningCredits,
            BeforeLevel,
            AfterBoss,
            BeforeCredits,
            Credits,
            Loss,
        }

        #endregion

        #region "EnumClasses.Levels"

        private struct LevelOb
        {
            public Image floorImg;
            public Dictionary<Enemys, int>[] waves;

            public LevelOb(Image _floorImg, Dictionary<Enemys, int>[] _waves)
            {
                floorImg = _floorImg;
                waves = _waves;
            }
        }

        #endregion

        #region "EnumClasses.Enemy"

        private class Enemy // Enemy class containg visual and code objects
        {
            public string id;
            public Enemys type;

            public PictureBox pb;

            public Panel healthPanel;
            public PictureBox healthPb;

            public Vector2 pos;
            public Weapons weapon;
            public int health;
            public int maxHealth;
            public float speed;

            public Vector2 gotoPos;
            public bool moving;
            public bool escaping;

            public Enemy(string _id, Enemys _type, Vector2 _pos, Image img, Size size, Weapons _weapon, int _health, float _speed) 
            {
                id = _id;
                type = _type;

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

                gotoPos = new Vector2();
                moving = false;
                escaping = false;
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
            public float dropRate;
            public float movementDeviation;

            public EnemyOb(Image _img, Vector2 _size, int _health, float _speed, Weapons _weapon, float _dropRate, float _movementDeviation)
            {
                img = _img;
                size = _size;
                health = _health;
                speed = _speed;
                weapon = _weapon;
                dropRate = _dropRate;
                movementDeviation = _movementDeviation;
            }
        }

        #endregion

        #region "EnumClasses.Weapons"

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
            public WeaponClass weaponClass;
            public Image img;
            public Image bulletImg;
            public float bulletSize; // Scaled relative
            public string country;

            public int damage;
            public float weight;
            public float velocity;
            public float firerate; // rate of fire (rps)
            public int penetration; // Amount of enemys before bullet is absorbed - until rocket explodes
            public int reload; // Time to reload (ms)
            public int magCapacity;
            public int maxAmmoMultiplier; // Amount of extra mags (maxAmmo = magCapacity * maxAmmoMultiplier)
            public float accuracy; // Relative value for amount of deviation between bullet target and mouse
            public float recoil; // Relative value for amount of movement in mouse when shot
            public float maxDistance; // Relative - Max ammo distance - when grenade explodes

            public float pushBack; // Relative value for how much player is pushed back when shoting - Only for large weapons
            public int shotgunShots; // Shotgun only
            public int shotgunSpread; // Shotgun only (degrees)
            public float explosionRadius; // Relative - Grenade and RPG only
            public int explosionDamage; // Damage at explosion radius (linearly fades to 0) - Grenade and RPG only

            public WeaponOb(string _name, WeaponClass _weaponClass, Image _img, Image _bulletImg, float _bulletSize, string _country, int _damage, float _weight, float _velocity, float _firerate, int _penetration, int _reload, int _magCapacity, int _maxAmmoMultiplier, float _accuracy, float _recoil, float _maxDistance, float _pushBack = 0, int _shotgunShots = 0, int _shotgunSpread = 0, float _explosionRadius = 0, int _explosionDamage = 0)
            {
                name = _name;
                weaponClass = _weaponClass;
                img = _img;
                bulletImg = _bulletImg;
                bulletSize = _bulletSize;
                country = _country;

                damage = _damage;
                weight = _weight;
                velocity = _velocity;
                firerate = _firerate;
                penetration = _penetration;
                reload = _reload;
                magCapacity = _magCapacity;
                maxAmmoMultiplier = _maxAmmoMultiplier;
                accuracy = _accuracy;
                recoil = _recoil;
                maxDistance = _maxDistance;

                pushBack = _pushBack;
                shotgunShots = _shotgunShots;
                shotgunSpread = _shotgunSpread;
                explosionRadius = _explosionRadius;
                explosionDamage = _explosionDamage;
            }
        }

        #endregion

        #region "EnumClasses.Bullet"

        class Bullet
        {
            public Vector2 pos;
            public Vector2 originalPos;
            public float posLimit;
            public Vector2 dir;
            public float speed;
            public int damage;
            public int penetration;
            public PictureBox pb;

            public Weapons fromWeapon;
            public bool playerBullet;
            public List<string> blacklist;

            public Bullet(Vector2 _pos, float _posLimit, Vector2 _dir, float _speed, int _damage, int _penetration, Image img, Size size, Weapons _fromWeapon, bool _playerBullet, List<string> _blacklist = null)
            {
                pos = _pos;
                originalPos = pos;
                posLimit = _posLimit;
                dir = _dir;
                speed = _speed;
                damage = _damage;
                penetration = _penetration;

                Bitmap bmp = new Bitmap(img.Width, img.Height); // Rotate bullet
                Graphics gfx = Graphics.FromImage(bmp);
                gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
                gfx.RotateTransform(dir.Angle(true));
                gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
                gfx.DrawImage(img, new Point(0, 0));
                gfx.Dispose();

                pb = new PictureBox(); // Add new picturebox (bullet) to the form
                pb.BackgroundImageLayout = ImageLayout.Stretch;
                pb.Size = size;
                pb.BackColor = Color.Transparent;
                pb.BackgroundImage = bmp;

                fromWeapon = _fromWeapon;
                playerBullet = _playerBullet;
                blacklist = _blacklist ?? new List<string>(); //blacklist = (_blacklist == null ? new List<string>() : _blacklist);
            }

            public void UpdatePos(float delta, Size formSize)
            {
                pos += dir * speed * delta;
                pb.Location = FromRelativeV2Center(pos, pb.Size, formSize);
            }

            public bool ReachedPosLimit(Size formSize)
            {
                return (pos - originalPos).ScaledMagnitude(formSize) >= posLimit;
            }
        }

        #endregion

        #region "EnumClasses.Explosion"

        private class Explosion
        {
            public Vector2 pos;
            public PictureBox pb;
            public int damage;
            public float radius;
            public float maxRadius;

            public bool playerExplosion;
            public List<string> blacklist;

            public Explosion(Vector2 _pos, Image img, int _damage, float _maxRadius, bool _playerExplosion, float initialRadius = 0, List<string> _blacklist = null)
            {
                pos = _pos;

                pb = new PictureBox(); // Add new picturebox (explosion) to the form
                pb.BackgroundImageLayout = ImageLayout.Stretch;
                pb.BackColor = Color.Transparent;
                pb.BackgroundImage = img;

                damage = _damage;
                radius = initialRadius;
                maxRadius = _maxRadius;

                playerExplosion = _playerExplosion;
                blacklist = _blacklist ?? new List<string>();
            }

            public void UpdateSize(float delta, Size formSize)
            {
                radius += Constants.explosionSpeed * delta;
                pb.Size = SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(new Vector2(radius), formSize), formSize));
                pb.Location = FromRelativeV2Center(pos, pb.Size, formSize);
            }

            public bool ReachedSizeLimit()
            {
                return radius >= maxRadius;
            }
        }

        #endregion

        #region "EnumClasses.Drops"

        #region "EnumClasses.Drops.Drop"

        private enum Drops : byte 
        { 
            Ammo,
            Health,
            WeaponBox,
            NextWave,
            NextLevel,
        }

        private struct DropOb
        {
            public Image img;
            public float size; // Scaled relative radius

            public DropOb(Image _img, float _size)
            {
                img = _img;
                size = _size * 2;
            }
        }

        class Drop
        {
            public Drops type;
            public Vector2 pos;
            public PictureBox pb;

            public Drop(Drops _type, Vector2 _pos, Image img, Size size)
            {
                type = _type;
                pos = _pos;

                pb = new PictureBox(); // Add new picturebox (drop) to the form
                pb.BackgroundImageLayout = ImageLayout.Stretch;
                pb.Size = size;
                pb.BackColor = Color.Transparent;
                pb.BackgroundImage = img;
            }
        }

        #endregion

        #region "EnumClasses.Drops.WeaponDrop"

        private class WeaponDrop
        {
            public Weapons weapon;
            public int magBullets;
            public int reserveBullets;

            public Vector2 pos;
            public PictureBox pb;

            public WeaponDrop(Weapons _weapon, int _magBullets, int _reserveBullets, Vector2 _pos, Image img, Size size)
            {
                weapon = _weapon;
                magBullets = _magBullets;
                reserveBullets = _reserveBullets;
                pos = _pos;

                pb = new PictureBox(); // Add new picturebox (weaponDrop) to the form
                pb.BackgroundImageLayout = ImageLayout.Zoom;
                pb.Size = size;
                pb.BackColor = Color.Transparent;
                pb.BackgroundImage = img;
            }
        }

        #endregion

        #endregion

        #region "EnumClasses.Inventory"

        private struct InventoryOb
        {
            public Weapons weapon;
            public int magBullets;
            public int reserveBullets;

            public InventoryOb(Weapons _weapon = Weapons.None, int _magBullets = 0, int _reserveBullets = 0)
            {
                weapon = _weapon;
                magBullets = _magBullets;
                reserveBullets = _reserveBullets;
            }
        }

        #endregion

        #region "EnumClasses.SoundEffects"

        private enum SoundEffects : byte
        {
            Shoot,
            PlayerDamage, // Not implemented yet
            AmmoPickup,
            HealthPickup,
            WeaponPickup,
            Explosion,
            Reload,
            NoAmmo,
            NextWave,
            EnemyDamage,
        }

        private class SoundEffectPlayer
        {
            public bool playing;
            public System.Windows.Media.MediaPlayer player;

            public SoundEffectPlayer()
            {
                playing = false;
                player = new System.Windows.Media.MediaPlayer();
            }
        }

        #endregion

        #endregion

        #region "Main"

        #region "Main.GlobalVars"

        private List<byte[]> meme = new List<byte[]>(); // Totally useful memory
        private bool memoryLeakOn = false;
        private Random rng = new Random();

        private List<Enemy> enemys = new List<Enemy>();
        private List<Bullet> bullets = new List<Bullet>();
        private List<Drop> drops = new List<Drop>();
        private List<WeaponDrop> weaponDrops = new List<WeaponDrop>();
        private List<Explosion> explosions = new List<Explosion>();

        private Vector2 playerPos = new Vector2(0.5f, 0.5f);
        private int playerHealth;
        private InventoryOb[] inventory = new InventoryOb[3] { new InventoryOb(Weapons.MGL105, 1000, 10), new InventoryOb(Weapons.AK47, 1000, 10), new InventoryOb() };

        private System.Windows.Media.MediaPlayer backgroundMusicPlayer = new System.Windows.Media.MediaPlayer();
        private SoundEffectPlayer[] soundEffects = new SoundEffectPlayer[Constants.concurrentSoundEffects];

        private Dictionary<Scenes, SceneOb> scenesData;
        private Dictionary<Cutscenes, string> cutscenesData;
        private LevelOb[] levelsData;
        private Dictionary<WeaponClass, WeaponClassOb> weaponClassData;
        private Dictionary<Weapons, WeaponOb> weaponsData;
        private Dictionary<Enemys, EnemyOb> enemysData;
        private Dictionary<Drops, DropOb> dropsData;
        private Dictionary<SoundEffects, string> soundEffectsData;

        private Scenes cScene = Scenes.Menu;
        private int cLevel;
        private int cWave;

        #endregion

        public mainForm()
        {
            InitializeComponent();

            #region "Main.Data"

            scenesData = new Dictionary<Scenes, SceneOb> { 
                {Scenes.Menu,     new SceneOb(main_menu_panel) }, 
                {Scenes.Cutscene, new SceneOb(main_cutscene_panel, CutsceneOnload) }, 
                {Scenes.Game,     new SceneOb(main_game_panel, GameOnLoad) }, 
            };

            cutscenesData = new Dictionary<Cutscenes, string> {
                {Cutscenes.OpeningCredits, "FullResources\\Cutscenes\\openingCredits.mp4" },
                {Cutscenes.BeforeLevel,    "FullResources\\Cutscenes\\beforeLevel{level}.mp4" },
                {Cutscenes.AfterBoss,      "FullResources\\Cutscenes\\afterBoss.mp4" },
                {Cutscenes.Credits,        "FullResources\\Cutscenes\\credits.mp4" },
                {Cutscenes.BeforeCredits,  "FullResources\\Cutscenes\\beforeCredits.mp4" },
                {Cutscenes.Loss,           "FullResources\\Cutscenes\\loss.mp4" }
            };

            levelsData = new LevelOb[] {
                new LevelOb(Properties.Resources.level_0Factory, new Dictionary<Enemys, int>[] { 
                    new Dictionary<Enemys, int> { { Enemys.Regular, 2 }, {Enemys.Tank, 2 }, { Enemys.Sniper, 3 }, { Enemys.Rowland, 1 }, { Enemys.Scout, 1 } },
                    //new Dictionary<Enemys, int> { { Enemys.Regular, 4 }, { Enemys.Scout, 2 }, { Enemys.Rowland, 4 } },
                    //new Dictionary<Enemys, int> { { Enemys.Regular, 3 }, { Enemys.Scout, 3 }, { Enemys.Rowland, 3 }, { Enemys.Tank, 1 } },
                }),
                new LevelOb(Properties.Resources.level_1FactoryOutside, new Dictionary<Enemys, int>[] {
                    new Dictionary<Enemys, int> { {Enemys.Regular, 2 } },
                }),
                new LevelOb(Properties.Resources.level_2Outside, new Dictionary<Enemys, int>[] {
                    new Dictionary<Enemys, int> { {Enemys.Regular, 2 } },
                }),
                new LevelOb(Properties.Resources.level_3Boss, new Dictionary<Enemys, int>[] {
                    new Dictionary<Enemys, int> { {Enemys.Boss, 1 } },
                }),
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
                {Weapons.None, new WeaponOb("None", WeaponClass.Pistol, Properties.Resources.weapon_none, Properties.Resources.bullet_other, 0, "None", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0) },
            //  Weapon,                            Name,           Class,                       Img,                                   BulletImg,                           BulletSize, Country,          Damage, Weight, Velocity, Firerate, Penetration Reload, MagCapacity, MaxAmmoMultiplier, Accuracy, Recoil, MaxDistance, PushBack, ShotgunShots, ShotgunSpread, ExplosionRadius, ExplosionDamage
                {Weapons.Glock19,     new WeaponOb("Glock-19",     WeaponClass.Pistol,          Properties.Resources.weapon_glock19,   Properties.Resources.bullet_pistol,  0.02f,      "Austria",        10,     1.00f,  0.20f,    0.50f,    1,          800,    15,          3,                 0.00f,    0.00f,  0.50f) },
                {Weapons.FiveSeven,   new WeaponOb("Five SeveN",   WeaponClass.Pistol,          Properties.Resources.weapon_fiveseven, Properties.Resources.bullet_pistol,  0.01f,      "Belgium",        10,     1.00f,  0.50f,    1.00f,    1,          500,    20,          3,                 0.00f,    0.00f,  1.00f) },
                {Weapons.DesertEagle, new WeaponOb("Desert Eagle", WeaponClass.Pistol,          Properties.Resources.weapon_deagle,    Properties.Resources.bullet_pistol,  0.01f,      "USA",            10,     1.00f,  0.50f,    1.00f,    1,          500,    7,           2,                 0.00f,    0.00f,  1.00f) },
                {Weapons.Galil,       new WeaponOb("Galil",        WeaponClass.AR,              Properties.Resources.weapon_galil,     Properties.Resources.bullet_other,   0.01f,      "Israel",         10,     1.00f,  0.50f,    1.00f,    1,          500,    35,          6,                 0.00f,    0.00f,  1.00f) },
                {Weapons.AMD65,       new WeaponOb("AMD-65",       WeaponClass.AR,              Properties.Resources.weapon_amd65,     Properties.Resources.bullet_other,   0.01f,      "Hungary",        10,     1.00f,  0.50f,    1.00f,    1,          500,    30,          6,                 0.00f,    0.00f,  1.00f) },
                {Weapons.AEK971,      new WeaponOb("AEK-971",      WeaponClass.AR,              Properties.Resources.weapon_aek971,    Properties.Resources.bullet_other,   0.01f,      "Russia",         10,     1.00f,  0.50f,    1.00f,    1,          500,    30,          6,                 0.00f,    0.00f,  1.00f) },
                {Weapons.AK47,        new WeaponOb("AK-47",        WeaponClass.AR,              Properties.Resources.weapon_ak47,      Properties.Resources.bullet_other,   0.02f,      "Russia",         10,     1.00f,  0.50f,    4.00f,    2,          500,    30,          6,                 0.05f,    0.02f,  0.75f) },
                {Weapons.M107,        new WeaponOb("M107",         WeaponClass.Marksman,        Properties.Resources.weapon_m107,      Properties.Resources.bullet_other,   0.01f,      "USA",            10,     1.00f,  0.50f,    1.00f,    1,          500,    1,           10,                0.00f,    0.00f,  1.00f) },
                {Weapons.L115A3,      new WeaponOb("L115A3",       WeaponClass.Marksman,        Properties.Resources.weapon_l115a3,    Properties.Resources.bullet_other,   0.01f,      "United Kingdom", 10,     1.00f,  0.50f,    1.00f,    1,          500,    2,           5,                 0.00f,    0.00f,  1.00f) },
                {Weapons.SCAR,        new WeaponOb("SCAR SSR",     WeaponClass.Marksman,        Properties.Resources.weapon_scar,      Properties.Resources.bullet_other,   0.01f,      "Belgium",        10,     1.00f,  0.50f,    1.00f,    1,          500,    10,          2,                 0.00f,    0.00f,  1.00f) },
                {Weapons.UMP,         new WeaponOb("UMP",          WeaponClass.SMG,             Properties.Resources.weapon_ump,       Properties.Resources.bullet_other,   0.01f,      "Germany",        10,     1.00f,  0.50f,    1.00f,    1,          500,    25,          10,                0.00f,    0.00f,  1.00f) },
                {Weapons.MAC10,       new WeaponOb("MAC-10",       WeaponClass.SMG,             Properties.Resources.weapon_mac10,     Properties.Resources.bullet_other,   0.01f,      "USA",            10,     1.00f,  0.50f,    1.00f,    1,          500,    30,          10,                0.00f,    0.00f,  1.00f) },
                {Weapons.Uzi,         new WeaponOb("Uzi",          WeaponClass.SMG,             Properties.Resources.weapon_uzi,       Properties.Resources.bullet_other,   0.01f,      "Israel",         10,     1.00f,  0.50f,    1.00f,    1,          500,    20,          10,                0.00f,    0.00f,  1.00f) },
                {Weapons.M249,        new WeaponOb("M249",         WeaponClass.LMG,             Properties.Resources.weapon_m249,      Properties.Resources.bullet_other,   0.01f,      "USA",            10,     1.00f,  0.50f,    1.00f,    1,          500,    30,          15,                0.00f,    0.00f,  1.00f) },
                {Weapons.M2,          new WeaponOb("M2",           WeaponClass.HMG,             Properties.Resources.weapon_m2,        Properties.Resources.bullet_other,   0.01f,      "USA",            10,     1.00f,  0.50f,    1.00f,    1,          500,    100,         4,                 0.00f,    0.00f,  1.00f) },
                {Weapons.FP6,         new WeaponOb("FP6",          WeaponClass.Shotgun,         Properties.Resources.weapon_fp6,       Properties.Resources.bullet_shotgun, 0.01f,      "Germany",        10,     1.00f,  0.50f,    1.00f,    1,          500,    6,           2,                 0.00f,    0.00f,  1.00f,       0.00f,    3,            10) },
                {Weapons.M1014,       new WeaponOb("M1014",        WeaponClass.Shotgun,         Properties.Resources.weapon_m1014,     Properties.Resources.bullet_shotgun, 0.04f,      "Italy",          10,     1.00f,  0.40f,    1.00f,    1,          500,    8,           3,                 0.00f,    0.00f,  0.30f,       0.00f,    5,            20) },
                {Weapons.MGL105,      new WeaponOb("MGL-105",      WeaponClass.GrenadeLauncher, Properties.Resources.weapon_mgl105,    Properties.Resources.bullet_grenade, 0.01f,      "South Africa",   0,      3.00f,  0.50f,    1.00f,    1000,       500,    6,           1,                 0.00f,    0.00f,  0.50f,       0.00f,    0,            0,             2f,              200) },
                {Weapons.RPG7,        new WeaponOb("RPG-7",        WeaponClass.RPG,             Properties.Resources.weapon_rpg7,      Properties.Resources.bullet_rpg,     0.01f,      "Russia",         0,      2.00f,  0.50f,    1.00f,    1,          500,    1,           4,                 0.00f,    0.00f,  1.00f,       0.00f,    0,            0,             0.40f,           40) },
            };

            enemysData = new Dictionary<Enemys, EnemyOb> {
            //   Enemy                       Img,                                Size,                      Health, Speed, Weapon,              DropRate, MovementDeviation
                {Enemys.Regular, new EnemyOb(Properties.Resources.enemy_regular, new Vector2(0.04f, 0.06f), 100,    0.10f, Weapons.Galil,       0.50f,    0.10f ) },
                {Enemys.Tank,    new EnemyOb(Properties.Resources.enemy_tank,    new Vector2(0.04f, 0.06f), 200,    0.02f, Weapons.RPG7,        0.50f,    0.00f ) },
                {Enemys.Scout,   new EnemyOb(Properties.Resources.enemy_scout,   new Vector2(0.04f, 0.06f), 25,     0.20f, Weapons.UMP,         0.50f,    0.02f ) },
                {Enemys.Sniper,  new EnemyOb(Properties.Resources.enemy_sniper,  new Vector2(0.03f, 0.04f), 50,     0.30f, Weapons.M107,        0.50f,    0.08f ) },
                {Enemys.Rowland, new EnemyOb(Properties.Resources.enemy_rowland, new Vector2(0.05f, 0.06f), 150,    0.150f, Weapons.None,       0.50f,    0.20f ) },
                {Enemys.Boss,    new EnemyOb(Properties.Resources.enemy_boss,    new Vector2(0.15f, 0.06f), 600,    0.00f, Weapons.DesertEagle, 0.50f,    0.00f ) },
            };

            dropsData = new Dictionary<Drops, DropOb> {
                {Drops.Ammo,      new DropOb(Properties.Resources.pickup_ammoBox,   0.015f) },
                {Drops.Health,    new DropOb(Properties.Resources.pickup_healthBox, 0.015f) },
                {Drops.WeaponBox, new DropOb(Properties.Resources.pickup_weaponBox, 0.030f) },
                {Drops.NextWave,  new DropOb(Properties.Resources.pickup_nextWave,  0.040f) },
                {Drops.NextLevel, new DropOb(Properties.Resources.pickup_nextLevel, 0.040f) },
            };

            soundEffectsData = new Dictionary<SoundEffects, string> {
                { SoundEffects.Shoot,        "FullResources\\SoundEffects\\shoot.mp3" },
                { SoundEffects.PlayerDamage, "FullResources\\SoundEffects\\playerDamage.mp3" },
                { SoundEffects.AmmoPickup,   "FullResources\\SoundEffects\\ammoPickup.mp3" },
                { SoundEffects.HealthPickup, "FullResources\\SoundEffects\\healthPickup.mp3" },
                { SoundEffects.WeaponPickup, "FullResources\\SoundEffects\\weaponPickup.mp3" },
                { SoundEffects.Explosion,    "FullResources\\SoundEffects\\explosion.mp3" },
                { SoundEffects.Reload,       "FullResources\\SoundEffects\\reload.mp3" },
                { SoundEffects.NoAmmo,       "FullResources\\SoundEffects\\noAmmo.mp3" },
                { SoundEffects.NextWave,     "FullResources\\SoundEffects\\nextWave.mp3" },
                { SoundEffects.EnemyDamage,  "FullResources\\SoundEffects\\enemyDamage.mp3" },
            };

            #endregion

            playerHealth = Constants.maxPlayerHealth;
            UpdatePlayerHealth();

            foreach (KeyValuePair<Scenes, SceneOb> s in scenesData)
                s.Value.panel.Visible = false;
            //LoadScene(Scenes.Cutscene, Cutscenes.OpeningCredits);

            cLevel = 0;
            LoadScene(Scenes.Game);

            for (int i = 0; i < soundEffects.Length; i++)
                soundEffects[i] = new SoundEffectPlayer();

            main_game_panel.Cursor = System.Windows.Forms.Cursors.Cross;

            this.HandleCreated += mainForm_HandleCreated;
            Application.Idle += GameLoop;
        }

        #endregion

        #region "Misc"

        private void LoadScene(Scenes s, object data = null) // Close current scene and initialise new scene with optional data
        {
            scenesData[cScene].panel.Visible = false;
            cScene = s;
            scenesData[cScene].panel.Visible = true;
            if (scenesData[cScene].onLoad != null) scenesData[cScene].onLoad(data);
        }

        private Vector2 GetMousePos(Size formSize)
        {
            return ToRelativeV2(PointToClient(MousePosition), formSize);
        }

        #region "Misc.Music"

        private void BackgroundMusicEnded(object sender, EventArgs e)
        {
            backgroundMusicPlayer.Position = TimeSpan.Zero;
            backgroundMusicPlayer.Play();
        }

        private void LoadBackgroundMusic(string file)
        {
            backgroundMusicPlayer.Stop();
            backgroundMusicPlayer.Open(new Uri(file, UriKind.Relative));
            backgroundMusicPlayer.MediaEnded += new EventHandler(BackgroundMusicEnded);
            backgroundMusicPlayer.Play();
        }

        private void SoundEffectEnded(object sender, EventArgs e, int i)
        {
            soundEffects[i].playing = false;
        }

        private void PlaySoundEffect(SoundEffects sound)
        {
            for (int i = 0; i < soundEffects.Length; i++)
            {
                if (!soundEffects[i].playing)
                {
                    soundEffects[i].playing = true;

                    soundEffects[i].player.Open(new Uri(soundEffectsData[sound], UriKind.Relative));
                    soundEffects[i].player.MediaEnded += new EventHandler((object sender, EventArgs e) => SoundEffectEnded(sender, e, i));
                    soundEffects[i].player.Play();

                    return;
                }
            }
        }

        #endregion

        #region "Misc.Random"

        private float GetFloatRng(float min = 0, float max = 1)
        {
            return (float)rng.NextDouble() * (max - min) + min;
        }

        private int GetIntRng(int min, int max)
        {
            return rng.Next(min, max);
        }

        #endregion

        #region "Misc.Collisions"

        private bool PointInStraightRect(Vector2 point, Vector2 rectPointA, Vector2 rectPointB) // Check if the point is within bounds of the rect (rectPointA = top left, rectPointB = bottom right)
        {
            return (point.x >= Math.Min(rectPointA.x, rectPointB.x)) && (point.x <= Math.Max(rectPointA.x, rectPointB.x)) &&
                   (point.y >= Math.Min(rectPointA.y, rectPointB.y)) && (point.y <= Math.Max(rectPointA.y, rectPointB.y));
        }

        private bool LineIntersectsStraightLine(Vector2 lineAPointA, Vector2 lineAPointB, Vector2 lineBPointA, Vector2 lineBPointB, bool vertical)
        {
            float lineAXCoefficient = (lineAPointB.y - lineAPointA.y) / (lineAPointB.x - lineAPointA.x); // y = (m)x +  c
            float lineAOffset = lineAXCoefficient * -lineAPointA.x + lineAPointA.y;                       // y =  mx  + (c)

            if (vertical)
            {
                if (Math.Min(lineAPointA.x, lineAPointB.x) <= lineBPointA.x && Math.Max(lineAPointA.x, lineAPointB.x) >= lineBPointA.x) // Line is within x bounds
                {
                    float lineATempY = lineAXCoefficient * lineBPointA.x + lineAOffset; // Find y value at x of line
                    if (Math.Min(lineBPointA.y, lineBPointB.y) <= lineATempY && Math.Max(lineBPointA.y, lineBPointB.y) >= lineATempY) // y value is within y bounds
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (Math.Min(lineAPointA.y, lineAPointB.y) <= lineBPointA.y && Math.Max(lineAPointA.y, lineAPointB.y) >= lineBPointA.y) // Line is within y bounds
                {
                    float lineATempX = (lineBPointA.y - lineAOffset) / lineAXCoefficient; // Find x value at y of line
                    if (Math.Min(lineBPointA.x, lineBPointB.x) <= lineATempX && Math.Max(lineBPointA.x, lineBPointB.x) >= lineATempX) // x value is within y bounds
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool LineIntersectsStraightRect(Vector2 linePointA, Vector2 linePointB, Vector2 rectPointA, Vector2 rectPointB) // Check if the line intersects any of the lines of the rect (rectPointA = top left, rectPointB = bottom right)
        {
            return LineIntersectsStraightLine(linePointA, linePointB, rectPointA, new Vector2(rectPointB.x, rectPointA.y), false) ||
                   LineIntersectsStraightLine(linePointA, linePointB, new Vector2(rectPointB.x, rectPointA.y), rectPointB, true) ||
                   LineIntersectsStraightLine(linePointA, linePointB, rectPointB, new Vector2(rectPointA.x, rectPointB.y), false) ||
                   LineIntersectsStraightLine(linePointA, linePointB, new Vector2(rectPointA.x, rectPointB.y), rectPointA, true);
        }

        private bool StraightRectIntersectsStraightRect(Vector2 rectAPointA, Vector2 rectAPointB, Vector2 rectBPointA, Vector2 rectBPointB) // Check if a rect intersects another rect (rectPointA = top left, rectPointB = bottom right)
        {
            return LineIntersectsStraightRect(rectAPointA, new Vector2(rectAPointB.x, rectAPointA.y), rectBPointA, rectBPointB) ||
                   LineIntersectsStraightRect(new Vector2(rectAPointB.x, rectAPointA.y), rectAPointB, rectBPointA, rectBPointB) ||
                   LineIntersectsStraightRect(rectAPointB, new Vector2(rectAPointA.x, rectAPointB.y), rectBPointA, rectBPointB) ||
                   LineIntersectsStraightRect(new Vector2(rectAPointA.x, rectAPointB.y), rectAPointA, rectBPointA, rectBPointB);
        }

        #endregion

        #endregion

        #region "Async"

        private void mainForm_HandleCreated(object sender, EventArgs e) // To start memory counter after the Window has been initialised
        {
            UpdateMemoryCounterLoopAsync();
            IncreaseMemoryLoopAsync();
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
                if (memoryLeakOn)
                { 
                    byte[] tempMeme = new byte[Constants.memoryIncrease];
                    //for (int i = 0; i < Constants.memoryIncrease; i++) // for non paging only
                    //    tempMeme[i] = 0;

                    meme.Add(tempMeme);
                }

                await Task.Delay(Constants.memoryIncreaseRate);
            }
        }

        #endregion

        #region "Menu"

        private void menu_startGame_button_MouseEnter(object sender, EventArgs e) // Swap start button with memory leak button ;)
        {
            Point tempLocation = menu_startMemoryLeak_button.Location;
            menu_startMemoryLeak_button.Location = menu_startGame_button.Location;
            menu_startGame_button.Location = tempLocation;
        }

        private void menu_exit_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menu_startMemoryLeak_button_Click(object sender, EventArgs e)
        {
            cLevel = 0;
            LoadScene(Scenes.Cutscene, Cutscenes.BeforeLevel);
        }

        #endregion

        #region "Cutscene"

        private bool playingCutscene = false;
        private async void LoadCutsceneAfterCutscene(Cutscenes cutscene)
        {
            while(playingCutscene)
            {
                await Task.Delay(Constants.loadCutsceneCheck);
            }

            LoadScene(Scenes.Cutscene, cutscene);
        }

        private async void CutsceneOnload(object data)
        {
            Cutscenes cutscene = (Cutscenes)data;
            playingCutscene = true;
            memoryLeakOn = false;

            string url = cutscenesData[cutscene];
            if (cutscene == Cutscenes.BeforeLevel) url = url.Replace("{level}", cLevel.ToString());

            backgroundMusicPlayer.Stop();
            if (cutscene == Cutscenes.OpeningCredits) LoadBackgroundMusic("FullResources\\Music\\menu.mp3");

            double duration = 4;// (int)(new WMPLib.WindowsMediaPlayer()).newMedia(url).duration;
            cutscene_media_windowsMediaPlayer.uiMode = "none";
            cutscene_media_windowsMediaPlayer.URL = url;
            await Task.Delay((int)(duration * 1000));

            switch(cutscene)
            {
                case Cutscenes.OpeningCredits:
                    LoadScene(Scenes.Menu);
                    break;

                case Cutscenes.BeforeLevel:
                    LoadScene(Scenes.Game);
                    break;

                case Cutscenes.AfterBoss:
                    LoadCutsceneAfterCutscene(Cutscenes.BeforeCredits);
                    break;

                case Cutscenes.BeforeCredits:
                    LoadCutsceneAfterCutscene(Cutscenes.Credits);
                    break;

                case Cutscenes.Credits:
                    // after credits
                    break;

                case Cutscenes.Loss:
                    // after loss
                    break;
            }

            cutscene_media_windowsMediaPlayer.URL = "";

            if (cScene != Scenes.Menu) memoryLeakOn = true;

            playingCutscene = false;
        }

        #endregion

        #region "Game"

        #region "Game.Load"

        private void GameOnLoad(object data)
        {
            game_floor_pictureBox.BackgroundImage = levelsData[cLevel].floorImg; // Set image of floor over drawing for lag purposes
            game_floor_pictureBox.Location = new Point(0, 0);
            game_floor_pictureBox.Size = this.Size;

            game_inventory_reloading_label.Visible = false;
            UpdateInventoryGraphics();

            playerPos = new Vector2(0.5f, 0.5f);
            game_player_pictureBox.Location = FromRelativeV2Center(playerPos, game_player_pictureBox.Size, main_game_panel.Size); // Have player start in centre
            game_player_pictureBox.Size = SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(Constants.playerSize, main_game_panel.Size), main_game_panel.Size));
            game_player_pictureBox.BringToFront();
            UpdatePlayerHealth();

            LoadBackgroundMusic("FullResources\\Music\\level" + cLevel.ToString() + ".mp3");

            for (int i = enemys.Count - 1; i >= 0; i--)
            {
                enemys[i].healthPb.Dispose();
                enemys[i].healthPanel.Dispose();
                enemys[i].pb.Dispose();
                enemys.RemoveAt(i);
            }
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].pb.Dispose();
                bullets.RemoveAt(i);
            }
            for (int i = drops.Count - 1; i >= 0; i--)
            {
                drops[i].pb.Dispose();
                drops.RemoveAt(i);
            }
            for (int i = weaponDrops.Count - 1; i >= 0; i--)
            {
                weaponDrops[i].pb.Dispose();
                weaponDrops.RemoveAt(i);
            }
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].pb.Dispose();
                explosions.RemoveAt(i);
            }

            cWave = 0;
            LoadWave();
        }

        private void LoadWave()
        {
            int enemyi = 0;
            foreach (KeyValuePair<Enemys, int> enemyType in levelsData[cLevel].waves[cWave]) // Create the enemys for the level and wave
            {
                for (int i = 0; i < enemyType.Value; i++)
                {
                    enemys.Add(new Enemy( // Instatiate enemys
                        cLevel.ToString() + "-" + 0 + enemyi.ToString(),
                        enemyType.Key,
                        new Vector2(GetFloatRng(0.1f, 0.9f), GetFloatRng(0.1f, 0.9f)),
                        enemysData[enemyType.Key].img,
                        SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(enemysData[enemyType.Key].size, main_game_panel.Size), main_game_panel.Size)),
                        enemysData[enemyType.Key].weapon,
                        enemysData[enemyType.Key].health,
                        enemysData[enemyType.Key].speed
                    ));

                    int j = enemys.Count - 1;
                    main_game_panel.Controls.Add(enemys[j].pb);
                    enemys[j].UpdatePos(main_game_panel.Size);
                    enemys[j].pb.BringToFront();
                    main_game_panel.Controls.Add(enemys[j].healthPanel);
                    enemys[j].UpdateHealth();
                    enemys[j].healthPanel.BringToFront();

                    InitialiseAI(enemyi);

                    enemyi++;
                }
            }
        }

        #endregion

        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

        float startFrame = 0;
        float delta = 0;

        float lastShot = 0;
        bool lastClick = false;
        float lastTryReload = 0;
        float lastTryShoot = 0;
        bool reloading = false;
        float lastWeaponPickup = 0;

        void GameLoop(object sender, EventArgs e)
        {
            if (cScene == Scenes.Game)
            {
                #region "Game.Movement"

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
                playerPos += FromScaledRelativeV2ToRealtiveV2(movement * Constants.playerSpeed * speedWeightModifier * delta, main_game_panel.Size); // Player Movement
                game_player_pictureBox.Location = FromRelativeV2Center(playerPos, game_player_pictureBox.Size, main_game_panel.Size);

                #endregion

                #region "Game.Drops"

                List<int> deleteDrops = new List<int>();
                for (int i = 0; i < drops.Count; i++)
                {
                    if ((drops[i].pos - playerPos).ScaledMagnitude(main_game_panel.Size) <= dropsData[drops[i].type].size) // Player should pick up box
                    {
                        switch (drops[i].type)
                        {
                            case Drops.Ammo:
                                PlaySoundEffect(SoundEffects.AmmoPickup);
                                for (int j = 0; j < inventory.Length; j++)
                                {
                                    int magCapacity = weaponsData[inventory[j].weapon].magCapacity;
                                    int maxAmmo = magCapacity * weaponsData[inventory[j].weapon].maxAmmoMultiplier;
                                    inventory[j].reserveBullets = Math.Min(inventory[j].reserveBullets + magCapacity, maxAmmo);
                                }
                                UpdateInventoryAmmo();
                                break;

                            case Drops.Health:
                                PlaySoundEffect(SoundEffects.HealthPickup);
                                playerHealth = Math.Min(playerHealth + Constants.healthPickupHealth, Constants.maxPlayerHealth);
                                UpdatePlayerHealth();
                                break;

                            case Drops.WeaponBox:
                                Weapons weapon = (Weapons)Enum.GetValues(typeof(Weapons)).GetValue(GetIntRng(1, weaponsData.Count));

                                weaponDrops.Add(new WeaponDrop(
                                    weapon,
                                    weaponsData[weapon].magCapacity,
                                    (weaponsData[weapon].maxAmmoMultiplier >= 2) ? weaponsData[weapon].magCapacity : 0,
                                    drops[i].pos,
                                    weaponsData[weapon].img,
                                    SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(Constants.weaponDropSize, main_game_panel.Size), main_game_panel.Size))
                                ));

                                int k = weaponDrops.Count - 1;
                                main_game_panel.Controls.Add(weaponDrops[k].pb);
                                weaponDrops[k].pb.Location = FromRelativeV2Center(weaponDrops[k].pos, weaponDrops[k].pb.Size, main_game_panel.Size);
                                weaponDrops[k].pb.BringToFront();

                                break;

                            case Drops.NextWave:
                                PlaySoundEffect(SoundEffects.NextWave);
                                cWave++;
                                LoadWave();
                                break;

                            case Drops.NextLevel:
                                if (cLevel == levelsData.Length - 1)
                                {
                                    LoadScene(Scenes.Cutscene, Cutscenes.AfterBoss);
                                }
                                else
                                {
                                    cLevel++;
                                    LoadScene(Scenes.Cutscene, Cutscenes.BeforeLevel);
                                }
                                break;
                        }

                        deleteDrops.Add(i);
                        drops[i].pb.Dispose();
                    }
                }
                for (int i = deleteDrops.Count - 1; i >= 0; i--)
                {
                    drops.RemoveAt(deleteDrops[i]);
                }

                List<int> deleteWeaponDrops = new List<int>();
                int originalWeaponDropsCount = weaponDrops.Count;
                for (int i = 0; i < originalWeaponDropsCount; i++)
                {
                    if ((weaponDrops[i].pos - playerPos).ScaledMagnitude(main_game_panel.Size) <= Math.Max(Constants.weaponDropSize.x, Constants.weaponDropSize.y)) // Player should be close enough to pick up weapon
                    {
                        int noneInventory = (inventory[0].weapon == Weapons.None) ? 1 : (inventory[1].weapon == Weapons.None) ? 1 : (inventory[2].weapon == Weapons.None) ? 2 : -1; 
                        if (noneInventory >= 0) // If there is currenty an empty space in inventory
                        {
                            PlaySoundEffect(SoundEffects.WeaponPickup);

                            inventory[noneInventory] = new InventoryOb(
                                weaponDrops[i].weapon,
                                weaponDrops[i].magBullets,
                                weaponDrops[i].reserveBullets
                            );
                            UpdateInventoryGraphics();

                            deleteWeaponDrops.Add(i);
                            weaponDrops[i].pb.Dispose();
                        }
                        else if (Keyboard.IsKeyDown(Key.E) && lastWeaponPickup < sw.ElapsedMilliseconds - Constants.weaponPickupDelay) // Should switch out weapon
                        {
                            PlaySoundEffect(SoundEffects.WeaponPickup);

                            weaponDrops.Add(new WeaponDrop(
                                inventory[0].weapon,
                                inventory[0].magBullets,
                                inventory[0].reserveBullets,
                                weaponDrops[i].pos,
                                weaponsData[inventory[0].weapon].img,
                                SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(Constants.weaponDropSize, main_game_panel.Size), main_game_panel.Size))
                            ));

                            int j = weaponDrops.Count - 1;
                            main_game_panel.Controls.Add(weaponDrops[j].pb);
                            weaponDrops[j].pb.Location = FromRelativeV2Center(weaponDrops[j].pos, weaponDrops[j].pb.Size, main_game_panel.Size);
                            weaponDrops[j].pb.BringToFront();

                            inventory[0] = new InventoryOb(
                                weaponDrops[i].weapon,
                                weaponDrops[0].magBullets,
                                weaponDrops[0].reserveBullets
                            );
                            UpdateInventoryGraphics();

                            deleteWeaponDrops.Add(i);
                            weaponDrops[i].pb.Dispose();

                            lastWeaponPickup = sw.ElapsedMilliseconds;
                        }
                    }
                }
                for (int i = deleteWeaponDrops.Count - 1; i >= 0; i--)
                {
                    weaponDrops.RemoveAt(deleteWeaponDrops[i]);
                }

                #endregion

                #region "Game.Shoot"

                bool click = (MouseButtons == MouseButtons.Left) && ClientRectangle.Contains(FromRelativeV2(GetMousePos(main_game_panel.Size), main_game_panel.Size));
                if (click) // Player clicks to try and shoot
                {
                    if (lastShot < sw.ElapsedMilliseconds - (1000 / weaponsData[inventory[0].weapon].firerate)) // Only shoot at firerate (1 / (rps / 1000))
                    {
                        if (weaponClassData[weaponsData[inventory[0].weapon].weaponClass].type == WeaponType.Auto || (weaponClassData[weaponsData[inventory[0].weapon].weaponClass].type == WeaponType.Semi && !lastClick)) // If weapon is semi make sure its a new click
                        {
                            if (lastRotate < sw.ElapsedMilliseconds - Constants.weaponSwitchCooldown) // Can only shoot after weapon switch cooldown after changing weapon
                            {
                                if (!reloading) // Cant shoot while reloading
                                {
                                    if (inventory[0].magBullets > 0) // Make sure there are bullets in the mag
                                    {
                                        inventory[0].magBullets--; // Remove bullet from mag
                                        UpdateInventoryAmmo();

                                        PlayerShoot(inventory[0].weapon);
                                        PlaySoundEffect(SoundEffects.Shoot);

                                        lastShot = sw.ElapsedMilliseconds;
                                    }
                                    else
                                    {
                                        if (lastTryShoot < sw.ElapsedMilliseconds - Constants.tryShootCooldown)
                                        {
                                            PlaySoundEffect(SoundEffects.NoAmmo);
                                            lastTryShoot = sw.ElapsedMilliseconds;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                lastClick = click;

                #endregion

                #region "Game.Reload"

                if (Keyboard.IsKeyDown(Key.R)) // Reload
                {
                    if (!reloading)
                    {
                        if (inventory[0].reserveBullets > 0) // If there are bullets in reserve
                        {
                            PlaySoundEffect(SoundEffects.Reload);
                            ReloadWeapon();
                        }
                        else
                        {
                            if (lastTryReload < sw.ElapsedMilliseconds - Constants.tryReloadCooldown)
                            {
                                PlaySoundEffect(SoundEffects.NoAmmo);
                                lastTryReload = sw.ElapsedMilliseconds;
                            }
                        }
                    }
                }

                #endregion

                #region "Game.Bullets"

                List<int> deleteBullets = new List<int>();
                for (int i = 0; i < bullets.Count; i++) // loop through every bullet
                {
                    Vector2 before = bullets[i].pos;
                    bullets[i].UpdatePos(delta, this.Size); // Move bullet in direction
                    Vector2 after = bullets[i].pos;

                    if (bullets[i].playerBullet)
                    {
                        for (int j = 0; j < enemys.Count; j++) // Check if enemy hit
                        {
                            if (!bullets[i].blacklist.Contains(enemys[j].id))
                            {
                                Vector2 enemyPos = ToRelativeV2(enemys[j].pb.Location, main_game_panel.Size);
                                if (LineIntersectsStraightRect(before, after, enemyPos, enemyPos + ToRelativeV2(SystemSizeToSystemPoint(enemys[j].pb.Size), main_game_panel.Size)))
                                {
                                    PlaySoundEffect(SoundEffects.EnemyDamage);

                                    enemys[j].health -= bullets[i].damage;
                                    enemys[j].UpdateHealth();

                                    bullets[i].blacklist.Add(enemys[j].id); // Add enemy to blacklist so it cant be hit again by the same bullet
                                    bullets[i].penetration -= 1;
                                }
                            }
                        }
                    }

                    if (bullets[i].ReachedPosLimit(main_game_panel.Size)) // If bullet distance reached
                    {
                        switch (weaponsData[bullets[i].fromWeapon].weaponClass)
                        {
                            case WeaponClass.GrenadeLauncher: // Grenade should explode
                                PlaySoundEffect(SoundEffects.Explosion);

                                explosions.Add(new Explosion(
                                    bullets[i].pos,
                                    Properties.Resources.effect_explosion,
                                    weaponsData[bullets[i].fromWeapon].explosionDamage,
                                    weaponsData[bullets[i].fromWeapon].explosionRadius,
                                    bullets[i].playerBullet
                                ));

                                int j = explosions.Count - 1;
                                main_game_panel.Controls.Add(explosions[j].pb);
                                explosions[j].UpdateSize(delta, main_game_panel.Size);
                                explosions[j].pb.BringToFront();

                                break;
                        }

                        deleteBullets.Add(i);
                        bullets[i].pb.Dispose();
                    }
                    else if (bullets[i].penetration <= 0) // If bullet penetration reached
                    {
                        switch (weaponsData[bullets[i].fromWeapon].weaponClass)
                        {
                            case WeaponClass.RPG: // Rocket should explode
                                PlaySoundEffect(SoundEffects.Explosion);

                                explosions.Add(new Explosion(
                                    bullets[i].pos,
                                    Properties.Resources.effect_explosion,
                                    weaponsData[bullets[i].fromWeapon].explosionDamage,
                                    weaponsData[bullets[i].fromWeapon].explosionRadius,
                                    bullets[i].playerBullet
                                ));

                                int j = explosions.Count - 1;
                                main_game_panel.Controls.Add(explosions[j].pb);
                                explosions[j].UpdateSize(delta, main_game_panel.Size);
                                explosions[j].pb.BringToFront();

                                break;
                        }

                        deleteBullets.Add(i);
                        bullets[i].pb.Dispose();
                    }
                    else if (bullets[i].pos.x > 1 || bullets[i].pos.x < 0 || bullets[i].pos.y > 1 || bullets[i].pos.y < 0) // If bullet off screen
                    {
                        deleteBullets.Add(i);
                        bullets[i].pb.Dispose();
                    }
                }
                for (int i = deleteBullets.Count - 1; i >= 0; i--)
                {
                    bullets.RemoveAt(deleteBullets[i]);
                }


                #endregion

                #region "Game.Explosions"

                List<int> deleteExplosions = new List<int>();
                for (int i = 0; i < explosions.Count; i++) // Loop through every explosion
                {
                    explosions[i].UpdateSize(delta, main_game_panel.Size);

                    Vector2 explosionRectA = ToRelativeV2(explosions[i].pb.Location, main_game_panel.Size);
                    Vector2 explosionRectB = explosionRectA + ToRelativeV2(SystemSizeToSystemPoint(explosions[i].pb.Size), main_game_panel.Size);

                    if (explosions[i].playerExplosion)
                    {
                        for (int j = 0; j < enemys.Count; j++)
                        {
                            if (!explosions[i].blacklist.Contains(enemys[j].id))
                            {
                                //Vector2 enemyPos = ToRelativeV2(enemys[j].pb.Location, main_game_panel.Size);
                                //if (StraightRectIntersectsStraightRect(explosionPos, explosionPos + ToRelativeV2(SystemSizeToSystemPoint(explosions[i].pb.Size), main_game_panel.Size), enemyPos, enemyPos + ToRelativeV2(SystemSizeToSystemPoint(enemys[j].pb.Size), main_game_panel.Size)))
                                if (PointInStraightRect(enemys[j].pos, explosionRectA, explosionRectB)) // If explosion goes into enemy
                                {
                                    PlaySoundEffect(SoundEffects.EnemyDamage);

                                    float distance = (explosions[i].pos - enemys[j].pos).Magnitude();
                                    float maxExplosion = explosions[i].maxRadius;
                                    float experiencedDamage = (explosions[i].damage / maxExplosion) * (-distance + maxExplosion); // damage = ( k / maxr )( -r + maxr)

                                    enemys[j].health -= (int)experiencedDamage;
                                    enemys[j].UpdateHealth();

                                    explosions[i].blacklist.Add(enemys[j].id); // Add enemy to blacklist so it cant be hit again by the same bullet
                                }
                            }
                        }
                    }

                    if (explosions[i].ReachedSizeLimit())
                    {
                        deleteExplosions.Add(i);
                        explosions[i].pb.Dispose();
                    }
                }
                for (int i = deleteExplosions.Count - 1; i >= 0; i--)
                {
                    explosions.RemoveAt(deleteExplosions[i]);
                }

                #endregion

                #region "Game.Enemys"

                List<int> deleteEnemys = new List<int>();
                for (int i = 0; i < enemys.Count; i++) // Loop through every enemy
                {
                    if (enemys[i].health <= 0) // Check if enemy is dead
                    {
                        deleteEnemys.Add(i);
                        enemys[i].healthPb.Dispose();
                        enemys[i].healthPanel.Dispose();
                        enemys[i].pb.Dispose();

                        continue;
                    }

                    EnemyAI(i, delta);
                }
                for (int i = deleteEnemys.Count - 1; i >= 0; i--)
                {
                    if (enemysData[enemys[deleteEnemys[i]].type].dropRate >= GetFloatRng()) // Enemy should drop
                    {
                        Drops dropType = (Constants.ammoDropChance >= GetFloatRng()) ? Drops.Ammo : Drops.Health; // Chooses whether enemy drops amo or health

                        drops.Add(new Drop( // Drop ammo/health box
                            dropType,
                            enemys[deleteEnemys[i]].pos,
                            dropsData[dropType].img,
                            SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(new Vector2(dropsData[dropType].size), main_game_panel.Size), main_game_panel.Size))
                        ));

                        int j = drops.Count - 1;
                        main_game_panel.Controls.Add(drops[j].pb);
                        drops[j].pb.Location = FromRelativeV2Center(drops[j].pos, drops[j].pb.Size, main_game_panel.Size);
                        drops[j].pb.BringToFront();
                    }

                    enemys.RemoveAt(deleteEnemys[i]);

                    if (enemys.Count == 0) // Wave defeated
                    {
                        drops.Add(new Drop( // Drop weapon box
                            Drops.WeaponBox,
                            new Vector2(GetFloatRng(0.1f, 0.9f), GetFloatRng(0.1f, 0.9f)),
                            dropsData[Drops.WeaponBox].img,
                            SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(new Vector2(dropsData[Drops.WeaponBox].size), main_game_panel.Size), main_game_panel.Size))
                        ));

                        int j = drops.Count - 1;
                        main_game_panel.Controls.Add(drops[j].pb);
                        drops[j].pb.Location = FromRelativeV2Center(drops[j].pos, drops[j].pb.Size, main_game_panel.Size);
                        drops[j].pb.BringToFront();


                        Drops dropType = (cWave == levelsData[cLevel].waves.Length - 1) ? Drops.NextLevel : Drops.NextWave; // Determins whether to drop next wave or next level object

                        drops.Add(new Drop( // Drop next level/wave object
                            dropType,
                            new Vector2(0.5f, 0.50f),
                            dropsData[dropType].img,
                            SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(new Vector2(dropsData[dropType].size), main_game_panel.Size), main_game_panel.Size))
                        ));

                        j = drops.Count - 1;
                        main_game_panel.Controls.Add(drops[j].pb);
                        drops[j].pb.Location = FromRelativeV2Center(drops[j].pos, drops[j].pb.Size, main_game_panel.Size);
                        drops[j].pb.BringToFront();
                    }
                }

                #endregion
            }

            delta = (sw.ElapsedMilliseconds - startFrame) / 1000; // Calculate deltatime for frame
            startFrame = sw.ElapsedMilliseconds;
        }

        #region "Game.EnemyAI"

        private void InitialiseAI(int i) // Initilse goto for enemys
        {
            switch (enemys[i].type)
            {
                case Enemys.Regular:
                    FindNewGoto(i, false);
                    break;

                case Enemys.Tank:
                    FindNewGoto(i, false);
                    break;

                case Enemys.Sniper:
                    enemys[i].gotoPos = new Vector2(enemys[i].pos.x > 0.5 ? GetFloatRng(0.75f, 0.95f) : GetFloatRng(0.05f, 0.25f), enemys[i].pos.y > 0.5 ? GetFloatRng(0.75f, 0.95f) : GetFloatRng(0.05f, 0.25f));
                    break;

                case Enemys.Scout:
                    FindNewGoto(i, false);
                    break;

                case Enemys.Rowland:
                    FindNewGoto(i, false);
                    break;

                case Enemys.Boss:
                    break;
            }

            enemys[i].moving = (enemys[i].type != Enemys.Boss); // Boss does not move
        }

        private void FindNewGoto(int i, bool forced) // Find new goto if finished previous or escaping
        {
            Vector2 tryGoto = new Vector2();

            switch (enemys[i].type) // Enemy AI movement
            {
                case Enemys.Regular:
                    tryGoto = playerPos;
                    enemys[i].moving = true;
                    break;

                case Enemys.Tank:
                    if ((playerPos - enemys[i].pos).ScaledMagnitude(main_game_panel.Size) <= EnemyConstants.tankPlayerRange)
                    {
                        tryGoto = enemys[i].pos - (playerPos - enemys[i].pos) * 0.1f;
                    }
                    else
                    {
                        tryGoto = playerPos;
                    }
                    break;

                case Enemys.Sniper:
                    if (forced)
                    {
                        if (((enemys[i].pos.x < 0.5) && (enemys[i].pos.y < 0.5) && (playerPos.x - enemys[i].pos.x < playerPos.y - enemys[i].pos.y)) || // Move the other way the player is approaching
                            ((enemys[i].pos.x > 0.5) && (enemys[i].pos.y < 0.5) && (enemys[i].pos.x - playerPos.x < playerPos.y - enemys[i].pos.y)) ||
                            ((enemys[i].pos.x > 0.5) && (enemys[i].pos.y > 0.5) && (enemys[i].pos.x - playerPos.x < enemys[i].pos.y - playerPos.y)) ||
                            ((enemys[i].pos.x < 0.5) && (enemys[i].pos.y > 0.5) && (playerPos.x - enemys[i].pos.x < enemys[i].pos.y - playerPos.y)))
                        {
                            tryGoto = new Vector2(1 - enemys[i].pos.x, enemys[i].pos.y);
                        }
                        else
                        {
                            tryGoto = new Vector2(enemys[i].pos.x, 1 - enemys[i].pos.y);
                        }
                        enemys[i].moving = true;
                    }
                    else
                    {
                        enemys[i].moving = false;
                    }
                    break;

                case Enemys.Scout:
                    if (forced)
                    {
                        tryGoto = playerPos;
                    }
                    else
                    {
                        tryGoto = new Vector2(enemys[i].pos.x + GetFloatRng(-1.0f, 1.0f), enemys[i].pos.y + GetFloatRng(-1.0f, 1.0f));
                    }
                    break;

                case Enemys.Rowland:
                    tryGoto = playerPos;
                    break;

                case Enemys.Boss:
                    break;
            }

            float deviation = enemysData[enemys[i].type].movementDeviation / 2;
            enemys[i].gotoPos = AddV2WithBounds(tryGoto, new Vector2(GetFloatRng(-deviation, deviation), GetFloatRng(-deviation, deviation)), new Vector2(0.05f), new Vector2(0.95f)); // Movement accuraccy
            enemys[i].escaping = forced;
        }

        private void EnemyAI(int i, float delta)
        {
            if (!enemys[i].escaping)
            {
                float playerDistance = (playerPos - enemys[i].pos).ScaledMagnitude(main_game_panel.Size);

                switch (enemys[i].type) // Enemy forced movement if a condition occurs
                {
                    case Enemys.Regular: // Moves to play if within regularPlayerRange it will stop until out
                        if (playerDistance <= EnemyConstants.regularPlayerRange)
                        {
                            enemys[i].moving = false;
                        }
                        else
                        {
                            if (!enemys[i].moving)
                            {
                                FindNewGoto(i, true);
                            }
                        }
                        break;

                    case Enemys.Tank: // Moves close to play and trys to maintain a tankPlayerRange range
                        FindNewGoto(i, false);
                        break;

                    case Enemys.Sniper: // Goes to an endge and will run if the play comes near
                        if (playerDistance <= EnemyConstants.sniperRunRange)
                        {
                            FindNewGoto(i, true);
                        }
                        break;

                    case Enemys.Scout: // Goes between player and area close to player
                        if (playerDistance >= EnemyConstants.scoutTooFarRange)
                        {
                            FindNewGoto(i, true);
                        }
                        break;

                    case Enemys.Rowland: // Runs everywhere around player
                        if (playerDistance >= EnemyConstants.rowlandTooFarRange)
                        {
                            FindNewGoto(i, true);
                        }
                        break;

                    case Enemys.Boss:
                        break;
                }
            }

            if (enemys[i].moving) // Move enemy
            {
                Vector2 toGoto = enemys[i].gotoPos - enemys[i].pos;

                if (toGoto.ScaledMagnitude(main_game_panel.Size) >= EnemyConstants.closeEnough)
                {
                    enemys[i].pos += FromScaledRelativeV2ToRealtiveV2(toGoto.Normalise() * enemysData[enemys[i].type].speed * delta, main_game_panel.Size);
                    enemys[i].UpdatePos(main_game_panel.Size);
                }
                else
                {
                    FindNewGoto(i, false);
                }
            }

            // Enemy shooting
        }

        #endregion

        #region "Game.Functions"

        private void UpdatePlayerHealth()
        {
            game_playerHealth_healthBar_pictureBox.Size = SystemPointToSystemSize(FromRelativeV2(new Vector2((float)playerHealth / Constants.maxPlayerHealth, 1), game_playerHealth_panel.Size));
            game_playerHealth_health_label.Text = playerHealth.ToString() + "/" + Constants.maxPlayerHealth.ToString();
        }

        private void CreateBullet(Vector2 pos, float posLimit, Vector2 dir, float speed, int damage, int penetration, Image img, Size size, Weapons fromWeapon, bool playerBullet)
        {
            bullets.Add(new Bullet(pos, posLimit, dir, speed, damage, penetration, img, size, fromWeapon, playerBullet )); // Instatiate bullet

            int bulleti = bullets.Count - 1;
            main_game_panel.Controls.Add(bullets[bulleti].pb);
            bullets[bulleti].pb.BringToFront();
        }

        private void PlayerShoot(Weapons weapon)
        {
            float accuracyVal = weaponsData[weapon].accuracy / 2;
            Vector2 aimPos = GetMousePos(main_game_panel.Size) + FromScaledRelativeV2ToRealtiveV2(new Vector2(GetFloatRng(-accuracyVal, accuracyVal), GetFloatRng(-accuracyVal, accuracyVal)), main_game_panel.Size); // Affected by accuracy
            Vector2 dir = (aimPos - playerPos).Normalise();

            switch (weaponsData[weapon].weaponClass) // Test if its a special weapon
            {
                case WeaponClass.Shotgun: // Multiple bullets at defined spread

                    float angle = dir.Angle(true);
                    int shots = weaponsData[weapon].shotgunShots;
                    int spread = weaponsData[weapon].shotgunSpread;

                    float angleDiff = (float)spread / (shots - 1);
                    float cAngle = angle - ((float)spread / 2);

                    for (int i = 0; i < shots; i++) 
                    {
                        CreateBullet(
                            playerPos,
                            weaponsData[weapon].maxDistance,
                            new Vector2(cAngle, true),
                            (float)weaponsData[weapon].velocity,
                            weaponsData[weapon].damage,
                            weaponsData[weapon].penetration,
                            weaponsData[weapon].bulletImg,
                            SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(new Vector2(weaponsData[weapon].bulletSize), main_game_panel.Size), main_game_panel.Size)),
                            weapon,
                            true
                        );

                        cAngle += angleDiff;
                    }

                    break;

                default:
                    CreateBullet(
                        playerPos,
                        weaponsData[weapon].maxDistance,
                        dir,
                        (float)weaponsData[weapon].velocity,
                        weaponsData[weapon].damage,
                        weaponsData[weapon].penetration,
                        weaponsData[weapon].bulletImg,
                        SystemPointToSystemSize(FromRelativeV2(FromScaledRelativeV2ToRealtiveV2(new Vector2(weaponsData[weapon].bulletSize), main_game_panel.Size), main_game_panel.Size)),
                        weapon,
                        true
                    );

                    break;
            }

            float recoilVal = weaponsData[weapon].recoil / 2;
            Vector2 recoil = FromScaledRelativeV2ToRealtiveV2(new Vector2(GetFloatRng(-recoilVal, recoilVal), GetFloatRng(-recoilVal, recoilVal)), this.Size);
            Vector2 newMousePos = ToRelativeV2(System.Windows.Forms.Cursor.Position, main_game_panel.Size) + recoil; // Change mouse position depending on recoil
            System.Windows.Forms.Cursor.Position = FromRelativeV2(newMousePos, main_game_panel.Size);
        }

        private async void ReloadWeapon()
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
            InventoryOb[] tempInventory = new InventoryOb[inventory.Length];
            Array.Copy(inventory, tempInventory, inventory.Length);

            if (next)
            {
                InventoryOb first = inventory[0];
                Array.Copy(tempInventory, 1, inventory, 0, inventory.Length - 1);
                inventory[inventory.Length - 1] = first;
            }
            else
            {
                InventoryOb last = inventory[inventory.Length - 1];
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

        #endregion
    }
}
