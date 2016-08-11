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
        private void IssueLoanFromFund_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbFunds.SelectedItem != null)
                {
                    Fund currentFund = cmbFunds.SelectedItem as Fund;
                    if (currentFund.HasLoan == true)
                    {
                        MessageBox.Show("وام دهی برای این صندوق  قبلا آغاز شده است", "پیام", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                        return;
                    }
                    if (currentFund.MembersCount == 0)
                    {
                        MessageBox.Show("صندوق خالی از عضو است", "پیام", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                        return;
                    }
                    currentFund.HasLoan = true;
                    DatabaseAccess.Update("Funds", "fundId", currentFund.FundId, currentFund, new List<String>() { "LoanAmount", "MembersCount", "Investment" });
                    DatabaseAccess.InitializeLoanRecordForFundMembers(currentFund);
                    RefreshLoansDataGrid();
                    MessageBox.Show("وام دهی از صندوق باموفقیت آغاز شد", "پیام", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        static bool IsInsideRefreshLoansDataGrid = false;
        private void RefreshLoansDataGrid()
        {
            try
            {
                if (!IsInsideRefreshLoansDataGrid)
                {
                    IsInsideRefreshLoansDataGrid = true;
                    Fund selectedFund = cmbFunds.SelectedItem as Fund;
                    if (selectedFund != null)
                    {
                        DataTable loanMembers = DatabaseAccess.ExecuteQuery(@"Select * From Loans INNER JOIN Members ON Loans.memberId = Members.memberId AND fundId = " + selectedFund.FundId);
                        var sel = dgLoans.SelectedIndex;
                        dgLoans.ItemsSource = loanMembers.DefaultView;
                        loanMembers.ColumnChanged += new DataColumnChangeEventHandler(loanMembers_ColumnChanged);
                        dgLoans.SelectedIndex = sel;
                    }
                    else
                    {
                        dgLoans.ItemsSource = null;
                    }
                    IsInsideRefreshLoansDataGrid = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }


        private void cmbFunds_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            RefreshLoansDataGrid();

        }

        private void btnLotteryBetweenRemaind_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgLoans.ItemsSource != null)
                {
                    HashSet<int> reservedTurns = new HashSet<int>();
                    var Max = dgLoans.ItemsSource.Cast<object>().ToList<object>().Count;
                    Random random = new Random();
                    foreach (DataRowView row in dgLoans.ItemsSource)
                    {
                        if ((LOAN_STATES)row["state"] != LOAN_STATES.PENDING)
                            reservedTurns.Add((int)row["turn"]);
                    }
                    foreach (DataRowView row in dgLoans.ItemsSource)
                    {
                        if ((LOAN_STATES)row["state"] == LOAN_STATES.PENDING)
                        {
                            int rand = random.Next(1, Max + 1);
                            while (reservedTurns.Contains(rand))
                            {
                                rand = random.Next(1, Max + 1);
                            }
                            reservedTurns.Add(rand);
                            row["turn"] = rand;
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

        private void btnLotteryBetweenTurnlesses_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgLoans.ItemsSource != null)
                {
                    HashSet<int> reservedTurns = new HashSet<int>();
                    var Max = dgLoans.ItemsSource.Cast<object>().ToList<object>().Count;
                    foreach (DataRowView row in dgLoans.ItemsSource)
                    {
                        reservedTurns.Add((int)row["turn"]);
                    }
                    Random random = new Random();
                    foreach (DataRowView row in dgLoans.ItemsSource)
                    {
                        if (((int)row["turn"] == -1) && ((LOAN_STATES)row["state"] == LOAN_STATES.PENDING))
                        {
                            int rand = random.Next(1, Max + 1);
                            while (reservedTurns.Contains(rand))
                            {
                                rand = random.Next(1, Max + 1);
                            }
                            reservedTurns.Add(rand);
                            row["turn"] = rand;
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

        private void RefreshPaymentsListView()
        {
            try
            {
                DataRowView currentRow = dgLoans.SelectedItem as DataRowView;
                Fund currentFund = cmbFunds.SelectedItem as Fund;
                if (currentRow != null && currentFund != null)
                {
                    lvPayments.ItemsSource = DatabaseAccess.GetPayments((int)currentRow["memberId"], DatabaseAccess.GetLoanId((int)currentRow["memberId"], currentFund.FundId));
                    currentFund.Investment = DatabaseAccess.GetFundInvestment(currentFund.FundId);
                }
                else
                {
                    lvPayments.ItemsSource = null;
                }
                dgLoans.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                dgLoans.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        private void CheckLoanState()
        {
            try
            {
                int loanId = (int)((dgLoans.SelectedItem as DataRowView)["loanId"]);
                int memberId = (int)((dgLoans.SelectedItem as DataRowView)["memberId"]);
                int currentLoanState = (int)((dgLoans.SelectedItem as DataRowView)["state"]);
                decimal loanAmount = (cmbFunds.SelectedItem as Fund).LoanAmount;
                decimal credit = DatabaseAccess.GetMemberCredit(memberId, loanId);
                switch ((LOAN_STATES)currentLoanState)
                {
                    case LOAN_STATES.PENDING:

                        break;
                    case LOAN_STATES.SETTLED:
                        if (credit < loanAmount)
                        {
                            MessageBoxResult res = MessageBox.Show(
                                "مبالغ موجود کمتر از بدهی فرد است\n" + "آیا مایل هستید که وضعیت وام به تسویه نشده تغییر یابد؟",
                                "سوال",
                                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                            switch (res)
                            {
                                case MessageBoxResult.Yes:
                                    (dgLoans.SelectedItem as DataRowView)["state"] = (int)LOAN_STATES.UN_SETTLED;
                                    break;
                            }
                        }
                        break;
                    case LOAN_STATES.UN_SETTLED:
                        if (credit >= loanAmount)
                        {
                            MessageBoxResult res = MessageBox.Show(
                                "بدهی این وام به اتمام رسیده است\n" + "آیا مایل هستید که وضعیت وام به تسویه شده تغییر یابد؟",
                                "سوال",
                                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                            switch (res)
                            {
                                case MessageBoxResult.Yes:
                                    (dgLoans.SelectedItem as DataRowView)["state"] = (int)LOAN_STATES.SETTLED;
                                    break;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        private void btnAddPayment_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView item = dgLoans.SelectedItem as DataRowView;
                if (item != null)
                {
                    AddPaymentDialog addPaymentDialog = new AddPaymentDialog();
                    addPaymentDialog.txtAmount.Text = (cmbFunds.SelectedItem as Fund) == null ? "0" : (cmbFunds.SelectedItem as Fund).InstallmentAmount.ToString();
                    addPaymentDialog.txtDate.Text = PersianDate.GregorianDateToPersianDate(DateTime.Now);
                    if (addPaymentDialog.ShowDialog() == true)
                    {
                        Payment newPayment = new Payment()
                        {
                            Amount = addPaymentDialog.Amount,
                            Date = String.IsNullOrEmpty(addPaymentDialog.txtDate.Text) ? PersianDate.Now : addPaymentDialog.txtDate.Text,
                            MemberId = (int)item["memberId"],
                            LoanId = DatabaseAccess.GetLoanId((int)item["memberId"], (cmbFunds.SelectedItem as Fund).FundId),
                            Description = String.IsNullOrWhiteSpace(addPaymentDialog.txtDesc.Text) ? "ندارد" : addPaymentDialog.txtDesc.Text
                        };
                        DatabaseAccess.Insert("Payments", "paymentId", newPayment, new List<String>() { "Number" });
                        RefreshPaymentsListView();
                        CheckLoanState();
                    }
                }
                else
                {
                    MessageBox.Show("ابتدا یک عضو را انتخاب کنید", "پیام", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        private void loansDataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                RefreshPaymentsListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        private void btnEditPayment_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Payment currentPayment = lvPayments.SelectedItem as Payment;
                if (currentPayment == null)
                {
                    MessageBox.Show("ابتدا یک پرداخت را انتخاب کنید", "پیام", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    return;
                }
                else
                {
                    AddPaymentDialog addPaymentDialog = new AddPaymentDialog();
                    addPaymentDialog.Title = "ویرایش پرداخت";
                    addPaymentDialog.txtDesc.Text = currentPayment.Description;
                    addPaymentDialog.txtDate.Text = currentPayment.Date;
                    addPaymentDialog.Amount = currentPayment.Amount;
                    if (addPaymentDialog.ShowDialog() == true)
                    {
                        Payment modifiedPayment = new Payment()
                        {
                            Amount = decimal.Parse((String.IsNullOrEmpty(addPaymentDialog.txtAmount.Text) ? "0" : addPaymentDialog.txtAmount.Text)),
                            Date = String.IsNullOrEmpty(addPaymentDialog.txtDate.Text) ? PersianDate.Now : addPaymentDialog.txtDate.Text,
                            MemberId = (int)(dgLoans.SelectedItem as DataRowView)["memberId"],
                            LoanId = DatabaseAccess.GetLoanId((int)(dgLoans.SelectedItem as DataRowView)["memberId"], (cmbFunds.SelectedItem as Fund).FundId),
                            Description = String.IsNullOrWhiteSpace(addPaymentDialog.txtDesc.Text) ? "ندارد" : addPaymentDialog.txtDesc.Text
                        };
                        DatabaseAccess.Update("Payments", "paymentId", currentPayment.PaymentId, modifiedPayment, new List<string>() { "Number" });
                        RefreshPaymentsListView();
                        CheckLoanState();
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

        private void btnRemovePayment_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Guard
                if (lvPayments.SelectedItems.Count == 0)
                    return;
                MessageBoxResult res = MessageBox.Show("آیا از حذف پرداخت(ها) مطمئن هستید؟\nاین کار برگشت پذیر نیست", "هشدار", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                if (res == MessageBoxResult.Yes)
                {
                    if (!DatabaseAccess.DeleteAll("Payments", "paymentId", lvPayments.SelectedItems.Cast<object>().ToList<object>()))
                    {
                        MessageBox.Show("حذف پرداخت(ها) با خطا مواجه شده است", "خطا", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    }
                    RefreshPaymentsListView();
                    CheckLoanState();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        private void lvPayments_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Payment;
            if (item != null)
            {
                this.btnEditPayment_Click_1(null, null);
            }
        }

        void loanMembers_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            try
            {
                var memberId = Convert.ToInt32(e.Row["memberId"]);
                if (e.Column.ColumnName == "turn")
                    DatabaseAccess.ExecuteNonQuery(@"Update Loans Set turn = " + e.ProposedValue + @" where memberId = " + memberId + " and loanId = " + DatabaseAccess.GetLoanId(memberId, (cmbFunds.SelectedItem as Fund).FundId));
                else if (e.Column.ColumnName == "state")
                    DatabaseAccess.ExecuteNonQuery(@"Update Loans Set state = " + e.ProposedValue + @" where memberId = " + memberId + " and loanId = " + DatabaseAccess.GetLoanId(memberId, (cmbFunds.SelectedItem as Fund).FundId));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        private bool isLoansDataGridManualEditComment;
        private void loansDataGrid_CellEditEnding_1(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (!isLoansDataGridManualEditComment)
                {
                    isLoansDataGridManualEditComment = true;
                    (sender as DataGrid).CommitEdit(DataGridEditingUnit.Row, true);
                    isLoansDataGridManualEditComment = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        private void tabLoans_Unselected_1(object sender, RoutedEventArgs e)
        {
            tabControl.Focus();
        }

        private void loansDataGrid_RowDetailsVisibilityChanged_1(object sender, DataGridRowDetailsEventArgs e)
        {
            try
            {
                if (e.DetailsElement.IsVisible == true)
                {
                    DataRowView member = dgLoans.SelectedItem as DataRowView;
                    decimal credit = DatabaseAccess.GetMemberCredit((int)member["memberId"], (int)member["loanId"]); ;
                    decimal indebted = 0;
                    SolidColorBrush stateColor = Brushes.Orange;

                    var loanAmount = (cmbFunds.SelectedItem as Fund).LoanAmount;

                    if (member != null)
                    {
                        switch ((LOAN_STATES)member["state"])
                        {
                            case LOAN_STATES.PENDING:
                                if (credit > loanAmount)
                                {
                                    stateColor = Brushes.Orange;
                                }
                                else
                                {
                                    stateColor = Brushes.Yellow;
                                }
                                indebted = 0;
                                break;
                            case LOAN_STATES.SETTLED:
                                if (credit <= loanAmount)
                                {
                                    stateColor = Brushes.Green;
                                    indebted = loanAmount - credit;
                                    credit = 0;
                                }
                                else
                                {
                                    indebted = 0;
                                    credit = credit - loanAmount;
                                    stateColor = Brushes.Orange;
                                }
                                break;
                            case LOAN_STATES.UN_SETTLED:
                                if (credit <= loanAmount)
                                {
                                    indebted = loanAmount - credit;
                                    credit = 0;
                                    stateColor = Brushes.Red;
                                }
                                else
                                {
                                    indebted = 0;
                                    credit = credit - loanAmount;
                                    stateColor = Brushes.Orange;
                                }
                                break;
                        }
                        (e.DetailsElement as Grid).DataContext = new LoanRowDetails() { Credit = credit, Indebted = indebted, Color = stateColor };
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
        private string GetLoanStateString(LOAN_STATES state)
        {
            switch (state)
            {
                case LOAN_STATES.PENDING:
                    return "در انتظار";
                case LOAN_STATES.SETTLED:
                    return "تسویه شده";
                case LOAN_STATES.UN_SETTLED:
                    return "تسویه نشده";
            }
            return "";
        }


        private void tabLoans_Selected_1(object sender, RoutedEventArgs e)
        {
            // TODO: Nothing.
        }
        private GridViewColumnHeader lvPaymentsSortCol = null;
        private SortAdorner lvPaymentsSortAdorner = null;
        private void lvPaymentsColumnHeader_Click_1(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (lvPaymentsSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(lvPaymentsSortCol).Remove(lvPaymentsSortAdorner);
                lvPayments.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (lvPaymentsSortCol == column && lvPaymentsSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            lvPaymentsSortCol = column;
            lvPaymentsSortAdorner = new SortAdorner(lvPaymentsSortCol, newDir);
            AdornerLayer.GetAdornerLayer(lvPaymentsSortCol).Add(lvPaymentsSortAdorner);
            lvPayments.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

    }

    public class LoanRowDetails
    {
        SolidColorBrush color;

        public SolidColorBrush Color
        {
            get { return color; }
            set { color = value; }
        }
        decimal credit, indebted;

        public decimal Indebted
        {
            get { return indebted; }
            set { indebted = value; }
        }

        public decimal Credit
        {
            get { return credit; }
            set { credit = value; }
        }
    }
}

