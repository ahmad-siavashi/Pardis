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
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using NLog;
using Pardis_Monetary_Fund.Utilities;
using Pardis_Monetary_Fund.InformationLayer;

namespace Pardis_Monetary_Fund
{
    /// <summary>
    /// Interaction logic for BackupDialog.xaml
    /// </summary>
    public partial class BackupDialog : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public BackupDialog()
        {
            InitializeComponent();
            string path = System.Environment.CurrentDirectory + @"\Backups";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void btnBackup_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = System.Environment.CurrentDirectory + @"\Backups";
                saveFileDialog.Filter = "SQL Server Compact Edition (*.sdf)|*.sdf";
                saveFileDialog.FileName = PersianDate.GregorianDateToPersianDate(DateTime.Now).Replace("/", "-") + "-" + PersianDate.ClockNow;
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.Copy(System.Environment.CurrentDirectory + @"\" + Pardis_Monetary_Fund.InformationLayer.DatabaseAccess.DatabaseFileName, saveFileDialog.FileName);
                    MessageBox.Show("پشتیبان گیری با موفقیت صورت پذیرفت", "پیام", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
            }
        }

        private void btnRestore_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = System.Environment.CurrentDirectory + @"\Backups";
                openFileDialog.Filter = "SQL Server Compact Edition (*.sdf)|*.sdf";
                if (openFileDialog.ShowDialog() == true)
                {
                    MessageBoxResult res = MessageBox.Show("آیا از جایگزینی اطمینان دارید؟\nدر صورتی که از اطلاعات فعلی پشتیبان گیری نکرده باشید، برگرداندن داده های فعلی غیر ممکن خواهد بود", "هشدار", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    if (res == MessageBoxResult.Yes)
                    {
                        File.Copy(openFileDialog.FileName, System.Environment.CurrentDirectory + @"\" + Pardis_Monetary_Fund.InformationLayer.DatabaseAccess.DatabaseFileName, true);
                        MessageBox.Show("جایگزینی با موفقیت صورت پذیرفت\nمی بایست برنامه را مجددا اجرا کنید", "پیام", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                        Application.Current.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
            }
        }
/*
        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            switch (Convert.ToInt32((sender as Slider).Value))
            {
                case 0:
                    txtAutoBackupState.Text = "خاموش";
                    break;
                case 1:
                    txtAutoBackupState.Text = "روزانه";
                    break;
                case 2:
                    txtAutoBackupState.Text = "هفتگی";
                    break;
                case 3:
                    txtAutoBackupState.Text = "ماهانه";
                    break;
            }
        }
/**/
    }
}
