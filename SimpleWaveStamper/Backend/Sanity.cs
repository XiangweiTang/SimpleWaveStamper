using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWaveStamper
{
    static class Sanity
    {
        public static void Requires(bool valid, string message)
        {
            if (!valid)
                throw new Exception(message);
        }

        public static void Requires(bool valid)
        {
            if (!valid)
                throw new Exception();
        }
    }
}
