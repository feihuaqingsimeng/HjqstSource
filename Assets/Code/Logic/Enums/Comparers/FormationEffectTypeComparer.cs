using Logic.Enums;
using System.Collections.Generic;

public class FormationEffectTypeComparer : IEqualityComparer<FormationEffectType>
{
    public bool Equals(FormationEffectType x, FormationEffectType y)
    {
        return x == y;
    }

    public int GetHashCode(FormationEffectType f)
    {
        return (int)f;
    }
}
