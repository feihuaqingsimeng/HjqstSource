using System.IO;
using System.Net;
using System.Text;

namespace Logic.Protocol
{
    /// <summary>
    /// 方便把网络字节序读为本地直接序
    /// 对string的（uint16长度的处理）
    /// </summary>
    public class MBinaryReader : BinaryReader
    {
        public MBinaryReader(byte[] bytes):base(new MemoryStream(bytes))
        {
        }

        public MBinaryReader(MemoryStream ms) : base(ms)
        {

        }

        //public override short ReadInt16()
        //{
        //    return IPAddress.NetworkToHostOrder(base.ReadInt16());
        //}

        //public override ushort ReadUInt16()
        //{
        //    return (ushort)(ReadInt16());
        //}

        //public override int ReadInt32()
        //{
        //    return IPAddress.NetworkToHostOrder(base.ReadInt32());
        //}

        //public override uint ReadUInt32()
        //{
        //    return (uint)(ReadInt32());
        //}

        //public override long ReadInt64()
        //{
        //    long height = (long)ReadInt32() & 0xFFFFFFFF;
        //    long low = (long)ReadInt32();
        //    low = low << 32;
        //    return height | low;
        //}

        //public override ulong ReadUInt64()
        //{ 
        //    return (ulong)ReadInt64();
        //}

        ////服务器没有字节顺序转换
        ////public override float ReadSingle()
        ////{
        ////    byte b1, b2, b3, b4;
        ////    b1 = base.ReadByte();
        ////    b2 = base.ReadByte();
        ////    b3 = base.ReadByte();
        ////    b4 = base.ReadByte();
        ////    byte[] bytes = { b3, b4, b1, b2 };
        ////    //byte[] bytes = { b4, b3, b2, b1 };
        ////    return System.BitConverter.ToSingle(bytes,0);
        ////}

        //public override string ReadString()
        //{
        //    ushort count = ReadUInt16();
        //    return Encoding.UTF8.GetString(base.ReadBytes((int)count));
        //}

        //public string ReadLongString()
        //{
        //    var count = ReadUInt32();
        //    return Encoding.UTF8.GetString(base.ReadBytes((int)count));
        //}

        public string ReadStringC()
        {
            long pos = BaseStream.Position;
            int count = 1;
            byte b = ReadByte();
            while (b != char.MinValue)
            {
                b = ReadByte();
                count++;
            }
            BaseStream.Position = pos;
            byte[] bytes = ReadBytes(count);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length - 1);
        }

        //public byte[] ReadSizedBytes()
        //{
        //    return base.ReadBytes((int)ReadUInt32());
        //}

        public override void Close()
        {
            base.Close();
            Dispose(true);
        }

    }
}
