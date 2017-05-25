using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandlebrotJuliaSets
{
    public partial class Form1 : Form
    {
        public static List<List<double>> pastValues = new List<List<double>>();
        public static double maxX;
        public static double minX;
        public static double maxY;
        public static double minY;
        public static double initMaxX = 1;
        public static double initMinX = -2.1;
        public static double initMaxY = 1.3;
        public static double initMinY = -1.3;
        public static int num;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //set zoom button to not visible
            btn2.Visible = false;
            bar1.Visible = false;
            bar2.Visible = false;
            bar1.Maximum = picBox1.Width;
            bar2.Maximum = picBox1.Height;

            //all initial zoom values
            maxX = 1;
            maxY = 1.3;
            minX = -2.1;
            minY = -1.3;

            //counter to monitor zoom levels
            num = 0;

            //keep track of zoom values in order to revert back to previous zoom
            pastValues.Add(new List<double>());
            pastValues[num].Add(maxX);
            pastValues[num].Add(maxY);
            pastValues[num].Add(minX);
            pastValues[num].Add(minY);

            Bitmap pic = Mandelbrotset(picBox1, maxX, minX, maxY, minY);
            Bitmap pic2 = Mandelbrotset(picBox2, maxX, minX, maxY, minY, 0, 0, true);
            picBox1.Image = pic;
            picBox2.Image = pic2;
            picBox1.Update(); picBox2.Update();

        }
        static Bitmap Mandelbrotset(PictureBox pictureBox1, double Xmax, double Xmin, double Ymax, double Ymin, double xc = 0, double yc = 0, bool julia = false)
        {
            Bitmap pic = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            double zy; double zx; double cy; double cx; double tempx = 0; //zx = real portion of z, zy = imaginary portion of z
            bool Zoomed = false; // check to see if picture is zoomed

            double xzoom = ((Xmax - Xmin) / Convert.ToDouble(pic.Width));// x-interval (at current zoom-level)
            double yzoom = ((Ymax - Ymin) / Convert.ToDouble(pic.Height));// y-interval (at current zoom-level)

            double lowX = (0 - pic.Width / 2) * xzoom;
            double lowY = (0 - pic.Height / 2) * yzoom;

            cx = Xmin;
            for ( int x = 0; x < Convert.ToDouble(pic.Width - 1); x++)
            {
                cy = Ymin;
                for (int y = 0; y < Convert.ToDouble(pic.Height - 1); y++)
                {
                    if (julia == true) { zx = x * xzoom - Math.Abs(lowX); zy = y * yzoom - Math.Abs(lowY); }
                    else
                    {
                        zx = 0; //copy value of local coordinate to zx
                        zy = 0; //copy value of local coordinate to zy
                    }
                    
                    int count = 0;

                    Zoomed = true;
                    while ((count < Math.Pow(10, 3)) && (Zoomed == true))
                    {
                        double zx2 = Math.Pow(zx, 2); //for normalization
                        double zy2 = Math.Pow(zy, 2); //for normalization
                        if ((zx2 + zy2) > 4)
                        {
                            Zoomed = false;
                        }
                        if (julia == true)
                        {
                            count += 1;
                            tempx = zx;
                            zx = (zx2 - zy2) + xc;
                            zy = 2 * (tempx * zy) + yc;
                            }
                        else
                        {
                            zy = 2 * (zx * zy) + cy;
                            zx = (zx2 - zy2) + cx;
                            count += 1;
                        }

                    }
                    pic.SetPixel(x, y, Color.FromArgb((count % 128) * 2, (count % 32) * 7, (count % 16) * 14));
                    cy += yzoom; // converted to local coordinates
                }
                cx += xzoom;
            }

            return pic;
        }

        private void changeCoordinates(double tempX, double tempY, double zoomValue = 1, bool mouseD = false, bool mouseDoub = false)
        {
            if (mouseD == true) { Bitmap pic2 = Mandelbrotset(picBox2, initMaxX, initMinX, initMaxY, initMinY, tempX, tempY, true); picBox2.Image = pic2; picBox2.Update(); }
            else if (mouseDoub == true)
            {
                //zoom into specific location
                maxX = (tempX + ((maxX - tempX) / zoomValue));
                maxY = (tempY + ((maxY - tempY) / zoomValue));
                minX = (tempX + ((minX - tempX) / zoomValue));
                minY = (tempY + ((minY - tempY) / zoomValue));

                //increment counter
                num += 1;

                //keep track of zoom values in order to revert back to previous zoom
                pastValues.Add(new List<double>());
                pastValues[num].Add(maxX);
                pastValues[num].Add(maxY);
                pastValues[num].Add(minX);
                pastValues[num].Add(minY);
                pastValues[num].Add(tempX);
                pastValues[num].Add(tempY);

                Bitmap pic = Mandelbrotset(picBox1, maxX, minX, maxY, minY);
                Bitmap pic2 = Mandelbrotset(picBox2, initMaxX, initMinX, initMaxY, initMinY, tempX, tempY, true);
                picBox1.Image = pic; picBox2.Image = pic2;
                picBox1.Update(); picBox2.Update();
                btn2.Visible = true;
                bar1.Visible = true;
                bar2.Visible = true;
                bar1.Value = (int)Math.Ceiling(((tempX - initMinX) / (initMaxX - initMinX)) * (Convert.ToDouble(picBox1.Width)));
                bar2.Value = (int)Math.Ceiling(((tempY - initMinY) / (initMaxY - initMinY)) * (Convert.ToDouble(picBox1.Height)));
            }
            else
            {
                //shift horizontally into specific location
                maxX = (maxX + ((tempX) / zoomValue));
                minX = (minX + ((tempX) / zoomValue));
                maxY = (maxY + ((tempY) / zoomValue));
                minY = (minY + ((tempY) / zoomValue));
                
                Bitmap pic = Mandelbrotset(picBox1, maxX, minX, maxY, minY);
                picBox1.Image = pic; picBox1.Update();
                
            }
        }

        private void picBox1_MouseDown(object sender, MouseEventArgs e)
        {
            double tempX = ((Convert.ToDouble(e.X) / Convert.ToDouble(picBox1.Width)) * (maxX - minX)) + minX;
            double tempY = ((Convert.ToDouble(e.Y) / Convert.ToDouble(picBox1.Height)) * (maxY - minY)) + minY;
            changeCoordinates(tempX, tempY, 1, true);
            
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            num -= 1;
            double tempX = 0;
            double tempY = 0;

            //make sure not visible at first step
            if (num <= 0) { btn2.Visible = false; bar1.Visible = false; bar2.Visible = false; }
            else {tempX = pastValues[num][4]; tempY = pastValues[num][5]; }

            maxX = pastValues[num][0];
            maxY = pastValues[num][1];
            minX = pastValues[num][2];
            minY = pastValues[num][3];

            Bitmap pic = Mandelbrotset(picBox1, maxX, minX, maxY, minY);
            Bitmap pic2 = Mandelbrotset(picBox2, initMaxX, initMinX, initMaxY, initMinY, tempX, tempY, true);
            picBox1.Image = pic; picBox2.Image = pic2;
            picBox1.Update(); picBox2.Update();

        }

        private void btn1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please double click on the larger Mandlebrot image to zoom in \nand click on the 'zoom out' button to zoom out \nAlso click once on any area of the image to view the accompanying julia set of the location just clicked \nand feel free to use the scroll bars to checkout the entire image \nonly problem is that it takes extremely long to load");
        }

        private void picBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            double zoomValue = 5;
            double tempX = ((Convert.ToDouble(e.X) / Convert.ToDouble(picBox1.Width)) * (maxX - minX)) + minX;
            double tempY = ((Convert.ToDouble(e.Y) / Convert.ToDouble(picBox1.Height)) * (maxY - minY)) + minY;
            changeCoordinates(tempX, tempY, zoomValue, false, true);

        }

        private void bar1_Scroll(object sender, ScrollEventArgs e)
        {
            double zoomValue = 1;
            double tempX = ((Convert.ToDouble(e.NewValue - e.OldValue) / Convert.ToDouble(picBox1.Width)) * (initMaxX - initMinX));
            changeCoordinates(tempX, 0, zoomValue);
            
        }

        private void bar1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void bar2_Scroll(object sender, ScrollEventArgs e)
        {
            double zoomValue = 1;
            double tempY = ((Convert.ToDouble(e.NewValue - e.OldValue) / Convert.ToDouble(picBox1.Height)) * (initMaxY - initMinY));
            changeCoordinates(0, tempY, zoomValue);
        }
    }
}
