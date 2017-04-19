using UnityEngine;
using System.Collections;
using UnityEditor;

public class BuildPath
{
    [MenuItem("CEngine/BuildPath &r")]
    public static void PrintPath()
    {
        GameObject obj = Selection.activeGameObject;
        string fullName = GetFullName(obj);
        if (string.IsNullOrEmpty(fullName))
            Debugger.Log("nothing !");
        else
        {
            string targetPath = string.Empty;
            if (fullName.Contains("ui_2d_root"))
                targetPath = fullName.Substring(fullName.IndexOf("ui_2d_root/") + 11);
            else
                targetPath = fullName;
            Debug.Log(targetPath);
        }
    }
    public static string GetFullName(GameObject obj)
    {
        if (obj == null)
        {
            return string.Empty;
        }
        if (obj.transform.parent != null)
        {
            return GetFullName(obj.transform.parent.gameObject) + "/" + obj.name;
        }
        else
        {
            return obj.name;
        }

    }
}
