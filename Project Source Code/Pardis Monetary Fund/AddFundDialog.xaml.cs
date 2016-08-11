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
using System.Collections.ObjectModel;
using Pardis_Monetary_Fund.DataModels;
using System.Text.RegularExpressions;
using NLog;
using Pardis_Monetary_Fund.Utilities;
using System.ComponentModel;

namespace Pardis_Monetary_Fund
{
    /// <summary>
    /// Interaction logic for AddFund.xaml
    /// </summary>
    public partial class AddFund : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        ObservableCollection<Member> listItems = new ObservableCollection<Member>();

        public ObservableCollection<Member> SelectedMembers
        {
            get { return listItems; }
            set { listItems = value; }
        }
        public AddFund()
        {
            InitializeComponent();
            lvSelectedMembers.ItemsSource = listItems;
        }

        private void btnAddMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectMemberDialog selectMemberDialog = new SelectMemberDialog();
                if (selectMemberDialog.ShowDialog() == true)
                    foreach (Pardis_Monetary_Fund.DataModels.Member m in selectMemberDialog.GetSelectedMembers())
                        if (!listItems.Contains(m))
                            listItems.Add(m);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
            }
        }

        private void btnDeleteMember_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Member m in lvSelectedMembers.SelectedItems.Cast<Member>().ToList<Member>())
                    listItems.Remove(m);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
            }
        }


        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


        private void txtInstallmentAmount_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtInstallmentAmount.Text))
                {
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("fa");
                    decimal valueBefore = decimal.Parse(txtInstallmentAmount.Text, System.Globalization.NumberStyles.AllowThousands);
                    txtInstallmentAmount.Text = String.Format(culture, "{0:N0}", valueBefore);
                    txtInstallmentAmount.Select(txtInstallmentAmount.Text.Length, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
            }
        }

        public decimal InstallmentAmount
        {
            get { return decimal.Parse(String.IsNullOrWhiteSpace(txtInstallmentAmount.Text) ? "0" : txtInstallmentAmount.Text, System.Globalization.NumberStyles.AllowThousands); }
            set
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("fa");
                txtInstallmentAmount.Text = String.Format(culture, "{0:N0}", value);
                txtInstallmentAmount.Select(txtInstallmentAmount.Text.Length, 0);
            }
        }

        private void txtInstallmentAmount_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void txtInstallmentAmount_Pasting_1(object sender, DataObjectPastingEventArgs e)
        {
            try
            {
                var text = e.DataObject.GetData(typeof(string)).ToString();
                if (!char.IsDigit(text, text.Length - 1))
                {
                    e.CancelCommand();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
            }
        }

        private GridViewColumnHeader lvSelectedMembersSortCol = null;
        private SortAdorner lvSelectedMembersSortAdorner = null;
        private void lvSelectedMembersColumnHeader_Click_1(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (lvSelectedMembersSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(lvSelectedMembersSortCol).Remove(lvSelectedMembersSortAdorner);
                lvSelectedMembers.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (lvSelectedMembersSortCol == column && lvSelectedMembersSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            lvSelectedMembersSortCol = column;
            lvSelectedMembersSortAdorner = new SortAdorner(lvSelectedMembersSortCol, newDir);
            AdornerLayer.GetAdornerLayer(lvSelectedMembersSortCol).Add(lvSelectedMembersSortAdorner);
            lvSelectedMembers.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

    }

}
