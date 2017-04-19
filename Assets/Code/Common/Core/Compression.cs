using ComponentAce.Compression.Libs.zlib;
using System.IO;
public class Compression
{
    public const int BUFFER_SIZE = 1024 * 1024;
    private static byte[] _buffer=new byte[BUFFER_SIZE];
    public static byte[] Uncompress(byte[] buff, int offset = 0)
    {
        //using (MemoryStream md = new MemoryStream())
        //{
        //    Stream d = new ZOutputStream(md);
        //    d.Write(buff, offset, buff.Length - offset);
        //    d.Close();
        //    return md.ToArray();
        //}
        ZStream zs = new ZStream();
        zs.next_in = buff;
        zs.avail_in = buff.Length;
        zs.next_out = _buffer;
        zs.avail_out = _buffer.Length;
        if (zlibConst.Z_OK != zs.inflateInit())
            return null;
        if (zlibConst.Z_STREAM_END != zs.inflate(zlibConst.Z_FINISH))
            return null;
        zs.inflateEnd();
        byte[] result =new byte[zs.total_out];
        System.Array.Copy(_buffer,result,zs.total_out);
        return result;
    }

    public static byte[] Compress(byte[] buff, int offset, int length)
    {
        //using (MemoryStream ms = new MemoryStream())
        //{
        //    Stream s = new ZOutputStream(ms, level);
        //    s.Write(src, offset, length);
        //    s.Close();
        //    return ms.ToArray();
        //}
        return null;
    }

    public static byte[] Compress(byte[] buff)
    {
        //using (MemoryStream ms = new MemoryStream())
        //{
        //    Stream s = new ZOutputStream(ms, level);
        //    s.Write(src, 0, (int)ms.Length);
        //    s.Close();
        //    return ms.ToArray();
        //}
        ZStream zs = new ZStream();
        zs.next_in = buff;
        zs.avail_in = buff.Length;
        zs.next_out = _buffer;
        zs.avail_out = _buffer.Length;
        if (zlibConst.Z_OK != zs.deflateInit(zlibConst.Z_BEST_COMPRESSION))
            return null;
        if (zlibConst.Z_STREAM_END != zs.deflate(zlibConst.Z_FINISH))
            return null;
        zs.deflateEnd();
        byte[] result=new byte[zs.total_out];
        System.Array.Copy(_buffer,result,zs.total_out);
        return result;
    }
}

