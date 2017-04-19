using System.IO;
using System.Text;
public static class BinaryReaderEx
{
    public static string ReadString(this BinaryReader reader, int len)
    {
        return Encoding.UTF8.GetString(reader.ReadBytes(len)).Replace("\0","");
    }
}

