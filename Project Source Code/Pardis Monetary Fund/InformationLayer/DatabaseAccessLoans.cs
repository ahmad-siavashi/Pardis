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
    public enum LoanDeleteState { OK, HasPayment };

    public static partial class DatabaseAccess
    {

        // Revised.
        public static List<Loan> GetLoans(int fundId)
        {
            List<Loan> loans = new List<Loan>();
            SqlCeConnection conn = null;
            try
            {
                SqlCeDataReader reader = ExecuteReader("Select loanId from Loans where fundId = " + fundId, ref conn);
                while (reader.Read())
                {
                    Loan loan = Get<Loan>("Loans", "loanId", (int)reader["loanId"]);
                    loans.Add(loan);
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
            return loans;
        }

        public static Loan GetLoan(int fundId, int memberId)
        {
            SqlCeConnection conn = null;
            try
            {
                SqlCeDataReader reader = ExecuteReader("Select loanId from Loans where fundId = " + fundId + " and memberId = " + memberId, ref conn);
                if (reader.Read())
                {
                    Loan loan = Get<Loan>("Loans", "loanId", (int)reader["loanId"]);
                    return loan;
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
            return null;
        }

        // Revised.
        public static int InsertLoan(Loan loan)
        {
            SqlCeConnection conn = new SqlCeConnection(DatabaseConnectionString);
            try
            {
                return Insert("Loans", "loanId", loan, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return -1;
            }
        }






        public static LoanDeleteState CanDeleteLoan(int loanId)
        {
            if ((int)DatabaseAccess.ExecuteScalar("Select Count(*) from Payments where loanId = " + loanId.ToString()) != 0)
            {
                return LoanDeleteState.HasPayment;
            }
            return LoanDeleteState.OK;
        }

        public static bool DeleteLoan(int loanId)
        {
            bool result = true;
            result &= Delete("Payments", "loanId", loanId);
            result &= Delete("Loans", "loanId", loanId);
            return result;
        }



        public static int GetLoanId(int memberId, int fundId)
        {
            return (int)ExecuteScalar("Select loanId from Loans where memberId = " + memberId + " and fundId = " + fundId);
        }

    }
}
