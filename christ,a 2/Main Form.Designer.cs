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
            this.startGameButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.memoryLeakButton = new System.Windows.Forms.Button();
            this.memoryUsedLabel = new System.Windows.Forms.Label();
            this.mainMenuPanel = new System.Windows.Forms.Panel();
            this.gamePanel = new System.Windows.Forms.Panel();
            this.cutscenePanel = new System.Windows.Forms.Panel();
            this.cutsceneMediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.mainMenuPanel.SuspendLayout();
            this.cutscenePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cutsceneMediaPlayer)).BeginInit();
            this.SuspendLayout();
            // 
            // startGameButton
            // 
            this.startGameButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.startGameButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.startGameButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(118)))), ((int)(((byte)(166)))));
            this.startGameButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.startGameButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.startGameButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startGameButton.Font = new System.Drawing.Font("Metropolis Thin", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startGameButton.Location = new System.Drawing.Point(460, 80);
            this.startGameButton.Name = "startGameButton";
            this.startGameButton.Size = new System.Drawing.Size(360, 125);
            this.startGameButton.TabIndex = 0;
            this.startGameButton.Text = "Start Game";
            this.startGameButton.UseVisualStyleBackColor = false;
            this.startGameButton.MouseEnter += new System.EventHandler(this.startGameButton_MouseEnter);
            // 
            // exitButton
            // 
            this.exitButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.exitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.exitButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(118)))), ((int)(((byte)(166)))));
            this.exitButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.exitButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.exitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitButton.Font = new System.Drawing.Font("Metropolis Thin", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitButton.Location = new System.Drawing.Point(460, 350);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(360, 125);
            this.exitButton.TabIndex = 0;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = false;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // memoryLeakButton
            // 
            this.memoryLeakButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.memoryLeakButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.memoryLeakButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(118)))), ((int)(((byte)(166)))));
            this.memoryLeakButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.memoryLeakButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.memoryLeakButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.memoryLeakButton.Font = new System.Drawing.Font("Metropolis Thin", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memoryLeakButton.Location = new System.Drawing.Point(460, 215);
            this.memoryLeakButton.Name = "memoryLeakButton";
            this.memoryLeakButton.Size = new System.Drawing.Size(360, 125);
            this.memoryLeakButton.TabIndex = 0;
            this.memoryLeakButton.Text = "Start Memory Leak";
            this.memoryLeakButton.UseVisualStyleBackColor = false;
            this.memoryLeakButton.Click += new System.EventHandler(this.memoryLeakButton_Click);
            // 
            // memoryUsedLabel
            // 
            this.memoryUsedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.memoryUsedLabel.Location = new System.Drawing.Point(1040, 20);
            this.memoryUsedLabel.Name = "memoryUsedLabel";
            this.memoryUsedLabel.Size = new System.Drawing.Size(220, 20);
            this.memoryUsedLabel.TabIndex = 0;
            this.memoryUsedLabel.Text = "Memory used: 68.42 GB";
            // 
            // mainMenuPanel
            // 
            this.mainMenuPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainMenuPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainMenuPanel.Controls.Add(this.startGameButton);
            this.mainMenuPanel.Controls.Add(this.memoryLeakButton);
            this.mainMenuPanel.Controls.Add(this.exitButton);
            this.mainMenuPanel.Location = new System.Drawing.Point(0, 0);
            this.mainMenuPanel.Name = "mainMenuPanel";
            this.mainMenuPanel.Size = new System.Drawing.Size(1280, 720);
            this.mainMenuPanel.TabIndex = 1;
            // 
            // gamePanel
            // 
            this.gamePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gamePanel.Location = new System.Drawing.Point(0, 0);
            this.gamePanel.Name = "gamePanel";
            this.gamePanel.Size = new System.Drawing.Size(1280, 720);
            this.gamePanel.TabIndex = 1;
            // 
            // cutscenePanel
            // 
            this.cutscenePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cutscenePanel.Controls.Add(this.cutsceneMediaPlayer);
            this.cutscenePanel.Location = new System.Drawing.Point(0, 0);
            this.cutscenePanel.Name = "cutscenePanel";
            this.cutscenePanel.Size = new System.Drawing.Size(1280, 720);
            this.cutscenePanel.TabIndex = 1;
            // 
            // cutsceneMediaPlayer
            // 
            this.cutsceneMediaPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cutsceneMediaPlayer.Enabled = true;
            this.cutsceneMediaPlayer.Location = new System.Drawing.Point(0, 0);
            this.cutsceneMediaPlayer.Name = "cutsceneMediaPlayer";
            this.cutsceneMediaPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cutsceneMediaPlayer.OcxState")));
            this.cutsceneMediaPlayer.Size = new System.Drawing.Size(1280, 720);
            this.cutsceneMediaPlayer.TabIndex = 0;
            // 
            // mainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.memoryUsedLabel);
            this.Controls.Add(this.gamePanel);
            this.Controls.Add(this.cutscenePanel);
            this.Controls.Add(this.mainMenuPanel);
            this.Font = new System.Drawing.Font("Metropolis Thin", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(129)))), ((int)(((byte)(204)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.MaximumSize = new System.Drawing.Size(1920, 1080);
            this.Name = "mainForm";
            this.Text = "Christ,a 2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.mainMenuPanel.ResumeLayout(false);
            this.cutscenePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cutsceneMediaPlayer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button startGameButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button memoryLeakButton;
        private System.Windows.Forms.Label memoryUsedLabel;
        private System.Windows.Forms.Panel mainMenuPanel;
        private System.Windows.Forms.Panel gamePanel;
        private System.Windows.Forms.Panel cutscenePanel;
        private AxWMPLib.AxWindowsMediaPlayer cutsceneMediaPlayer;
    }
}

