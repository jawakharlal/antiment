using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace ConsoleApp1
{
    internal class FullScreenForm : Form
    {
        private PictureBox pictureBox;

        public FullScreenForm(Rectangle bounds)
        {
            this.Bounds = bounds;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;

            pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = Image.FromFile("background.png"),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            this.Controls.Add(pictureBox);

            // Захват фокуса окна
            this.Activated += (sender, args) => this.Focus();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            InputBlocker.BlockInput(); // Заблокировать клавиатуру и мышь
        }

        // Обработчик для нажатия клавиши 9
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.D9)
            {
                Console.WriteLine("Key 9 pressed, unlocking input...");
                InputBlocker.UnblockInput();
                System.Windows.Forms.Application.Exit(); // Закрыть все окна
                return true; // Остановить дальнейшую обработку
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}