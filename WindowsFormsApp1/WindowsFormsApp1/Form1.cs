﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Math;
using System.Collections;
using System.IO;

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
                    d = Math.Sqrt(Math.Pow((Convert.ToDouble(i - lines) / 2.0), 2) + Math.Pow((Convert.ToDouble(j - columns) / 2.0), 2));
                    data[i, j] = data[i, j] * Math.Exp(-(d * d) / (frequency * frequency * 2.0));
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
            DCT_A1.CopyTo(DCT_A_tmp1);

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
                    DCT_R[i, j] = (DCT_R[i, j] + DCT_R1[i, j]) / R;
                    DCT_G[i, j] = (DCT_G[i, j] + DCT_G1[i, j]) / R;
                    DCT_B[i, j] = (DCT_B[i, j] + DCT_B1[i, j]) / R;
                    DCT_A[i, j] = (DCT_A[i, j] + DCT_A1[i, j]) / R;
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
                textBox2.Text = ((numerator_R / (Math.Sqrt(W1_R) * Math.Sqrt(W2_R)) + numerator_G / (Math.Sqrt(W1_G) * Math.Sqrt(W2_G)) + numerator_B / (Math.Sqrt(W1_B) * Math.Sqrt(W2_B))) / 3.0).ToString();
                textBox3.Text = ((numerator_R1 / (Math.Sqrt(W1_R1) * Math.Sqrt(W2_R1)) + numerator_G1 / (Math.Sqrt(W1_G1) * Math.Sqrt(W2_G1)) + numerator_B1 / (Math.Sqrt(W1_B1) * Math.Sqrt(W2_B1))) / 3.0).ToString();
            }

            //Копирование исходного изображения для PSNR 
            byte[] rgbValues_PSNR = new byte[bytes1];
            if (checkBox9.Checked==true)
            {
                DCT_R.CopyTo(DCT_R_tmp1);
                DCT_G.CopyTo(DCT_G_tmp1);
                DCT_B.CopyTo(DCT_B_tmp1);
                //DCT_A.CopyTo(DCT_A_tmp1);
                IDCT_test(DCT_R_tmp1); // IDCT по каждому каналу 
                IDCT_test(DCT_G_tmp1);
                IDCT_test(DCT_B_tmp1);
                //IDCT_test(DCT_A);
                k = 0;
                for (int i = 0; i < bmp.Height; i++) //Перезаполнение байтового массива для вывода картинки
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        if (DCT_R_tmp1[i, j] <= 0)
                        {
                            rgbValues_PSNR[k] = 0;
                        }
                        else if ((DCT_R_tmp1[i, j] >= 255))
                        {
                            rgbValues_PSNR[k] = 255;
                        }
                        else
                        {
                            rgbValues_PSNR[k] = Convert.ToByte(DCT_R_tmp1[i, j]);
                        }

                        if (DCT_G_tmp1[i, j] <= 0)
                        {
                            rgbValues_PSNR[k + 1] = 0;
                        }
                        else if ((DCT_G_tmp1[i, j] >= 255))
                        {
                            rgbValues_PSNR[k + 1] = 255;
                        }
                        else
                        {
                            rgbValues_PSNR[k + 1] = Convert.ToByte(DCT_G_tmp1[i, j]);
                        }

                        if (DCT_B_tmp1[i, j] <= 0)
                        {
                            rgbValues_PSNR[k + 2] = 0;
                        }
                        else if ((DCT_B_tmp1[i, j] >= 255))
                        {
                            rgbValues_PSNR[k + 2] = 255;
                        }
                        else
                        {
                            rgbValues_PSNR[k + 2] = Convert.ToByte(DCT_B_tmp1[i, j]);
                        }


                        if (DCT_A_tmp1[i, j] <= 0)
                        {
                            rgbValues_PSNR[k + 3] = 0;
                        }
                        else if ((DCT_A_tmp1[i, j] >= 255))
                        {
                            rgbValues_PSNR[k + 3] = 255;
                        }
                        else
                        {
                            rgbValues_PSNR[k + 3] = Convert.ToByte(DCT_A_tmp1[i, j]);
                        }

                        k += 4;
                    }
                }


            }
            //Встраивание в средних частотах
            if (checkBox3.Checked == true)
            {
                try
                {
                    byte[] byteArray;
                    BitArray bits;
                    if (label7.Text!="") //Если открыт файл, то записывается файл, иначе текст их текстбокса 
                    {
                        byteArray = File.ReadAllBytes(label7.Text);
                    }
                    else
                    {
                        byteArray = System.Text.Encoding.UTF8.GetBytes(textBox1.Text);
                    }

                    if (checkBox10.Checked == true) //Условие проведения тестирования (если тестирование то заполняем битовый массив пикселями Ч/б изображения)
                    {
                        Bitmap bmp_test = new Bitmap(pictureBox5.Image);
                       
                        // Lock the bitmap's bits.  
                        Rectangle rect_test = new Rectangle(0, 0, bmp_test.Width, bmp_test.Height);
                        System.Drawing.Imaging.BitmapData bmpData_test =
                            bmp_test.LockBits(rect_test, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                            System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
                       

                        // Get the address of the first line.
                        IntPtr ptr_test = bmpData_test.Scan0;
                        // Declare an array to hold the bytes of the bitmap.

                        int bytes_test = Math.Abs(bmpData_test.Stride) * bmp_test.Height;
                        byte[] rgbValues_test = new byte[bytes_test];                       

                        // Copy the RGB values into the array.                       
                        System.Runtime.InteropServices.Marshal.Copy(ptr_test, rgbValues_test, 0, bytes_test);
                        bits = new BitArray(bytes_test);
                        for (int t = 0; t < bytes_test; t++)
                        {
                            if (rgbValues_test[t] == 255)
                            {
                                bits[t] = true;
                            }
                            else if(rgbValues_test[t] == 0)
                            {
                                bits[t] = false;
                            }
                            else
                            {
                                textBox_testlog.Text = textBox_testlog.Text + " " + rgbValues_test[t].ToString();
                            }
                          
                            
                        }
                        textBox_testlog.Text = "Количество бит=" + bits.Length.ToString() + " Количество байт=" + rgbValues_test.Length.ToString();
                        //for(int t=0;t<10;t++)
                        //{
                        //    textBox_testlog.Text = textBox_testlog.Text + " " + rgbValues_test[t].ToString();
                        //}

                    }
                    else
                    {
                        bits = new BitArray(byteArray);
                    }
                        
                    textBox6.Text = textBox6.Text + " " + bits.Length.ToString();
                    /* for (int t = 0; t < bits.Length; t++)
                     {
                         textBox6.Text = textBox6.Text + " " +t+"-"+ bits[t].ToString()+"-"+ Convert.ToInt32(bits[t]);
                     }*/

                    // for (int t = 0; t < byteArray.Length; t++)
                    // {
                    //     textBox6.Text = textBox6.Text + " " + t + "-" + byteArray[t].ToString();
                    // }
                    //for (int t = 0; t < bits.Length; t++)
                    //{
                    //    textBox6.Text = textBox6.Text + " " + t + "-" + Convert.ToInt32(bits[t]).ToString();
                    //}
                    //textBox6.Text = textBox6.Text + "------";
                    if ((bmp.Width * bmp.Height <= bits.Length) || (bits.Length == 0))
                    {
                        throw new Exception("Ошибка");
                    }
                    int i = 0, j = 0, size = 0, flag = 0, count_bit = 0, q = 6;
                    //textBox1.Text = Convert.ToInt32(bits[count_bit]).ToString();
                    //BitArray bits_debug = new BitArray(byteArray);
                    while (size < bmp.Width * bmp.Height - 2)
                    {
                        if ((i == 0) && (j == 0))
                        {
                            size++;
                            i++;
                            flag = 0;

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        while ((flag == 0) && (i > 0) && (j < bmp.Width - 1))
                        {

                            size++;
                            i--;
                            j++;

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }


                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        //textBox1.Text += "\r\n";
                        if ((i == 0) && (j < bmp.Width - 1) && (flag == 0))
                        {
                            flag = 1;
                            j++;
                            size++;

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                // count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                // count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";

                        }
                        else if ((i < bmp.Height - 1) && (j == bmp.Width - 1) && (flag == 0))
                        {
                            flag = 1;
                            i++;
                            size++;

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                // count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        while ((flag == 1) && (i < bmp.Height - 1) && (j > 0))
                        {
                            size++;
                            i++;
                            j--;

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + (q / 2) * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j] )/ q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q *(Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        //textBox1.Text += "\r\n";
                        if ((i < bmp.Height - 1) && (j == 0) && (flag == 1))
                        {
                            flag = 0;
                            i++;
                            size++;

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j] )/ q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        else if ((i == bmp.Height - 1) && (j < bmp.Width - 1) && (flag == 1))
                        {
                            flag = 0;
                            j++;
                            size++;

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }


                    }

                    textBox1.Text = textBox1.Text + "   Байтового массива равен " + byteArray.Length.ToString();
                    //byte[] bytes_debug = new byte[Convert.ToInt32(Math.Ceiling(bits_debug.Count / 8.0))];
                    //bits_debug.CopyTo(bytes_debug, 0);
                    // for (int t = 0; t < bytes_debug.Length; t++)
                    //{
                    //    textBox6.Text = textBox6.Text + " " + t + "-" + bytes_debug[t].ToString();
                    // }
                    //for (int t = 0; t < bits.Length; t++)
                    //{
                    //    textBox6.Text = textBox6.Text + " " + t + "-" + Convert.ToInt32(bits_debug[t]).ToString();
                    //}
                    //int kolvo_oshibok = 0;
                    //for (int t = 0; t < bits.Length; t++)
                    //{
                    //    if (bits[t] != bits_debug[t])
                    //        kolvo_oshibok++;

                    //}
                    //textBox6.Text = textBox6.Text + "Ошибка = " + (Convert.ToDouble(kolvo_oshibok) / Convert.ToDouble(bits.Length) * 100).ToString();
                    //textBox6.Text = textBox6.Text + "Сообщение -> " + System.Text.Encoding.UTF8.GetString(bytes_debug);


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //Встраивание в высоких частотах
            if (checkBox4.Checked == true)
            {
                try
                {

                    BitArray bits;
                    byte[] byteArray;
                    if (label7.Text != "") //Если открыт файл, то записывается файл, иначе текст их текстбокса 
                    {
                        byteArray = File.ReadAllBytes(label7.Text);
                    }
                    else
                    {
                        byteArray = System.Text.Encoding.UTF8.GetBytes(textBox1.Text);
                    }
                    if (checkBox10.Checked == true) //Условие проведения тестирования (если тестирование то заполняем битовый массив пикселями Ч/б изображения)
                    {
                        Bitmap bmp_test = new Bitmap(pictureBox5.Image);

                        // Lock the bitmap's bits.  
                        Rectangle rect_test = new Rectangle(0, 0, bmp_test.Width, bmp_test.Height);
                        System.Drawing.Imaging.BitmapData bmpData_test =
                            bmp_test.LockBits(rect_test, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                            System.Drawing.Imaging.PixelFormat.Format1bppIndexed);


                        // Get the address of the first line.
                        IntPtr ptr_test = bmpData_test.Scan0;
                        // Declare an array to hold the bytes of the bitmap.

                        int bytes_test = Math.Abs(bmpData_test.Stride) * bmp_test.Height;
                        byte[] rgbValues_test = new byte[bytes_test];

                        // Copy the RGB values into the array.                       
                        System.Runtime.InteropServices.Marshal.Copy(ptr_test, rgbValues_test, 0, bytes_test);
                        bits = new BitArray(bytes_test);
                        for (int t = 0; t < bytes_test; t++)
                        {
                            if (rgbValues_test[t] == 255)
                            {
                                bits[t] = true;
                            }
                            else if (rgbValues_test[t] == 0)
                            {
                                bits[t] = false;
                            }
                            else
                            {
                                textBox_testlog.Text = textBox_testlog.Text + " " + rgbValues_test[t].ToString();
                            }


                        }
                        textBox_testlog.Text = "Количество бит=" + bits.Length.ToString() + " Количество байт=" + rgbValues_test.Length.ToString();
                        //for(int t=0;t<10;t++)
                        //{
                        //    textBox_testlog.Text = textBox_testlog.Text + " " + rgbValues_test[t].ToString();
                        //}

                    }
                    else
                    {
                        bits = new BitArray(byteArray);
                    }

                   
                    textBox6.Text = textBox6.Text + " " + bits.Length.ToString();
                    /* for (int t = 0; t < bits.Length; t++)
                     {
                         textBox6.Text = textBox6.Text + " " +t+"-"+ bits[t].ToString()+"-"+ Convert.ToInt32(bits[t]);
                     }*/

                    // for (int t = 0; t < byteArray.Length; t++)
                    // {
                    //     textBox6.Text = textBox6.Text + " " + t + "-" + byteArray[t].ToString();
                    // }
                    //for (int t = 0; t < bits.Length; t++)
                    //{
                    //    textBox6.Text = textBox6.Text + " " + t + "-" + Convert.ToInt32(bits[t]).ToString();
                    //}
                    //textBox6.Text = textBox6.Text + "------";
                    if ((bmp.Width * bmp.Height <= bits.Length) || (bits.Length == 0))
                    {
                        throw new Exception("Ошибка");
                    }
                    int i = 0, j = 0, size = 0, flag = 0, count_bit = 0, q = 6;
                    //textBox1.Text = Convert.ToInt32(bits[count_bit]).ToString();
                    //BitArray bits_debug = new BitArray(byteArray);
                    while (size < bmp.Width * bmp.Height - 2)
                    {
                        if ((i == 0) && (j == 0))
                        {
                            size++;
                            i++;
                            flag = 0;

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        while ((flag == 0) && (i > 0) && (j < bmp.Width - 1))
                        {

                            size++;
                            i--;
                            j++;

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }


                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        //textBox1.Text += "\r\n";
                        if ((i == 0) && (j < bmp.Width - 1) && (flag == 0))
                        {
                            flag = 1;
                            j++;
                            size++;

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                // count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                // count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";

                        }
                        else if ((i < bmp.Height - 1) && (j == bmp.Width - 1) && (flag == 0))
                        {
                            flag = 1;
                            i++;
                            size++;

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                // count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        while ((flag == 1) && (i < bmp.Height - 1) && (j > 0))
                        {
                            size++;
                            i++;
                            j--;

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + (q / 2) * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j] )/ q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q *(Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        //textBox1.Text += "\r\n";
                        if ((i < bmp.Height - 1) && (j == 0) && (flag == 1))
                        {
                            flag = 0;
                            i++;
                            size++;

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j] )/ q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        else if ((i == bmp.Height - 1) && (j < bmp.Width - 1) && (flag == 1))
                        {
                            flag = 0;
                            j++;
                            size++;

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }


                    }

                    textBox1.Text = textBox1.Text + "   Байтового массива равен " + byteArray.Length.ToString();
                    //byte[] bytes_debug = new byte[Convert.ToInt32(Math.Ceiling(bits_debug.Count / 8.0))];
                    //bits_debug.CopyTo(bytes_debug, 0);
                    // for (int t = 0; t < bytes_debug.Length; t++)
                    //{
                    //    textBox6.Text = textBox6.Text + " " + t + "-" + bytes_debug[t].ToString();
                    // }
                    //for (int t = 0; t < bits.Length; t++)
                    //{
                    //    textBox6.Text = textBox6.Text + " " + t + "-" + Convert.ToInt32(bits_debug[t]).ToString();
                    //}
                    //int kolvo_oshibok = 0;
                    //for (int t = 0; t < bits.Length; t++)
                    //{
                    //    if (bits[t] != bits_debug[t])
                    //        kolvo_oshibok++;

                    //}
                    //textBox6.Text = textBox6.Text + "Ошибка = " + (Convert.ToDouble(kolvo_oshibok) / Convert.ToDouble(bits.Length) * 100).ToString();
                    //textBox6.Text = textBox6.Text + "Сообщение -> " + System.Text.Encoding.UTF8.GetString(bytes_debug);


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //Встраивание в низких частотах
            if (checkBox7.Checked == true)
            {
                try
                {

                    BitArray bits;
                    byte[] byteArray;
                    if (label7.Text != "") //Если открыт файл, то записывается файл, иначе текст их текстбокса 
                    {
                        byteArray = File.ReadAllBytes(label7.Text);
                    }
                    else
                    {
                        byteArray = System.Text.Encoding.UTF8.GetBytes(textBox1.Text);
                    }
                    if (checkBox10.Checked == true) //Условие проведения тестирования (если тестирование то заполняем битовый массив пикселями Ч/б изображения)
                    {
                        Bitmap bmp_test = new Bitmap(pictureBox5.Image);

                        // Lock the bitmap's bits.  
                        Rectangle rect_test = new Rectangle(0, 0, bmp_test.Width, bmp_test.Height);
                        System.Drawing.Imaging.BitmapData bmpData_test =
                            bmp_test.LockBits(rect_test, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                            System.Drawing.Imaging.PixelFormat.Format1bppIndexed);


                        // Get the address of the first line.
                        IntPtr ptr_test = bmpData_test.Scan0;
                        // Declare an array to hold the bytes of the bitmap.

                        int bytes_test = Math.Abs(bmpData_test.Stride) * bmp_test.Height;
                        byte[] rgbValues_test = new byte[bytes_test];

                        // Copy the RGB values into the array.                       
                        System.Runtime.InteropServices.Marshal.Copy(ptr_test, rgbValues_test, 0, bytes_test);
                        bits = new BitArray(bytes_test);
                        for (int t = 0; t < bytes_test; t++)
                        {
                            if (rgbValues_test[t] == 255)
                            {
                                bits[t] = true;
                            }
                            else if (rgbValues_test[t] == 0)
                            {
                                bits[t] = false;
                            }
                            else
                            {
                                textBox_testlog.Text = textBox_testlog.Text + " " + rgbValues_test[t].ToString();
                            }


                        }
                        textBox_testlog.Text = "Количество бит=" + bits.Length.ToString() + " Количество байт=" + rgbValues_test.Length.ToString();
                        //for(int t=0;t<10;t++)
                        //{
                        //    textBox_testlog.Text = textBox_testlog.Text + " " + rgbValues_test[t].ToString();
                        //}

                    }
                    else
                    {
                        bits = new BitArray(byteArray);
                    }
                   
                    textBox6.Text = textBox6.Text + " " + bits.Length.ToString();
                    /* for (int t = 0; t < bits.Length; t++)
                     {
                         textBox6.Text = textBox6.Text + " " +t+"-"+ bits[t].ToString()+"-"+ Convert.ToInt32(bits[t]);
                     }*/

                    // for (int t = 0; t < byteArray.Length; t++)
                    // {
                    //     textBox6.Text = textBox6.Text + " " + t + "-" + byteArray[t].ToString();
                    // }
                    //for (int t = 0; t < bits.Length; t++)
                    //{
                    //    textBox6.Text = textBox6.Text + " " + t + "-" + Convert.ToInt32(bits[t]).ToString();
                    //}
                    //textBox6.Text = textBox6.Text + "------";
                    if ((bmp.Width * bmp.Height <= bits.Length) || (bits.Length == 0))
                    {
                        throw new Exception("Ошибка");
                    }
                    int i = 0, j = 0, size = 0, flag = 0, count_bit = 0, q = 6;
                    //textBox1.Text = Convert.ToInt32(bits[count_bit]).ToString();
                    //BitArray bits_debug = new BitArray(byteArray);
                    while (size < bmp.Width * bmp.Height - 2)
                    {
                        if ((i == 0) && (j == 0))
                        {
                            size++;
                            i++;
                            flag = 0;

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        while ((flag == 0) && (i > 0) && (j < bmp.Width - 1))
                        {

                            size++;
                            i--;
                            j++;

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }


                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        //textBox1.Text += "\r\n";
                        if ((i == 0) && (j < bmp.Width - 1) && (flag == 0))
                        {
                            flag = 1;
                            j++;
                            size++;

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                // count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                // count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";

                        }
                        else if ((i < bmp.Height - 1) && (j == bmp.Width - 1) && (flag == 0))
                        {
                            flag = 1;
                            i++;
                            size++;

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                // count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        while ((flag == 1) && (i < bmp.Height - 1) && (j > 0))
                        {
                            size++;
                            i++;
                            j--;

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + (q / 2) * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j] )/ q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q *(Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        //textBox1.Text += "\r\n";
                        if ((i < bmp.Height - 1) && (j == 0) && (flag == 1))
                        {
                            flag = 0;
                            i++;
                            size++;

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j] )/ q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        else if ((i == bmp.Height - 1) && (j < bmp.Width - 1) && (flag == 1))
                        {
                            flag = 0;
                            j++;
                            size++;

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                //textBox6.Text = textBox6.Text + "penis->" + (DCT_R[i, j]).ToString();
                                DCT_R[i, j] = Math.Sign(DCT_R[i, j]) * (q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_R[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_G[i, j]).ToString();
                                DCT_G[i, j] = Math.Sign(DCT_G[i, j]) * (q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q *(Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_G[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            if ((count_bit < bits.Length - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                // textBox6.Text = textBox6.Text + "penis->" + (DCT_B[i, j]).ToString();
                                DCT_B[i, j] = Math.Sign(DCT_B[i, j]) * (q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)) + q / 2 * Convert.ToInt32(bits[count_bit]));

                                //if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                //{
                                //    bits_debug[count_bit] = true;
                                //}
                                //else
                                //{
                                //    bits_debug[count_bit] = false;
                                //}
                                //textBox6.Text = textBox6.Text + "pipiska->" + (DCT_B[i, j]).ToString();
                                count_bit++;
                                //count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }


                    }

                    textBox1.Text = textBox1.Text + "   Байтового массива равен " + byteArray.Length.ToString();
                    //byte[] bytes_debug = new byte[Convert.ToInt32(Math.Ceiling(bits_debug.Count / 8.0))];
                    //bits_debug.CopyTo(bytes_debug, 0);
                    // for (int t = 0; t < bytes_debug.Length; t++)
                    //{
                    //    textBox6.Text = textBox6.Text + " " + t + "-" + bytes_debug[t].ToString();
                    // }
                    //for (int t = 0; t < bits.Length; t++)
                    //{
                    //    textBox6.Text = textBox6.Text + " " + t + "-" + Convert.ToInt32(bits_debug[t]).ToString();
                    //}
                    //int kolvo_oshibok = 0;
                    //for (int t = 0; t < bits.Length; t++)
                    //{
                    //    if (bits[t] != bits_debug[t])
                    //        kolvo_oshibok++;

                    //}
                    //textBox6.Text = textBox6.Text + "Ошибка = " + (Convert.ToDouble(kolvo_oshibok) / Convert.ToDouble(bits.Length) * 100).ToString();
                    //textBox6.Text = textBox6.Text + "Сообщение -> " + System.Text.Encoding.UTF8.GetString(bytes_debug);


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            // Accord.Math.CosineTransform.IDCT(DCT_R); // IDCT по каждому каналу 
            // Accord.Math.CosineTransform.IDCT(DCT_G);
            // Accord.Math.CosineTransform.IDCT(DCT_B);

            IDCT_test(DCT_R); // IDCT по каждому каналу 
            IDCT_test(DCT_G);
            IDCT_test(DCT_B);
            //IDCT_test(DCT_A);

            k = 0;
            for (int i = 0; i < bmp.Height; i++) //Перезаполнение байтового массива для вывода картинки
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    if (DCT_R[i, j] <= 0)
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
                        rgbValues[k + 1] = 0;
                    }
                    else if ((DCT_G[i, j] >= 255))
                    {
                        rgbValues[k + 1] = 255;
                    }
                    else
                    {
                        rgbValues[k + 1] = Convert.ToByte(DCT_G[i, j]);
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
            // расчет корреляции
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


            //Его величество PSNR            
            if (checkBox9.Checked == true)
            {
                
                    double psnr = 0, psnr1 = 0, psnr2 = 0;
                    double MSE = 0, MSE1 = 0, MSE2 = 0;
                    for (int i = 0; i < bytes1; i+=4)
                    {

                    MSE = MSE + (rgbValues_PSNR[i] - rgbValues[i]) * (rgbValues_PSNR[i] - rgbValues[i]);
                    MSE1 = MSE1 + (rgbValues_PSNR[i+1] - rgbValues[i+1]) * (rgbValues_PSNR[i+1] - rgbValues[i+1]);
                    MSE2 = MSE2 + (rgbValues_PSNR[i+2] - rgbValues[i+2]) * (rgbValues_PSNR[i+2] - rgbValues[i+2]);
                    
            }
                    
                MSE = MSE / ((double)bmp.Width * (double)bmp.Height );
                MSE1 = MSE1 / ((double)bmp.Width * (double)bmp.Height);
                MSE2 = MSE2 / ((double)bmp.Width * (double)bmp.Height);
                psnr = 20 * Math.Log( 255.0d / Math.Sqrt(MSE),10);
                psnr1 = 20 * Math.Log(  255.0d / Math.Sqrt(MSE1),10);
                psnr2 = 20 * Math.Log( 255.0d / Math.Sqrt(MSE2),10);
                psnr = (psnr + psnr1 + psnr2) / 3.0;
                label4.Text = psnr.ToString();                

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

        private void button1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            if (ofd.ShowDialog(this) == DialogResult.OK)
                pictureBox4.Image = Image.FromFile(ofd.FileName);

        }

        private void button_back_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox3.Image);

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


            //textBox1.Text = textBox1.Text + bmp.Width.ToString() + "x" + bmp.Height.ToString() + "        " + bytes.ToString() + "=" + (3* bmp.Height * bmp.Width).ToString() + "   ";
            Accord.Math.CosineTransform.DCT(DCT_R); // DCT по каждому каналу 
            Accord.Math.CosineTransform.DCT(DCT_G);
            Accord.Math.CosineTransform.DCT(DCT_B);
            //Accord.Math.CosineTransform.DCT(DCT_A);// Альфа канал под вопросом
            //Извлечение из средних частот
            if (checkBox5.Checked == true)
            { 
                try
            {
                int lenght_bitarr = Convert.ToInt32(textBox7.Text) * 8;
                BitArray bits = new BitArray(lenght_bitarr);
                textBox6.Text = textBox6.Text + " " + lenght_bitarr.ToString();

                int i = 0, j = 0, size = 0, flag = 0, count_bit = 0, q = 6;
                //textBox1.Text = Convert.ToInt32(bits[count_bit]).ToString();
                while (size < bmp.Width * bmp.Height - 2)
                {
                    if ((i == 0) && (j == 0))
                    {
                        size++;
                        i++;
                        flag = 0;

                        //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                    }
                    while ((flag == 0) && (i > 0) && (j < bmp.Width - 1))
                    {

                        size++;
                        i--;
                        j++;

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }


                        //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                    }
                    //textBox1.Text += "\r\n";
                    if ((i == 0) && (j < bmp.Width - 1) && (flag == 0))
                    {
                        flag = 1;
                        j++;
                        size++;

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";

                    }
                    else if ((i < bmp.Height - 1) && (j == bmp.Width - 1) && (flag == 0))
                    {
                        flag = 1;
                        i++;
                        size++;

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                    }
                    while ((flag == 1) && (i < bmp.Height - 1) && (j > 0))
                    {
                        size++;
                        i++;
                        j--;

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                    }
                    //textBox1.Text += "\r\n";
                    if ((i < bmp.Height - 1) && (j == 0) && (flag == 1))
                    {
                        flag = 0;
                        i++;
                        size++;

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                    }
                    else if ((i == bmp.Height - 1) && (j < bmp.Width - 1) && (flag == 1))
                    {
                        flag = 0;
                        j++;
                        size++;

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 < size) && (size < 2 * bmp.Width * bmp.Height / 3))
                        {
                            if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                            {
                                bits[count_bit] = true;
                            }
                            else
                            {
                                bits[count_bit] = false;
                            }
                            count_bit++;
                        }

                        //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                    }

                }

                    /* for (int t = 0; t < bits.Length; t++)
                     {
                         textBox6.Text = textBox6.Text + " " + t + "-"  + bits[t].ToString();
                     }*/
                    if (checkBox10.Checked == true) //Проверка на тест, если тест вытаскиваем картинку
                    {


                        // делаем массив байтов для записи в картинку, где 1 биту будет равно 4 байта
                        ///System.Drawing.Imaging.PixelFormat format = //Canonical;
                        //Bitmap bmp_test = new Bitmap(Convert.ToInt32(23), Convert.ToInt32(44), System.Drawing.Imaging.PixelFormat.Canonical);

                        //int width = bmp_test.Width;
                        //int height = bmp_test.Height;

                        //Rectangle rect_test = new Rectangle(0, 0, width, height);
                        //System.Drawing.Imaging.BitmapData bmpData_test = bmp_test.LockBits(rect_test, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp_test.PixelFormat);
                        //IntPtr ptr_test = bmpData_test.Scan0;

                        //int bytes_test = Math.Abs(bmpData_test.Stride) * height;
                        //textBox6.Text = textBox6.Text + " bytes_test=" + bytes_test.ToString();
                        //byte[] rgbValues_test = new byte[bytes_test];

                        Bitmap bmp_test = new Bitmap(pictureBox5.Image);
                        // Lock the bitmap's bits.  
                        Rectangle rect_test = new Rectangle(0, 0, bmp_test.Width, bmp_test.Height);
                        System.Drawing.Imaging.BitmapData bmpData_test =
                            bmp_test.LockBits(rect_test, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                            System.Drawing.Imaging.PixelFormat.Format1bppIndexed);


                        // Get the address of the first line.
                        IntPtr ptr_test = bmpData_test.Scan0;
                        // Declare an array to hold the bytes of the bitmap.

                        int bytes_test = Math.Abs(bmpData_test.Stride) * bmp_test.Height;
                        byte[] rgbValues_test = new byte[bytes_test];
                        byte[] rgbValues_test_tmp = new byte[bytes_test];
                        
                        System.Runtime.InteropServices.Marshal.Copy(ptr_test, rgbValues_test_tmp, 0, bytes_test);
                        for (int t = 0; t < bits.Length; t++)
                        {
                            if (bits[t] == true) // пишем черный цвет в пиксель
                            {
                                rgbValues_test[t] = 255; //R

                            }
                            else //иначе пишем белый
                            {
                                rgbValues_test[t] = 0; //R
                            }
                        }
                        System.Runtime.InteropServices.Marshal.Copy(rgbValues_test, 0, ptr_test, bytes_test);
                        bmp_test.UnlockBits(bmpData_test);
                        pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox6.Image = bmp_test;
                       
                        int kolvo_oshibok = 0;
                        for (int t = 0; t < bits.Length; t++)
                        {
                            if (rgbValues_test[t] != rgbValues_test_tmp[t])
                                kolvo_oshibok++;

                        }
                        textBox_testlog.Text = textBox_testlog.Text + "  Ошибка = " + (Convert.ToDouble(kolvo_oshibok) / Convert.ToDouble(bits.Length) * 100).ToString();
                    }

                    else
                    {
                        byte[] bytes_exit = new byte[Convert.ToInt32(Math.Ceiling(bits.Count / 8.0))]; //Теоретически должно переводить массив бит в массив байт
                        bits.CopyTo(bytes_exit, 0);

                        if (label7.Text != "") //Если открыт файл, то вытягивается файл, иначе вытягивается текст и заносится в текстбокса 
                        {
                            File.WriteAllBytes("C:\\Users\\Public\\Documents\\Выход.txt", bytes_exit);
                            byte[] byteArray = byteArray = File.ReadAllBytes(label7.Text);
                            BitArray bits_debug = new BitArray(byteArray);
                            int kolvo_oshibok = 0;
                            for (int t = 0; t < bits.Length; t++)
                            {
                                if (bits[t] != bits_debug[t])
                                    kolvo_oshibok++;

                            }
                            textBox6.Text = textBox6.Text + "  Ошибка = " + (Convert.ToDouble(kolvo_oshibok) / Convert.ToDouble(bits.Length) * 100).ToString();
                        }
                        else
                        {
                            textBox6.Text = textBox6.Text + System.Text.Encoding.UTF8.GetString(bytes_exit);
                            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(textBox1.Text);
                            BitArray bits_debug = new BitArray(byteArray);
                            int kolvo_oshibok = 0;
                            for (int t = 0; t < bits.Length; t++)
                            {
                                if (bits[t] != bits_debug[t])
                                    kolvo_oshibok++;

                            }
                            textBox6.Text = textBox6.Text + "  Ошибка = " + (Convert.ToDouble(kolvo_oshibok) / Convert.ToDouble(bits.Length) * 100).ToString();
                        }
                    }

               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            }
            //Извлечение из высоких частот
            if (checkBox6.Checked == true)
            {
                try
                {
                    int lenght_bitarr = Convert.ToInt32(textBox7.Text) * 8;
                    BitArray bits = new BitArray(lenght_bitarr);
                    textBox6.Text = textBox6.Text + " " + lenght_bitarr.ToString();

                    int i = 0, j = 0, size = 0, flag = 0, count_bit = 0, q = 6;
                    //textBox1.Text = Convert.ToInt32(bits[count_bit]).ToString();
                    while (size < bmp.Width * bmp.Height - 2)
                    {
                        if ((i == 0) && (j == 0))
                        {
                            size++;
                            i++;
                            flag = 0;

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        while ((flag == 0) && (i > 0) && (j < bmp.Width - 1))
                        {

                            size++;
                            i--;
                            j++;

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }


                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        //textBox1.Text += "\r\n";
                        if ((i == 0) && (j < bmp.Width - 1) && (flag == 0))
                        {
                            flag = 1;
                            j++;
                            size++;

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";

                        }
                        else if ((i < bmp.Height - 1) && (j == bmp.Width - 1) && (flag == 0))
                        {
                            flag = 1;
                            i++;
                            size++;

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        while ((flag == 1) && (i < bmp.Height - 1) && (j > 0))
                        {
                            size++;
                            i++;
                            j--;

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        //textBox1.Text += "\r\n";
                        if ((i < bmp.Height - 1) && (j == 0) && (flag == 1))
                        {
                            flag = 0;
                            i++;
                            size++;

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        else if ((i == bmp.Height - 1) && (j < bmp.Width - 1) && (flag == 1))
                        {
                            flag = 0;
                            j++;
                            size++;

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (size > 2 * bmp.Width * bmp.Height / 3))
                            {
                                if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }

                    }

                    /* for (int t = 0; t < bits.Length; t++)
                     {
                         textBox6.Text = textBox6.Text + " " + t + "-"  + bits[t].ToString();
                     }*/
                    if (checkBox10.Checked == true) //Проверка на тест, если тест вытаскиваем картинку
                    {


                        // делаем массив байтов для записи в картинку, где 1 биту будет равно 4 байта
                        ///System.Drawing.Imaging.PixelFormat format = //Canonical;
                        //Bitmap bmp_test = new Bitmap(Convert.ToInt32(23), Convert.ToInt32(44), System.Drawing.Imaging.PixelFormat.Canonical);

                        //int width = bmp_test.Width;
                        //int height = bmp_test.Height;

                        //Rectangle rect_test = new Rectangle(0, 0, width, height);
                        //System.Drawing.Imaging.BitmapData bmpData_test = bmp_test.LockBits(rect_test, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp_test.PixelFormat);
                        //IntPtr ptr_test = bmpData_test.Scan0;

                        //int bytes_test = Math.Abs(bmpData_test.Stride) * height;
                        //textBox6.Text = textBox6.Text + " bytes_test=" + bytes_test.ToString();
                        //byte[] rgbValues_test = new byte[bytes_test];

                        Bitmap bmp_test = new Bitmap(pictureBox5.Image);
                        // Lock the bitmap's bits.  
                        Rectangle rect_test = new Rectangle(0, 0, bmp_test.Width, bmp_test.Height);
                        System.Drawing.Imaging.BitmapData bmpData_test =
                            bmp_test.LockBits(rect_test, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                            System.Drawing.Imaging.PixelFormat.Format1bppIndexed);


                        // Get the address of the first line.
                        IntPtr ptr_test = bmpData_test.Scan0;
                        // Declare an array to hold the bytes of the bitmap.

                        int bytes_test = Math.Abs(bmpData_test.Stride) * bmp_test.Height;
                        byte[] rgbValues_test = new byte[bytes_test];
                        byte[] rgbValues_test_tmp = new byte[bytes_test];

                        System.Runtime.InteropServices.Marshal.Copy(ptr_test, rgbValues_test_tmp, 0, bytes_test);
                        for (int t = 0; t < bits.Length; t++)
                        {
                            if (bits[t] == true) // пишем черный цвет в пиксель
                            {
                                rgbValues_test[t] = 255; //R

                            }
                            else //иначе пишем белый
                            {
                                rgbValues_test[t] = 0; //R
                            }
                        }
                        System.Runtime.InteropServices.Marshal.Copy(rgbValues_test, 0, ptr_test, bytes_test);
                        bmp_test.UnlockBits(bmpData_test);
                        pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox6.Image = bmp_test;

                        int kolvo_oshibok = 0;
                        for (int t = 0; t < bits.Length; t++)
                        {
                            if (rgbValues_test[t] != rgbValues_test_tmp[t])
                                kolvo_oshibok++;

                        }
                        textBox_testlog.Text = textBox_testlog.Text + "  Ошибка = " + (Convert.ToDouble(kolvo_oshibok) / Convert.ToDouble(bits.Length) * 100).ToString();
                    }

                    else
                    {
                        byte[] bytes_exit = new byte[Convert.ToInt32(Math.Ceiling(bits.Count / 8.0))]; //Теоретически должно переводить массив бит в массив байт
                        bits.CopyTo(bytes_exit, 0);

                        if (label7.Text != "") //Если открыт файл, то вытягивается файл, иначе вытягивается текст и заносится в текстбокса 
                        {
                            File.WriteAllBytes("C:\\Users\\Public\\Documents\\Выход.txt", bytes_exit);
                            byte[] byteArray = byteArray = File.ReadAllBytes(label7.Text);
                            BitArray bits_debug = new BitArray(byteArray);
                            int kolvo_oshibok = 0;
                            for (int t = 0; t < bits.Length; t++)
                            {
                                if (bits[t] != bits_debug[t])
                                    kolvo_oshibok++;

                            }
                            textBox6.Text = textBox6.Text + "  Ошибка = " + (Convert.ToDouble(kolvo_oshibok) / Convert.ToDouble(bits.Length) * 100).ToString();
                        }
                        else
                        {
                            textBox6.Text = textBox6.Text + System.Text.Encoding.UTF8.GetString(bytes_exit);
                            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(textBox1.Text);
                            BitArray bits_debug = new BitArray(byteArray);
                            int kolvo_oshibok = 0;
                            for (int t = 0; t < bits.Length; t++)
                            {
                                if (bits[t] != bits_debug[t])
                                    kolvo_oshibok++;

                            }
                            textBox6.Text = textBox6.Text + "  Ошибка = " + (Convert.ToDouble(kolvo_oshibok) / Convert.ToDouble(bits.Length) * 100).ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //Извлечение из низких частот
            if (checkBox8.Checked == true)
            {
                try
                {
                    int lenght_bitarr = Convert.ToInt32(textBox7.Text) * 8;
                    BitArray bits = new BitArray(lenght_bitarr);
                    textBox6.Text = textBox6.Text + " " + lenght_bitarr.ToString();

                    int i = 0, j = 0, size = 0, flag = 0, count_bit = 0, q = 6;
                    //textBox1.Text = Convert.ToInt32(bits[count_bit]).ToString();
                    while (size < bmp.Width * bmp.Height - 2)
                    {
                        if ((i == 0) && (j == 0))
                        {
                            size++;
                            i++;
                            flag = 0;

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        while ((flag == 0) && (i > 0) && (j < bmp.Width - 1))
                        {

                            size++;
                            i--;
                            j++;

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }


                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        //textBox1.Text += "\r\n";
                        if ((i == 0) && (j < bmp.Width - 1) && (flag == 0))
                        {
                            flag = 1;
                            j++;
                            size++;

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";

                        }
                        else if ((i < bmp.Height - 1) && (j == bmp.Width - 1) && (flag == 0))
                        {
                            flag = 1;
                            i++;
                            size++;

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        while ((flag == 1) && (i < bmp.Height - 1) && (j > 0))
                        {
                            size++;
                            i++;
                            j--;

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        //textBox1.Text += "\r\n";
                        if ((i < bmp.Height - 1) && (j == 0) && (flag == 1))
                        {
                            flag = 0;
                            i++;
                            size++;

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }
                        else if ((i == bmp.Height - 1) && (j < bmp.Width - 1) && (flag == 1))
                        {
                            flag = 0;
                            j++;
                            size++;

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q))) > Math.Abs(DCT_R[i, j] - Math.Sign(DCT_R[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_R[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q))) > Math.Abs(DCT_G[i, j] - Math.Sign(DCT_G[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_G[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            if ((count_bit < lenght_bitarr - 1) && (bmp.Width * bmp.Height / 3 > size))
                            {
                                if (Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q))) > Math.Abs(DCT_B[i, j] - Math.Sign(DCT_B[i, j]) * (q / 2 + q * (Math.Floor(Math.Abs(DCT_B[i, j]) / q)))))
                                {
                                    bits[count_bit] = true;
                                }
                                else
                                {
                                    bits[count_bit] = false;
                                }
                                count_bit++;
                            }

                            //textBox1.Text = textBox1.Text + " " + size + "-[" + i + "][" + j + "]";
                        }

                    }

                    /* for (int t = 0; t < bits.Length; t++)
                     {
                         textBox6.Text = textBox6.Text + " " + t + "-"  + bits[t].ToString();
                     }*/
                    if (checkBox10.Checked == true) //Проверка на тест, если тест вытаскиваем картинку
                    {


                        // делаем массив байтов для записи в картинку, где 1 биту будет равно 4 байта
                        ///System.Drawing.Imaging.PixelFormat format = //Canonical;
                        //Bitmap bmp_test = new Bitmap(Convert.ToInt32(23), Convert.ToInt32(44), System.Drawing.Imaging.PixelFormat.Canonical);

                        //int width = bmp_test.Width;
                        //int height = bmp_test.Height;

                        //Rectangle rect_test = new Rectangle(0, 0, width, height);
                        //System.Drawing.Imaging.BitmapData bmpData_test = bmp_test.LockBits(rect_test, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp_test.PixelFormat);
                        //IntPtr ptr_test = bmpData_test.Scan0;

                        //int bytes_test = Math.Abs(bmpData_test.Stride) * height;
                        //textBox6.Text = textBox6.Text + " bytes_test=" + bytes_test.ToString();
                        //byte[] rgbValues_test = new byte[bytes_test];

                        Bitmap bmp_test = new Bitmap(pictureBox5.Image);
                        // Lock the bitmap's bits.  
                        Rectangle rect_test = new Rectangle(0, 0, bmp_test.Width, bmp_test.Height);
                        System.Drawing.Imaging.BitmapData bmpData_test =
                            bmp_test.LockBits(rect_test, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                            System.Drawing.Imaging.PixelFormat.Format1bppIndexed);


                        // Get the address of the first line.
                        IntPtr ptr_test = bmpData_test.Scan0;
                        // Declare an array to hold the bytes of the bitmap.

                        int bytes_test = Math.Abs(bmpData_test.Stride) * bmp_test.Height;
                        byte[] rgbValues_test = new byte[bytes_test];
                        byte[] rgbValues_test_tmp = new byte[bytes_test];

                        System.Runtime.InteropServices.Marshal.Copy(ptr_test, rgbValues_test_tmp, 0, bytes_test);
                        for (int t = 0; t < bits.Length; t++)
                        {
                            if (bits[t] == true) // пишем черный цвет в пиксель
                            {
                                rgbValues_test[t] = 255; //R

                            }
                            else //иначе пишем белый
                            {
                                rgbValues_test[t] = 0; //R
                            }
                        }
                        System.Runtime.InteropServices.Marshal.Copy(rgbValues_test, 0, ptr_test, bytes_test);
                        bmp_test.UnlockBits(bmpData_test);
                        pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox6.Image = bmp_test;

                        int kolvo_oshibok = 0;
                        for (int t = 0; t < bits.Length; t++)
                        {
                            if (rgbValues_test[t] != rgbValues_test_tmp[t])
                                kolvo_oshibok++;

                        }
                        textBox_testlog.Text = textBox_testlog.Text + "  Ошибка = " + (Convert.ToDouble(kolvo_oshibok) / Convert.ToDouble(bits.Length) * 100).ToString();
                    }

                    else
                    {
                        byte[] bytes_exit = new byte[Convert.ToInt32(Math.Ceiling(bits.Count / 8.0))]; //Теоретически должно переводить массив бит в массив байт
                        bits.CopyTo(bytes_exit, 0);

                        if (label7.Text != "") //Если открыт файл, то вытягивается файл, иначе вытягивается текст и заносится в текстбокса 
                        {
                            File.WriteAllBytes("C:\\Users\\Public\\Documents\\Выход.txt", bytes_exit);
                            byte[] byteArray = byteArray = File.ReadAllBytes(label7.Text);
                            BitArray bits_debug = new BitArray(byteArray);
                            int kolvo_oshibok = 0;
                            for (int t = 0; t < bits.Length; t++)
                            {
                                if (bits[t] != bits_debug[t])
                                    kolvo_oshibok++;

                            }
                            textBox6.Text = textBox6.Text + "  Ошибка = " + (Convert.ToDouble(kolvo_oshibok) / Convert.ToDouble(bits.Length) * 100).ToString();
                        }
                        else
                        {
                            textBox6.Text = textBox6.Text + System.Text.Encoding.UTF8.GetString(bytes_exit);
                            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(textBox1.Text);
                            BitArray bits_debug = new BitArray(byteArray);
                            int kolvo_oshibok = 0;
                            for (int t = 0; t < bits.Length; t++)
                            {
                                if (bits[t] != bits_debug[t])
                                    kolvo_oshibok++;

                            }
                            textBox6.Text = textBox6.Text + "  Ошибка = " + (Convert.ToDouble(kolvo_oshibok) / Convert.ToDouble(bits.Length) * 100).ToString();
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }



        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button_openfile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                label7.Text = openFileDialog1.FileName;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            if (ofd.ShowDialog(this) == DialogResult.OK)
                pictureBox5.Image = Image.FromFile(ofd.FileName);
        }
    }
        
    
}
