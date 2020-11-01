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
        public string SavingPath => "Saving.bin";
        public string AudioPath { get; set; } = "";
        public int AudioLength { get; set; } = 0;
        public List<int> TimeStampPointList { get; set; } = new List<int>();
        public void Save()
        {
            File.WriteAllBytes(SavingPath, ValueToBytes());
        }

        public void Load()
        {
            BytesToValue();
        }
        public void Clear()
        {
            if (File.Exists(SavingPath))
                File.Delete(SavingPath);
        }
        private byte[] ValueToBytes()
        {
            var pathBytes = Encoding.UTF8.GetBytes(AudioPath);
            int length = 8 + pathBytes.Length + TimeStampPointList.Count * 4;
            byte[] bytes = new byte[length];
            Buffer.BlockCopy(BitConverter.GetBytes(pathBytes.Length), 0, bytes, 0, 4);
            Buffer.BlockCopy(pathBytes, 0, bytes, 4, pathBytes.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(AudioLength), 0, bytes, 4 + pathBytes.Length, 4);
            for (int i = 0; i < TimeStampPointList.Count; i++)
                Buffer.BlockCopy(BitConverter.GetBytes(TimeStampPointList[i]), 0, bytes, 4 * i + pathBytes.Length + 8, 4);
            return bytes;
        }
        private void BytesToValue()
        {
            var bytes = File.ReadAllBytes(SavingPath);
            int pathLength = BitConverter.ToInt32(bytes, 0);
            AudioPath = Encoding.UTF8.GetString(bytes, 4, pathLength);
            AudioLength = BitConverter.ToInt32(bytes, pathLength + 4);
            TimeStampPointList = new List<int>();
            for (int i = 0; i * 4 + pathLength + 8 < bytes.Length; i++)
                TimeStampPointList.Add(BitConverter.ToInt32(bytes, i * 4 + pathLength + 8));
        }
        public void ConverToTimeStampText(string hmsText, string sText)
        {
            int pre = 0;
            using (StreamWriter hmsSw = new StreamWriter(hmsText))
            using (StreamWriter sSw = new StreamWriter(sText))
            {
                for (int i = 0; i < TimeStampPointList.Count; i++)
                {
                    int current = TimeStampPointList[i];
                    hmsSw.WriteLine($"{Common.MilisecondsToString(pre)}\t{Common.MilisecondsToString(current)}");
                    sSw.WriteLine($"{pre/1000.0}\t{current/1000.0}");
                    pre = current;
                }
                sSw.WriteLine($"{pre/1000.0}\t{AudioLength/1000.0}");
                hmsSw.WriteLine($"{Common.MilisecondsToString(pre)}\t{Common.MilisecondsToString(AudioLength)}");
            }
        }
    }
}
