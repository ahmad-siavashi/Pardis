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

        private ObservableCollectionWithItemNotify<Fund> funds = null;

        public void RefreshFunds()
        {
            funds = DatabaseAccess.GetFunds();
            funds.CollectionChanged += funds_CollectionChanged;
            lvFunds.ItemsSource = funds;
            cmbFunds.ItemsSource = funds;
            RefreshReportFundsComboBox();
        }

        void funds_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            cmbFunds.ItemsSource = lvFunds.ItemsSource;
            RefreshReportFundsComboBox();
        }

        // Revised.
        private void btnAddFund_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                AddFund addFundDialog = new AddFund();
                addFundDialog.InstallmentAmount = 0;
                if (addFundDialog.ShowDialog() == true)
                {
                    Fund newFund = new Fund()
                    {
                        EstablishmentDate = PersianDate.GregorianDateToPersianDate(DateTime.Now),
                        InstallmentAmount = addFundDialog.InstallmentAmount,
                        MembersCount = addFundDialog.SelectedMembers.Count
                    };
                    int newFundId = DatabaseAccess.InsertFund(newFund, addFundDialog.SelectedMembers.Cast<Member>().ToList<Member>());
                    funds.Add(DatabaseAccess.GetFund(newFundId));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        // Revised.
        private void btnDeleteFund_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvFunds.SelectedItems.Count > 0)
                {
                    MessageBoxResult res = MessageBox.Show("آیا از حذف صندوق(ها) اطمینان دارید؟\nصندوق به همراه تمامی وام ها و پرداخت های آن حذف می شود\nاین کار برگشت پذیر نیست", "تاییدیه", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    if (res == MessageBoxResult.Yes)
                    {
                        foreach (Fund fund in lvFunds.SelectedItems.Cast<Fund>().ToList<Fund>())
                        {
                            if (DatabaseAccess.CanDeleteFund(fund.FundId) == FundDeleteState.HasLoan)
                            {
                                MessageBox.Show("نمی توانید صندوقی که وام های آن تسویه نشده اند را حذف کنید\nصندوق شماره " + fund.Number, "خطا", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                                return;
                            }
                        }
                        foreach (Fund fund in lvFunds.SelectedItems.Cast<Fund>().ToList<Fund>())
                        {
                            if (DatabaseAccess.DeleteFund(fund.FundId))
                            {
                                (lvFunds.ItemsSource as ObservableCollectionWithItemNotify<Fund>).Remove(fund);
                            }
                            else
                            {
                                MessageBox.Show("در حذف صندوق خطایی رخ داده است", "خطا", MessageBoxButton.OK, MessageBoxImage.Stop);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        // Revised.
        private void btnEditFund_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Fund fund = lvFunds.SelectedItem as Fund;
                if (fund != null)
                {
                    if (fund.HasLoan == true)
                    {
                        MessageBox.Show("نمی توانید صندوقی که در حال وام دهی است را ویرایش کنید", "پیام", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                        return;
                    }
                    AddFund addFundDialog = new AddFund();
                    addFundDialog.Title = "ویرایش صندوق";
                    List<Member> members = DatabaseAccess.GetFundMembers(fund.FundId);
                    foreach (var m in members)
                        addFundDialog.SelectedMembers.Add(m);
                    addFundDialog.InstallmentAmount = fund.InstallmentAmount;
                    if (addFundDialog.ShowDialog() == true)
                    {
                        DatabaseAccess.Delete("Members_Funds", "fundId", fund.FundId);
                        fund.InstallmentAmount = addFundDialog.InstallmentAmount;
                        fund.MembersCount = addFundDialog.SelectedMembers.Count;
                        DatabaseAccess.Update("Funds", "fundId", fund.FundId, fund, new List<String>() { "LoanAmount", "MembersCount", "Investment" });
                        DatabaseAccess.AddFundMembers(fund.FundId, addFundDialog.SelectedMembers.Cast<Member>().ToList<Member>());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }
        private void lvFunds_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Fund;
            if (item != null)
            {
                this.btnEditFund_Click_1(null, null);
            }
        }

        // Revised.
        private void tabFunds_Selected_1(object sender, RoutedEventArgs e)
        {
            if (funds == null)
                RefreshFunds();
        }

        private void lvFunds_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Delete)
                {
                    btnDeleteFund_Click_1(null, null);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        private GridViewColumnHeader lvFundsSortCol = null;
        private SortAdorner lvFundsSortAdorner = null;
        private void lvFundsColumnHeader_Click_1(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (lvFundsSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(lvFundsSortCol).Remove(lvFundsSortAdorner);
                lvFunds.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (lvFundsSortCol == column && lvFundsSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            lvFundsSortCol = column;
            lvFundsSortAdorner = new SortAdorner(lvFundsSortCol, newDir);
            AdornerLayer.GetAdornerLayer(lvFundsSortCol).Add(lvFundsSortAdorner);
            lvFunds.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }
    }
}
