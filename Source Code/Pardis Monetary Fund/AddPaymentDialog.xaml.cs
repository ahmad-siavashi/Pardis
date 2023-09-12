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
using NLog;
using Pardis_Monetary_Fund.Utilities;

namespace Pardis_Monetary_Fund
{
    /// <summary>
    /// Interaction logic for AddPaymentDialog.xaml
    /// </summary>
    public partial class AddPaymentDialog : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public AddPaymentDialog()
        {
            InitializeComponent();
            Utilities.Keyboard.ToPersianLayout(this);
        }

        private void btnSavePayment_Click_1(object sender, RoutedEventArgs e)
        {
            if (txtDate.Background == Brushes.Tomato)
            {
                MessageBox.Show("تاریخ وارد شده معتبر نمی باشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
            else
            {
                this.DialogResult = true;
            }
        }

        private void txtAmount_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtAmount.Text))
                {
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("fa");
                    decimal valueBefore = decimal.Parse(txtAmount.Text, System.Globalization.NumberStyles.AllowThousands);
                    txtAmount.Text = String.Format(culture, "{0:N0}", valueBefore);
                    txtAmount.Select(txtAmount.Text.Length, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
            }
        }
        public decimal Amount
        {
            get { return decimal.Parse(String.IsNullOrWhiteSpace(txtAmount.Text) ? "0" : txtAmount.Text, System.Globalization.NumberStyles.AllowThousands); }
            set
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("fa");
                txtAmount.Text = String.Format(culture, "{0:N0}", value);
                txtAmount.Select(txtAmount.Text.Length, 0);
            }
        }

        private void txtAmount_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void txtAmount_Pasting_1(object sender, DataObjectPastingEventArgs e)
        {
            var text = e.DataObject.GetData(typeof(string)).ToString();
            if (!char.IsDigit(text, text.Length - 1))
            {
                e.CancelCommand();
            }
        }

        private void txtDate_TextChanged_1(object sender, TextChangedEventArgs e)
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
