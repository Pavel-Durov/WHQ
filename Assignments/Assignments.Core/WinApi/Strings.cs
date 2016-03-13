using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Assignments.Core.WinApi
{
    public class Strings
    {
        [DllImport("user32.dll")]
        public static extern int wsprintf([Out] StringBuilder lpOut, string lpFmt, __arglist);

        [DllImport("string.h", SetLastError = true)]
        public static extern string _wcsdup([Out] StringBuilder lpOut);

    }
}
