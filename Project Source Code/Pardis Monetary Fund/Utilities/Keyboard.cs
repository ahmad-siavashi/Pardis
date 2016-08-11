using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Pardis_Monetary_Fund.Utilities
{
    public static class Keyboard
    {
        public static void ToPersianLayout(DependencyObject target)
        {
            InputLanguageManager.SetInputLanguage(target, System.Globalization.CultureInfo.CreateSpecificCulture("fa"));
        }
    }
}
