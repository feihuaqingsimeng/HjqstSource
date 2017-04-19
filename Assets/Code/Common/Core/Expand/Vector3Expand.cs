using System.Collections;
using System.Text;
using UnityEngine;

public static class Vector3Expand
{
    public static string ToCustomString(this Vector3 vec, char split = ',')
    {
        StringBuilder result = new StringBuilder();

        result.Append(vec.x.ToString());
        result.Append(split);
        result.Append(vec.y.ToString());
        result.Append(split);
        result.Append(vec.z.ToString());

        return result.ToString();
    }

    public static string ToCustomString(this Vector3 vec, char[] splits)
    {
        StringBuilder result = new StringBuilder();

        result.Append(vec.x.ToString());
        result.Append(splits);
        result.Append(vec.y.ToString());
        result.Append(splits);
        result.Append(vec.z.ToString());

        return result.ToString();
    }
}
