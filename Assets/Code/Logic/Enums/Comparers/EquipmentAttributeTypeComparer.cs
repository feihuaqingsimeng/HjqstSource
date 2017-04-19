using Logic.Enums;
using System.Collections.Generic;

public class EquipmentAttributeTypeComparer : IEqualityComparer<EquipmentAttributeType>
{
    public bool Equals(EquipmentAttributeType x, EquipmentAttributeType y)
    {
        return x == y;
    }

    public int GetHashCode(EquipmentAttributeType e)
    {
        return (int)e;
    }
}
