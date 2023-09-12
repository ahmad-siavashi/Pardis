using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Pardis_Monetary_Fund.InformationLayer;
using Pardis_Monetary_Fund.Utilities;

namespace Pardis_Monetary_Fund
{
    public partial class MainWindow : Window
    {
        // Members Commands
        private void SaveMembersCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                dgMembers.Focus();
                if (DatabaseAccess.SubmitMembers(MembersTable))
                {
                    btnsaveMembers.Background = null;
                    System.Windows.MessageBox.Show("اطلاعات با موفقیت ذخیره شدند", "پیام", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                }
                else
                {
                    MessageBox.Show("در ذخیره سازی اطلاعات خطایی رخ داده است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                }
                RefreshMembers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        private void CommandBinding_CanExecute_1(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand SaveMembers = new RoutedUICommand
            (
                "ذخیره",
                "SaveMembers",
                typeof(CustomCommands),
                new InputGestureCollection()
                { 
                    new KeyGesture(Key.S, ModifierKeys.Control) 
                }
            );
    }
}
