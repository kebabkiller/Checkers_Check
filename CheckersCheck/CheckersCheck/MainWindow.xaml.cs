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

namespace CheckersCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SolidColorBrush defaultBrush = new SolidColorBrush(Colors.White);
            SolidColorBrush alternateBrush = new SolidColorBrush(Colors.Gray);
            for (int i = 0; i < 64; i++)
            {
                Grid cell = new Grid();
                if ((i + i/8)%2 == 0)
                {
                    cell.Background = defaultBrush;
                    ChessBoard.Children.Add(cell);
                }
                else
                {
                    cell.Background = alternateBrush;
                    ChessBoard.Children.Add(cell);
                }
            }
        }
    }
}
