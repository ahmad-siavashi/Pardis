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
using Pardis_Monetary_Fund.Utilities;

namespace Pardis_Monetary_Fund
{
    /// <summary>
    /// Interaction logic for AddMemberDialog.xaml
    /// </summary>
    public partial class AddMemberDialog : Window
    {
        public AddMemberDialog()
        {
            InitializeComponent();
            Utilities.Keyboard.ToPersianLayout(this);
        }

        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            if (txtRegistrationDate.Background == Brushes.Tomato)
            {
                MessageBox.Show("تاریخ وارد شده معتبر نمی باشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
            else
            {
                this.DialogResult = true;
            }
        }

        private void txtRegistrationDate_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            Xceed.Wpf.Toolkit.MaskedTextBox txtBox = sender as Xceed.Wpf.Toolkit.MaskedTextBox;
            try
            {
                PersianDate.PersianDateToGregorianDate(txtBox.Text);
                txtBox.Background = null;
            }
            catch
            {
                txtBox.Background = Brushes.Tomato;
            }
        }
    }
}
