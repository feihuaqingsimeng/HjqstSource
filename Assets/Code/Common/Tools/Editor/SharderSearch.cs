using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

public static class SharderSearch
{
    //static Dictionary<string, Shader> shaderDic = new Dictionary<string, Shader>();
    //[MenuItem("CEngine/Tools/Search Shader")]
    //static void Search()
    //{
    //    shaderDic.Clear();
    //    DirectoryInfo di = new DirectoryInfo(@"Assets/Res/Particles/Prefebs");
    //    Search(di);
    //    string strs = "";
    //    foreach (var sd in shaderDic)
    //        strs += sd.Key + "\n";
    //    Debugger.LogError(strs);
    //}

    //static void Search(DirectoryInfo di)
    //{
    //    foreach (var fi in di.GetFiles("*.prefab"))
    //    {
    //        string fullName = fi.FullName.Replace(Path.DirectorySeparatorChar, '/');
    //        Object selectObj = AssetDatabase.LoadAssetAtPath("Assets" + fullName.Substring(Application.dataPath.Length), typeof(Object));
    //        GameObject go = selectObj as GameObject;
    //        foreach (var r in go.GetComponentsInChildren<Renderer>(true))
    //        {
    //            Shader s = r.sharedMaterial.shader;
    //            shaderDic.TryAdd(s.name, s);
    //        }
    //    }
    //    foreach (var d in di.GetDirectories())
    //    {
    //        Search(d);
    //    }
    //}
}
