﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Xml;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Interop;

namespace CheckersCheck.Menu
{
    /// <summary>
    /// Interaction logic for CalibrationPage.xaml
    /// </summary>
    public partial class CalibrationPage : ISwitchable
    {
        private int binaryTreshold;
        private int saturation;
        private int mode;
        private bool darkRigth;
        private bool dotsRigth;
        private bool contrast;
        private int gaussianValue;
        private bool gaussian;
        DispatcherTimer timer;

        List<MCvBox2D> boxList;
        Capture capture;

        System.Windows.Forms.PictureBox pictureBox1;

        public string dataFilePath;

        public CalibrationPage()
        {
            contrast = false;
            gaussianValue = 3;
            gaussian = false;
            boxList = new List<MCvBox2D>();//create a camera captue
            capture = new Capture();
            InitializeComponent();

            pictureBox1 = new System.Windows.Forms.PictureBox();
            this.formsHost.Child = pictureBox1;
            timer = new DispatcherTimer();

            saveToFile("bleh");

            InitializeComponent();
        }

        #region ISwitchable Members

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new CalibrationPage2());
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new DetailsPage());
        }

        #endregion

        private void runCamera(object sender, EventArgs e)
        {
            if (checkBox1.IsChecked == true)
                contrast = true;
            else
                contrast = false;

            if (checkBox2.IsChecked == true)
                gaussian = true;
            else
                gaussian = false;


            switch (mode)
            {
                case 0:
                    mode0(capture);
                    break;

                case 1:
                    mode1(capture);
                    break;
                /*
                case 2:
                    mode2(capture);
                    break;*/
                case 3:
                    mode3(capture);
                    break;

            }

        }

        public void saveToFile(string message)
        {
            
            XmlTextWriter textWriter = new XmlTextWriter(@"D:\CCCData.xml", null);

            textWriter.WriteStartDocument();
            textWriter.WriteComment("Checkers Check configuration data");

            textWriter.WriteStartElement("Configuration_data");

            textWriter.WriteStartElement("Players");

            textWriter.WriteStartElement("1 Name");
            textWriter.WriteString("player 1 name");
            textWriter.WriteEndElement();


            textWriter.WriteStartElement("2 Name"); 
            textWriter.WriteString("player 2 name");
            textWriter.WriteEndElement();

            textWriter.WriteEndDocument();
            
            textWriter.Close();

        }

        private void mode0(Capture capture)
        {
            pictureBox1.Image = detectBoxes(capture.QueryFrame()).ToBitmap();
        }

        private void mode1(Capture capture)
        {
            pictureBox1.Image = boardCheck(capture.QueryFrame()).ToBitmap();
        }

        /*private void mode2(Capture capture)
        {
            pictureBox1.Image = capture.QueryFrame().ToBitmap();

            Image<Bgr, Byte> previous = capture.QueryFrame();
            System.Threading.Thread.Sleep(500);
            Image<Bgr, Byte> last = capture.QueryFrame();

            bool isMove = detectMovement(previous, last);

            if (isMove == true)
                MessageBox.Show("There is a movement around here!");
            else { }
        }*/

        private void mode3(Capture capture)
        {
            pictureBox1.Image = piecesCheck(capture.QueryFrame()).ToBitmap();
        }

        private Image<Bgr, Byte> detectBoxes(Image<Bgr, Byte> img)
        {
            Image<Bgr, Byte> result = img;

            Image<Gray, Byte> grayImage = img.Convert<Gray, Byte>();


            //grayImage._EqualizeHist();


            grayImage = grayImage.PyrDown().PyrUp();
            if (gaussian == true)
                //grayImage = grayImage.SmoothGaussian(gaussianValue);

                //cannyFrame._Dilate(1);
                grayImage._Dilate(3);

            //Image<Gray, byte> cannyFrame = grayImage.Canny(120, 120);

            //if(binary == true)
            grayImage = grayImage.ThresholdBinary(new Gray(binaryTreshold), new Gray(255));

            //Image<Gray, Byte> cannyEdges = grayImage.Canny(120, 120);
            Bitmap grayBitmap = grayImage.ToBitmap();
            //pictureBox2.Image = grayImage.ToBitmap();

            boxList = new List<MCvBox2D>();

            using (MemStorage storage = new MemStorage()) //allocate storage for contour approximation
                for (Contour<System.Drawing.Point> contours = grayImage.FindContours(); contours != null; contours = contours.HNext)
                {
                    Contour<System.Drawing.Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, storage);

                    if (contours.Area > 250 && contours.Area < 20000) //only consider contours with area greater than 250
                    {
                        if (currentContour.Total == 4) //The contour has 4 vertices.
                        {
                            #region determine if all the angles in the contour are within the range of [80, 100] degree
                            bool isRectangle = true;
                            System.Drawing.Point[] pts = currentContour.ToArray();
                            LineSegment2D[] edges = Emgu.CV.PointCollection.PolyLine(pts, true);
                            
                            for (int i = 0; i < edges.Length; i++)
                            {
                                double angle = Math.Abs(
                                   edges[(i + 1) % edges.Length].GetExteriorAngleDegree(edges[i]));
                                if (angle < 80 || angle > 100)
                                {
                                    isRectangle = false;
                                    break;
                                }
                            }
                            #endregion

                            if (isRectangle) boxList.Add(currentContour.GetMinAreaRect());
                        }
                    }
                }

            #region draw triangles and rectangles
            Image<Bgr, Byte> rectangleImage = result;
            foreach (MCvBox2D box in boxList)
            {
                rectangleImage.Draw(box, new Bgr(System.Drawing.Color.DarkOrange), 2);
            }
            #endregion

            try
            {
                for (int i = 0; i < 32; i++)
                {
                    if (i == 0)
                        rectangleImage.Draw(new CircleF(new PointF(boxList[i].center.X, boxList[i].center.Y), 3), new Bgr(System.Drawing.Color.Blue), 3);
                    else
                        rectangleImage.Draw(new CircleF(new PointF(boxList[i].center.X, boxList[i].center.Y), 3), new Bgr(System.Drawing.Color.Red), 3);
                }
            }
            catch { }

            return rectangleImage;
        }

        private Image<Bgr, Byte> boardCheck(Image<Bgr, Byte> img)
        {
            #region draw triangles and rectangles
            Image<Bgr, Byte> result = img;
            foreach (MCvBox2D box in boxList)
            {
                result.Draw(box, new Bgr(System.Drawing.Color.DarkOrange), 2);
            }
            #endregion

            result.Draw(new CircleF(new PointF(boxList[0].center.X, boxList[0].center.Y), 3), new Bgr(System.Drawing.Color.Blue), 3);

            return result;
        }

        private Image<Hls, Byte> piecesCheck(Image<Bgr, Byte> img)
        {
            int channel = 1;
            int black = 30;
            int white = 99;

            /*#region draw triangles and rectangles

            foreach (MCvBox2D box in boxList)
            {
                img.Draw(box, new Bgr(Color.DarkOrange), 2);
            }
            #endregion*/

            Image<Hls, Byte> result = new Image<Hls, byte>(img.Bitmap).PyrDown().PyrUp();

            if (gaussian == true)
                result = result.SmoothGaussian(gaussianValue);

            if (contrast == true)
                result._EqualizeHist();

            result[2] += saturation;

            //Image<Gray, Byte> temp = result[2];

            int countBlack;
            int countWhite;

            for (int i = 0; i < 32; i++)
            {
                int x = (int)boxList[i].center.X;
                int y = (int)boxList[i].center.Y;

                countWhite = 0;
                countBlack = 0;

                byte asd = result.Data[y, x, 1];

                if (asd > white)
                    //countWhite++;
                    result.Draw(new CircleF(boxList[i].center, 3), new Hls(120, 50, 100), 3);
                if (asd < black)
                    //countBlack++;
                    result.Draw(new CircleF(boxList[i].center, 3), new Hls(220, 60, 100), 3);

                #region count Colors
                /*
                for (int j = 0; j < 22; j++)
                {

                    if ((asd = result.Data[y + 1 + j, x, 1]) > white)
                        countWhite++;
                    result.Draw(new CircleF(new PointF(y, x), 5), new Hls(220, 60, 100), 5);    
                        
                    if ((asd = result.Data[y + 1 + j, x, 1]) < black)
                        countBlack++;

                    if ((asd = result.Data[y - 1 - j, x, 1]) > white)
                        countWhite++;
                    if ((asd = result.Data[y - 1 - j, x, 1]) < black)
                        countBlack++;

                    if ((asd = result.Data[y, x + 1 + j, 1]) > white)
                        countWhite++;
                    if ((asd = result.Data[y, x + 1 + j, 1]) < black)
                        countBlack++;

                    if ((asd = result.Data[y, x - 1 - j, 1]) > white)
                        countWhite++;
                    if ((asd = result.Data[y, x - 1 - j, 1]) < black)
                        countBlack++;

                    if ((asd = result.Data[y - 1 - j, x - 1 - j, 1]) > white)
                        countWhite++;
                    if ((asd = result.Data[y - 1 - j, x - 1 - j, 1]) < black)
                        countBlack++;

                    if ((asd = result.Data[y + 1 + j, x - 1 - j, 1]) > white)
                        countWhite++;
                    if ((asd = result.Data[y + 1 + j, x - 1 - j, 1]) < black)
                        countBlack++;

                    if ((asd = result.Data[y - 1 - j, x + 1 + j, 1]) > white)
                        countWhite++;
                    if ((asd = result.Data[y - 1 - j, x + 1 + j, 1]) < black)
                        countBlack++;

                    if ((asd = result.Data[y + 1 + j, x + 1 + j, 1]) > white)
                        countWhite++;
                    if ((asd = result.Data[y + 1 + j, x + 1 + j, 1]) < black)
                        countBlack++;

                }
                */
                #endregion
                /*
                if (countWhite > 100)
                    result.Draw(new CircleF(boxList[i].center, 3), new Hls(120, 50, 100), 3);

                if (countBlack > 100)
                    result.Draw(new CircleF(boxList[i].center, 3), new Hls(220, 60, 100), 3);*/
            }

            return result;
        }

        private void trackBar1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            binaryTreshold = (int)trackBar1.Value;
        }

        private void trackBar2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            saturation = (int)trackBar2.Value;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mode = 1;
            timer.Stop();
            timer.Tick -= new EventHandler(runCamera);
            timer.Tick += new EventHandler(runCamera);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            mode = 0;
            timer.Stop();
            timer.Tick -= new EventHandler(runCamera);
            timer.Tick += new EventHandler(runCamera);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            mode = 3;
            timer.Stop();
            timer.Tick -= new EventHandler(runCamera);
            timer.Tick += new EventHandler(runCamera);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
        }

    }
}
