using Logic.Enums;
using System.Collections.Generic;

public class BaseResTypeComparer : IEqualityComparer<BaseResType>
{
    public bool Equals(BaseResType x, BaseResType y)
    {
        return x == y;
    }

    public int GetHashCode(BaseResType b)
    {
        return (int)b;
    }
}
