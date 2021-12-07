namespace christ_a_2
{
    partial class mainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.menu_startGame_button = new System.Windows.Forms.Button();
            this.menu_exit_button = new System.Windows.Forms.Button();
            this.menu_startMemoryLeak_button = new System.Windows.Forms.Button();
            this.main_memoryCounter_label = new System.Windows.Forms.Label();
            this.main_menu_panel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.main_game_panel = new System.Windows.Forms.Panel();
            this.playerPictureBox = new System.Windows.Forms.PictureBox();
            this.game_floor_pictureBox = new System.Windows.Forms.PictureBox();
            this.main_cutscene_panel = new System.Windows.Forms.Panel();
            this.cutscene_media_windowsMediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.main_menu_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.main_game_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.game_floor_pictureBox)).BeginInit();
            this.main_cutscene_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cutscene_media_windowsMediaPlayer)).BeginInit();
            this.SuspendLayout();
            // 
            // menu_startGame_button
            // 
            this.menu_startGame_button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.menu_startGame_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.menu_startGame_button.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(118)))), ((int)(((byte)(166)))));
            this.menu_startGame_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.menu_startGame_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.menu_startGame_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.menu_startGame_button.Font = new System.Drawing.Font("Metropolis Thin", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu_startGame_button.Location = new System.Drawing.Point(460, 80);
            this.menu_startGame_button.Name = "menu_startGame_button";
            this.menu_startGame_button.Size = new System.Drawing.Size(360, 125);
            this.menu_startGame_button.TabIndex = 0;
            this.menu_startGame_button.Text = "Start Game";
            this.menu_startGame_button.UseVisualStyleBackColor = false;
            this.menu_startGame_button.MouseEnter += new System.EventHandler(this.menu_startGame_button_MouseEnter);
            // 
            // menu_exit_button
            // 
            this.menu_exit_button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.menu_exit_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.menu_exit_button.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(118)))), ((int)(((byte)(166)))));
            this.menu_exit_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.menu_exit_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.menu_exit_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.menu_exit_button.Font = new System.Drawing.Font("Metropolis Thin", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu_exit_button.Location = new System.Drawing.Point(460, 350);
            this.menu_exit_button.Name = "menu_exit_button";
            this.menu_exit_button.Size = new System.Drawing.Size(360, 125);
            this.menu_exit_button.TabIndex = 0;
            this.menu_exit_button.Text = "Exit";
            this.menu_exit_button.UseVisualStyleBackColor = false;
            this.menu_exit_button.Click += new System.EventHandler(this.menu_exit_button_Click);
            // 
            // menu_startMemoryLeak_button
            // 
            this.menu_startMemoryLeak_button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.menu_startMemoryLeak_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.menu_startMemoryLeak_button.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(118)))), ((int)(((byte)(166)))));
            this.menu_startMemoryLeak_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.menu_startMemoryLeak_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.menu_startMemoryLeak_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.menu_startMemoryLeak_button.Font = new System.Drawing.Font("Metropolis Thin", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu_startMemoryLeak_button.Location = new System.Drawing.Point(460, 215);
            this.menu_startMemoryLeak_button.Name = "menu_startMemoryLeak_button";
            this.menu_startMemoryLeak_button.Size = new System.Drawing.Size(360, 125);
            this.menu_startMemoryLeak_button.TabIndex = 0;
            this.menu_startMemoryLeak_button.Text = "Start Memory Leak";
            this.menu_startMemoryLeak_button.UseVisualStyleBackColor = false;
            this.menu_startMemoryLeak_button.Click += new System.EventHandler(this.menu_startMemoryLeak_button_Click);
            // 
            // main_memoryCounter_label
            // 
            this.main_memoryCounter_label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.main_memoryCounter_label.Location = new System.Drawing.Point(1040, 20);
            this.main_memoryCounter_label.Name = "main_memoryCounter_label";
            this.main_memoryCounter_label.Size = new System.Drawing.Size(220, 20);
            this.main_memoryCounter_label.TabIndex = 0;
            this.main_memoryCounter_label.Text = "Memory used: 68.42 GB";
            // 
            // main_menu_panel
            // 
            this.main_menu_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main_menu_panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.main_menu_panel.Controls.Add(this.menu_startGame_button);
            this.main_menu_panel.Controls.Add(this.menu_startMemoryLeak_button);
            this.main_menu_panel.Controls.Add(this.menu_exit_button);
            this.main_menu_panel.Controls.Add(this.pictureBox1);
            this.main_menu_panel.Location = new System.Drawing.Point(0, 0);
            this.main_menu_panel.Name = "main_menu_panel";
            this.main_menu_panel.Size = new System.Drawing.Size(1280, 720);
            this.main_menu_panel.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImage = global::christ_a_2.Properties.Resources.menu;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1280, 720);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // main_game_panel
            // 
            this.main_game_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main_game_panel.Controls.Add(this.playerPictureBox);
            this.main_game_panel.Controls.Add(this.game_floor_pictureBox);
            this.main_game_panel.Location = new System.Drawing.Point(0, 0);
            this.main_game_panel.Name = "main_game_panel";
            this.main_game_panel.Size = new System.Drawing.Size(1280, 720);
            this.main_game_panel.TabIndex = 1;
            this.main_game_panel.Paint += new System.Windows.Forms.PaintEventHandler(this.main_game_panel_Paint);
            // 
            // playerPictureBox
            // 
            this.playerPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.playerPictureBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("playerPictureBox.BackgroundImage")));
            this.playerPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.playerPictureBox.Location = new System.Drawing.Point(587, 346);
            this.playerPictureBox.Name = "playerPictureBox";
            this.playerPictureBox.Size = new System.Drawing.Size(45, 77);
            this.playerPictureBox.TabIndex = 1;
            this.playerPictureBox.TabStop = false;
            // 
            // game_floor_pictureBox
            // 
            this.game_floor_pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.game_floor_pictureBox.BackgroundImage = global::christ_a_2.Properties.Resources.level1FloorFactory;
            this.game_floor_pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.game_floor_pictureBox.Location = new System.Drawing.Point(0, 0);
            this.game_floor_pictureBox.Name = "game_floor_pictureBox";
            this.game_floor_pictureBox.Size = new System.Drawing.Size(1280, 720);
            this.game_floor_pictureBox.TabIndex = 0;
            this.game_floor_pictureBox.TabStop = false;
            // 
            // main_cutscene_panel
            // 
            this.main_cutscene_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main_cutscene_panel.Controls.Add(this.cutscene_media_windowsMediaPlayer);
            this.main_cutscene_panel.Location = new System.Drawing.Point(0, 0);
            this.main_cutscene_panel.Name = "main_cutscene_panel";
            this.main_cutscene_panel.Size = new System.Drawing.Size(1280, 720);
            this.main_cutscene_panel.TabIndex = 3;
            // 
            // cutscene_media_windowsMediaPlayer
            // 
            this.cutscene_media_windowsMediaPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cutscene_media_windowsMediaPlayer.Enabled = true;
            this.cutscene_media_windowsMediaPlayer.Location = new System.Drawing.Point(0, 0);
            this.cutscene_media_windowsMediaPlayer.Name = "cutscene_media_windowsMediaPlayer";
            this.cutscene_media_windowsMediaPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cutscene_media_windowsMediaPlayer.OcxState")));
            this.cutscene_media_windowsMediaPlayer.Size = new System.Drawing.Size(1280, 720);
            this.cutscene_media_windowsMediaPlayer.TabIndex = 0;
            // 
            // mainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.main_memoryCounter_label);
            this.Controls.Add(this.main_game_panel);
            this.Controls.Add(this.main_menu_panel);
            this.Controls.Add(this.main_cutscene_panel);
            this.Font = new System.Drawing.Font("Metropolis Thin", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(129)))), ((int)(((byte)(204)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.MaximumSize = new System.Drawing.Size(1920, 1080);
            this.Name = "mainForm";
            this.Text = "Christ,a 2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.main_menu_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.main_game_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.playerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.game_floor_pictureBox)).EndInit();
            this.main_cutscene_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cutscene_media_windowsMediaPlayer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button menu_startGame_button;
        private System.Windows.Forms.Button menu_exit_button;
        private System.Windows.Forms.Button menu_startMemoryLeak_button;
        private System.Windows.Forms.Label main_memoryCounter_label;
        private System.Windows.Forms.Panel main_menu_panel;
        private System.Windows.Forms.Panel main_game_panel;
        private System.Windows.Forms.PictureBox game_floor_pictureBox;
        private System.Windows.Forms.PictureBox playerPictureBox;
        private System.Windows.Forms.Panel main_cutscene_panel;
        private AxWMPLib.AxWindowsMediaPlayer cutscene_media_windowsMediaPlayer;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

