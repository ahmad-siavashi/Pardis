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
using Pardis_Monetary_Fund.Utilities;
using System.Windows.Threading;
using Pardis_Monetary_Fund.InformationLayer;
using System.Collections.ObjectModel;
using Pardis_Monetary_Fund.DataModels;
using System.Data;
using NLog;
using System.ComponentModel;

namespace Pardis_Monetary_Fund
{
    public partial class MainWindow : Window
    {
        public void RefreshStatusBar()
        {
            // Status Bar
            statusBarDate.DataContext = PersianDate.Now;
        }
        private void btnBackup_Click_1(object sender, RoutedEventArgs e)
        {
            new BackupDialog().ShowDialog();
        }

        private void btnInfo_Click_1(object sender, RoutedEventArgs e)
        {
            new AboutDialog().ShowDialog();
        }
    }
}