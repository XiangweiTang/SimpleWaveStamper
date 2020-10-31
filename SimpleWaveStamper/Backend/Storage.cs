using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SimpleWaveStamper
{
    class Storage
    {
        public Storage() { }
        public string AudioPath { get; private set; } = "";
        private void WriteStringToBytes(FileStream fs, string chunkName, string chunkValue)
        {
            byte[] header = Encoding.ASCII.GetBytes(chunkName);
            Sanity.Requires(header.Length == 4, "Invalid chunk name.");            
            byte[] bytes = Encoding.UTF8.GetBytes(chunkValue);
            byte[] length = BitConverter.GetBytes(bytes.Length);
            fs.Write(header, (int)fs.Position, 4);
            fs.Write(length, (int)fs.Position, 4);
            fs.Write(bytes, (int)fs.Position, bytes.Length);
        }
    }
}
