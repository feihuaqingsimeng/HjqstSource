using Logic.Enums;
using System.Collections.Generic;

public class FormationTeamTypeComparer : IEqualityComparer<FormationTeamType>
{

    public bool Equals(FormationTeamType x, FormationTeamType y)
    {
        return x == y;
    }

    public int GetHashCode(FormationTeamType f)
    {
        return (int)f;
    }
}
