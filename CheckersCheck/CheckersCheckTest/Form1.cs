using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace CheckersCheckTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            runCamera();
        }

        private void runCamera()
        {
            ImageViewer viewer = new ImageViewer(); //create an image viewer
            Capture capture = new Capture(); //create a camera captue
            Application.Idle += new EventHandler(delegate(object sender, EventArgs e)
            {  //run this until application closed (close button click on image viewer)
                viewer.Image = checkCircles(capture.QueryFrame()); //draw the image obtained from camera
            });
            viewer.ShowDialog(); //show the image viewer    
        }

        private Image<Bgr, Byte> checkCircles(Image<Bgr, Byte> img)
        {
            Image<Bgr, Byte> result = img;
            Image<Gray, Byte> grayImage = img.Convert<Gray, Byte>().PyrDown().PyrUp();
            
            Gray cannyThreshold = new Gray(180);
            Gray cannyThresholdLinking = new Gray(120);
            Gray circleAccumulatorThreshold = new Gray(120);

            CircleF[] circles = grayImage.HoughCircles(
            cannyThreshold,
            circleAccumulatorThreshold,
            5.0, //Resolution of the accumulator used to detect centers of the circles
            10.0, //min distance 
            5, //min radius
            50 //max radius
            )[0]; //Get the circles from the first channel

            foreach (CircleF circle in circles)
                result.Draw(circle, new Bgr(Color.Red), 2);

            return result;
        }
    }
}
