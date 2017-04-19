using Logic.Enums;
using System.Collections.Generic;

public class ConsumeTipTypeComparer : IEqualityComparer<ConsumeTipType>
{
    public bool Equals(ConsumeTipType x, ConsumeTipType y)
    {
        return x == y;
    }

    public int GetHashCode(ConsumeTipType c)
    {
        return (int)c;
    }
}
