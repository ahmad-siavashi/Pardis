using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pardis_Monetary_Fund.Utilities
{
    public static class Debug
    {
        [Conditional("DEBUG")]
        public static void DebugBreak()
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
    }
}
