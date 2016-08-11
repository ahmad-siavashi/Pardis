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
using Pardis_Monetary_Fund.InformationLayer;
using Pardis_Monetary_Fund.DataModels;
using NLog;
using Pardis_Monetary_Fund.Utilities;
using System.ComponentModel;

namespace Pardis_Monetary_Fund
{
    /// <summary>
    /// Interaction logic for SelectMember.xaml
    /// </summary>
    public partial class SelectMemberDialog : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public SelectMemberDialog()
        {
            try
            {
                InitializeComponent();
                lvMembers.ItemsSource = DatabaseAccess.GetMembers();
                System.ComponentModel.ICollectionView view = CollectionViewSource.GetDefaultView(lvMembers.ItemsSource);
                view.Filter = filter;
                Utilities.Keyboard.ToPersianLayout(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
            }
        }

        public bool filter(object item)
        {
            if (String.IsNullOrEmpty(txtSearch.Text))
            {
                return true;
            }
            else
            {
                string wildcard = txtSearch.Text;
                Member member = item as Member;
                if (member.RegistrationId.Contains(wildcard) || member.FirstName.Contains(wildcard) || member.LastName.Contains(wildcard))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void txtSearch_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvMembers.ItemsSource).Refresh();
        }

        private void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public List<Member> GetSelectedMembers()
        {
            return lvMembers.SelectedItems.Cast<Member>().ToList<Member>();
        }

        private void lvMembers_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Member;
            if (item != null)
            {
                this.DialogResult = true;
            }
        }

        private GridViewColumnHeader lvMembersSortCol = null;
        private SortAdorner lvMembersSortAdorner = null;
        private void lvMembersColumnHeader_Click_1(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (lvMembersSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(lvMembersSortCol).Remove(lvMembersSortAdorner);
                lvMembers.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (lvMembersSortCol == column && lvMembersSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            lvMembersSortCol = column;
            lvMembersSortAdorner = new SortAdorner(lvMembersSortCol, newDir);
            AdornerLayer.GetAdornerLayer(lvMembersSortCol).Add(lvMembersSortAdorner);
            lvMembers.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

    }


}
