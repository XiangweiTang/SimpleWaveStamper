using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWaveStamper
{
    static class FormOps
    {
        public static string ShrinkPath(string s, int maxLength = 50)
        {
            if (s.Length <= maxLength)
                return s;
            string prefix = s.Substring(0, 12);
            int suffixLength = maxLength - 3 - prefix.Length;
            string suffix = s.Substring(s.Length - suffixLength);
            return $"{prefix}...{suffix}";
        }
    }
}
