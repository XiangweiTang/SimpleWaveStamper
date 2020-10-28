using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWaveStamper
{
    static class Mci
    {
        [DllImport("winmm.dll")]
        private static extern void mciSendString(string cmdString, string returnString, int cchReturn, int callBack);
        public static void Run(string cmdString)
        {
            mciSendString(cmdString, "", 0, 0);
        }
    }
}
