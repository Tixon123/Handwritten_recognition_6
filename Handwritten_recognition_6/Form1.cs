using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Handwritten_recognition_6
{
	public partial class Form1 : Form
	{

		int lenght = 200;
		int weight = 200;
		public Form1()
		{
			InitializeComponent();

		}

		public void button1_Click(object sender, EventArgs e)
		{
			Bitmap bmp = new Bitmap(hand_text.Image);
			Otsu.ApplyOtsuThreshold(ref bmp); // метод Отцу
			hand_text.Image = bmp;
			bmp.Save("diplom_black.png", System.Drawing.Imaging.ImageFormat.Png);

			
			hand_text.Image = Image.FromFile("D:\\VSProjects\\Handwritten_recognition_6\\Handwritten_recognition_6\\bin\\Debug\\diplom_black.png");
			bool[][] t = Skeletonizator.Image2Bool(hand_text.Image);
			t = Skeletonizator.ZhangSuenThinning(t);
			Image img = Skeletonizator.Bool2Image(t);
			Bitmap bitmap = new Bitmap(img);
			bitmap.Save("diplom_skeleton.png", System.Drawing.Imaging.ImageFormat.Png);
		}

		private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Bitmap bitmap;
			hand_text.AutoSize = true;
			bitmap = new Bitmap(lenght, weight);
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Изображения (*.JPG, *.GIF, *.PNG, *.BMP)|*.jpg;*.gif;*.png;*.bmp;";
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				Image image = Image.FromFile(dialog.FileName);
				hand_text.Image = new Bitmap(Image.FromFile(dialog.FileName), lenght, weight);
			}
			else
			{
				hand_text.Image = new Bitmap(hand_text.Width, hand_text.Height);
				Graphics.FromImage(hand_text.Image).Clear(Color.White);
				hand_text.Refresh();
			}

			this.AutoSize = true;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Bitmap bmp = new Bitmap(hand_text.Image);
			int[,] matrix = new int[bmp.Height, bmp.Width]; // матрица изображения из 0 и 1
			for (int i = 0; i < bmp.Height; i++)
			{
				for (int j = 0; j < bmp.Width; j++)
				{
					if (bmp.GetPixel(j, i) == Color.FromArgb(255, 0, 0, 0)) matrix[i, j] = 1;
				}
			}
			int m = matrix.GetLength(0);
			int n = matrix.GetLength(1);
			using (StreamWriter sw = new StreamWriter("Результат.txt"))
			{
				for (int i = 0; i < m; i++)
				{
					for (int j = 0; j < n; j++)
					{
						sw.Write(matrix[i, j]);
					}
					sw.Write("\r\n");
				}
			}
			
			var up_bord = (x:0, y:0);
			var down_bord = (x:100, y:100);
			var left_bord = (x:100, y:100);
			var right_bord = (x:100, y:100);
			//Tuple<int, int> up_bord, down_bord, left_bord, right_bord;
			for (int i = 0; i < lenght; i++)
            {
                for (int j = 0; j < weight; j++)
                {
					if (matrix[j, j] == 1)
                    {
						up_bord.x = i;
						up_bord.y = j;
					}
					break;
				}
            }
			for (int i = lenght; i > 0; i--)
			{
				for (int j = weight; j > 0; j--)
				{
					if (matrix[j, j] == 1)
					{
						down_bord.x = i;
						down_bord.y = j;
					}
					break;
				}
			}
			textBox1.Text = up_bord.x.ToString();
			textBox1.Text = up_bord.y.ToString();
			textBox1.Text = down_bord.x.ToString();
			textBox1.Text = down_bord.y.ToString();
		}
	}
}
