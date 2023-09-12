using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pardis_Monetary_Fund.Utilities;
using System.Windows.Data;
using NLog;
using Pardis_Monetary_Fund.DataModels;
using System.Windows;

namespace Pardis_Monetary_Fund
{
    public class PersianDateUIConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            if (value is System.DBNull)
            {
                DateTime dt = DateTime.Now;
                String date = string.Format("{2}{1:D2}{0:D2}", pc.GetDayOfMonth(dt), pc.GetMonth(dt), pc.GetYear(dt));
                return date;
            }
            else
            {
                DateTime dt = DateTime.Parse((string)value);
                String date = string.Format("{2}{1:D2}{0:D2}", pc.GetDayOfMonth(dt), pc.GetMonth(dt), pc.GetYear(dt));
                return date;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            try
            {
                string[] date = (value as string).Split('/');
                return pc.ToDateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]), 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                DateTime dt = DateTime.Now;
                return pc.ToDateTime(pc.GetYear(dt), pc.GetMonth(dt), pc.GetDayOfMonth(dt), 0, 0, 0, 0); ;
            }
        }
    }

    public class TurnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int val = (int)value;
            if (val == -1)
            {
                return "ندارد";
            }
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value.Equals("ندارد"))
                {
                    return -1;
                }
                int val = int.Parse((string)value);
                if (val > 0) return val;
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }

    public class TurnConverterEditMode : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int val = (int)value;
            if (val == -1)
            {
                return "";
            }
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                int val = int.Parse((string)value);
                if (val > 0) return val;
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }

    public class LoanStateConverter : IValueConverter
    {
        private static Logger logger1 = LogManager.GetCurrentClassLogger();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                switch ((LOAN_STATES)value)
                {
                    case LOAN_STATES.UN_SETTLED:
                        return "تسویه نشده";
                    case LOAN_STATES.SETTLED:
                        return "تسویه شده";
                    case LOAN_STATES.PENDING:
                        return "در انتظار";
                    default:
                        throw new Exception("Undefied Loan State Value.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger1.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string val = (string)value;
                switch (val)
                {
                    case "تسویه نشده":
                        return LOAN_STATES.UN_SETTLED;
                    case "تسویه شده":
                        return LOAN_STATES.SETTLED;
                    case "در انتظار":
                        return LOAN_STATES.PENDING;
                    default:
                        throw new Exception("Undefied Loan State Value.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger1.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return null;
            }
        }
    }
}
