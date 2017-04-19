using System.IO;
using System.Net;

namespace Logic.Protocol
{
    /// <summary>
    /// 方便把本地字节序写为网络字节序
    /// 增加写string时写入一个uint16长度
    /// </summary>
    public class MBinaryWriter : BinaryWriter
    {
        public MBinaryWriter()
        {
            OutStream = new MemoryStream();
        }

        public void Clear()
        {
            OutStream.SetLength(0);
        }

        public byte[] ToArray()
        {
            return ((MemoryStream)OutStream).ToArray();
        }

        //public override void Write(short value)
        //{
        //    base.Write(IPAddress.HostToNetworkOrder(value));
        //}

        //public override void Write(ushort value)
        //{
        //    Write((short)value);
        //}

        //public override void Write(int value)
        //{
        //    base.Write(IPAddress.HostToNetworkOrder(value));
        //}

        //public override void Write(uint value)
        //{
        //    Write((int)value);
        //}

        //public override void Write(long value)
        //{
        //    Write((ulong)value);
        //}

        //public override void Write(ulong value)
        //{
        //    uint height = (uint)((value & 0XFFFFFFFF00000000) >> 32);
        //    uint low = (uint)(value & (ulong)0X00000000FFFFFFFF);
        //    Write(low);
        //    Write(height);
        //}

        ////服务器没有字节顺序转换
        ////public override void Write(float value)
        ////{
        ////    byte[] bytes = System.BitConverter.GetBytes(value);
        ////    byte b1, b2, b3, b4;
        ////    b1 = bytes[2];
        ////    b2 = bytes[3];
        ////    b3 = bytes[0];
        ////    b4 = bytes[1];

        ////    //b1 = bytes[3];
        ////    //b2 = bytes[2];
        ////    //b3 = bytes[1];
        ////    //b4 = bytes[0];

        ////    Write(b1);
        ////    Write(b2);
        ////    Write(b3);
        ////    Write(b4);
        ////}

        //public override void Write(string value)
        //{
        //    var data = System.Text.Encoding.UTF8.GetBytes(value);
        //    var len = data.Length;

        //    Write((ushort)len);
        //    base.Write(data);
        //}

        //public void WriteLongString(string value)
        //{
        //    var data = System.Text.Encoding.UTF8.GetBytes(value);
        //    var len = data.Length;

        //    Write((uint)len);
        //    base.Write(data);
        //}

        public void WriteStringC(string value)
        {
            var data = System.Text.Encoding.UTF8.GetBytes(value);
            base.Write(data);
            base.Write(char.MinValue);
        }

        //public void WriteSizedBytes(byte[] data)
        //{
        //    Write((uint)data.Length);
        //    base.Write(data);
        //}

        public override void Close()
        {
            base.Close();
            OutStream.Close();
            Dispose(true);
            OutStream.Dispose();
        }
    }
}
