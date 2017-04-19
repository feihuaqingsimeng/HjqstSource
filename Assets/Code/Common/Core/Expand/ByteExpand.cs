using System;
using System.Text;
public static class ByteExpand
{
    public static string ToByteString(this byte[] buf, string splitStr = ",")
    {
        StringBuilder build = new StringBuilder();
        for (int i = 0, count = buf.Length; i < count; i++)
        {
            build.Append(buf[i] + splitStr);
        }
        build.AppendLine();
        return build.ToString();
    }

    public static byte[] Uncompress(this byte[] buff, int offset = 0)
    {
        return Compression.Uncompress(buff, offset);
    }

    public static byte[] Compress(this byte[] src, int offset, int length)
    {
        return Compression.Compress(src, offset, length);
    }

    public static byte[] Compress(this byte[] src)
    {
        return Compression.Compress(src);
    }
}

