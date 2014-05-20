﻿using System;
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

namespace CheckersCheck.Menu
{
    /// <summary>
    /// Interaction logic for CalibrationPage2.xaml
    /// </summary>
    public partial class CalibrationPage2 : UserControl, ISwitchable
    {
        public CalibrationPage2()
        {
            InitializeComponent();
        }

        #region ISwitchable Members

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new GamePage());
        }

        #endregion

       
    }
}
