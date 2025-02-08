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
        private GlobalKeyboardHook _globalKeyboardHook;

        public Form1()
        {
            InitializeComponent();
            trainer = new TrainerMain { UpdateStates = UpdateStates };

            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;

        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            Debug.WriteLine(e.KeyboardData.VirtualCode);

            //if (e.KeyboardData.VirtualCode != GlobalKeyboardHook.VkSnapshot) return;

            // seems, not needed in the life.
            //if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown &&
            //    e.KeyboardData.Flags == GlobalKeyboardHook.LlkhfAltdown)
            //{
            //    MessageBox.Show("Alt + Print Screen");
            //    e.Handled = true;
            //}
            //else

            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {


                switch (e.KeyboardData.VirtualCode)
                {
                    case 45: // F12
                       AttachGameButton_Click(null, null);
                        break;
                    case 112: // F1
                        enableESPBox.Checked = !enableESPBox.Checked;
                        break;
                    case 113: // F2
                        enableAimbotBox.Checked = !enableAimbotBox.Checked;
                        break;
                    default:
                        return;
                }

            e.Handled = true;
            }
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


        private void espEnableBox_CheckedChanged(object sender, EventArgs e)
        {
            trainer.EspEnabled = enableESPBox.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            trainer.AimbotEnabled = enableAimbotBox.Checked;
        }

        private void keepAttributesBox_CheckedChanged(object sender, EventArgs e)
        {
            if(trainer.isAttached)
            {
                trainer.lifeValue = (int)LifeInput.Value;
                trainer.ammo1Value = (int)ammoInput.Value;
                trainer.grenadesValue = (int)GrenadesInput.Value;
                trainer.KeepAttributes = keepAttributesBox.Checked;
            }
        }

        private void LifeInput_ValueChanged(object sender, EventArgs e)
        {
            trainer.lifeValue = (int)LifeInput.Value;
        }

        private void ammoInput_ValueChanged(object sender, EventArgs e)
        {
            trainer.ammo1Value = (int)ammoInput.Value;
        }

        private void GrenadesInput_ValueChanged(object sender, EventArgs e)
        {
            trainer.grenadesValue = (int)GrenadesInput.Value;
        }
    }
}
