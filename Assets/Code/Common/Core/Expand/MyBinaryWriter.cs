using System;
using System.IO;
using System.Text;

public static class BinaryWriterEx
{
    [Obsolete("方法已过时请使用Write()")]
    public static void writeUnsignedInt(this BinaryWriter writer, uint v) { writer.Write(v); }
    [Obsolete("方法已过时请使用Write()")]
    public static void writeFloat(this BinaryWriter writer, float v) { writer.Write(v); }
    [Obsolete("方法已过时请使用Write()")]
    public static void writeUnsignedShort(this BinaryWriter writer, ushort v) { writer.Write(v); }
    [Obsolete("方法已过时请使用Write()")]
    public static void writeShort(this BinaryWriter writer, short v) { writer.Write(v); }
    [Obsolete("方法已过时请使用Write()")]
    public static void writeInt(this BinaryWriter writer, int v) { writer.Write(v); }
    [Obsolete("方法已过时请使用Write()")]
    public static void writeByte(this BinaryWriter writer, sbyte v) { writer.Write(v); }
    [Obsolete("方法已过时请使用Write()")]
    public static void writeUnsignedByte(this BinaryWriter writer, byte v) { writer.Write(v); }
    [Obsolete("方法已过时请使用Write()")]
    public static void writeBoolean(this BinaryWriter writer, bool v) { writer.Write(v); }
    [Obsolete("方法已过时请使用Write()")]
    public static void writeDouble(this BinaryWriter writer, double v) { writer.Write(v); }
    [Obsolete("方法已过时请使用Write()")]
    public static void writeMulong(this BinaryWriter writer, uint v) { writer.Write(v); }
    
    public static void writeString(this BinaryWriter writer, string str, int len)
    {
        if (str == null)
            str = string.Empty;
        byte[] buf = Encoding.UTF8.GetBytes(str);
        if (buf.Length != len)
        {
            byte[] nbuf = new byte[len];
            for (int i = 0; i < nbuf.Length && i < buf.Length; i++)
            {
                nbuf[i] = buf[i];
            }
            writer.Write(nbuf);

        }
        else
            writer.Write(buf);
    }
}
