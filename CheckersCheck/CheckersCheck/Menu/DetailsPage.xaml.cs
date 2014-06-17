using System;
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

namespace CheckersCheck.Menu
{
    /// <summary>
    /// Interaction logic for DetailsPage.xaml
    /// </summary>
    public partial class DetailsPage : UserControl, ISwitchable
    {
        public TempData tmpData;
        private string[] data;
        public DetailsPage()
        {
            InitializeComponent();
            tmpData = new TempData();
        }

        #region ISwitchable Members

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(SaveDetails())
                Switcher.Switch(new CalibrationPage(TextBoxPlayer1.Text,TextBoxPlayer2.Text));
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new DetailsPage());
        }

        #endregion

        public bool SaveDetails()
        {
            if(TextBoxPlayer1.Text != "")
                tmpData.playerWhite = TextBoxPlayer1.Text;
            else
            {
                MessageBox.Show("Nie wprowadzono poprawnie nazw graczy", "Checkers Check");
                return false;
            }

            if(TextBoxPlayer2.Text != "")
                tmpData.playerBlack = TextBoxPlayer2.Text;
            else
            {
                MessageBox.Show("Nie wprowadzono poprawnie nazw graczy", "Checkers Check");
                return false;
            }        

            return true;
        }


        //public bool SaveDetails()
        //{
        //    if (TextBoxPlayer1.Text == "")
        //    // tmpData.playerWhite = TextBoxPlayer1.Text;
        //    {
        //        MessageBox.Show("Checkers Check", "Nie wprowadzono poprawnie nazw graczy");
        //        return false;
        //    }

        //    if (TextBoxPlayer2.Text == "")
        //    // tmpData.playerBlack = TextBoxPlayer2.Text;
        //    {
        //        MessageBox.Show("Checkers Check", "Nie wprowadzono poprawnie nazw graczy");
        //        return false;
        //    }
        //    else
        //    {
        //        data = new string[] { TextBoxPlayer1.Text, TextBoxPlayer2.Text };
        //    }

        //    File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\data", data);

        //    return true;
        //}
       
    }
}
