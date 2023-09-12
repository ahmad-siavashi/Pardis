using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using NLog;
using System.Windows;

namespace Pardis_Monetary_Fund.Utilities
{
    public class PersianDate
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static PersianCalendar pc = new PersianCalendar();

        public static String Now
        {
            get
            {
                return PersianDate.GregorianDateToPersianDate(DateTime.Now, true);
            }
        }

        public static String ClockNow
        {
            get
            {
                var now = DateTime.Now;
                return pc.GetHour(now) + "-" + pc.GetMinute(now) + "-" + pc.GetSecond(now);
            }
        }

        private static Dictionary<String, String> DAYS = new Dictionary<String, String>()
    {
        {"Saturday", "شنبه"},
        {"Sunday", "یک شنبه"},
        {"Monday", "دو شنبه"},
        {"Tuesday", "سه شنبه"},
        {"Wednesday", "چهار شنبه"},
        {"Thursday", "پنج شنبه"},
        {"Friday", "جمعه"}
    };

        public static string GregorianDateToPersianDate(string date)
        {
            try
            {
                System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
                DateTime dt = DateTime.Parse(date);
                return string.Format("{2}/{1:D2}/{0:D2}", pc.GetDayOfMonth(dt), pc.GetMonth(dt), pc.GetYear(dt));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return "";
            }
        }

        public static string GregorianDateToPersianDate(DateTime dt, bool showDayOfWeek = false)
        {
            try
            {
                System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
                if (!showDayOfWeek)
                {
                    return string.Format("{2}/{1:D2}/{0:D2}", pc.GetDayOfMonth(dt), pc.GetMonth(dt), pc.GetYear(dt));
                }
                else
                {
                    return string.Format("{0}, {3}/{2:D2}/{1:D2}", DAYS[pc.GetDayOfWeek(dt).ToString()], pc.GetDayOfMonth(dt), pc.GetMonth(dt), pc.GetYear(dt));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                logger.Error(ex.InnerException, "Message: {0}, StackTrace: {1}, TargetSite: {2}, Data: {3}, Source: {4}", ex.Message, ex.StackTrace, ex.TargetSite, ex.Data, ex.Source);
                Debug.DebugBreak();
                return "";
            }
        }

        /// <exception cref="Exception">When input string is not in the correct form.</exception>
        public static DateTime PersianDateToGregorianDate(string date)
        {
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            string[] s = date.Split('/');
            return pc.ToDateTime(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]), 0, 0, 0, 0);
        }

        private PersianDate() { }

    }
}
