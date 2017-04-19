using Logic.UI;
using System.Collections.Generic;

public class EUISortingLayerComparer : IEqualityComparer<EUISortingLayer>
{
    public bool Equals(EUISortingLayer x, EUISortingLayer y)
    {
        return x == y;
    }

    public int GetHashCode(EUISortingLayer e)
    {
        return (int)e;
    }
}
