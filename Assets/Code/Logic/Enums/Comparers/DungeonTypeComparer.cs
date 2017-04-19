using Logic.Enums;
using System.Collections.Generic;

public class DungeonTypeComparer : IEqualityComparer<DungeonType>
{
    public bool Equals(DungeonType x, DungeonType y)
    {
        return x == y;
    }

    public int GetHashCode(DungeonType d)
    {
        return (int)d;
    }
}
