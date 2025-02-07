namespace AssaultCubeTrainer
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.debugTextBox = new System.Windows.Forms.RichTextBox();
            this.gameProcessTextBox = new System.Windows.Forms.TextBox();
            this.readMemoryButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lifeLabel = new System.Windows.Forms.Label();
            this.lifeValueLabel = new System.Windows.Forms.Label();
            this.LifeInput = new System.Windows.Forms.NumericUpDown();
            this.setLifeBut = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.grenadesLabel = new System.Windows.Forms.Label();
            this.GrenadesInput = new System.Windows.Forms.NumericUpDown();
            this.setGrenadeBut = new System.Windows.Forms.Button();
            this.gameProcessLabel = new System.Windows.Forms.Label();
            this.gameProcessIdTextBox = new System.Windows.Forms.TextBox();
            this.gameProcessIdLabel = new System.Windows.Forms.Label();
            this.ammoInput = new System.Windows.Forms.NumericUpDown();
            this.setAmmoButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.ammo1Label = new System.Windows.Forms.Label();
            this.ammo1ValueLabel = new System.Windows.Forms.Label();
            this.setAttbutton = new System.Windows.Forms.Button();
            this.sizeValueLabel = new System.Windows.Forms.Label();
            this.enableESPBox = new System.Windows.Forms.CheckBox();
            this.enableAimbotBox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LifeInput)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GrenadesInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ammoInput)).BeginInit();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // debugTextBox
            // 
            this.debugTextBox.Enabled = false;
            this.debugTextBox.Location = new System.Drawing.Point(12, 271);
            this.debugTextBox.Name = "debugTextBox";
            this.debugTextBox.Size = new System.Drawing.Size(451, 163);
            this.debugTextBox.TabIndex = 0;
            this.debugTextBox.Text = "";
            // 
            // gameProcessTextBox
            // 
            this.gameProcessTextBox.Enabled = false;
            this.gameProcessTextBox.Location = new System.Drawing.Point(363, 39);
            this.gameProcessTextBox.Name = "gameProcessTextBox";
            this.gameProcessTextBox.Size = new System.Drawing.Size(100, 20);
            this.gameProcessTextBox.TabIndex = 1;
            this.gameProcessTextBox.Text = "ac_client";
            // 
            // readMemoryButton
            // 
            this.readMemoryButton.Location = new System.Drawing.Point(363, 144);
            this.readMemoryButton.Name = "readMemoryButton";
            this.readMemoryButton.Size = new System.Drawing.Size(75, 23);
            this.readMemoryButton.TabIndex = 2;
            this.readMemoryButton.Text = "Attach game";
            this.readMemoryButton.UseVisualStyleBackColor = true;
            this.readMemoryButton.Click += new System.EventHandler(this.AttachGameButton_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lifeLabel);
            this.flowLayoutPanel1.Controls.Add(this.lifeValueLabel);
            this.flowLayoutPanel1.Controls.Add(this.LifeInput);
            this.flowLayoutPanel1.Controls.Add(this.setLifeBut);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 25);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(147, 100);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // lifeLabel
            // 
            this.lifeLabel.AutoSize = true;
            this.lifeLabel.Location = new System.Drawing.Point(3, 0);
            this.lifeLabel.Name = "lifeLabel";
            this.lifeLabel.Size = new System.Drawing.Size(24, 13);
            this.lifeLabel.TabIndex = 1;
            this.lifeLabel.Text = "Life";
            // 
            // lifeValueLabel
            // 
            this.lifeValueLabel.AutoSize = true;
            this.lifeValueLabel.Location = new System.Drawing.Point(33, 0);
            this.lifeValueLabel.Name = "lifeValueLabel";
            this.lifeValueLabel.Size = new System.Drawing.Size(10, 13);
            this.lifeValueLabel.TabIndex = 4;
            this.lifeValueLabel.Text = "-";
            // 
            // LifeInput
            // 
            this.LifeInput.Location = new System.Drawing.Point(3, 16);
            this.LifeInput.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.LifeInput.Name = "LifeInput";
            this.LifeInput.Size = new System.Drawing.Size(120, 20);
            this.LifeInput.TabIndex = 0;
            this.LifeInput.Value = new decimal(new int[] {
            159,
            0,
            0,
            0});
            // 
            // setLifeBut
            // 
            this.setLifeBut.Location = new System.Drawing.Point(3, 42);
            this.setLifeBut.Name = "setLifeBut";
            this.setLifeBut.Size = new System.Drawing.Size(79, 23);
            this.setLifeBut.TabIndex = 2;
            this.setLifeBut.Text = "Set Life";
            this.setLifeBut.UseVisualStyleBackColor = true;
            this.setLifeBut.Click += new System.EventHandler(this.setLifeBut_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.grenadesLabel);
            this.flowLayoutPanel2.Controls.Add(this.GrenadesInput);
            this.flowLayoutPanel2.Controls.Add(this.setGrenadeBut);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(192, 25);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(145, 100);
            this.flowLayoutPanel2.TabIndex = 4;
            // 
            // grenadesLabel
            // 
            this.grenadesLabel.AutoSize = true;
            this.grenadesLabel.Location = new System.Drawing.Point(3, 0);
            this.grenadesLabel.Name = "grenadesLabel";
            this.grenadesLabel.Size = new System.Drawing.Size(53, 13);
            this.grenadesLabel.TabIndex = 0;
            this.grenadesLabel.Text = "Grenades";
            // 
            // GrenadesInput
            // 
            this.GrenadesInput.Location = new System.Drawing.Point(3, 16);
            this.GrenadesInput.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.GrenadesInput.Name = "GrenadesInput";
            this.GrenadesInput.Size = new System.Drawing.Size(120, 20);
            this.GrenadesInput.TabIndex = 1;
            this.GrenadesInput.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // setGrenadeBut
            // 
            this.setGrenadeBut.Location = new System.Drawing.Point(3, 42);
            this.setGrenadeBut.Name = "setGrenadeBut";
            this.setGrenadeBut.Size = new System.Drawing.Size(81, 23);
            this.setGrenadeBut.TabIndex = 2;
            this.setGrenadeBut.Text = "Set Grenades";
            this.setGrenadeBut.UseVisualStyleBackColor = true;
            this.setGrenadeBut.Click += new System.EventHandler(this.setGrenadeBut_Click);
            // 
            // gameProcessLabel
            // 
            this.gameProcessLabel.AutoSize = true;
            this.gameProcessLabel.Location = new System.Drawing.Point(360, 23);
            this.gameProcessLabel.Name = "gameProcessLabel";
            this.gameProcessLabel.Size = new System.Drawing.Size(76, 13);
            this.gameProcessLabel.TabIndex = 0;
            this.gameProcessLabel.Text = "Game Process";
            // 
            // gameProcessIdTextBox
            // 
            this.gameProcessIdTextBox.Enabled = false;
            this.gameProcessIdTextBox.Location = new System.Drawing.Point(363, 98);
            this.gameProcessIdTextBox.Name = "gameProcessIdTextBox";
            this.gameProcessIdTextBox.Size = new System.Drawing.Size(100, 20);
            this.gameProcessIdTextBox.TabIndex = 5;
            // 
            // gameProcessIdLabel
            // 
            this.gameProcessIdLabel.AutoSize = true;
            this.gameProcessIdLabel.Location = new System.Drawing.Point(360, 82);
            this.gameProcessIdLabel.Name = "gameProcessIdLabel";
            this.gameProcessIdLabel.Size = new System.Drawing.Size(59, 13);
            this.gameProcessIdLabel.TabIndex = 6;
            this.gameProcessIdLabel.Text = "Process ID";
            // 
            // ammoInput
            // 
            this.ammoInput.Location = new System.Drawing.Point(3, 16);
            this.ammoInput.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.ammoInput.Name = "ammoInput";
            this.ammoInput.Size = new System.Drawing.Size(120, 20);
            this.ammoInput.TabIndex = 0;
            this.ammoInput.Value = new decimal(new int[] {
            999,
            0,
            0,
            0});
            // 
            // setAmmoButton
            // 
            this.setAmmoButton.Location = new System.Drawing.Point(3, 42);
            this.setAmmoButton.Name = "setAmmoButton";
            this.setAmmoButton.Size = new System.Drawing.Size(79, 23);
            this.setAmmoButton.TabIndex = 2;
            this.setAmmoButton.Text = "Set Ammo";
            this.setAmmoButton.UseVisualStyleBackColor = true;
            this.setAmmoButton.Click += new System.EventHandler(this.setAmmoButton_Click);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.ammo1Label);
            this.flowLayoutPanel3.Controls.Add(this.ammo1ValueLabel);
            this.flowLayoutPanel3.Controls.Add(this.ammoInput);
            this.flowLayoutPanel3.Controls.Add(this.setAmmoButton);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(12, 144);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(147, 100);
            this.flowLayoutPanel3.TabIndex = 7;
            // 
            // ammo1Label
            // 
            this.ammo1Label.AutoSize = true;
            this.ammo1Label.Location = new System.Drawing.Point(3, 0);
            this.ammo1Label.Name = "ammo1Label";
            this.ammo1Label.Size = new System.Drawing.Size(36, 13);
            this.ammo1Label.TabIndex = 1;
            this.ammo1Label.Text = "Ammo";
            // 
            // ammo1ValueLabel
            // 
            this.ammo1ValueLabel.AutoSize = true;
            this.ammo1ValueLabel.Location = new System.Drawing.Point(45, 0);
            this.ammo1ValueLabel.Name = "ammo1ValueLabel";
            this.ammo1ValueLabel.Size = new System.Drawing.Size(10, 13);
            this.ammo1ValueLabel.TabIndex = 3;
            this.ammo1ValueLabel.Text = "-";
            // 
            // setAttbutton
            // 
            this.setAttbutton.Location = new System.Drawing.Point(363, 221);
            this.setAttbutton.Name = "setAttbutton";
            this.setAttbutton.Size = new System.Drawing.Size(79, 23);
            this.setAttbutton.TabIndex = 3;
            this.setAttbutton.Text = "Set Attributes";
            this.setAttbutton.UseVisualStyleBackColor = true;
            this.setAttbutton.Click += new System.EventHandler(this.setAttbutton_Click);
            // 
            // sizeValueLabel
            // 
            this.sizeValueLabel.AutoSize = true;
            this.sizeValueLabel.Location = new System.Drawing.Point(360, 121);
            this.sizeValueLabel.Name = "sizeValueLabel";
            this.sizeValueLabel.Size = new System.Drawing.Size(0, 13);
            this.sizeValueLabel.TabIndex = 9;
            // 
            // enableESPBox
            // 
            this.enableESPBox.AutoSize = true;
            this.enableESPBox.Location = new System.Drawing.Point(192, 144);
            this.enableESPBox.Name = "enableESPBox";
            this.enableESPBox.Size = new System.Drawing.Size(83, 17);
            this.enableESPBox.TabIndex = 10;
            this.enableESPBox.Text = "Enable ESP";
            this.enableESPBox.UseVisualStyleBackColor = true;
            this.enableESPBox.CheckedChanged += new System.EventHandler(this.espEnableBox_CheckedChanged);
            // 
            // enableAimbotBox
            // 
            this.enableAimbotBox.AutoSize = true;
            this.enableAimbotBox.Location = new System.Drawing.Point(192, 186);
            this.enableAimbotBox.Name = "enableAimbotBox";
            this.enableAimbotBox.Size = new System.Drawing.Size(94, 17);
            this.enableAimbotBox.TabIndex = 11;
            this.enableAimbotBox.Text = "Enable Aimbot";
            this.enableAimbotBox.UseVisualStyleBackColor = true;
            this.enableAimbotBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 446);
            this.Controls.Add(this.enableAimbotBox);
            this.Controls.Add(this.enableESPBox);
            this.Controls.Add(this.sizeValueLabel);
            this.Controls.Add(this.flowLayoutPanel3);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.gameProcessIdLabel);
            this.Controls.Add(this.setAttbutton);
            this.Controls.Add(this.gameProcessIdTextBox);
            this.Controls.Add(this.gameProcessLabel);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.readMemoryButton);
            this.Controls.Add(this.gameProcessTextBox);
            this.Controls.Add(this.debugTextBox);
            this.Name = "Form1";
            this.Text = "Assault_Cube_Trainer";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LifeInput)).EndInit();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GrenadesInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ammoInput)).EndInit();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox debugTextBox;
        private System.Windows.Forms.TextBox gameProcessTextBox;
        private System.Windows.Forms.Button readMemoryButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label gameProcessLabel;
        private System.Windows.Forms.TextBox gameProcessIdTextBox;
        private System.Windows.Forms.Label gameProcessIdLabel;
        private System.Windows.Forms.Label lifeLabel;
        private System.Windows.Forms.NumericUpDown LifeInput;
        private System.Windows.Forms.Button setLifeBut;
        private System.Windows.Forms.Label grenadesLabel;
        private System.Windows.Forms.NumericUpDown GrenadesInput;
        private System.Windows.Forms.Button setGrenadeBut;
        private System.Windows.Forms.NumericUpDown ammoInput;
        private System.Windows.Forms.Button setAmmoButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label ammo1Label;
        private System.Windows.Forms.Label ammo1ValueLabel;
        private System.Windows.Forms.Button setAttbutton;
        private System.Windows.Forms.Label lifeValueLabel;
        private System.Windows.Forms.Label sizeValueLabel;
        private System.Windows.Forms.CheckBox enableESPBox;
        private System.Windows.Forms.CheckBox enableAimbotBox;
    }
}

