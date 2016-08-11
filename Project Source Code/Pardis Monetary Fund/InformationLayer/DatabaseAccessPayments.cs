using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pardis_Monetary_Fund.DataModels;
using System.Data.SqlServerCe;
using System.Windows;
using Pardis_Monetary_Fund.Utilities;

namespace Pardis_Monetary_Fund.InformationLayer
{
    public static partial class DatabaseAccess
    {


        // Revised.
        public static List<Payment> GetPayments(int memberId, int loanId)
        {
            List<Payment> payments = new List<Payment>();
            SqlCeConnection conn = null;
            try
            {
                SqlCeDataReader reader = ExecuteReader("Select paymentId from Payments where memberId = " + memberId + " and loanId = " + loanId, ref conn);
                while (reader.Read())
                {
                    Payment payment = Get<Payment>("Payments", "paymentId", (int)reader["paymentId"]);
                    payment.Number = Convert.ToInt32(ExecuteScalar("Select Count(*) From Payments where memberId = " + memberId + " and loanId = " + loanId + " and paymentId <= " + payment.PaymentId));
                    payments.Add(payment);
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
            return payments;
        }
    }
}
