using Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Numerics;

namespace AssaultCubeTrainer
{
    public partial class Form1 : Form
    {
        TrainerMain trainer;

        public Form1()
        {
            InitializeComponent();
            trainer = new TrainerMain { UpdateStates = UpdateStates };
        }

        

        private void UpdateLabelSafe(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                label.Invoke((MethodInvoker)delegate
                {
                    label.Text = text;
                });
            }
            else
            {
                label.Text = text;
            }
        }

        private Boolean UpdateStates(Entity localPlayer, Size gameProcessSize)
        {       
            string hp = localPlayer.hp < 0 ? "0" : localPlayer.hp.ToString();
            string ammo = localPlayer.ammo1 < 0 ? "0" : localPlayer.ammo1.ToString();
            UpdateLabelSafe(lifeValueLabel, hp);
            UpdateLabelSafe(ammo1ValueLabel, ammo);
            UpdateLabelSafe(sizeValueLabel, gameProcessSize.Width + "x" + gameProcessSize.Height);
            return true;
        }
       
        
        private void AttachGameButton_Click(object sender, EventArgs e)
        {
            if (trainer.isAttached)
            {
                MessageBox.Show("Game already attached");
                return ;
            }

            trainer.TryStartHacking(gameProcessTextBox.Text);
            if (trainer.isAttached)
            {
                gameProcessIdTextBox.Text = trainer.gameProcessId;
                Console.WriteLine("Game attached");
                return;
            }
            MessageBox.Show("Game not found");
            return ;
        }

        private void setLifeBut_Click(object sender, EventArgs e)
        {
            if(!trainer.isAttached)
            {
                MessageBox.Show("Game is not attached");
                return ;
            } 
            string lifeValue = LifeInput.Value.ToString();
            trainer.TrySetLife(lifeValue);
            debugTextBox.Text+= "Life set to: " + lifeValue+"\n";
        }

        private void setGrenadeBut_Click(object sender, EventArgs e)
        {
            if (!trainer.isAttached)
            {
                MessageBox.Show("Game is not attached");
                return;
            }
            string value = GrenadesInput.Value.ToString();
            trainer.TrySetGrenadesValue(value);
            debugTextBox.Text += "Grenades set to: " + value + "\n";
        }
        private void setAmmoButton_Click(object sender, EventArgs e)
        {
            if (!trainer.isAttached)
            {
                MessageBox.Show("Game is not attached");
                return;
            }
            string value = ammoInput.Value.ToString();
            trainer.TrySetAmmo1Value(value);
            debugTextBox.Text += "Ammo set to: " + value + "\n";
        }

        private void setAttbutton_Click(object sender, EventArgs e)
        {
            if (trainer.isAttached)
            {
                string lifeValue = LifeInput.Value.ToString();
                string ammoValue = ammoInput.Value.ToString();
                string grenadesValue = GrenadesInput.Value.ToString();
                trainer.TrySetAttributes(lifeValue, ammoValue, grenadesValue);
                debugTextBox.Text += "Attributes set to: HP:" + lifeValue + " ammo1:" + ammoValue + " grenades:" + grenadesValue + "\n";
                return ;
            }
        }

        private void espEnableBox_CheckedChanged(object sender, EventArgs e)
        {
            trainer.EspEnabled = enableESPBox.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            trainer.AimbotEnabled = enableAimbotBox.Checked;
        }

        
    }
}
