using System;
using System.Collections.Generic;
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

        int blackLightness;
        int whiteLightness;

        List<MCvBox2D> boxList;
        Capture capture;

        System.Windows.Forms.PictureBox pictureBox1;

        Game currentGame;
        Game previousGame;
       
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

            

            blackLightness = 30;
            whiteLightness = 130;

            InitializeComponent();
        }

        #region ISwitchable Members

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (leftRadio.IsChecked == true)
                {
                    Switcher.Switch(new CalibrationPage2());
                }
                if (rightRadio.IsChecked == true)
                {
                    Switcher.Switch(new CalibrationPage2());
                }
            }
            catch
            { 
            
            }
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
                
                case 2:
                    mode2(capture);
                    break;
                case 3:
                    mode3(capture);
                    break;

            }

        }

        
        private void mode0(Capture capture)
        {
            pictureBox1.Image = detectBoxes(capture.QueryFrame()).ToBitmap();
        }

        private void mode1(Capture capture)
        {
            pictureBox1.Image = boardCheck(capture.QueryFrame()).ToBitmap();
        }

        private void mode2(Capture capture)
        {
            pictureBox1.Image = piecesCheck2(capture.QueryFrame()).ToBitmap();
        }

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
            Image<Hls, Byte> result = new Image<Hls, byte>(img.Bitmap).PyrDown().PyrUp();

            if (gaussian == true)
                result = result.SmoothGaussian(gaussianValue);

            if (contrast == true)
                result._EqualizeHist();

            //result[2] += saturation;

            int countBlack;
            int countWhite;

            for (int i = 0; i < 32; i++)
            {
                int x = (int)boxList[i].center.X;
                int y = (int)boxList[i].center.Y;

                countWhite = 0;
                countBlack = 0;

                byte asd = result.Data[y, x, 1];

                if (asd > whiteLightness)
                {
                    //countWhite++;
                    result.Draw(new CircleF(boxList[i].center, 3), new Hls(120, 50, 100), 3);
                }
                if (asd < blackLightness)
                {
                    //countBlack++;
                    result.Draw(new CircleF(boxList[i].center, 3), new Hls(220, 60, 100), 3);
                }
            }

            return result;
        }

        private Image<Hls, Byte> piecesCheck2(Image<Bgr, Byte> img)
        {
            Image<Hls, Byte> result = new Image<Hls, byte>(img.Bitmap).PyrDown().PyrUp();

            Game a = new Game(leftRadio.IsChecked.Value);

            if (gaussian == true)
                result = result.SmoothGaussian(gaussianValue);

            if (contrast == true)
                result._EqualizeHist();

            //result[2] += saturation;

            int countBlack;
            int countWhite;

            List<int> gameState = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                gameState.Add(2);
            }

            for (int i = 0; i < 32; i++)
            {
                int x = (int)boxList[i].center.X;
                int y = (int)boxList[i].center.Y;

                countWhite = 0;
                countBlack = 0;

                byte asd = result.Data[y, x, 1];

                if (asd > whiteLightness)
                {
                    countWhite++;
                    gameState[i] = 0;
                    result.Draw(new CircleF(boxList[i].center, 3), new Hls(120, 50, 100), 3);
                }
                if (asd < blackLightness)
                {
                    countBlack++;
                    gameState[i] = 1;
                    result.Draw(new CircleF(boxList[i].center, 3), new Hls(220, 60, 100), 3);
                }
            }

            previousGame = a;

            a.updateStatus(gameState);

            currentGame = a;

            return result;
        }

        private MoveObj movePiece(Game current, Game previous)
        {
            GameField a = new GameField(2);
            GameField b = new GameField(2);

            MoveObj coord = new MoveObj();
            coord.color = 10;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (current.board[i, j].color != previous.board[i, j].color)
                    {
                        if (current.board[i, j].color == 2)
                        {
                            coord.prev_i = i;
                            coord.prev_j = j;
                            coord.color = previous.board[i, j].color;
                        }

                        if (previous.board[i, j].color == 2)
                        {
                            coord.curr_i = i;
                            coord.curr_j = j;
                        }
                    }
                }
            }

            return coord;
        }

        private void trackBar1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            binaryTreshold = (int)trackBar1.Value;
        }

        private void trackBar2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            blackLightness = (int)trackBar2.Value;
        }

        private void trackBar3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            whiteLightness = (int)trackBar3.Value;
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

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            currentGame = new Game(leftRadio.IsChecked.Value);
            previousGame = new Game(leftRadio.IsChecked.Value);

            mode = 2;
            timer.Stop();
            timer.Tick -= new EventHandler(runCamera);
            timer.Tick += new EventHandler(runCamera);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            timer.Tick -= new EventHandler(runCamera);
            System.Windows.Forms.MessageBox.Show("wykonaj ruch");
            MoveObj asd = new MoveObj();
            pictureBox1.Image = piecesCheck2(capture.QueryFrame()).ToBitmap();
            asd = movePiece(currentGame, previousGame);
            timer.Tick += new EventHandler(runCamera);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
        }
    }
}
