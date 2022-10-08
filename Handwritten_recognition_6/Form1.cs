using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Handwritten_recognition_6
{
	public partial class Form1 : Form
    {
		public Form1()
        {
            InitializeComponent();
			
		}

        public void button1_Click(object sender, EventArgs e)
        {
			Bitmap bmp = new Bitmap(hand_text.Image);
			Otsu.ApplyOtsuThreshold(ref bmp); // метод Отцу
			hand_text.Image = bmp;
		}

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bitmap;
			hand_text.AutoSize = true;
			bitmap = new Bitmap(700, 500);
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Изображения (*.JPG, *.GIF, *.PNG, *.BMP)|*.jpg;*.gif;*.png;*.bmp;";
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				Image image = Image.FromFile(dialog.FileName);
				hand_text.Image = new Bitmap(Image.FromFile(dialog.FileName), 700, 500);
			}
			else
			{
				hand_text.Image = new Bitmap(hand_text.Width, hand_text.Height);
				Graphics.FromImage(hand_text.Image).Clear(Color.White);
				hand_text.Refresh();
			}
			
			this.AutoSize = true;
        }
    }
}
