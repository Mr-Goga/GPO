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

            byte[] rgbValues_tmp = new byte[bytes];
            

            int bytes1 = Math.Abs(bmpData1.Stride) * bmp1.Height;
            byte[] rgbValues1 = new byte[bytes1];

            // Copy the RGB values into the array.
            //System.Runtime.InteropServices.Marshal.Copy(ptr, DCT1, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgbValues1, 0, bytes1);
            rgbValues.CopyTo(rgbValues_tmp);

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


            //textBox1.Text = textBox1.Text + bmp.Width.ToString() + "x" + bmp.Height.ToString() + "        " + bytes.ToString() + "=" + (3* bmp.Height * bmp.Width).ToString() + "   ";
            Accord.Math.CosineTransform.DCT(DCT_R); // DCT по каждому каналу 
            Accord.Math.CosineTransform.DCT(DCT_G);
            Accord.Math.CosineTransform.DCT(DCT_B);
            //Accord.Math.CosineTransform.DCT(DCT_A);// Альфа канал под вопросом
            

            Accord.Math.CosineTransform.DCT(DCT_R1); // DCT по каждому каналу 
            Accord.Math.CosineTransform.DCT(DCT_G1);
            Accord.Math.CosineTransform.DCT(DCT_B1);
            //Accord.Math.CosineTransform.DCT(DCT_A1);// Альфа канал под вопросом

            
            //Расчет корреляции копирование исходных данных частот            
                double[,] DCT_R_tmp = new double[bmp1.Height, bmp1.Width]; //Массивы для частот каналов
                double[,] DCT_G_tmp = new double[bmp1.Height, bmp1.Width];
                double[,] DCT_B_tmp = new double[bmp1.Height, bmp1.Width];
                double[,] DCT_A_tmp = new double[bmp1.Height, bmp1.Width];
                DCT_R.CopyTo(DCT_R_tmp);
                DCT_G.CopyTo(DCT_G_tmp);
                DCT_B.CopyTo(DCT_B_tmp);
                DCT_R.CopyTo(DCT_R_tmp);

                double[,] DCT_R_tmp1 = new double[bmp1.Height, bmp1.Width]; //Массивы для частот каналов
                double[,] DCT_G_tmp1 = new double[bmp1.Height, bmp1.Width];
                double[,] DCT_B_tmp1 = new double[bmp1.Height, bmp1.Width];
                double[,] DCT_A_tmp1 = new double[bmp1.Height, bmp1.Width];
                DCT_R1.CopyTo(DCT_R_tmp1);
                DCT_G1.CopyTo(DCT_G_tmp1);
                DCT_B1.CopyTo(DCT_B_tmp1);
                DCT_R1.CopyTo(DCT_R_tmp1);
            
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


            //Расчет корреляции
            if (checkBox1.Checked == true)
            {
                DCT_R_tmp1.CopyTo(DCT_R1);
                DCT_G_tmp1.CopyTo(DCT_G1);
                DCT_B_tmp1.CopyTo(DCT_B1);
                DCT_R_tmp1.CopyTo(DCT_R1);


                double numerator_R = 0, W1_R = 0, W2_R = 0, numerator_G = 0, W1_G = 0, W2_G = 0, numerator_B = 0, W1_B = 0, W2_B = 0;
                double numerator_R1 = 0, W1_R1 = 0, W2_R1 = 0, numerator_G1 = 0, W1_G1 = 0, W2_G1 = 0, numerator_B1 = 0, W1_B1 = 0, W2_B1 = 0;
                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {

                        //Первая картинка
                        numerator_R = numerator_R + DCT_R_tmp[i, j] * DCT_R[i, j];
                        W1_R = W1_R + DCT_R_tmp[i, j] * DCT_R_tmp[i, j];
                        W2_R = W2_R + DCT_R[i, j] * DCT_R[i, j];
                       
                        numerator_G = numerator_G + DCT_G_tmp[i, j] * DCT_G[i, j];
                        W1_G = W1_G + DCT_G_tmp[i, j] * DCT_G_tmp[i, j];
                        W2_G = W2_G + DCT_G[i, j] * DCT_G[i, j];

                        numerator_B = numerator_B + DCT_B_tmp[i, j] * DCT_B[i, j];
                        W1_B = W1_B + DCT_B_tmp[i, j] * DCT_B_tmp[i, j];
                        W2_B = W2_B + DCT_B[i, j] * DCT_B[i, j];
                        
                        //Вторая картинка
                        numerator_R1 = numerator_R1 + DCT_R1[i, j] * DCT_R[i, j];
                        W1_R1 = W1_R1 + DCT_R1[i, j] * DCT_R1[i, j];
                        W2_R1 = W2_R1 + DCT_R[i, j] * DCT_R[i, j];

                        numerator_G1 = numerator_G1 + DCT_G1[i, j] * DCT_G[i, j];
                        W1_G1 = W1_G1 + DCT_G1[i, j] * DCT_G1[i, j];
                        W2_G1 = W2_G1 + DCT_G[i, j] * DCT_G[i, j];

                        numerator_B1 = numerator_B1 + DCT_B1[i, j] * DCT_B[i, j];
                        W1_B1 = W1_B1 + DCT_B1[i, j] * DCT_B1[i, j];
                        W2_B1 = W2_B1 + DCT_B[i, j] * DCT_B[i, j];
                        // я знаю что это говнокод, самому грустно(
                    }

                }
                textBox2.Text = ((numerator_R / (Math.Sqrt(W1_R) * Math.Sqrt(W2_R))+ numerator_G / (Math.Sqrt(W1_G) * Math.Sqrt(W2_G))+ numerator_B / (Math.Sqrt(W1_B) * Math.Sqrt(W2_B)))/3.0).ToString();
                textBox3.Text = ((numerator_R1 / (Math.Sqrt(W1_R1) * Math.Sqrt(W2_R1)) + numerator_G1 / (Math.Sqrt(W1_G1) * Math.Sqrt(W2_G1)) + numerator_B1 / (Math.Sqrt(W1_B1) * Math.Sqrt(W2_B1))) / 3.0).ToString();
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

            if (checkBox2.Checked == true)
            {

                double numerator_R = 0, W1_R = 0, W2_R = 0, numerator_G = 0, W1_G = 0, W2_G = 0, numerator_B = 0, W1_B = 0, W2_B = 0;
                double numerator_R1 = 0, W1_R1 = 0, W2_R1 = 0, numerator_G1 = 0, W1_G1 = 0, W2_G1 = 0, numerator_B1 = 0, W1_B1 = 0, W2_B1 = 0;

                for (int j = 0; j < rgbValues.Length; j += 4)
                {

                    //Первая картинка
                    numerator_R = numerator_R + rgbValues_tmp[j] * rgbValues[j];
                    W1_R = W1_R + rgbValues_tmp[j] * rgbValues_tmp[j];
                    W2_R = W2_R + rgbValues[j] * rgbValues[j];

                    numerator_G = numerator_G + rgbValues_tmp[j + 1] * rgbValues[j + 1];
                    W1_G = W1_G + rgbValues_tmp[j + 1] * rgbValues_tmp[j + 1];
                    W2_G = W2_G + rgbValues[j + 1] * rgbValues[j + 1];

                    numerator_B = numerator_B + rgbValues_tmp[j + 2] * rgbValues[j + 2];
                    W1_B = W1_B + rgbValues_tmp[j + 2] * rgbValues_tmp[j + 2];
                    W2_B = W2_B + rgbValues[j + 2] * rgbValues[j + 2];

                    //Вторая картинка
                    numerator_R1 = numerator_R1 + rgbValues1[j] * rgbValues[j];
                    W1_R1 = W1_R1 + rgbValues1[j] * rgbValues1[j];
                    W2_R1 = W2_R1 + rgbValues[j] * rgbValues[j];

                    numerator_G1 = numerator_G1 + rgbValues1[j + 1] * rgbValues[j + 1];
                    W1_G1 = W1_G1 + rgbValues1[j + 1] * rgbValues1[j + 1];
                    W2_G1 = W2_G1 + rgbValues[j + 1] * rgbValues[j + 1];

                    numerator_B1 = numerator_B1 + rgbValues1[j + 2] * rgbValues[j + 2];
                    W1_B1 = W1_B1 + rgbValues1[j + 2] * rgbValues1[j + 2];
                    W2_B1 = W2_B1 + rgbValues[j + 2] * rgbValues[j + 2];
                    // я знаю что это говнокод, самому грустно(

                }
                textBox4.Text = ((numerator_R / (Math.Sqrt(W1_R) * Math.Sqrt(W2_R)) + numerator_G / (Math.Sqrt(W1_G) * Math.Sqrt(W2_G)) + numerator_B / (Math.Sqrt(W1_B) * Math.Sqrt(W2_B))) / 3.0).ToString();
                textBox5.Text = ((numerator_R1 / (Math.Sqrt(W1_R1) * Math.Sqrt(W2_R1)) + numerator_G1 / (Math.Sqrt(W1_G1) * Math.Sqrt(W2_G1)) + numerator_B1 / (Math.Sqrt(W1_B1) * Math.Sqrt(W2_B1))) / 3.0).ToString();
            }
            
            if (checkBox3.Checked == true)
            {
                int i =0 ,j=0, j_tmp= bmp.Width- bmp.Width/4, size = 0,flag=0;
                while (size<bmp.Width* bmp.Height-2)
                {
                   if ((i==0)&&(j==0))
                    {
                        size++;
                        i++;
                        flag = 0;
                        textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                    }
                   while ((flag == 0)&&(i>0)&&(j< bmp.Width-1))
                    {
                       
                        size++;
                        i--;
                        j++;
                        textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                    }
                    textBox1.Text += "\r\n";
                    if ((i==0)&&(j< bmp.Width-1)&&(flag==0))
                    {
                        flag = 1;
                        j++;
                        size++;
                        textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";

                    }
                   else if((i< bmp.Height-1)&&(j==bmp.Width - 1) && (flag == 0))
                    {
                        flag = 1;
                        i++;
                        size++;
                        textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                    }
                   while((flag==1)&&(i< bmp.Height - 1)&&(j>0))
                    {                        
                        size++;
                        i++;
                        j--;
                        textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                    }
                    textBox1.Text += "\r\n";
                    if ((i< bmp.Height - 1)&&(j==0)&&(flag==1))
                    {
                        flag = 0;
                        i++;
                        size++;
                        textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                    }
                   else if((i== bmp.Height - 1)&&(j< bmp.Width - 1)&&(flag==1))
                    {
                        flag = 0;
                        j++;
                        size++;
                        textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                    }
                    

                }
                
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
      
    }
}
