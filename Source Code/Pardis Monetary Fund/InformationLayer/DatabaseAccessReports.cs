using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Pardis_Monetary_Fund.DataModels;
using Pardis_Monetary_Fund.Utilities;
using System.Windows;

namespace Pardis_Monetary_Fund.InformationLayer
{
    public partial class DatabaseAccess
    {
        public static DataTable GetFundPayments(int fundId, string registrationId, string fromDate, string toDate)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("شماره صندوق", typeof(int));
                dt.Columns.Add("شناسه عضویت", typeof(string));
                dt.Columns.Add("نام", typeof(string));
                dt.Columns.Add("نام خانوادگی", typeof(string));
                dt.Columns.Add("مبلغ", typeof(decimal));
                dt.Columns.Add("تاریخ پرداخت", typeof(string));
                var fundNumber = GetFund(fundId).Number;
                DateTime dtFrom;
                DateTime dtTo;
                try
                {
                    dtFrom = PersianDate.PersianDateToGregorianDate(fromDate);
                }
                catch (Exception ex)
                {
                    dtFrom = DateTime.Parse("1900/01/01");
                }
                try
                {
                    dtTo = PersianDate.PersianDateToGregorianDate(fromDate);
                }
                catch (Exception ex)
                {
                    dtTo = DateTime.Now;
                }
                if (String.IsNullOrWhiteSpace(registrationId))
                {
                    foreach (var loan in GetLoans(fundId))
                    {
                        foreach (var payment in GetPayments(loan.MemberId, loan.LoanId))
                        {
                            if (PersianDate.PersianDateToGregorianDate(payment.Date) >= dtFrom && PersianDate.PersianDateToGregorianDate(payment.Date) <= dtTo)
                            {
                                var row = dt.NewRow();
                                row.BeginEdit();
                                var member = Get<Member>("Members", "memberId", loan.MemberId);
                                row["شناسه عضویت"] = member.RegistrationId;
                                row["نام"] = member.FirstName;
                                row["نام خانوادگی"] = member.LastName;
                                row["شماره صندوق"] = fundNumber;
                                row["مبلغ"] = payment.Amount;
                                row["تاریخ پرداخت"] = payment.Date;
                                row.EndEdit();
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
                else
                {
                    var member = Get<Member>("Members", "registrationId", registrationId);
                    if (member != null)
                    {
                        var loan = GetLoan(fundId, member.MemberId);
                        if (loan != null)
                        {
                            {
                                foreach (var payment in GetPayments(loan.MemberId, loan.LoanId))
                                {
                                    if (PersianDate.PersianDateToGregorianDate(payment.Date) >= dtFrom && PersianDate.PersianDateToGregorianDate(payment.Date) <= dtTo)
                                    {
                                        var row = dt.NewRow();
                                        row.BeginEdit();
                                        row["شناسه عضویت"] = member.RegistrationId;
                                        row["نام"] = member.FirstName;
                                        row["نام خانوادگی"] = member.LastName;
                                        row["شماره صندوق"] = fundNumber;
                                        row["مبلغ"] = payment.Amount;
                                        row["تاریخ پرداخت"] = payment.Date;
                                        row.EndEdit();
                                        dt.Rows.Add(row);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (DataColumn col in dt.Columns)
                    col.ReadOnly = true;
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return null;
            }
        }

        public static DataTable GetMemberPayments(int fundId, string registrationId, string fromDate, string toDate)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("شماره صندوق", typeof(int));
                dt.Columns.Add("شناسه عضویت", typeof(string));
                dt.Columns.Add("نام", typeof(string));
                dt.Columns.Add("نام خانوادگی", typeof(string));
                dt.Columns.Add("مبلغ", typeof(decimal));
                var fundNumber = GetFund(fundId).Number;
                DateTime dtFrom;
                DateTime dtTo;
                try
                {
                    dtFrom = PersianDate.PersianDateToGregorianDate(fromDate);
                }
                catch (Exception ex)
                {
                    dtFrom = DateTime.Parse("1900/01/01");
                }
                try
                {
                    dtTo = PersianDate.PersianDateToGregorianDate(fromDate);
                }
                catch (Exception ex)
                {
                    dtTo = DateTime.Now;
                }
                if (String.IsNullOrWhiteSpace(registrationId))
                {
                    foreach (var loan in GetLoans(fundId))
                    {
                        decimal sum = 0;
                        foreach (var payment in GetPayments(loan.MemberId, loan.LoanId))
                        {
                            if (PersianDate.PersianDateToGregorianDate(payment.Date) >= dtFrom && PersianDate.PersianDateToGregorianDate(payment.Date) <= dtTo)
                            {
                                sum += payment.Amount;
                            }
                        }
                        var row = dt.NewRow();
                        row.BeginEdit();
                        var member = Get<Member>("Members", "memberId", loan.MemberId);
                        row["شناسه عضویت"] = member.RegistrationId;
                        row["نام"] = member.FirstName;
                        row["نام خانوادگی"] = member.LastName;
                        row["شماره صندوق"] = fundNumber;
                        row["مبلغ"] = sum;
                        row.EndEdit();
                        dt.Rows.Add(row);
                    }
                }
                else
                {
                    var member = Get<Member>("Members", "registrationId", registrationId);
                    if (member != null)
                    {
                        var loan = GetLoan(fundId, member.MemberId);
                        if (loan != null)
                        {

                            decimal sum = 0;
                            foreach (var payment in GetPayments(loan.MemberId, loan.LoanId))
                            {
                                if (PersianDate.PersianDateToGregorianDate(payment.Date) >= dtFrom && PersianDate.PersianDateToGregorianDate(payment.Date) <= dtTo)
                                {
                                    sum += payment.Amount;
                                }
                            }
                            var row = dt.NewRow();
                            row.BeginEdit();
                            row["شناسه عضویت"] = member.RegistrationId;
                            row["نام"] = member.FirstName;
                            row["نام خانوادگی"] = member.LastName;
                            row["شماره صندوق"] = fundNumber;
                            row["مبلغ"] = sum;
                            row.EndEdit();
                            dt.Rows.Add(row);

                        }
                    }
                }
                foreach (DataColumn col in dt.Columns)
                    col.ReadOnly = true;
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return null;
            }
        }
    }
}
