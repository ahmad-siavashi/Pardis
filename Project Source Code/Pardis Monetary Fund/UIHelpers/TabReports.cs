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

        private void dgReport_AutoGeneratingColumn_1(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header == "مبلغ")
            {
                DataGridTemplateColumn dgTemplateCol = new DataGridTemplateColumn();
                dgTemplateCol.Header = "مبلغ";
                dgTemplateCol.SortMemberPath = "مبلغ";
                FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));
                Binding binding = new Binding("مبلغ");
                binding.StringFormat = "{0:n0}";
                binding.Mode = BindingMode.Default;
                textBlock.SetValue(TextBlock.TextProperty, binding);
                textBlock.SetValue(TextBlock.FontFamilyProperty, new FontFamily("Arial"));
                DataTemplate dataTemplate = new DataTemplate();
                dataTemplate.VisualTree = textBlock;
                dgTemplateCol.CellTemplate = dataTemplate;
                e.Column = dgTemplateCol;
            }
        }

        private void btnViewReport_Click_1(object sender, RoutedEventArgs e)
        {
            DataTable result = new DataTable();
            var fund = cmbReportFunds.SelectedItem as FundReportItem;
            if (fund != null)
            {
                var fundId = Convert.ToInt32(fund.FundId);
                var registrationId = txtRegistrationId.Text;
                var fromDate = txtFromDate.Text;
                var toDate = txtToDate.Text;
                switch (cmbReport.SelectedIndex)
                {
                    case 0:
                        if (fundId != -1)
                        {
                            result = DatabaseAccess.GetFundPayments(fundId, registrationId, fromDate, toDate);
                        }
                        else
                        {
                            foreach (var f in funds)
                            {
                                result.Merge(DatabaseAccess.GetFundPayments(f.FundId, registrationId, fromDate, toDate));
                            }
                        }
                        break;
                    case 1:
                        if (fundId != -1)
                        {
                            result = DatabaseAccess.GetMemberPayments(fundId, registrationId, fromDate, toDate);
                        }
                        else
                        {
                            foreach (var f in funds)
                            {
                                result.Merge(DatabaseAccess.GetMemberPayments(f.FundId, registrationId, fromDate, toDate));
                            }
                        }
                        break;
                    default:
                        return;
                }
                dgReport.ItemsSource = result.DefaultView;
            }
        }

        private void RefreshReportFundsComboBox(){
            ObservableCollection<FundReportItem> funds = new ObservableCollection<FundReportItem>();
            if (this.funds.Count > 0)
            {
                funds.Add(new FundReportItem() { FundId = -1, Name = "همه" });
            }
            foreach (var fund in this.funds)
                funds.Add(new FundReportItem() { FundId = fund.FundId, Name = fund.ToString() });
            cmbReportFunds.ItemsSource = funds;
            cmbReportFunds.SelectedIndex = 0;
        }

        private void tabReports_Selected_1(object sender, RoutedEventArgs e)
        {
            if (cmbReportFunds.ItemsSource == null)
                RefreshReportFundsComboBox();
        }

        private void txtFromDate_TextChanged_1(object sender, TextChangedEventArgs e)
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

        private void txtToDate_TextChanged_1(object sender, TextChangedEventArgs e)
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

    class FundReportItem
    {
        int fundId;
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int FundId
        {
            get { return fundId; }
            set { fundId = value; }
        }
    }

}
