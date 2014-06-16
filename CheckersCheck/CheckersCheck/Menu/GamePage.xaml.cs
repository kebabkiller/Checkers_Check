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

using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Interop;
using System.Timers;
using WpfAnimatedGif;

namespace CheckersCheck.Menu
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : System.Windows.Controls.UserControl, ISwitchable
    {
        int x = 0;
        private int time = 0;
        private TempData tmpData;
        private DispatcherTimer clockTimer;
        private DispatcherTimer cameraTimer;
        static System.Timers.Timer _timer;
        Capture capture;
        System.Windows.Forms.PictureBox pictureBox1;
        public List<Piece> PiecesList;

        public GamePage()
        {
            InitializeComponent();
            tmpData = new TempData();
            PiecesList =  new List<Piece>();

            MakeBoard();
            MoveDone();
            SetStarCoords();

            playerWhiteInfoTitle.Text = tmpData.playerWhite;
            playerBlackInfoTitle.Text = tmpData.playerBlack;
            piecesCountWhite.Text = tmpData.whitePieces.ToString();
            kingsCountWhite.Text = tmpData.whiteKing.ToString();
            piecesCountBlack.Text = tmpData.blackPieces.ToString();
            kingsCountBlack.Text = tmpData.blackKing.ToString();


            pictureBox1 = new System.Windows.Forms.PictureBox();
            capture = new Capture(1);

            this.formsHost.Child = pictureBox1;
            cameraTimer = new DispatcherTimer();

            cameraTimer.Tick += new EventHandler(runCamera);
            cameraTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            cameraTimer.Start();
                  
            
        }

        #region ISwitchable Members

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        

        #endregion

        void timer_Tick(object sender, EventArgs e)
        {
            time++;
            GameTimeTextBlock.Text = string.Format("{0}:{1}:{2}",time/360, time/60, time%60);
        }

        private void runCamera(object sender, EventArgs e)
        {
            pictureBox1.Image = capture.QueryFrame().ToBitmap();
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

        private void Animation(Grid grd, double x, double y, int speed)
        {
            ThicknessAnimation ta = new ThicknessAnimation();
            ta.From = grd.Margin;
            ta.To = new Thickness(x, 0, 0, y);
            ta.Duration = new Duration(TimeSpan.FromSeconds(speed));

            grd.BeginAnimation(Grid.MarginProperty, ta);

            ta.Completed += new EventHandler((sender, e) => MoveAnimationCompleted(sender, e, grd));//!!nie działa
            pieceZindex(grd);
        }

        public async Task pieceZindex(Grid grd)
        {
            await Task.Delay(3000);
            grd.SetValue(System.Windows.Controls.Panel.ZIndexProperty, 1);
        }   

        public void MoveAnimationCompleted(object sender, EventArgs e, Grid grd)//!!!nie jest wywoływane 
        {
            grd.SetValue(System.Windows.Controls.Panel.ZIndexProperty, 1);
        }

        private void FastTo(string pieceId, int x, int y)
        {
            Animation(FindChild<Grid>(BoardBorder, pieceId), coords[y], coords[x], 0);
        }

        private void MoveTo(string pieceId, int x, int y)
        {
            Animation(FindChild<Grid>(BoardBorder, pieceId), coords[x], coords[y], 2);

            FindChild<Grid>(BoardBorder, pieceId).SetValue(System.Windows.Controls.Panel.ZIndexProperty, 99999);

            Piece active = PiecesList.Find(p => p.id == pieceId);
            active.x = x;
            active.y = y;

            PiecesList.Remove(PiecesList.Find(p => p.id == pieceId));
            PiecesList.Add(active);

        }

        private void MoveAndCapture(string movingPieceId, int mX, int mY, string capturePieceId)
        {
            Animation(FindChild<Grid>(BoardBorder, movingPieceId), coords[mX], coords[mY], 3);           
            
            Piece active = PiecesList.Find(p => p.id == movingPieceId);
            active.x = mX;
            active.y = mY;

            PiecesList.Remove(PiecesList.Find(p => p.id == movingPieceId));
            PiecesList.Add(active);

            Piece captured = PiecesList.Find(p => p.id == capturePieceId);
            captured.underCapture = true;

            PiecesList.Remove(PiecesList.Find(p => p.id == capturePieceId));
            PiecesList.Add(captured);

            Animation(FindChild<Grid>(BoardBorder, "sword"), coords[captured.x], coords[captured.y], 0);
            swordState(true);
        }

        private void MoveAndCapture(string movingPieceId, int mX, int mY, int capturedX, int capturedY)
        {
            Animation(FindChild<Grid>(BoardBorder, movingPieceId), coords[mX], coords[mY], 3);
            Animation(FindChild<Grid>(BoardBorder, "sword"), coords[capturedX], coords[capturedY], 0);
            swordState(true);

            Piece active = PiecesList.Find(p => p.id == movingPieceId);
            active.x = mX;
            active.y = mY;

            PiecesList.Remove(PiecesList.Find(p => p.id == movingPieceId));
            PiecesList.Add(active);

            Piece captured = PiecesList.Find(p => p.x == capturedX && p.y == capturedY);
            captured.underCapture = true;

            PiecesList.Remove(PiecesList.Find(p => p.x == capturedX && p.y == capturedY));
            PiecesList.Add(captured);
            
        }

        private void swordState(bool state)
        {          
            if (state)
            {
                swordAnimation();
            } 

            else if (state == false) 
            {
                sword.Visibility = Visibility.Hidden;
            }
        }

        public async Task swordAnimation()
        {
            await Task.Delay(1300);
            sword.Visibility = Visibility.Visible;
            var controller = ImageBehavior.GetAnimationController(swordGif);
            controller.Play();
        }           
        

        private void SetStarCoords()
        {
            int wId=1;
            for (int i = 1; i < 4; i++)
            {
                for(int j=1; j<9; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                        {
                            FastTo("w" + wId.ToString(), i, j);
                            Piece tmp = new Piece();
                            tmp.id = "w" + wId.ToString();
                            tmp.x = i;
                            tmp.y = j;
                            tmp.underCapture = false;
                            tmp.isDame = false;

                            PiecesList.Add(tmp);
                            wId++;
                        }
                    }
                    else if(i % 2 != 0)
                    {
                        if (j % 2 != 0)
                        {
                            FastTo("w" + wId.ToString(), i, j);
                            Piece tmp = new Piece();
                            tmp.id = "w" + wId.ToString();
                            tmp.x = i;
                            tmp.y = j;
                            tmp.underCapture = false;
                            tmp.isDame = false;

                            PiecesList.Add(tmp);
                            wId++;
                        }
                    }
                }
            }

            int bId = 1;
            for (int i = 6; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                        {
                            FastTo("b" + bId.ToString(), i, j);
                            Piece tmp = new Piece();
                            tmp.id = "b" + bId.ToString();
                            tmp.x = i;
                            tmp.y = j;
                            tmp.underCapture = false;
                            tmp.isDame = false;

                            PiecesList.Add(tmp);
                            bId++;
                        }
                    }
                    else if (i % 2 != 0)
                    {
                        if (j % 2 != 0)
                        {
                            FastTo("b" + bId.ToString(), i, j);
                            Piece tmp = new Piece();
                            tmp.id = "b" + bId.ToString();
                            tmp.x = i;
                            tmp.y = j;
                            tmp.underCapture = false;
                            tmp.isDame = false;

                            PiecesList.Add(tmp);
                            bId++;
                        }
                    }
                }
            }


            //FastTo("b01", 1, 1);
            //Piece b01;
            //b01.id = "b01";
            //b01.x = 1;
            //b01.y = 1;
            //b01.underCapture = false;
            //b01.isDame = false;
            ////PiecesList.Add(b01);

            //FastTo("w2", 1, 3);
            //FastTo("w3", 1, 5);
            //FastTo("w4", 1, 7);
            //FastTo("w5", 2, 2);
            //FastTo("w6", 2, 4);
            //FastTo("w7", 2, 6);
            //FastTo("w8", 2, 8);
            //FastTo("w9", 3, 1);
            //FastTo("w10", 3, 3);
            //FastTo("w11", 3, 5);
            //FastTo("w12", 3, 7);

            //FastTo("b1", 6, 2);
            //FastTo("b2", 6, 4);
            //FastTo("b3", 6, 6);
            //FastTo("b4", 6, 8);
            //FastTo("b5", 7, 1);
            //FastTo("b6", 7, 3);
            //FastTo("b7", 7, 5);
            //FastTo("b8", 7, 7);
            //FastTo("b9", 8, 2);
            //FastTo("b10", 8, 4);
            //FastTo("b11", 8, 6);
            //FastTo("b12", 8, 8);

        }

        
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                T childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);

                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        Dictionary<int, double> coords = new Dictionary<int, double>()
	        {
	            {1, -385},
	            {2, -275},
	            {3, -165},
	            {4, -55},
                {5, 55},
	            {6, 165},
	            {7, 275},
	            {8, 385}
	        };
        
        //private void MovePiece(Image Piece, int Lat, int Long)
        //{
        //    //DoubleAnimation da = new DoubleAnimation(360, 0, new Duration(TimeSpan.FromSeconds(3)));
        //    //RotateTransform rt = new RotateTransform();

        //    //image1.RenderTransform = rt;
        //    //image1.RenderTransformOrigin = new Point(0.5, 0.5);
        //    //da.RepeatBehavior = RepeatBehavior.Forever;
        //    //rt.BeginAnimation(RotateTransform.AngleProperty, da);


        //    var top = Canvas.GetTop(Piece);
        //    var left = Canvas.GetLeft(Piece);
        //    //TranslateTransform trans = new TranslateTransform();
        //    //Piece.RenderTransform = trans;
        //    //DoubleAnimation anim1 = new DoubleAnimation(top, Lat - top, TimeSpan.FromSeconds(1));
        //    //DoubleAnimation anim2 = new DoubleAnimation(left, Long - left, TimeSpan.FromSeconds(1));
        //    //trans.BeginAnimation(TranslateTransform.XProperty, anim1);
        //    //trans.BeginAnimation(TranslateTransform.YProperty, anim2);


        //    Storyboard story = new Storyboard();
        //    DoubleAnimation anim1 = new DoubleAnimation(top, Lat + top, TimeSpan.FromSeconds(1));
        //    DoubleAnimation anim2 = new DoubleAnimation(left, Long + left, TimeSpan.FromSeconds(1));

        //    story.Children.Add(anim2);
        //    Storyboard.SetTargetName(anim2, Piece.Name);
        //    Storyboard.SetTargetProperty(anim2, new PropertyPath(Canvas.LeftProperty));

        //    story.Children.Add(anim1);
        //    Storyboard.SetTargetName(anim1, Piece.Name);
        //    Storyboard.SetTargetProperty(anim1, new PropertyPath(Canvas.TopProperty));

        //    story.Begin(this);

        //    Canvas.SetLeft(Piece, Lat);
        //    Canvas.SetTop(Piece, Long);

        //}

        
        private void AnimationCompleted(object sender, RoutedEventArgs e)
        {
            swordState(false);

            Piece captured = PiecesList.Find(p => p.underCapture == true);
            if(captured.id != null)
            {

                captured.underCapture = false;
                captured.x = 0;
                captured.y = 0;
            
                FindChild<Grid>(BoardBorder, captured.id).Visibility = Visibility.Hidden;

                //aktualizacja tablicy wyników
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            MoveAndCapture(TextBoxIdPionka.Text, int.Parse(TextBoxIntX.Text), int.Parse(TextBoxIntY.Text), int.Parse(TextBoxIntX_Copy.Text), int.Parse(TextBoxIntY_Copy.Text));
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
                System.Windows.MessageBox.Show("Error: ", ex.ToString());
            }
        }

        public void MoveDone()
        {
            if (tmpData.isWhiteTurn)
                CurrentPlayerTextBlock.Text = tmpData.playerWhite;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MoveTo(TextBoxIdPionka.Text, int.Parse(TextBoxIntX.Text), int.Parse(TextBoxIntY.Text));
        }
    }

    public struct Piece
    {
        public string id;
        public int x;
        public int y;
        public bool underCapture;
        public bool isDame;
    };
}
