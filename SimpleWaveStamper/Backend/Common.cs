using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWaveStamper
{
    public static class Common
    {
        public static string MilisecondsToString(int miliseconds)
        {
            int mSeconds = miliseconds % 1000;
            int r = miliseconds / 1000;
            int seconds = r % 60;
            r = r / 60;
            int minutes = r % 60;
            r = r / 60;
            int hour = r;
            return $"{hour:00}:{minutes:00}:{seconds:00}.{mSeconds:000}";
        }
    }
}
