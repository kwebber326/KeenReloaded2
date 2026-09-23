using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Utilities
{
    public static class StringExtensions
    {
        public static string ToTxtExtension(this string str)
        {
            if (str == null) return null;

            if (!str.EndsWith(".txt"))
                str += ".txt";

            return str;
        }
    }
}
