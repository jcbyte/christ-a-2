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
            this.startGameButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.memoryLeakButton = new System.Windows.Forms.Button();
            this.memoryUsedLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startGameButton
            // 
            this.startGameButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.startGameButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(118)))), ((int)(((byte)(166)))));
            this.startGameButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.startGameButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.startGameButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startGameButton.Font = new System.Drawing.Font("Metropolis Thin", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startGameButton.Location = new System.Drawing.Point(59, 99);
            this.startGameButton.Name = "startGameButton";
            this.startGameButton.Size = new System.Drawing.Size(325, 128);
            this.startGameButton.TabIndex = 0;
            this.startGameButton.Text = "Start Game";
            this.startGameButton.UseVisualStyleBackColor = false;
            this.startGameButton.Click += new System.EventHandler(this.startGameButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.exitButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(118)))), ((int)(((byte)(166)))));
            this.exitButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.exitButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.exitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitButton.Font = new System.Drawing.Font("Metropolis Thin", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitButton.Location = new System.Drawing.Point(59, 243);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(325, 128);
            this.exitButton.TabIndex = 1;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = false;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // memoryLeakButton
            // 
            this.memoryLeakButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.memoryLeakButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(118)))), ((int)(((byte)(166)))));
            this.memoryLeakButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.memoryLeakButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.memoryLeakButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.memoryLeakButton.Font = new System.Drawing.Font("Metropolis Thin", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memoryLeakButton.Location = new System.Drawing.Point(59, 387);
            this.memoryLeakButton.Name = "memoryLeakButton";
            this.memoryLeakButton.Size = new System.Drawing.Size(325, 128);
            this.memoryLeakButton.TabIndex = 2;
            this.memoryLeakButton.Text = "Start Memory Leak";
            this.memoryLeakButton.UseVisualStyleBackColor = false;
            this.memoryLeakButton.Click += new System.EventHandler(this.memoryLeakButton_Click);
            // 
            // memoryUsedLabel
            // 
            this.memoryUsedLabel.AutoSize = true;
            this.memoryUsedLabel.Location = new System.Drawing.Point(1207, 20);
            this.memoryUsedLabel.Name = "memoryUsedLabel";
            this.memoryUsedLabel.Size = new System.Drawing.Size(193, 19);
            this.memoryUsedLabel.TabIndex = 3;
            this.memoryUsedLabel.Text = "Memory used: 68 GB";
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.ClientSize = new System.Drawing.Size(1467, 658);
            this.Controls.Add(this.memoryUsedLabel);
            this.Controls.Add(this.memoryLeakButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.startGameButton);
            this.Font = new System.Drawing.Font("Metropolis Thin", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(129)))), ((int)(((byte)(204)))));
            this.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.Name = "mainForm";
            this.Text = "Christ,a 2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startGameButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button memoryLeakButton;
        private System.Windows.Forms.Label memoryUsedLabel;
    }
}

