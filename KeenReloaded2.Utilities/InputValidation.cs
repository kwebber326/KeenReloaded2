using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Utilities
{
    public static class InputValidation
    {
        public static string SanitizeStringForIntegerNumerics(string text)
        {
            var numeric = "1234567890";
            string result = string.Empty;
            foreach (char c in text)
            {
                if (numeric.Contains(c))
                {
                    result += c;
                }
            }
            return result;
        }
        public static string SanitizeFileNameInput(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return fileName;

            string result = fileName.Replace(@"/", "").Replace(@"\", "").Replace(".", "");
            return result;
        }
    }
}
