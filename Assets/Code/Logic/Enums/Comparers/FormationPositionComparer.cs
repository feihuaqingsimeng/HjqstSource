using Logic.Enums;
using System.Collections.Generic;

public class FormationPositionComparer : IEqualityComparer<FormationPosition>
{
    public bool Equals(FormationPosition x, FormationPosition y)
    {
        return x == y;
    }

    public int GetHashCode(FormationPosition f)
    {
        return (int)f;
    }
}
