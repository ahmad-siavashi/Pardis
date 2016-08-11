using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlServerCe;
using System.Windows;
using Pardis_Monetary_Fund.DataModels;
using Pardis_Monetary_Fund.Utilities;

namespace Pardis_Monetary_Fund.InformationLayer
{
    public enum MemberDeleteState { OK, HasFund };
    public static partial class DatabaseAccess
    {
        // Needs Attention, Error Pron.
        public static bool SubmitMembers(DataTable dataTable)
        {
            try
            {
                var duplicate =
                    from row in dataTable.AsEnumerable()
                    where !String.IsNullOrEmpty(row["registrationId"].ToString())
                    group row by row["registrationId"] into d
                    where d.Count() > 1
                    select d;

                if (duplicate.Any())
                {
                    System.Windows.MessageBox.Show("شناسه عضویت نباید قبلا انتخاب شده باشد\nتغییرات شما ذخیره نشده اند\nلطفا اطلاعات را برای دخیره سازی اصلاح کنید", "خطا", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Intentionally left blank!
            }
            SqlCeConnection conn = new SqlCeConnection(DatabaseConnectionString);

            try
            {
                SqlCeDataAdapter dataAdapter = new SqlCeDataAdapter("Select * From Members", conn);
                SqlCeCommandBuilder commandBuilder = new SqlCeCommandBuilder(dataAdapter);
                dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
                dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();
                dataAdapter.Update(dataTable);
                dataTable.AcceptChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return false;
            }
        }

        // Revised.
        public static DataTable RetrieveMembers()
        {
            return ExecuteQuery("Select * From Members");
        }


        // Revised.
        public static List<Member> GetMembers()
        {
            List<Member> members = new List<Member>();
            SqlCeConnection conn = null;
            try
            {
                SqlCeDataReader reader = ExecuteReader("Select memberId from Members", ref conn);
                while (reader.Read())
                {
                    Member member = Get<Member>("Members", "memberId", (int)reader["memberId"]);
                    members.Add(member);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
            }
            finally
            {
                conn.Close();
            }
            return members;
        }

        public static MemberDeleteState CanDeleteMember(int memberId)
        {
            if ((int)DatabaseAccess.ExecuteScalar("Select Count(*) from Members_Funds where memberId = " + memberId) != 0)
            {
                return MemberDeleteState.HasFund;
            }
            return MemberDeleteState.OK;
        }

        // Revised.
        public static Decimal GetMemberCredit(int memberId, int loanId)
        {
            var credit = ExecuteScalar("Select Sum(amount) from Payments Where memberId = " + memberId.ToString() + " and loanId = " + loanId.ToString());
            if (credit is DBNull)
                return 0M;
            else
                return Convert.ToDecimal(credit);
        }
    }
}
