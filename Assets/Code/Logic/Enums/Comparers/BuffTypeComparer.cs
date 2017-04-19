using Logic.Enums;
using System.Collections.Generic;
public class BuffTypeComparer : IEqualityComparer<BuffType>
{
    public bool Equals(BuffType x, BuffType y)
    {
        return x == y;
    }

    public int GetHashCode(BuffType b)
    {
        return (int)b;
    }
}
