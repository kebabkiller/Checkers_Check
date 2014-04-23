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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CheckersCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int x = 0;
        public MainWindow()
        {
            InitializeComponent();
            SolidColorBrush whiteBrush = new SolidColorBrush(Colors.White);
            SolidColorBrush blackBrush = new SolidColorBrush(Colors.Gray);
            for (int i = 0; i < 64; i++)
            {
                Grid cell = new Grid();
                if ((i + i/8)%2 == 0)
                {
                    cell.Background = whiteBrush;
                    
                    ChessBoard.Children.Add(cell);
                }
                else
                {
                    cell.Background = blackBrush;
                    ChessBoard.Children.Add(cell);
                }
            }
            
        }

        private void MovePiece(Image Piece, int Lat, int Long)
        {
            //DoubleAnimation da = new DoubleAnimation(360, 0, new Duration(TimeSpan.FromSeconds(3)));
            //RotateTransform rt = new RotateTransform();

            //image1.RenderTransform = rt;
            //image1.RenderTransformOrigin = new Point(0.5, 0.5);
            //da.RepeatBehavior = RepeatBehavior.Forever;
            //rt.BeginAnimation(RotateTransform.AngleProperty, da);


            var top = Canvas.GetTop(Piece);
            var left = Canvas.GetLeft(Piece);
            //TranslateTransform trans = new TranslateTransform();
            //Piece.RenderTransform = trans;
            //DoubleAnimation anim1 = new DoubleAnimation(top, Lat - top, TimeSpan.FromSeconds(1));
            //DoubleAnimation anim2 = new DoubleAnimation(left, Long - left, TimeSpan.FromSeconds(1));
            //trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            //trans.BeginAnimation(TranslateTransform.YProperty, anim2);


            Storyboard story = new Storyboard();
            DoubleAnimation anim1 = new DoubleAnimation(top, Lat + top, TimeSpan.FromSeconds(1));
            DoubleAnimation anim2 = new DoubleAnimation(left, Long + left , TimeSpan.FromSeconds(1));

            story.Children.Add(anim2);
            Storyboard.SetTargetName(anim2, Piece.Name);
            Storyboard.SetTargetProperty(anim2, new PropertyPath(Canvas.LeftProperty));

            story.Children.Add(anim1);
            Storyboard.SetTargetName(anim1, Piece.Name);
            Storyboard.SetTargetProperty(anim1, new PropertyPath(Canvas.TopProperty));

            story.Begin(this);

            Canvas.SetLeft(Piece, Lat);
            Canvas.SetTop(Piece, Long);
    
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (x == 0)
            {
                MovePiece(image1, 70, 70);
            
                x++;
            }
            else if (x == 1)
            {
                MovePiece(image1, 50, -70);
                x--;
            }
        }


    }
}
