using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjReader.Common {

    public static class StringExtensions {

        public static float ParseInvariantFloat(this string floatString) {
            return float.Parse(floatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        public static int ParseInvariantInt(this string intString) {
            return int.Parse(intString, CultureInfo.InvariantCulture.NumberFormat);
        }

        public static bool EqualsOrdinalIgnoreCase(this string str, string s) {
            return str.Equals(s, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsNullOrEmpty(this string str) {
            return string.IsNullOrEmpty(str);
        }
    }
}
