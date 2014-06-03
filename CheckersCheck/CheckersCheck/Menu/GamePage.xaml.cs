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
using System.Threading;

using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CheckersCheck.Menu
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : UserControl, ISwitchable
    {
        int x = 0;
        private int time = 0;
        private TempData tmpData;
        private DispatcherTimer timer;
        public GamePage()
        {
            InitializeComponent();
            tmpData = new TempData();

            MakeBoard();
            MoveDone();

            playerWhiteInfoTitle.Text = tmpData.playerWhite;
            playerBlackInfoTitle.Text = tmpData.playerBlack;
            piecesCountWhite.Text = tmpData.whitePieces.ToString();
            kingsCountWhite.Text = tmpData.whiteKing.ToString();
            piecesCountBlack.Text = tmpData.blackPieces.ToString();
            kingsCountBlack.Text = tmpData.blackKing.ToString();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        #region ISwitchable Members

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new CalibrationPage());
        }

        #endregion

        void timer_Tick(object sender, EventArgs e)
        {
            time++;
            GameTimeTextBlock.Text = string.Format("{0}:{1}:{2}",time/360, time/60, time%60);
        }

        public void MakeBoard()
        {
            TextBlock textBlockLeft;
            TextBlock textBlockRight;
            TextBlock textBlockDown;
            TextBlock textBlockUp;
            Rectangle rectangle;

            SolidColorBrush whiteBrush = new SolidColorBrush(Colors.White);
            SolidColorBrush blackBrush = new SolidColorBrush(Colors.Gray);

            rectangle = new Rectangle();

            rectangle.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC863"));
            rectangle.SetValue(Grid.ColumnProperty, 9);
            rectangle.SetValue(Grid.RowProperty, 0);
            ChessBoard.Children.Add(rectangle);

            rectangle = new Rectangle();
            rectangle.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC863"));
            rectangle.SetValue(Grid.ColumnProperty, 0);
            rectangle.SetValue(Grid.RowProperty, 9);
            ChessBoard.Children.Add(rectangle);

            rectangle = new Rectangle();
            rectangle.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#620726"));
            rectangle.SetValue(Grid.ColumnProperty, 0);
            rectangle.SetValue(Grid.RowProperty, 0);
            ChessBoard.Children.Add(rectangle);

            rectangle = new Rectangle();
            rectangle.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#620726"));
            rectangle.SetValue(Grid.ColumnProperty, 9);
            rectangle.SetValue(Grid.RowProperty, 9); 
            ChessBoard.Children.Add(rectangle);


            for (int i = 1; i < 9; i++)
            {
                textBlockLeft = new TextBlock();
                textBlockRight = new TextBlock();
                textBlockDown = new TextBlock();
                textBlockUp = new TextBlock();

                textBlockLeft.Text = Environment.NewLine + i;
                textBlockLeft.SetValue(Grid.ColumnProperty,0);
                textBlockLeft.SetValue(Grid.RowProperty, 9-i);
                textBlockLeft.FontSize = 15;
                textBlockLeft.TextAlignment = TextAlignment.Center;
                if (i%2 != 0)
                {
                    textBlockLeft.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC863"));
                    textBlockLeft.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#620726"));
                }
                else
                {
                    textBlockLeft.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#620726"));
                    textBlockLeft.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC863"));
                }
                
                textBlockRight.Text = Environment.NewLine + i;
                textBlockRight.SetValue(Grid.ColumnProperty, 9);
                textBlockRight.SetValue(Grid.RowProperty, 9-i);
                textBlockRight.FontSize = 15;
                textBlockRight.TextAlignment = TextAlignment.Center;
                if (i % 2 != 0)
                {
                    textBlockRight.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#620726"));
                    textBlockRight.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC863"));
                }
                else
                {
                    textBlockRight.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC863"));
                    textBlockRight.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#620726"));
                }

                textBlockUp.Text = ((char)(64+i)).ToString();
                textBlockUp.SetValue(Grid.ColumnProperty, i);
                textBlockUp.SetValue(Grid.RowProperty, 0);
                textBlockUp.FontSize = 15;
                textBlockUp.TextAlignment = TextAlignment.Center;
                if (i % 2 == 0)
                {
                    textBlockUp.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC863"));
                    textBlockUp.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#620726"));
                }
                else
                {
                    textBlockUp.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#620726"));
                    textBlockUp.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC863"));
                }

                textBlockDown.Text =((char)(64 + i)).ToString();
                textBlockDown.SetValue(Grid.ColumnProperty, i);
                textBlockDown.SetValue(Grid.RowProperty, 9);
                textBlockDown.FontSize = 15;
                textBlockDown.TextAlignment = TextAlignment.Center;
                if (i % 2 == 0)
                {
                    textBlockDown.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#620726"));
                    textBlockDown.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC863"));
                }
                else
                {
                    textBlockDown.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC863"));
                    textBlockDown.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#620726"));
                }


                int ij = i;

                for(int j=1; j<9; j++)
                {
                    rectangle = new Rectangle();

                    if (ij%2 == 0)
                    {
                        rectangle.Fill = blackBrush;
                        rectangle.SetValue(Grid.ColumnProperty, j);
                        rectangle.SetValue(Grid.RowProperty, i);
                        ij++;
                    }
                    else
                    {
                        rectangle.Fill = whiteBrush;
                        rectangle.SetValue(Grid.ColumnProperty, j);
                        rectangle.SetValue(Grid.RowProperty, i);
                        ij--;
                    }

                    ChessBoard.Children.Add(rectangle);
                }

                ChessBoard.Children.Add(textBlockLeft);
                ChessBoard.Children.Add(textBlockRight);
                ChessBoard.Children.Add(textBlockUp);
                ChessBoard.Children.Add(textBlockDown);
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
            DoubleAnimation anim2 = new DoubleAnimation(left, Long + left, TimeSpan.FromSeconds(1));

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

        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            Thread gameClockThread = new Thread(new ThreadStart(GameClock));
            gameClockThread.Start();
        }

        public void GameClock()
        {
            try
            {
                while (true)
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: ", ex.ToString());
            }
        }

        public void MoveDone()
        {
            if (tmpData.isWhiteTurn)
                CurrentPlayerTextBlock.Text = tmpData.playerWhite;

        }

        
    }
}
