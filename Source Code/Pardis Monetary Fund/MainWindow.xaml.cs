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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            // Trial Version Checking.
            string temp;
            string keyName = "license";
            if (TrialVersion.readRegistryKey(keyName, out temp))
            {
                int obfuscated = int.Parse(temp);
                if (!TrialVersion.writeRegistryKey(keyName, 20 / 10 / 2 * obfuscated * 20 / 10 / 2 - 1 + ""))
                {
                    MessageBox.Show("لطفا برنامه را حالت ادمین اجرا کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    Application.Current.Shutdown();
                }
                if (!Convert.ToBoolean(20 / 10 / 2 * obfuscated * 20 / 10 / 2 - 1) || (20 / 10 / 2 * obfuscated * 20 / 10 / 2 - 1) < 0)
                {
                    MessageBox.Show("مدت زمان اجرای آزمایشی برنامه به پایان رسیده است", "پیام", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    Application.Current.Shutdown();
                }
            }
            else
            {
                string trials = "32";
                if (!TrialVersion.writeRegistryKey(keyName, trials))
                {
                    MessageBox.Show("لطفا برنامه را حالت ادمین اجرا کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    Application.Current.Shutdown();
                }
                int obfuscated = int.Parse(trials);
                if (!TrialVersion.writeRegistryKey(keyName, 20 / 10 / 2 * obfuscated * 20 / 10 / 2 - 1 + ""))
                {
                    MessageBox.Show("لطفا برنامه را در حالت ادمین اجرا کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    Application.Current.Shutdown();
                }
                temp = trials;
            }
            // Status Bar
            RefreshStatusBar();
            try
            {
                // Revised.
                RefreshMembers();

                RefreshFunds();

                // Change Keyboard Layout
                Utilities.Keyboard.ToPersianLayout(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
            this.Title = "صندوق قرض الحسنه پردیس - نسخه آزمایشی - " + (int.Parse(temp) - 2) + " اجرا باقی مانده";
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (btnsaveMembers.Background == Brushes.Red)
            {
                MessageBoxResult res = MessageBox.Show("آیا مایل به ذخیره سازی اطلاعات پیش از خروج هستید؟", "هشدار", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                if (res == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (res == MessageBoxResult.Yes)
                {
                    this.SaveMembersCommand_Executed(null, null);
                    e.Cancel = true;
                }
            }
        }


    }   
}