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
            double lines = data.GetLength(0);
            double columns = data.GetLength(1);
            double d;
            for (int i = 0; i < lines; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    d = Math.Sqrt(Math.Pow((Convert.ToDouble(i-lines)/2.0),2) + Math.Pow((Convert.ToDouble(j - columns) / 2.0), 2));
                    data[i, j] = data[i, j] * Math.Exp(-(d*d)/( frequency*frequency*2.0)) ;     
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
            Bitmap bmp1 = new Bitmap(pictureBox2.Image);
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);
            Rectangle rect1 = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
            System.Drawing.Imaging.BitmapData bmpData1 =
                bmp1.LockBits(rect1, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp1.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            IntPtr ptr1 = bmpData1.Scan0;
            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            int bytes1 = Math.Abs(bmpData1.Stride) * bmp1.Height;
            byte[] rgbValues1 = new byte[bytes1];

            // Copy the RGB values into the array.
            //System.Runtime.InteropServices.Marshal.Copy(ptr, DCT1, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgbValues1, 0, bytes1);

            double[,] DCT_R = new double[bmp.Height, bmp.Width]; //Массивы для частот каналов
            double[,] DCT_G = new double[bmp.Height, bmp.Width];
            double[,] DCT_B = new double[bmp.Height, bmp.Width];
            double[,] DCT_A = new double[bmp.Height, bmp.Width];

            double[,] DCT_R1 = new double[bmp1.Height, bmp1.Width]; //Массивы для частот каналов
            double[,] DCT_G1 = new double[bmp1.Height, bmp1.Width];
            double[,] DCT_B1 = new double[bmp1.Height, bmp1.Width];
            double[,] DCT_A1 = new double[bmp1.Height, bmp1.Width];
            int k = 0;
            for (int i = 0; i < bmp.Height; i++) //Заполнение матриц каналов
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    DCT_R[i, j] = Convert.ToDouble(rgbValues[k]);
                    DCT_G[i, j] = Convert.ToDouble(rgbValues[k + 1]);
                    DCT_B[i, j] = Convert.ToDouble(rgbValues[k + 2]);
                    DCT_A[i, j] = Convert.ToDouble(rgbValues[k + 3]);

                    DCT_R1[i, j] = Convert.ToDouble(rgbValues1[k]);
                    DCT_G1[i, j] = Convert.ToDouble(rgbValues1[k + 1]);
                    DCT_B1[i, j] = Convert.ToDouble(rgbValues1[k + 2]);
                    DCT_A1[i, j] = Convert.ToDouble(rgbValues1[k + 3]);
                    k += 4;
                }
            }

            for (int i = 3; i < 40; i+=4)
            {
                textBox1.Text = textBox1.Text + DCT_A[i, 0] + "   ";
            }




            textBox1.Text = textBox1.Text + bmp.Width.ToString() + "x" + bmp.Height.ToString() + "        " + bytes.ToString() + "=" + (3* bmp.Height * bmp.Width).ToString() + "   ";
            Accord.Math.CosineTransform.DCT(DCT_R); // DCT по каждому каналу 
            Accord.Math.CosineTransform.DCT(DCT_G);
            Accord.Math.CosineTransform.DCT(DCT_B);
            //Accord.Math.CosineTransform.DCT(DCT_A);// Альфа канал под вопросом


            Accord.Math.CosineTransform.DCT(DCT_R1); // DCT по каждому каналу 
            Accord.Math.CosineTransform.DCT(DCT_G1);
            Accord.Math.CosineTransform.DCT(DCT_B1);
            //Accord.Math.CosineTransform.DCT(DCT_A1);// Альфа канал под вопросом
            // Фильтр Гаусса

            double frequencyGaus = Convert.ToDouble(textBox_frequencyGaus1.Text);
            GaussFilter(DCT_R, frequencyGaus);
            GaussFilter(DCT_G, frequencyGaus);
            GaussFilter(DCT_B, frequencyGaus);
            //GaussFilter(DCT_A, frequencyGaus);

            double frequencyGaus1 = Convert.ToDouble(textBox_frequencyGaus2.Text);
            GaussFilter(DCT_R1, frequencyGaus1);
            GaussFilter(DCT_G1, frequencyGaus1);
            GaussFilter(DCT_B1, frequencyGaus1);
            //GaussFilter(DCT_A1, frequencyGaus1);

            double R = (frequencyGaus + frequencyGaus1) / (Math.Sqrt(Convert.ToDouble(bmp.Width * bmp.Width + bmp.Height * bmp.Height) / 2.0));

            for (int i = 0; i < bmp.Height; i++) //Заполнение матриц каналов
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    DCT_R[i, j] = (DCT_R[i, j] + DCT_R1[i, j])/R;
                    DCT_G[i, j] = (DCT_G[i, j] + DCT_G1[i, j])/ R;
                    DCT_B[i, j] = (DCT_B[i, j] + DCT_B1[i, j])/ R;
                    DCT_A[i, j] = (DCT_A[i, j] + DCT_A1[i, j])/ R;
                }
            }







            // Accord.Math.CosineTransform.IDCT(DCT_R); // IDCT по каждому каналу 
            //Accord.Math.CosineTransform.IDCT(DCT_G);
            //Accord.Math.CosineTransform.IDCT(DCT_B);

            IDCT_test(DCT_R); // IDCT по каждому каналу 
            IDCT_test(DCT_G);
            IDCT_test(DCT_B);
            //IDCT_test(DCT_A);

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
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.Image = bmp;
            // Unlock the bits.
            bmp.UnlockBits(bmpData);
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image != null) //если в pictureBox есть изображение
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
                        pictureBox3.Image.Save(savedialog.FileName);
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

        private void button_open2_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            if (ofd.ShowDialog(this) == DialogResult.OK)
                pictureBox2.Image = Image.FromFile(ofd.FileName);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
