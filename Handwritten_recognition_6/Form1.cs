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

		int lenght = 512;
		int weight = 512;
		public Form1()
		{
			InitializeComponent();

		}
		public static Image Crop(Image image, Rectangle selection)
		{
			Bitmap bmp = image as Bitmap;

			// Check if it is a bitmap:
			if (bmp == null)
				throw new ArgumentException("No valid bitmap");

			// Crop the image:
			Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);

			// Release the resources:
			image.Dispose();

			return cropBmp;
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

			var up_bord = (x: -1, y: -1);
			var down_bord = (x: 0, y: 0);
			var left_bord = (x: 0, y: 1000000);
			var right_bord = (x: 0, y: 0);
			var left_bord_tmp = (x: 0, y: 1000000);
			var right_bord_tmp = (x: 0, y: -1);
			//Tuple<int, int> up_bord, down_bord, left_bord, right_bord;
			for (int i = 0; i < lenght; i++) // верхняя и нижняя границы
			{
				for (int j = 0; j < weight; j++)
				{
					if (matrix[i, j] == 1 && up_bord.x == -1)
					{
						up_bord.x = i;
						up_bord.y = j;
					}
					if (matrix[i, j] == 1)
					{
						down_bord.x = i;
						down_bord.y = j;
					}
				}
			}
			for (int i = 0; i < lenght; i++) // левая и правая границы
			{
				for (int j = 0; j < weight; j++)
				{
					if (matrix[i, j] == 1 && left_bord_tmp.y > j)
					{
						left_bord_tmp = (i, j);
					}
					if (matrix[i, j] == 1 && right_bord_tmp.y < j)
					{
						right_bord_tmp = (i, j);
					}
				}
				if (left_bord.y > left_bord_tmp.y)
				{
					left_bord = left_bord_tmp;
				}
				if (right_bord.y < right_bord_tmp.y)
				{
					right_bord = right_bord_tmp;
				}
			}


			using (StreamWriter sw = new StreamWriter("Результат_прямоугольник.txt"))
			{
				bool flag = false;
				for (int i = 0; i < lenght; i++)
				{
					for (int j = 0; j < weight; j++)
					{
						if (i >= up_bord.x && i <= down_bord.x && j >= left_bord.y && j <= right_bord.y)
						{
							sw.Write(matrix[i, j]);
							flag = true;
						}
					}
					if (flag)
					{
						sw.Write("\r\n");
						flag = false;
					}
				}
			}

			Image cropimage = Crop(hand_text.Image, new Rectangle(left_bord.y, up_bord.x, right_bord.y - left_bord.y, down_bord.x - up_bord.x));
			Bitmap bitmap = new Bitmap(cropimage);
			bitmap.Save("diplom_crop.png", System.Drawing.Imaging.ImageFormat.Png);

			textBox1.Text = up_bord.x.ToString() +
			" " + up_bord.y.ToString() +
			" " + down_bord.x.ToString() +
			" " + down_bord.y.ToString() +
			" " + left_bord.x.ToString() +
			" " + left_bord.y.ToString() +
			" " + right_bord.x.ToString() +
			" " + right_bord.y.ToString();

			Image img = Image.FromFile("D:\\VSProjects\\Handwritten_recognition_6\\Handwritten_recognition_6\\bin\\Debug\\diplom_black.png");
			using (Graphics gr = Graphics.FromImage(img))
			{
				gr.DrawRectangle(new Pen(Color.Red, 2), left_bord.y, up_bord.x, right_bord.y - left_bord.y, down_bord.x - up_bord.x);
			}
			hand_text.Image = img;

			Bitmap crop_img = new Bitmap("diplom_crop.png");
			int[,] crop_matrix = new int[crop_img.Height, crop_img.Width]; // матрица изображения из 0 и 1
			for (int i = 0; i < crop_img.Height; i++)
			{
				for (int j = 0; j < crop_img.Width; j++)
				{
					if (crop_img.GetPixel(j, i) == Color.FromArgb(255, 0, 0, 0)) crop_matrix[i, j] = 1;
				}
			}

			int crop_m = crop_matrix.GetLength(0);
			int crop_n = crop_matrix.GetLength(1);
			List<int> sum_of_1 = new List<int>();
			using (StreamWriter sw = new StreamWriter("Результат_единиц_в_строках.txt"))
			{
				for (int i = 0; i < crop_m; i++)
				{
					int sum_1 = 0;
					for (int j = 0; j < crop_n; j++)
					{
						if (crop_matrix[i, j] == 1)
						{
							sum_1++;
						}
					}
					sum_of_1.Add(sum_1);
					sw.Write(sum_1);
					sw.Write("\r\n");
				}
			}

			int max_in_list = sum_of_1.Max();
			int idx = 0;
			for (int i = 0; i < sum_of_1.Count(); i++)
			{
				if (sum_of_1[i] >= max_in_list)
				{
					idx = i;
				}
			}
			textBox1.Clear();
			textBox1.Text = idx.ToString();

			int sum_0 = 0, j_left = 0;
			List<(int, int, int)> sum_of_0 = new List<(int, int, int)>();

			int numb_btw_words = 45;

			for (int j = 0; j < crop_n; j++)
			{
				if (j == crop_n - 1)
					break;
				if (crop_matrix[idx, j] == 1 && crop_matrix[idx, j + 1] == 0)
				{
					sum_0 = 0;
					j_left = j;
					do
					{
						sum_0++;
						j++;
						if (j == crop_n - 1)
							break;
					}
					while (crop_matrix[idx, j + 1] == 0);
					sum_of_0.Add((j_left, j + 1, sum_0));
				}
			}
			using (StreamWriter sw = new StreamWriter("Результат_нулей_в_строке.txt"))
			{
				for (int i = 0; i < sum_of_0.Count; i++)
				{
					sw.Write(sum_of_0[i]);
					sw.Write("\r\n");
				}
			}
			List<(int, int, int)> find_words = new List<(int, int, int)>();
			for (int i = 0; i < sum_of_0.Count; i++)
			{
				if (sum_of_0[i].Item3 >= numb_btw_words)
				{
					find_words.Add(sum_of_0[i]);
				}
			}
			for (int i = 0; i < find_words.Count; i++)
			{
				Console.WriteLine(find_words[i]);
			}

			List<int> words_borders = new List<int>();
			words_borders.Add(0);

			for (int k = 0; k < find_words.Count; k++)
			{
				for (int j = find_words[k].Item1; j < find_words[k].Item2; j++)
				{
					int i;
					for (i = 0; i < crop_m; i++)
					{
						if (crop_matrix[i, j] == 1)
						{
							break;
						}
					}
					if (i == crop_m)
					{
						words_borders.Add(j);
						break;
					}
				}
			}
			words_borders.Add(crop_n);
			foreach (var item in words_borders)
			{
				Console.WriteLine(item);
			}
			Image img_crop = Image.FromFile("diplom_crop.png");
			for (int i = 0; i < words_borders.Count - 1; i++)
			{
				Image img_crop_1 = (Image)img_crop.Clone();
				Image cropimage_words = Crop(img_crop_1, new Rectangle(words_borders[i], 0, words_borders[i + 1] - words_borders[i], crop_m));
				Bitmap bitmap1 = new Bitmap(cropimage_words);
				bitmap1.Save("D:\\VSProjects\\Handwritten_recognition_6\\Handwritten_recognition_6\\bin\\Debug\\Words\\word_" + i + ".png", System.Drawing.Imaging.ImageFormat.Png);
			}
		}
	}
}