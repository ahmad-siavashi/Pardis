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
        private DataTable membersTable;

        private DataTable MembersTable
        {
            get { return membersTable; }
            set { membersTable = value; }
        }

        public void RefreshMembers()
        {
            // Members List
            MembersTable = DatabaseAccess.RetrieveMembers();
            MembersTable.DefaultView.ListChanged += this.MembersListDefaultView_ListChanged;
            dgMembers.ItemsSource = MembersTable.DefaultView;
        }

        public void MembersListDefaultView_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            btnsaveMembers.Background = Brushes.Red;
            // TODO: There must be a better solution.
            if (e.ListChangedType == System.ComponentModel.ListChangedType.ItemAdded)
            {
                List<Xceed.Wpf.Toolkit.MaskedTextBox> list = new List<Xceed.Wpf.Toolkit.MaskedTextBox>();
                ViewControls.FindChildGroup<Xceed.Wpf.Toolkit.MaskedTextBox>(dgMembers, "maskedtxtRegistrationDate", ref list);
                if (list.Last().Text == "____/__/__")
                    list.Last().Text = PersianDate.GregorianDateToPersianDate(DateTime.Now);
                (sender as DataView)[e.NewIndex]["registrationDate"] = list.Last().Text;

            }
        }
        private bool isManualEditComment;
        private void membersDataGrid_CellEditEnding_1(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (!isManualEditComment)
                {
                    isManualEditComment = true;
                    (sender as DataGrid).CommitEdit(DataGridEditingUnit.Row, true);
                    isManualEditComment = false;
                    btnsaveMembers.Background = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        private void txtmemberSearch_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                Brush b = btnsaveMembers.Background;
                if (String.IsNullOrEmpty(txtmemberSearch.Text))
                {
                    membersTable.DefaultView.RowFilter = "";
                }
                else
                {
                    String filter = "";
                    foreach (System.Data.DataColumn col in membersTable.Columns)
                    {
                        if (col.ColumnName != "memberId")
                        {
                            filter += col.ColumnName + " LIKE '%" + txtmemberSearch.Text + "%'";
                            filter += " OR ";
                        }
                    }
                    filter = filter.Substring(0, filter.LastIndexOf("OR"));
                    membersTable.DefaultView.RowFilter = filter;
                }
                CollectionViewSource.GetDefaultView(membersTable).Refresh();
                btnsaveMembers.Background = b;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        private void membersDataGrid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Device.Target.GetType().Name == "DataGridCell")
                {
                    if (e.Key == Key.Delete)
                    {
                        MessageBoxResult res = MessageBox.Show("آیا از حذف عضو(ها) اطمینان دارید؟\nاین کار برگشت پذیر نیست", "تاییدیه", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                        if (res == MessageBoxResult.Yes)
                        {

                            foreach (object row in dgMembers.SelectedItems)
                            {
                                if (row is DataRowView)
                                {
                                    var id = (row as DataRowView)["memberId"];
                                    if (!(id is DBNull) && DatabaseAccess.CanDeleteMember(Convert.ToInt32(id)) != MemberDeleteState.OK)
                                    {
                                        MessageBox.Show("نمی توانید عضوی که در صندوقی مشارکت دارد را حذف کنید", "هشدار", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                                        e.Handled = true;
                                        return;
                                    }
                                }
                            }
                            e.Handled = false;
                        }
                        else
                        {
                            e.Handled = true;
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

        private void tabMembers_Unselected_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnsaveMembers.Background == Brushes.Red)
                {
                    MessageBoxResult res = MessageBox.Show("آیا مایل به ذخیره سازی تغییرات هستید؟", "پیام", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    if (res == MessageBoxResult.Yes)
                    {
                        if (DatabaseAccess.SubmitMembers(MembersTable))
                        {
                            btnsaveMembers.Background = null;
                        }
                        else
                        {
                            MessageBox.Show("در ذخیره سازی اطلاعات خطایی رخ داده است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                tabControl.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
        }

        private void btnAddMember_Click_1(object sender, RoutedEventArgs e)
        {
            AddMemberDialog addMemberDialog = new AddMemberDialog();
            addMemberDialog.txtRegistrationDate.Text = PersianDate.GregorianDateToPersianDate(DateTime.Now);
            if (addMemberDialog.ShowDialog() == true)
            {
                DataView dv = dgMembers.ItemsSource as DataView;
                DataRowView newRow = dv.AddNew();
                newRow["registrationId"] = addMemberDialog.txtRegistrationId.Text;
                newRow["firstName"] = addMemberDialog.txtFirstName.Text;
                newRow["lastName"] = addMemberDialog.txtLastName.Text;
                newRow["fatherName"] = addMemberDialog.txtFatherName.Text;
                newRow["phoneNumber"] = addMemberDialog.txtPhoneNumber.Text;
                newRow["nationalCode"] = addMemberDialog.txtNationalCode.Text;
                newRow["registrationDate"] = addMemberDialog.txtRegistrationDate.Text;
                newRow.EndEdit();
            }
        }

        private void btnEditMember_Click_1(object sender, RoutedEventArgs e)
        {
            if (dgMembers.SelectedItem is DataRowView)
            {
                DataRowView selectedItem = dgMembers.SelectedItem as DataRowView;
                AddMemberDialog addMemberDialog = new AddMemberDialog();
                addMemberDialog.Title = "ویرایش عضو";
                addMemberDialog.txtFirstName.Text = selectedItem["firstName"] as string;
                addMemberDialog.txtLastName.Text = selectedItem["lastName"] as string;
                addMemberDialog.txtFatherName.Text = selectedItem["fatherName"] as string;
                addMemberDialog.txtNationalCode.Text = selectedItem["nationalCode"] as string;
                addMemberDialog.txtPhoneNumber.Text = selectedItem["phoneNumber"] as string;
                addMemberDialog.txtRegistrationDate.Text = selectedItem["registrationDate"] as string;
                addMemberDialog.txtRegistrationId.Text = selectedItem["registrationId"] as string;
                if (addMemberDialog.ShowDialog() == true)
                {
                    selectedItem.BeginEdit();
                    selectedItem["registrationId"] = addMemberDialog.txtRegistrationId.Text;
                    selectedItem["firstName"] = addMemberDialog.txtFirstName.Text;
                    selectedItem["lastName"] = addMemberDialog.txtLastName.Text;
                    selectedItem["fatherName"] = addMemberDialog.txtFatherName.Text;
                    selectedItem["phoneNumber"] = addMemberDialog.txtPhoneNumber.Text;
                    selectedItem["nationalCode"] = addMemberDialog.txtNationalCode.Text;
                    selectedItem["registrationDate"] = addMemberDialog.txtRegistrationDate.Text;
                    selectedItem.EndEdit();
                }
            }
        }

        private void btnRemoveMembers_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgMembers.SelectedItems.Count > 0)
                {
                    MessageBoxResult res = MessageBox.Show("آیا از حذف عضو(ها) اطمینان دارید؟\nاین کار برگشت پذیر نیست", "تاییدیه", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                    if (res == MessageBoxResult.Yes)
                    {
                        foreach (object row in dgMembers.SelectedItems)
                        {
                            if (row is DataRowView)
                            {
                                var id = (row as DataRowView)["memberId"];
                                if (!(id is DBNull) && DatabaseAccess.CanDeleteMember(Convert.ToInt32(id)) != MemberDeleteState.OK)
                                {
                                    MessageBox.Show("نمی توانید عضوی که در صندوقی مشارکت دارد را حذف کنید", "هشدار", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
                                    return;
                                }
                            }
                        }
                        foreach (object row in dgMembers.SelectedItems.Cast<object>().ToList<object>())
                        {
                            if (row is DataRowView)
                                (row as DataRowView).Delete();
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

        private void maskedtxtRegistrationDate_GotFocus_1(object sender, RoutedEventArgs e)
        {
            btnsaveMembers.Background = Brushes.Red;
        }

        private void maskedtxtRegistrationDate_TextChanged_1(object sender, TextChangedEventArgs e)
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
