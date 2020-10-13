using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Math;

namespace WindowsFormsApp1
{
    

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void GaussFilter(double[,] data, double frequency)
        {
            int lines = data.GetLength(0);
            int columns = data.GetLength(1);
            double d;
            for (int i = 0; i < lines; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    d = Math.Sqrt(Math.Pow((i-lines/2),2) + Math.Pow((j - columns / 2), 2));
                    data[i, j] = data[i, j] * Math.Exp(-(d*d)/( frequency*frequency)) ;     
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            if (ofd.ShowDialog(this) == DialogResult.OK)
                pictureBox1.Image = Image.FromFile(ofd.FileName);
        }



        public static void IDCT_test(double[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            double[] row = new double[cols];
            double[] col = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < row.Length; j++)
                    row[j] = data[i, j];

                //DCT(row);
                Accord.Math.CosineTransform.IDCT(row);

                for (int j = 0; j < row.Length; j++)
                    data[i, j] = row[j];
            }

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < col.Length; i++)
                    col[i] = data[i, j];
                Accord.Math.CosineTransform.IDCT(col);
               // DCT(col);

                for (int i = 0; i < col.Length; i++)
                    data[i, j] = col[i];
            }
        }


        private void button_do_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Image);

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
           
            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];   

            // Copy the RGB values into the array.
            //System.Runtime.InteropServices.Marshal.Copy(ptr, DCT1, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            double[,] DCT_R = new double[bmp.Height, bmp.Width]; //Массивы для частот каналов
            double[,] DCT_G = new double[bmp.Height, bmp.Width];
            double[,] DCT_B = new double[bmp.Height, bmp.Width];
            double[,] DCT_A = new double[bmp.Height, bmp.Width];
            int k = 0;
            for (int i = 0; i < bmp.Height; i++) //Заполнение матриц каналов
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    DCT_R[i, j] = Convert.ToDouble(rgbValues[k]);
                    DCT_G[i, j] = Convert.ToDouble(rgbValues[k + 1]);
                    DCT_B[i, j] = Convert.ToDouble(rgbValues[k + 2]);
                    DCT_A[i, j] = Convert.ToDouble(rgbValues[k + 3]);
                    k += 4;
                }
            }
            textBox1.Text = textBox1.Text + bmp.Width.ToString() + "x" + bmp.Height.ToString() + "   penis" + bytes.ToString() + "=" + (3* bmp.Height * bmp.Width).ToString() + "   ";
            Accord.Math.CosineTransform.DCT(DCT_R); // DCT по каждому каналу 
            Accord.Math.CosineTransform.DCT(DCT_G);
            Accord.Math.CosineTransform.DCT(DCT_B);
            Accord.Math.CosineTransform.DCT(DCT_A);// Альфа канал под вопросом

            // Фильтр Гаусса

            double frequencyGaus = Convert.ToDouble(textBox_frequencyGaus.Text);
            GaussFilter(DCT_R, frequencyGaus);
            GaussFilter(DCT_G, frequencyGaus);
            GaussFilter(DCT_B, frequencyGaus);
            //GaussFilter(DCT_A, frequencyGaus);






            //Accord.Math.CosineTransform.IDCT(DCT_R); // IDCT по каждому каналу 
            //Accord.Math.CosineTransform.IDCT(DCT_G);
            //Accord.Math.CosineTransform.IDCT(DCT_B);

            IDCT_test(DCT_R); // IDCT по каждому каналу 
            IDCT_test(DCT_G);
            IDCT_test(DCT_B);
            IDCT_test(DCT_A);
            k = 0;
            for (int i = 0; i < bmp.Height; i++) //Перезаполнение байтового массива для вывода картинки
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    if(DCT_R[i, j] <= 0)
                    {
                        rgbValues[k] = 0;
                    }
                    else if ((DCT_R[i, j] >= 255))
                    {
                        rgbValues[k] = 255;
                    }
                    else
                    {
                        rgbValues[k] = Convert.ToByte(DCT_R[i, j]);
                    }

                    if (DCT_G[i, j] <= 0)
                    {
                        rgbValues[k+1] = 0;
                    }
                    else if ((DCT_G[i, j] >= 255))
                    {
                        rgbValues[k+1] = 255;
                    }
                    else
                    {
                        rgbValues[k+1] = Convert.ToByte(DCT_G[i, j]);
                    }

                    if (DCT_B[i, j] <= 0)
                    {
                        rgbValues[k + 2] = 0;
                    }
                    else if ((DCT_B[i, j] >= 255))
                    {
                        rgbValues[k + 2] = 255;
                    }
                    else
                    {
                        rgbValues[k + 2] = Convert.ToByte(DCT_B[i, j]);
                    }

                   
                    if (DCT_A[i, j] <= 0)
                    {
                        rgbValues[k + 3] = 0;
                    }
                    else if ((DCT_A[i, j] >= 255))
                    {
                        rgbValues[k + 3] = 255;
                    }
                    else
                    {
                        rgbValues[k + 3] = Convert.ToByte(DCT_A[i, j]);
                    }
                    
                    k += 4;
                }
            }



            for (int i=0; i < 20; i++)
            {
                textBox1.Text = textBox1.Text + rgbValues[i] + "   ";
            }



           





            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = bmp;
            // Unlock the bits.
            bmp.UnlockBits(bmpData);
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null) //если в pictureBox есть изображение
            {
                //создание диалогового окна "Сохранить как..", для сохранения изображения
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                savedialog.OverwritePrompt = true;
                //отображать ли предупреждение, если пользователь указывает несуществующий путь
                savedialog.CheckPathExists = true;
                //список форматов файла, отображаемый в поле "Тип файла"
                savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                //отображается ли кнопка "Справка" в диалоговом окне
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
                {
                    try
                    {
                        pictureBox2.Image.Save(savedialog.FileName);
                        //pictureBox2.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.png);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
