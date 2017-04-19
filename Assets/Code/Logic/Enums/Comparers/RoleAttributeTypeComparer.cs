using Logic.Enums;
using System.Collections.Generic;

public class RoleAttributeTypeComparer : IEqualityComparer<RoleAttributeType>
{
    public bool Equals(RoleAttributeType x, RoleAttributeType y)
    {
        return x == y;
    }

    public int GetHashCode(RoleAttributeType r)
    {
        return (int)r;
    }
}
