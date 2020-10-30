using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWaveStamper
{
    class Storage
    {
        public Storage() { }
        List<byte[]> InternalSave = new List<byte[]>();
        public void AddString(string chunkName, string value)
        {
            StringChunk c = new StringChunk(chunkName) { ChunkContent = value };
            InternalSave.Add(c.Merge());
        }
    }

    abstract class Chunk
    {
        public string ChunkName { get; private set; }
        public object ChunkContent { get; set; }
        public Chunk(string chunkName)
        {
            Sanity.Requires(ChunkName.Length == 4, "Invalid chunk name size.");
            Sanity.Requires(ChunkName.All(x => x < 256), "Invalid chunk name.");
            ChunkName = chunkName;
        }
        protected abstract byte[] ConvertContentToBytes();
        public byte[] Merge()
        {
            var contentBytes = ConvertContentToBytes();
            int dataBytesLength = contentBytes.Length;
            byte[] fullChunk = new byte[dataBytesLength + 8];
            byte[] nameBytes = Encoding.ASCII.GetBytes(ChunkName);
            byte[] lengthBytes = BitConverter.GetBytes(dataBytesLength);
            Buffer.BlockCopy(nameBytes, 0, fullChunk, 0, 4);
            Buffer.BlockCopy(lengthBytes, 0, fullChunk, 4, 4);
            Buffer.BlockCopy(contentBytes, 0, fullChunk, 8, dataBytesLength);
            return fullChunk;
        }
    }
    class StringChunk : Chunk
    {
        public StringChunk(string chunkName) : base(chunkName) { }
        protected override byte[] ConvertContentToBytes()
        {
            return Encoding.UTF8.GetBytes((string)ChunkContent);
        }
    }
}
