using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;
using System.Collections.ObjectModel;
using Pardis_Monetary_Fund.DataModels;
using System.Windows;
using NLog;
using System.Data;
using Pardis_Monetary_Fund.Utilities;

namespace Pardis_Monetary_Fund.InformationLayer
{
    public enum FundDeleteState { OK, HasLoan };

    public static partial class DatabaseAccess
    {
        // Revised.
        public static ObservableCollectionWithItemNotify<Fund> GetFunds()
        {
            ObservableCollectionWithItemNotify<Fund> funds = new ObservableCollectionWithItemNotify<Fund>();
            SqlCeConnection conn = null;
            try
            {
                SqlCeDataReader reader = ExecuteReader("Select fundId from Funds", ref conn);
                while (reader.Read())
                {
                    Fund fund = GetFund((int)reader["fundId"]);
                    funds.Add(fund);
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
            return funds;
        }

        public static decimal GetFundInvestment(int fundId)
        {
            decimal Investment = 0;
            foreach (Loan loan in DatabaseAccess.GetLoans(fundId))
            {
                var sum = ExecuteScalar("Select Sum(amount) from Payments where loanId = " + loan.LoanId);
                Investment += (sum is DBNull) ? 0m : Convert.ToDecimal(sum);
            }
            return Investment;
        }

        // Revised.
        public static Fund GetFund(int fundId)
        {
            SqlCeConnection conn = null;
            try
            {
                SqlCeDataReader reader = ExecuteReader("Select * from Funds where fundId = " + fundId, ref conn);
                if (reader.Read())
                {
                    Fund fund = Get<Fund>("Funds", "fundId", (int)reader["fundId"]);
                    fund.Number = Convert.ToInt32(reader["number"]);
                    fund.Investment = GetFundInvestment(fundId);
                    fund.MembersCount = CountMembersOfFund(fund.FundId);
                    return fund;
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return null;
            }
            finally
            {
                conn.Close();
            }
        }
        /**/

        // Revised.
        public static int CountMembersOfFund(int fundId)
        {
            try
            {
                return (int)ExecuteScalar("Select Count(*) from Members_Funds Where fundId = " + fundId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return 0;
            }
        }

        // Revised.
        public static bool AddFundMembers(decimal fundId, List<Member> members)
        {
            try
            {
                foreach (Member member in members)
                {
                    ExecuteNonQuery("Insert Into Members_Funds(memberId, fundId) Values(" + member.MemberId + ", " + fundId + ")");
                }
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
        public static int InsertFund(Fund fund, List<Member> members)
        {
            SqlCeConnection conn = new SqlCeConnection(DatabaseConnectionString);
            try
            {
                var query = ExecuteScalar("Select Max(number) From Funds");
                fund.Number = Convert.ToInt32(query is DBNull?0:query) + 1;
                var fundId = Convert.ToInt32(Insert("Funds", "fundId", fund, new List<String>(){"Investment", "LoanAmount", "MembersCount"}));
                DatabaseAccess.AddFundMembers(fundId, members);
                return Convert.ToInt32(fundId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return -1;
            }
        }

        public static FundDeleteState CanDeleteFund(int fundId)
        {
            var SettledFunds = (int)DatabaseAccess.ExecuteScalar("Select Count(*) from Loans where state = " + (int)LOAN_STATES.SETTLED + "AND fundId = " + fundId.ToString());
            var PendingFunds = (int)DatabaseAccess.ExecuteScalar("Select Count(*) from Loans where state = " + (int)LOAN_STATES.PENDING + "AND fundId = " + fundId.ToString());
            var UnSelltedFunds = (int)DatabaseAccess.ExecuteScalar("Select Count(*) from Loans where state = " + (int)LOAN_STATES.UN_SETTLED + "AND fundId = " + fundId.ToString());

            if ((((SettledFunds != 0 && PendingFunds == 0) || (SettledFunds == 0 && PendingFunds != 0)) && UnSelltedFunds == 0) || (UnSelltedFunds == 0 && SettledFunds == 0 && PendingFunds == 0))
            {
                return FundDeleteState.OK;
            }
            return FundDeleteState.HasLoan;
        }

        public static bool DeleteFund(int fundId)
        {
            // TODO: Make it a transaction.
            bool result = true;
            List<Loan> loans = GetLoans(fundId);
            foreach (Loan loan in loans)
            {
                result &= DeleteLoan(loan.LoanId);
            }
            result &= Delete("Funds", "fundId", fundId);
            result &= Delete("Members_Funds", "fundId", fundId);
            return result;
        }

        public static bool InitializeLoanRecordForFundMembers(Fund fund)
        {
            try
            {
                List<Member> members = DatabaseAccess.GetFundMembers(fund.FundId);
                foreach (Member member in members)
                {
                    Insert("Loans", "loanId", new Loan() { FundId = fund.FundId, MemberId = member.MemberId, IssueDate = PersianDate.Now, Turn = -1, State = LOAN_STATES.PENDING, Credit = 0M, Indebted = 0M }, null);
                }
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

        public static List<Member> GetFundMembers(int fundId)
        {
            List<Member> members = new List<Member>();
            SqlCeConnection conn = new SqlCeConnection(DatabaseConnectionString);
            SqlCeCommand cmd = new SqlCeCommand("Select * from Members where memberId = @Id", conn);
            try
            {
                DataTable dt = Retrieve("Members_Funds", "fundId", fundId);
                conn.Open();
                foreach (DataRow row in dt.Rows)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Id", row["memberId"]);
                    SqlCeDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        Member member = Get<Member>("Members", "memberId", (int)reader["memberId"]);
                        members.Add(member);
                    }
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
    }
}
