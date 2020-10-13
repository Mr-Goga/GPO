using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
        private void GaussFilter (double [,] data, double frequency)
        {
            int lines = data.GetLength(0);
            int columns = data.GetLength(1);
            double d, r;
            for (int i = 0; i < lines; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    data[i, j] = data[i, j] * frequency;
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
            double[] DCT1 = new double[bytes];
            // Copy the RGB values into the array.
            //System.Runtime.InteropServices.Marshal.Copy(ptr, DCT1, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            rgbValues.CopyTo(DCT1,0);
            Accord.Math.CosineTransform.DCT(DCT1);
            
            for (int i=0; i < 15; i++)
            {
                textBox1.Text = textBox1.Text + rgbValues[i] + "-" + DCT1[i] + "  ";
            }









            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
           
            // Unlock the bits.
            bmp.UnlockBits(bmpData);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            double[,] test = new double[5,8];
            string txt = null;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    test[i, j] = rnd.Next(0, 10);
                    txt = txt + " " + test[i, j].ToString();
                }
            }
            GaussFilter(test, 5);
            txt = txt + "__";
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    txt = txt + " " + test[i, j].ToString();
                }
            }
            textBox2.Text = txt;
            
        }
    }
}
