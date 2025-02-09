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
            this.sizeValueLabel = new System.Windows.Forms.Label();
            this.enableESPBox = new System.Windows.Forms.CheckBox();
            this.enableAimbotBox = new System.Windows.Forms.CheckBox();
            this.setAttributesBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LifeInput)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GrenadesInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ammoInput)).BeginInit();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // debugTextBox
            // 
            this.debugTextBox.Enabled = false;
            this.debugTextBox.Location = new System.Drawing.Point(12, 273);
            this.debugTextBox.Name = "debugTextBox";
            this.debugTextBox.Size = new System.Drawing.Size(454, 79);
            this.debugTextBox.TabIndex = 0;
            this.debugTextBox.Text = "";
            // 
            // gameProcessTextBox
            // 
            this.gameProcessTextBox.Enabled = false;
            this.gameProcessTextBox.Location = new System.Drawing.Point(3, 16);
            this.gameProcessTextBox.Name = "gameProcessTextBox";
            this.gameProcessTextBox.Size = new System.Drawing.Size(73, 20);
            this.gameProcessTextBox.TabIndex = 1;
            this.gameProcessTextBox.Text = "ac_client";
            // 
            // readMemoryButton
            // 
            this.readMemoryButton.Location = new System.Drawing.Point(3, 81);
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
            this.flowLayoutPanel1.Size = new System.Drawing.Size(147, 70);
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
            999,
            0,
            0,
            0});
            this.LifeInput.ValueChanged += new System.EventHandler(this.LifeInput_ValueChanged);
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
            this.flowLayoutPanel2.Location = new System.Drawing.Point(12, 179);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(145, 70);
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
            99,
            0,
            0,
            0});
            this.GrenadesInput.ValueChanged += new System.EventHandler(this.GrenadesInput_ValueChanged);
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
            this.gameProcessLabel.Location = new System.Drawing.Point(3, 0);
            this.gameProcessLabel.Name = "gameProcessLabel";
            this.gameProcessLabel.Size = new System.Drawing.Size(76, 13);
            this.gameProcessLabel.TabIndex = 0;
            this.gameProcessLabel.Text = "Game Process";
            // 
            // gameProcessIdTextBox
            // 
            this.gameProcessIdTextBox.Enabled = false;
            this.gameProcessIdTextBox.Location = new System.Drawing.Point(3, 55);
            this.gameProcessIdTextBox.Name = "gameProcessIdTextBox";
            this.gameProcessIdTextBox.Size = new System.Drawing.Size(73, 20);
            this.gameProcessIdTextBox.TabIndex = 5;
            // 
            // gameProcessIdLabel
            // 
            this.gameProcessIdLabel.AutoSize = true;
            this.gameProcessIdLabel.Location = new System.Drawing.Point(3, 39);
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
            this.ammoInput.ValueChanged += new System.EventHandler(this.ammoInput_ValueChanged);
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
            this.flowLayoutPanel3.Location = new System.Drawing.Point(12, 101);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(147, 72);
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
            this.enableESPBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enableESPBox.Location = new System.Drawing.Point(3, 26);
            this.enableESPBox.Name = "enableESPBox";
            this.enableESPBox.Size = new System.Drawing.Size(93, 17);
            this.enableESPBox.TabIndex = 10;
            this.enableESPBox.Text = "Enable ESP";
            this.enableESPBox.UseVisualStyleBackColor = true;
            this.enableESPBox.CheckedChanged += new System.EventHandler(this.espEnableBox_CheckedChanged);
            // 
            // enableAimbotBox
            // 
            this.enableAimbotBox.AutoSize = true;
            this.enableAimbotBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enableAimbotBox.Location = new System.Drawing.Point(3, 49);
            this.enableAimbotBox.Name = "enableAimbotBox";
            this.enableAimbotBox.Size = new System.Drawing.Size(107, 17);
            this.enableAimbotBox.TabIndex = 11;
            this.enableAimbotBox.Text = "Enable Aimbot";
            this.enableAimbotBox.UseVisualStyleBackColor = true;
            this.enableAimbotBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // setAttributesBox
            // 
            this.setAttributesBox.AutoSize = true;
            this.setAttributesBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setAttributesBox.Location = new System.Drawing.Point(3, 72);
            this.setAttributesBox.Name = "setAttributesBox";
            this.setAttributesBox.Size = new System.Drawing.Size(87, 17);
            this.setAttributesBox.TabIndex = 12;
            this.setAttributesBox.Text = "Set Values";
            this.setAttributesBox.UseVisualStyleBackColor = true;
            this.setAttributesBox.CheckedChanged += new System.EventHandler(this.keepAttributesBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(35, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(2);
            this.label1.Size = new System.Drawing.Size(25, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "F1";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(35, 49);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(2);
            this.label2.Size = new System.Drawing.Size(25, 17);
            this.label2.TabIndex = 14;
            this.label2.Text = "F2";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(35, 72);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(2);
            this.label3.Size = new System.Drawing.Size(25, 17);
            this.label3.TabIndex = 15;
            this.label3.Text = "F3";
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.flowLayoutPanel5);
            this.flowLayoutPanel4.Controls.Add(this.flowLayoutPanel6);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(165, 25);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(198, 190);
            this.flowLayoutPanel4.TabIndex = 16;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.Controls.Add(this.label4);
            this.flowLayoutPanel5.Controls.Add(this.enableESPBox);
            this.flowLayoutPanel5.Controls.Add(this.enableAimbotBox);
            this.flowLayoutPanel5.Controls.Add(this.setAttributesBox);
            this.flowLayoutPanel5.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(119, 187);
            this.flowLayoutPanel5.TabIndex = 0;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.Controls.Add(this.label5);
            this.flowLayoutPanel6.Controls.Add(this.label1);
            this.flowLayoutPanel6.Controls.Add(this.label2);
            this.flowLayoutPanel6.Controls.Add(this.label3);
            this.flowLayoutPanel6.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel6.Location = new System.Drawing.Point(128, 3);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(67, 187);
            this.flowLayoutPanel6.TabIndex = 1;
            // 
            // flowLayoutPanel7
            // 
            this.flowLayoutPanel7.Controls.Add(this.gameProcessLabel);
            this.flowLayoutPanel7.Controls.Add(this.gameProcessTextBox);
            this.flowLayoutPanel7.Controls.Add(this.gameProcessIdLabel);
            this.flowLayoutPanel7.Controls.Add(this.gameProcessIdTextBox);
            this.flowLayoutPanel7.Controls.Add(this.readMemoryButton);
            this.flowLayoutPanel7.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel7.Location = new System.Drawing.Point(366, 25);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            this.flowLayoutPanel7.Size = new System.Drawing.Size(101, 116);
            this.flowLayoutPanel7.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(2);
            this.label4.Size = new System.Drawing.Size(50, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "Cheats";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 3);
            this.label5.Margin = new System.Windows.Forms.Padding(3);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(2);
            this.label5.Size = new System.Drawing.Size(57, 17);
            this.label5.TabIndex = 16;
            this.label5.Text = "Hotkeys";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 364);
            this.Controls.Add(this.flowLayoutPanel7);
            this.Controls.Add(this.flowLayoutPanel4);
            this.Controls.Add(this.sizeValueLabel);
            this.Controls.Add(this.flowLayoutPanel3);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel1);
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
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel7.PerformLayout();
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
        private System.Windows.Forms.Label lifeValueLabel;
        private System.Windows.Forms.Label sizeValueLabel;
        private System.Windows.Forms.CheckBox enableESPBox;
        private System.Windows.Forms.CheckBox enableAimbotBox;
        private System.Windows.Forms.CheckBox setAttributesBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}

