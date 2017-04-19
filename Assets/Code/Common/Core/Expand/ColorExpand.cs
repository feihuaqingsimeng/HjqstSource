using System.Collections;
using System.Text;
using UnityEngine;

public static class ColorExpand
{
    public static string ToCustomString(this Color color, char split = ',')
    {
        StringBuilder result = new StringBuilder();

        result.Append(((int)(color.r*255)).ToString());
        result.Append(split);
        result.Append(((int)(color.g * 255)).ToString());
        result.Append(split);
        result.Append(((int)(color.b * 255)).ToString());
        result.Append(split);
        result.Append(((int)(color.a * 255)).ToString());

        return result.ToString();
    }

    public static string ToCustomString(this Color color, char[] splits)
    {
        StringBuilder result = new StringBuilder();

        result.Append(((int)(color.r * 255)).ToString());
        result.Append(splits);
        result.Append(((int)(color.g * 255)).ToString());
        result.Append(splits);
        result.Append(((int)(color.b * 255)).ToString());
        result.Append(splits);
        result.Append(((int)(color.a * 255)).ToString());

        return result.ToString();
    }
}
