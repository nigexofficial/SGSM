using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

// This shit code was writen in 15 minutes.
//                                      - NiGex

namespace SnapGameSaveManager
{
    public partial class MainForm : Form
    {
        private Dictionary<string, string> hatComboBoxMap = new Dictionary<string, string>()
        {
            { "Duck\0", "Duck" },
            { "BlackSpecial\0", "Black Special" },
            { "PreOrderHat\0", "Pre-Order Hat" },
            { "AppleCap\0", "Apple Cap" }
        };

        private Dictionary<string, string> colorComboBoxMap = new Dictionary<string, string>()
        {
            { "Grey\0", "Grey" },
            { "Red\0", "Red" },
            { "Yellow\0", "Yellow" },
            { "Blue\0", "Blue" },
            { "Black\0", "Black" },
            { "Green\0", "Green" },
            { "Dark Blue\0", "Dark Blue (its green for some reason)" }
        };

        public MainForm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(SNAP_GAME_SAVE_PATH))
                {
                    object currentHat = regKey.GetValue("Hat_h193458456", new byte[1] { 0 });
                    object currentColor = regKey.GetValue("Color_h212387320", new byte[1] { 0 });
                    object preOrder = regKey.GetValue("PreOrder_h2656643596", 0);

                    if (preOrder.GetType() == typeof(int))
                        checkBoxPreOrderHat.Checked = (int)preOrder > 0;

                    if (currentColor.GetType() == typeof(byte[]))
                    {
                        string color = Encoding.UTF8.GetString((byte[])currentColor).Trim();

                        if (colorComboBoxMap.ContainsKey(color))
                            comboBoxColor.Text = colorComboBoxMap[color];
                        else
                            comboBoxColor.Text = "Grey";
                    }

                    if (currentHat.GetType() == typeof(byte[]))
                    {
                        string hat = Encoding.UTF8.GetString((byte[])currentHat).Trim();

                        if (hatComboBoxMap.ContainsKey(hat))
                            comboBoxHat.Text = hatComboBoxMap[hat];
                        else
                            comboBoxHat.Text = "None";
                    }
                }

                MessageBox.Show("Snap Game save was loaded successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lastStatusLabel.Text = "Save was loaded!";
            }
            catch
            {
                MessageBox.Show("Failed to load Snap Game save. Did you launch it atleast once?",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lastStatusLabel.Text = "Failed to load save!";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            checkBoxPreOrderHat.Checked = false;
            comboBoxColor.Text = "Grey";
            comboBoxHat.Text = "None";
            lastStatusLabel.Text = "Default values applied!";
        }

        private byte[] generateSaveString(string str)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            byte[] bytes = { };

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(strBytes);

                    ms.Position = 0;
                    using (BinaryReader reader = new BinaryReader(ms))
                        bytes = reader.ReadBytes((int)ms.Length);
                }
            }

            return bytes;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(SNAP_GAME_SAVE_PATH))
                {
                    string hat = "None";
                    string color = "Grey";

                    foreach (string key in colorComboBoxMap.Keys)
                    {
                        if (colorComboBoxMap[key] == comboBoxColor.Text)
                            color = key;
                    }

                    foreach (string key in hatComboBoxMap.Keys)
                    {
                        if (hatComboBoxMap[key] == comboBoxHat.Text)
                            hat = key;
                    }

                    regKey.SetValue("Hat_h193458456", generateSaveString(hat));
                    regKey.SetValue("Color_h212387320", generateSaveString(color));
                    regKey.SetValue("PreOrder_h2656643596", checkBoxPreOrderHat.Checked ? 1 : 0);
                }

                MessageBox.Show("Snap Game save was applied successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lastStatusLabel.Text = "Save was applied successfully!";
            }
            catch
            {
                MessageBox.Show("Failed to apply Snap Game save.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lastStatusLabel.Text = "Failed to apply save!";
            }
        }

        private const string SNAP_GAME_SAVE_PATH = @"Software\IQ GAMING STUDIO\Snap Game";
    }
}
