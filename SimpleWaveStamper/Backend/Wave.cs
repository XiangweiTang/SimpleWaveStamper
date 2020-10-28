using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SimpleWaveStamper
{
    class Wave
    {
        public Wave() { }
        byte[] FormatChunk = null;
        public int DataLength { get; private set; } = -1;
        public int SampleRate { get; private set; } = -1;
        public int ByteRate { get; private set; } = -1;

        public void Load(string wavPath)
        {
            using (FileStream fs = new FileStream(wavPath, FileMode.Open, FileAccess.Read))
            {
                Sanity.Requires(fs.Length >= 44, $"Invalid wave file, length is {fs.Length}.");
                Sanity.Requires(GetString(fs, 4) == "RIFF", "Expected RIFF header.");
                int riffLength = GetInt(fs);
                Sanity.Requires(riffLength + 8 == fs.Length, "Broken RIFF chunk.");

                Sanity.Requires(GetString(fs, 4) == "WAVE", "Expected WAVE header.");
                while (ReadChunk(fs)) ;
                PostCheck();
            }
        }

        private string GetString(FileStream fs, int count)
        {
            byte[] bytes = new byte[count];
            fs.Read(bytes, 0, count);
            return Encoding.ASCII.GetString(bytes);
        }

        private int GetInt(FileStream fs)
        {
            byte[] bytes = new byte[4];
            fs.Read(bytes, 0, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        private bool ReadChunk(FileStream fs)
        {
            if (fs.Position == fs.Length)
                return false;
            Sanity.Requires(fs.Position + 8 <= fs.Length, "Broken header");
            string header = GetString(fs, 4);
            int chunkSize = GetInt(fs);
            Sanity.Requires(fs.Position + chunkSize <= fs.Length, "Broken chunk.");
            switch (header)
            {
                case "fmt ":
                    FormatChunk = new byte[chunkSize];
                    fs.Read(FormatChunk, 0, chunkSize);
                    break;
                case "data":
                    DataLength = chunkSize;
                    fs.Seek(chunkSize, SeekOrigin.Current);
                    break;
                default:
                    fs.Seek(chunkSize, SeekOrigin.Current);
                    break;
            }
            return true;
        }

        private void PostCheck()
        {
            Sanity.Requires(FormatChunk != null, "Missing format chunk.");
            Sanity.Requires(FormatChunk.Length >= 16, "Broken format chunk.");
            Sanity.Requires(DataLength >= 0, "Missing data chunk.");
            SampleRate = BitConverter.ToInt32(FormatChunk, 4);
            ByteRate = BitConverter.ToInt32(FormatChunk, 8);
        }
    }
}
