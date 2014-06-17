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
        Capture capture;
        System.Windows.Forms.PictureBox pictureBox1;
        public List<Piece> PiecesList;
        public bool gameIsOn = false;
        bool isWhiteTurn = true;
        string playerWhite, playerBlack;
        int piecesWhite = 12, piecesBlack = 12, dameWhite = 0, dameBlack = 0;

        public GamePage(string player1, string player2)
        {
            InitializeComponent();
            tmpData = new TempData();
            PiecesList =  new List<Piece>();

            MakeBoard();
            MoveDone();
            SetStarCoords();

            playerWhiteInfoTitle.Text = player1;
            playerBlackInfoTitle.Text = player2;
            piecesCountWhite.Text = piecesWhite.ToString();
            kingsCountWhite.Text = dameWhite.ToString();
            piecesCountBlack.Text = piecesBlack.ToString();
            kingsCountBlack.Text = dameBlack.ToString();

            pictureBox1 = new System.Windows.Forms.PictureBox();
            capture = new Capture(1);

            this.formsHost.Child = pictureBox1;
            cameraTimer = new DispatcherTimer();

            cameraTimer.Tick += new EventHandler(runCamera);
            cameraTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            cameraTimer.Start();

            clockTimer = new DispatcherTimer();
            clockTimer.Interval = new TimeSpan(0, 0, 1);
            clockTimer.Tick += timer_Tick;
            clockTimer.Start();

            playerWhite = player1;
            playerBlack = player2;
            CurrentPlayerTextBlock.Text = playerWhite;            
        }

        #region ISwitchable

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        

        #endregion

        #region Camera

        private void runCamera(object sender, EventArgs e)
        {
            pictureBox1.Image = capture.QueryFrame().ToBitmap();
        }

        #endregion

        #region WPF Game Play

        void timer_Tick(object sender, EventArgs e)
        {
            time++;
            GameTimeTextBlock.Text = string.Format("{0}:{1}:{2}", time / 360, time / 60, time % 60);
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
                textBlockLeft.SetValue(Grid.ColumnProperty, 0);
                textBlockLeft.SetValue(Grid.RowProperty, 9 - i);
                textBlockLeft.FontSize = 15;
                textBlockLeft.TextAlignment = TextAlignment.Center;
                if (i % 2 != 0)
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
                textBlockRight.SetValue(Grid.RowProperty, 9 - i);
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

                textBlockUp.Text = ((char)(64 + i)).ToString();
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

                textBlockDown.Text = ((char)(64 + i)).ToString();
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

                for (int j = 1; j < 9; j++)
                {
                    rectangle = new Rectangle();

                    if (ij % 2 == 0)
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

            pieceZindex(grd);
        }

        public async Task pieceZindex(Grid grd)
        {
            await Task.Delay(2900);
            grd.SetValue(System.Windows.Controls.Panel.ZIndexProperty, 1);
            MoveDone();
        }

        /// <summary>
        /// Metoda do zmiany połorzenia (natychmiastowego)
        /// </summary>
        /// <param name="pieceId">id</param>
        /// <param name="x">współrzędna X pola docelowego</param>
        /// <param name="y">współrzędna Y pola docelowego</param>
        private void FastTo(string pieceId, int x, int y)
        {
            Animation(FindChild<Grid>(BoardBorder, pieceId), coords[y], coords[x], 0);
        }

        /// <summary>
        /// Metoda do wielokrotnego ruchu normalnego
        /// </summary>
        /// <param name="pieceId">id pionka</param>
        /// <param name="x">współrzędna X pola docelowego</param>
        /// <param name="y">współrzędna Y pola docelowego</param>
        /// <param name="doubleMove">true pozwala na kolejny ruch</param>
        private void MoveTo(string pieceId, int x, int y, bool doubleMove)
        {
            Animation(FindChild<Grid>(BoardBorder, pieceId), coords[x], coords[y], 2);

            FindChild<Grid>(BoardBorder, pieceId).SetValue(System.Windows.Controls.Panel.ZIndexProperty, 99999);

            Piece active = PiecesList.Find(p => p.id == pieceId);
            active.x = x;
            active.y = y;

            PiecesList.Remove(PiecesList.Find(p => p.id == pieceId));
            PiecesList.Add(active);

            if (!doubleMove)
                isWhiteTurn = !isWhiteTurn;
        }

        /// <summary>
        /// Metoda do jednokrotnego ruchu normalnego
        /// </summary>
        /// <param name="pieceId">id pionka</param>
        /// <param name="x">współrzędna X pola docelowego</param>
        /// <param name="y">współrzędna Y pola docelowego</param>
        private void MoveTo(string pieceId, int x, int y)
        {
            MoveTo(pieceId, x, y, false);
        }

        /// <summary>
        /// Metoda do wielokrotnego ruchu ze zbiciem po id
        /// </summary>
        /// <param name="movingPieceId">id pionka bijącego</param>
        /// <param name="mX">współrzędna X pola docelowego</param>
        /// <param name="mY">współrzędna Y pola docelowego</param>
        /// <param name="capturePieceId">id pionka bitego</param>
        /// <param name="doubleMove">true pozwala na kolejny ruch</param>
        private void MoveAndCapture(string movingPieceId, int mX, int mY, string capturePieceId, bool doubleMove)
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

            if (!doubleMove)
                isWhiteTurn = !isWhiteTurn;
        }

        /// <summary>
        /// Metoda do wielokrotnego ruchu ze zbiciem po współrzędnych
        /// </summary>
        /// <param name="movingPieceId">id pionka bijącego</param>
        /// <param name="mX">współrzędna X pola docelowego</param>
        /// <param name="mY">współrzędna Y pola docelowego</param>
        /// <param name="capturedX">współrzędna X bitego pionka</param>
        /// <param name="capturedY">współrzędna Y bitego pionka</param>
        /// /// <param name="doubleMove">true pozwala na kolejny ruch</param>
        private void MoveAndCapture(string movingPieceId, int mX, int mY, int capturedX, int capturedY, bool doubleMove)
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

            PiecesList.Remove(PiecesList.Find(p => p.id == captured.id));
            PiecesList.Add(captured);

            if (!doubleMove)
                isWhiteTurn = !isWhiteTurn;
        }

        /// <summary>
        /// Metoda do jednokrotnego ruchu ze zbiciem po id
        /// </summary>
        /// <param name="movingPieceId">id pionka bijącego</param>
        /// <param name="mX">współrzędna X pola docelowego</param>
        /// <param name="mY">współrzędna Y pola docelowego</param>
        /// <param name="capturePieceId">id pionka bitego</param>
        private void MoveAndCapture(string movingPieceId, int mX, int mY, string capturePieceId)
        {
            MoveAndCapture(movingPieceId, mX, mY, capturePieceId, false);
        }

        /// <summary>
        /// Metoda do jednokrotnego ruchu ze zbiciem po współrzędnych
        /// </summary>
        /// <param name="movingPieceId">id pionka bijącego</param>
        /// <param name="mX">współrzędna X pola docelowego</param>
        /// <param name="mY">współrzędna Y pola docelowego</param>
        /// <param name="capturedX">współrzędna X bitego pionka</param>
        /// <param name="capturedY">współrzędna Y bitego pionka</param>
        private void MoveAndCapture(string movingPieceId, int mX, int mY, int capturedX, int capturedY)
        {
            MoveAndCapture(movingPieceId, mX, mY, capturedX, capturedY, false);
        }

        private void becomeDame(string pieceId)
        {
            Piece active = PiecesList.Find(p => p.id == pieceId);
            active.isDame = true;

            PiecesList.Remove(PiecesList.Find(p => p.id == pieceId));
            PiecesList.Add(active);

            if (active.id.StartsWith("w"))
            {
                dameWhite++;
                kingsCountWhite.Text = dameWhite.ToString();

                FindChild<Grid>(BoardBorder, pieceId).Background = new ImageBrush
                {
                    ImageSource =
                      new BitmapImage(
                        new Uri(@"C:\Users\Mikolaj\Documents\GitHub\Checkers_Check\CheckersCheck\CheckersCheck\Images\dameWhite.png", UriKind.Relative)
                      )
                };
            }
            else if (active.id.StartsWith("b"))
            {
                dameBlack++;
                kingsCountBlack.Text = dameBlack.ToString();

                FindChild<Grid>(BoardBorder, pieceId).Background = new ImageBrush
                {
                    ImageSource =
                      new BitmapImage(
                        new Uri(@"C:\Users\Mikolaj\Documents\GitHub\Checkers_Check\CheckersCheck\CheckersCheck\Images\dameBlack.png", UriKind.Relative)
                      )
                };
            }

            
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
            int wId = 1;
            for (int i = 1; i < 4; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                        {
                            FastTo("w" + wId.ToString(), i, j);
                            Piece tmp = new Piece();
                            tmp.id = "w" + wId.ToString();
                            tmp.x = j;
                            tmp.y = i;
                            tmp.underCapture = false;
                            tmp.isDame = false;

                            PiecesList.Add(tmp);
                            wId++;
                        }
                    }
                    else if (i % 2 != 0)
                    {
                        if (j % 2 != 0)
                        {
                            FastTo("w" + wId.ToString(), i, j);
                            Piece tmp = new Piece();
                            tmp.id = "w" + wId.ToString();
                            tmp.x = j;
                            tmp.y = i;
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
                            tmp.x = j;
                            tmp.y = i;
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
                            tmp.x = j;
                            tmp.y = i;
                            tmp.underCapture = false;
                            tmp.isDame = false;

                            PiecesList.Add(tmp);
                            bId++;
                        }
                    }
                }
            }
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

        private void AnimationCompleted(object sender, RoutedEventArgs e)
        {
            swordState(false);

            List<Piece> pls = new List<Piece>();
            pls = PiecesList.Where(p => p.underCapture == true).ToList();

            Piece captured = PiecesList.Find(p => p.underCapture == true);
            if (captured.id != null)
            {
                PiecesList.Remove(PiecesList.Find(p => p.id == captured.id));

                FindChild<Grid>(BoardBorder, captured.id.ToString()).Visibility = Visibility.Hidden;
                if(captured.id.StartsWith("w"))
                {
                    piecesWhite--;
                    piecesCountWhite.Text = piecesWhite.ToString();
                }
                else if (captured.id.StartsWith("b"))
                {
                    piecesBlack--;
                    piecesCountBlack.Text = piecesBlack.ToString();
                }

                //MoveDone();
            }
        }

        public void GameClock()
        {
            try
            {
                while (gameIsOn)
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
            if (isWhiteTurn)
                CurrentPlayerTextBlock.Text = playerWhite;
            else if (isWhiteTurn == false) 
            {
                CurrentPlayerTextBlock.Text = playerBlack;
            }
        }

        #endregion

        #region Buttons

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MoveTo(TextBoxIdPionka.Text, int.Parse(TextBoxIntX.Text), int.Parse(TextBoxIntY.Text));
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            MoveAndCapture(TextBoxIdPionka.Text, int.Parse(TextBoxIntX.Text), int.Parse(TextBoxIntY.Text), int.Parse(TextBoxIntX_Copy.Text), int.Parse(TextBoxIntY_Copy.Text));
        }

        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            gameIsOn = true;
            Thread gameClockThread = new Thread(new ThreadStart(GameClock));
            gameClockThread.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            becomeDame(TextBoxIdPionka.Text);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MoveAndCapture(TextBoxIdPionka.Text, int.Parse(TextBoxIntX.Text), int.Parse(TextBoxIntY.Text), int.Parse(TextBoxIntX_Copy.Text), int.Parse(TextBoxIntY_Copy.Text), true);
        }
        
        #endregion    
    
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
