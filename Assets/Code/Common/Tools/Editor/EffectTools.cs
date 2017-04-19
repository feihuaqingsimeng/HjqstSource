using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
namespace Common.Tools.Editor
{
    public static class EffectTools
    {
        [MenuItem("Tools/Find reference", false, 400)]
        private static void FindReference()
        {
            Object obj = Selection.activeObject;
            if (!obj)
            {
                Debugger.Log("please select a file !");
                return;
            }

            if (obj is Material)
            {
                Material m = obj as Material;
                Debugger.Log(obj.name + " " + obj.GetType());
                Debugger.Log("reference shader:" + m.shader.name);
                foreach (var path in FindPrefabPathByMaterial(m))
                {
                    Debugger.Log("reference prefab:" + path);
                }
                Debugger.Log("reference texture:" + m.mainTexture.name);
            }
            else if (obj is Texture2D)
            {
                Debugger.Log(obj.name + " " + obj.GetType());
                Texture2D texture2D = obj as Texture2D;
                foreach (var path in FindMaterialPathByTexture2D(texture2D))
                {
                    Debugger.Log("reference material:" + path);
                }
                foreach (Material m in FindMaterialByTexture2D(texture2D))
                {
                    if (!m) continue;
                    Debugger.Log("reference shader:" + m.shader.name);
                    foreach (var path in FindPrefabPathByMaterial(m))
                    {
                        Debugger.Log("reference prefab:" + path);
                    }
                }

            }
            else if (obj is Shader)
            {
                Debugger.Log(obj.name + " " + obj.GetType());
                Shader shader = obj as Shader;
                foreach (var path in FindMaterialPathByShader(shader))
                {
                    Debugger.Log("reference material:" + path);
                }
                foreach (var m in FindMaterialByShader(shader))
                {
                    if (!m) continue;
                    foreach (var path in FindPrefabPathByMaterial(m))
                    {
                        Debugger.Log("reference prefab:" + path);
                    }
                }
            }
            else
            {
                Debugger.Log(obj.name + "  unsupport this type:" + obj.GetType());
            }
            System.GC.Collect();
        }

        [MenuItem("Tools/Find material references", false, 400)]
        private static void FindMaterialReferences()
        {
            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();
            string dir = Application.dataPath;
            dir = dir + "/Res/Resources/effects/";
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo[] files = di.GetFiles("*.mat", SearchOption.AllDirectories);
            int count = 0;
            foreach (FileInfo f in files)
            {
                count++;
                EditorUtility.DisplayProgressBar("提示", "当前进度", (float)count / files.Length);
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                Material m = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as Material;

                //if (FindPrefabPathByMaterial(m).Count == 0)
                result.Add(new KeyValuePair<string, int>(assetsPath, FindPrefabPathByMaterial(m).Count));
            }
            EditorUtility.ClearProgressBar();
            result.Sort(Sort);
            Debugger.Log("material references info:-------------------------------------------------------------------");
            foreach (var kvp in result)
            {
                Debugger.Log(kvp.Key + ":" + kvp.Value);
            }
        }

        private static int Sort(KeyValuePair<string, int> a, KeyValuePair<string, int> b)
        {
            if (a.Value < b.Value)
                return -1;
            if (a.Value > b.Value)
                return 1;
            return 0;
        }

        [MenuItem("Tools/Find texture references", false, 400)]
        private static void FindTextureReferences()
        {
            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();
            string dir = Application.dataPath;
            dir = dir + "/Res/Resources/effects/";
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo[] pngFiles = di.GetFiles("*.png", SearchOption.AllDirectories);
            FileInfo[] tgaFiles = di.GetFiles("*.tga", SearchOption.AllDirectories);
            FileInfo[] jpgFiles = di.GetFiles("*.jpg", SearchOption.AllDirectories);
            FileInfo[] psdFiles = di.GetFiles("*.psd", SearchOption.AllDirectories);
            int count = 0;
            float total = pngFiles.Length + tgaFiles.Length + jpgFiles.Length + psdFiles.Length;
            foreach (FileInfo f in pngFiles)
            {
                count++;
                EditorUtility.DisplayProgressBar("提示", "当前进度", (float)count / total);
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                Texture2D t = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as Texture2D;
                result.Add(new KeyValuePair<string, int>(assetsPath, FindMaterialPathByTexture2D(t).Count));
            }
            foreach (FileInfo f in tgaFiles)
            {
                count++;
                EditorUtility.DisplayProgressBar("提示", "当前进度", (float)count / total);
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                Texture2D t = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as Texture2D;
                result.Add(new KeyValuePair<string, int>(assetsPath, FindMaterialPathByTexture2D(t).Count));
            }
            foreach (FileInfo f in jpgFiles)
            {
                count++;
                EditorUtility.DisplayProgressBar("提示", "当前进度", (float)count / total);
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                Texture2D t = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as Texture2D;
                result.Add(new KeyValuePair<string, int>(assetsPath, FindMaterialPathByTexture2D(t).Count));
            }
            foreach (FileInfo f in psdFiles)
            {
                count++;
                EditorUtility.DisplayProgressBar("提示", "当前进度", (float)count / total);
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                Texture2D t = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as Texture2D;
                result.Add(new KeyValuePair<string, int>(assetsPath, FindMaterialPathByTexture2D(t).Count));
            }
            EditorUtility.ClearProgressBar();
            result.Sort(Sort);
            Debugger.Log("texture references info:-------------------------------------------------------------------");
            foreach (var kvp in result)
            {
                Debugger.Log(kvp.Key + ":" + kvp.Value);
            }
        }

        private static List<string> FindPrefabPathByMaterial(Material material)
        {
            List<string> result = new List<string>();
            string dir = Application.dataPath;
            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo f in di.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                GameObject prefab = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as GameObject;
                if (!prefab)
                {
                    Debugger.LogError("can find prefab:" + assetsPath);
                    continue;
                }
                Renderer[] rs = prefab.GetComponentsInChildren<Renderer>(true);
                foreach (var r in rs)
                {
                    foreach (Material v in r.sharedMaterials)
                    {
                        if (v == material)
                            if (!result.Contains(assetsPath))
                                result.Add(assetsPath);
                    }
                    Material m = r.sharedMaterial;
                    if (m == material)
                    {
                        if (!result.Contains(assetsPath))
                            result.Add(assetsPath);
                    }
                }
            }
            return result;
        }

        private static List<GameObject> FindPrefabByMaterial(Material material)
        {
            List<GameObject> result = new List<GameObject>();
            string dir = Application.dataPath;
            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo f in di.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                GameObject prefab = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as GameObject;
                Renderer[] rs = prefab.GetComponentsInChildren<Renderer>(true);
                foreach (var r in rs)
                {
                    foreach (Material v in r.sharedMaterials)
                    {
                        if (v == material)
                            if (!result.Contains(prefab))
                                result.Add(prefab);
                    }
                    Material m = r.sharedMaterial;
                    if (m == material)
                    {
                        if (!result.Contains(prefab))
                            result.Add(prefab);
                    }
                }
            }
            return result;
        }

        private static List<string> FindMaterialPathByTexture2D(Texture2D texture2D)
        {
            List<string> result = new List<string>();
            string dir = Application.dataPath;
            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo f in di.GetFiles("*.mat", SearchOption.AllDirectories))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                Material m = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as Material;

                if (m.mainTexture == texture2D)
                    if (!result.Contains(assetsPath))
                        result.Add(assetsPath);
            }
            return result;
        }

        private static List<Material> FindMaterialByTexture2D(Texture2D texture2D)
        {
            List<Material> result = new List<Material>();
            string dir = Application.dataPath;
            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo f in di.GetFiles("*.mat", SearchOption.AllDirectories))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                Material m = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as Material;

                if (m.mainTexture == texture2D)
                    if (!result.Contains(m))
                        result.Add(m);
            }
            return result;
        }

        private static List<string> FindMaterialPathByShader(Shader shader)
        {
            List<string> result = new List<string>();
            string dir = Application.dataPath;
            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo f in di.GetFiles("*.mat", SearchOption.AllDirectories))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                Material m = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as Material;
                if (m.shader == shader)
                    if (!result.Contains(assetsPath))
                        result.Add(assetsPath);
            }
            return result;
        }

        private static List<Material> FindMaterialByShader(Shader shader)
        {
            List<Material> result = new List<Material>();
            string dir = Application.dataPath;
            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo f in di.GetFiles("*.mat", SearchOption.AllDirectories))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                Material m = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as Material;
                if (m.shader == shader)
                    if (!result.Contains(m))
                        result.Add(m);
            }
            return result;
        }

        [MenuItem("Tools/Find DDS", false, 400)]
        private static void FindeDDS()
        {
            string dir = Application.dataPath;
            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo f in di.GetFiles("*.dds", SearchOption.AllDirectories))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                Debugger.Log(assetsPath);
            }
        }

        [MenuItem("Tools/Remove DDS", false, 400)]
        private static void RemoveDDS()
        {
            string dir = Application.dataPath;
            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo f in di.GetFiles("*.dds", SearchOption.AllDirectories))
            {
                File.Delete(f.FullName);
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Find Default-Particle Mat", false, 400)]
        private static void FindDefaultParticleMat()
        {
            string dir = Application.dataPath;
            dir = dir + "/Res/Resources/effects/prefabs";
            DirectoryInfo di = new DirectoryInfo(dir);
            bool hasSTH = false;
            foreach (FileInfo f in di.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                GameObject go = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as GameObject;
                Renderer[] rs = go.GetComponentsInChildren<Renderer>(true);
                foreach (var r in rs)
                {
                    Material m = r.sharedMaterial;
                    if (m)
                    {
                        //Debugger.Log(m.name);
                        if (m.name == "Default-Particle")
                        {
                            hasSTH = true;
                            Debugger.Log("shader:" + m.shader.name);
                            Debugger.Log(assetsPath);
                        }
                    }
                    Material[] ms = r.sharedMaterials;
                    foreach (var v in ms)
                    {
                        if (!v) continue;
                        if (v.name == "Default-Particle")
                        {
                            hasSTH = true;
                            Debugger.Log("shader:" + v.shader.name);
                            Debugger.Log(assetsPath);
                        }
                    }
                }
            }
            if (!hasSTH)
                Debugger.Log("nothing");
        }

        [MenuItem("Tools/Find Custom-Particle Mat", false, 400)]
        private static void FindCustomParticleMat()
        {
            List<string> shaderNames = new List<string>() { "Custom/Particles/Additive", "Custom/Particles/~Additive-Multiply", "Custom/Particles/Additive (Soft)", "Custom/Particles/Alpha Blended", 
                "Custom/Particles/Blend", "Custom/Particles/Multiply", "Custom/Particles/Multiply (Double)", "Custom/Particles/Alpha Blended Premultiply", "Custom/Particles/VertexLit Blended" };
            string dir = Application.dataPath;
            dir = dir + "/Res/Resources/effects";
            DirectoryInfo di = new DirectoryInfo(dir);
            bool hasSTH = false;
            foreach (FileInfo f in di.GetFiles("*.mat", SearchOption.AllDirectories))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                Material mat = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Material)) as Material;
                if (mat)
                {
                    //Debugger.Log(m.name);
                    if (shaderNames.Contains(mat.shader.name))
                    {
                        hasSTH = true;
                        Debugger.Log("mat:" + mat.name);
                        Debugger.Log(assetsPath);
                    }
                }
            }
            if (!hasSTH)
                Debugger.Log("nothing");
        }

        [MenuItem("Tools/Replace All Materail Particle Shader To Custom", false, 400)]
        private static void ReplaceAllMaterailParticleShader2Custom()
        {
            List<string> shaderNames = new List<string>() { "Particles/Additive", "Particles/~Additive-Multiply", "Particles/Additive (Soft)", "Particles/Alpha Blended", 
                "Particles/Blend", "Particles/Multiply", "Particles/Multiply (Double)", "Particles/Alpha Blended Premultiply", "Particles/VertexLit Blended" };
            string dir = Application.dataPath;
            dir = dir + "/Res/Resources/effects/prefabs";
            DirectoryInfo di = new DirectoryInfo(dir);
            bool hasSTH = false;
            foreach (FileInfo f in di.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                if (!f.Name.Contains("_mirror")) continue;
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                GameObject go = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as GameObject;
                Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
                foreach (var r in renderers)
                {
                    Material m = r.sharedMaterial;
                    if (shaderNames.Contains(m.shader.name))
                    {
                        hasSTH = true;
                        Debugger.Log("original shader:" + m.shader.name);
                        m.shader = Shader.Find("Custom/" + m.shader.name);
                        Debugger.Log("new shader:" + m.shader.name);
                        Debugger.Log("----------------------------------------------分割线--------------------------------------------------");
                    }
                    Material[] ms = r.sharedMaterials;
                    foreach (var v in ms)
                    {
                        if (!v) continue;
                        if (shaderNames.Contains(v.shader.name))
                        {
                            hasSTH = true;
                            Debugger.Log("original shader:" + v.shader.name);
                            v.shader = Shader.Find("Custom/" + v.shader.name);
                            Debugger.Log("new shader:" + v.shader.name);
                            Debugger.Log("----------------------------------------------分割线--------------------------------------------------");
                        }
                    }
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (!hasSTH)
                Debugger.Log("nothing");
        }

        [MenuItem("Tools/Replace Selected Materail Particle Shader To Custom", false, 400)]
        private static void ReplaceSelectedMaterailParticleShader2Custom()
        {
            List<string> shaderNames = new List<string>() { "Particles/Additive", "Particles/~Additive-Multiply", "Particles/Additive (Soft)", "Particles/Alpha Blended", 
                "Particles/Blend", "Particles/Multiply", "Particles/Multiply (Double)", "Particles/Alpha Blended Premultiply", "Particles/VertexLit Blended" };
            Object obj = Selection.activeObject;
            if (!obj)
            {
                Debugger.Log("selected noting!");
                return;
            }
            //if (!obj.name.Contains("_mirror"))
            //{
            //    Debugger.Log("selected target need not mirror !!");
            //    return;
            //}
            bool hasSTH = false;
            if (obj is GameObject)
            {
                GameObject go = obj as GameObject;
                Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
                foreach (var r in renderers)
                {
                    Material m = r.sharedMaterial;
                    if (shaderNames.Contains(m.shader.name))
                    {
                        hasSTH = true;
                        Debugger.Log("original shader:" + m.shader.name);
                        m.shader = Shader.Find("Custom/" + m.shader.name);
                        Debugger.Log("new shader:" + m.shader.name);
                        Debugger.Log("----------------------------------------------分割线--------------------------------------------------");
                    }
                    Material[] ms = r.sharedMaterials;
                    foreach (var v in ms)
                    {
                        if (!v) continue;
                        if (shaderNames.Contains(v.shader.name))
                        {
                            hasSTH = true;
                            Debugger.Log("original shader:" + v.shader.name);
                            v.shader = Shader.Find("Custom/" + v.shader.name);
                            Debugger.Log("new shader:" + v.shader.name);
                            Debugger.Log("----------------------------------------------分割线--------------------------------------------------");
                        }
                    }
                }
            }
            else if (obj is Material)
            {
                Material m = obj as Material;
                if (shaderNames.Contains(m.shader.name))
                {
                    hasSTH = true;
                    Debugger.Log("original shader:" + m.shader.name);
                    m.shader = Shader.Find("Custom/" + m.shader.name);
                    Debugger.Log("new shader:" + m.shader.name);
                    Debugger.Log("----------------------------------------------分割线--------------------------------------------------");
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (!hasSTH)
                Debugger.Log("nothing");
        }

        [MenuItem("Tools/Find Miss Shader Mat Of Effects", false, 400)]
        private static void FindMissShaderMatOfEffects()
        {
            string dir = Application.dataPath;
            dir = dir + "/Res/Resources/effects/prefabs";
            DirectoryInfo di = new DirectoryInfo(dir);
            bool hasSTH = false;
            foreach (FileInfo f in di.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                GameObject go = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as GameObject;
                Renderer[] rs = go.GetComponentsInChildren<Renderer>(true);
                foreach (var r in rs)
                {
                    Material m = r.sharedMaterial;
                    if (m)
                    {
                        //Debugger.Log(m.name);
                        if (m.shader.name == "Hidden/InternalErrorShader")
                        {
                            hasSTH = true;
                            Debugger.Log("shader:" + m.shader.name);
                            Debugger.Log(assetsPath);
                        }
                    }
                    Material[] ms = r.sharedMaterials;
                    foreach (var v in ms)
                    {
                        if (!v) continue;
                        if (v.shader.name == "Hidden/InternalErrorShader")
                        {
                            hasSTH = true;
                            Debugger.Log("shader:" + v.shader.name);
                            Debugger.Log(assetsPath);
                        }
                    }
                }
            }
            if (!hasSTH)
                Debugger.Log("nothing");
        }

        [MenuItem("Tools/Replace Miss Shader Mat To ParticleAdd", false, 400)]
        private static void ReplaceMissShaderMat2ParticleAdd()
        {
            string dir = Application.dataPath;
            dir = dir + "/Res/Resources/effects/prefabs";
            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo f in di.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                GameObject go = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as GameObject;
                Renderer[] rs = go.GetComponentsInChildren<Renderer>(true);
                foreach (var r in rs)
                {
                    Material m = r.sharedMaterial;
                    if (m)
                    {
                        //Debugger.Log(m.name);
                        if (m.shader.name == "Hidden/InternalErrorShader")
                        {
                            m.shader = Shader.Find("Custom/Particles/Additive");
                        }
                    }
                    Material[] ms = r.sharedMaterials;
                    foreach (var v in ms)
                    {
                        if (!v) continue;
                        if (v.shader.name == "Hidden/InternalErrorShader")
                        {
                            m.shader = Shader.Find("Custom/Particles/Additive");
                        }
                    }
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debugger.Log("Replace Miss Shader Mat To Custom/Particles/Additive Over ");
        }


        [MenuItem("Tools/Find Unuse Effects", false, 400)]
        static void FindUnuseEffects()
        {
            List<Logic.Effect.Model.EffectData> effects = Logic.Effect.Model.EffectData.GetEffectDatas().GetValues();
            string dir = Application.dataPath;
            dir = dir + "/Res/Resources/effects/prefabs";
            DirectoryInfo di = new DirectoryInfo(dir);
            Debugger.Log("unuse effects list:");
            foreach (FileInfo f in di.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string name = f.Name.Replace(".prefab", string.Empty);
                bool isContain = false;
                foreach (var e in effects)
                {
                    if (name == e.effectName)
                    {
                        isContain |= true;
                        break;
                    }
                    if (isContain)
                        break;
                }
                if (!isContain)
                {
                    string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                    string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                    Debugger.Log(assetsPath);
                }
            }
        }

        [MenuItem("Tools/Turn off Shadow of Effects", false, 400)]
        private static void TurnoffShadowofEffects()
        {
            //Object obj = Selection.activeObject;
            //if (!obj)
            //{
            //    Debugger.Log("please select a file !");
            //    return;
            //}
            //string dir = "/Res/Resources/effects/prefabs";
            //string path = AssetDatabase.GetAssetPath(obj);
            //if (!path.Contains(dir)) return;
            DirectoryInfo dir = new DirectoryInfo("Assets/Res/Resources/effects/prefabs");
            foreach (var f in dir.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                GameObject go = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Object)) as GameObject;
                TurnoffShadowofEffect(go);
            }
            Debugger.Log("Turn off Shadow Success !");
            //if()
        }

        private static void TurnoffShadowofEffect(GameObject go)
        {
            Renderer[] rs = go.GetComponentsInChildren<Renderer>(true);
            foreach (var r in rs)
            {
                r.receiveShadows = false;
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }

        [MenuItem("Tools/Find Null Animator Controller", false, 400)]
        static void FindNullAnimatorController()
        {
            string dir = Application.dataPath;
            dir = dir + "/Res/Resources/effects/prefabs";
            DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo f in di.GetFiles("*.prefab", SearchOption.AllDirectories))
            {
                string name = f.Name.Replace(".prefab", string.Empty);
                string fullName = f.FullName.Replace(Path.DirectorySeparatorChar, '/');
                string assetsPath = "Assets" + fullName.Substring(Application.dataPath.Length);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetsPath);
                Animator[] anims = go.GetComponentsInChildren<Animator>(true);
                if (anims != null && anims.Length > 0)
                {
                    bool isNull = false;
                    for (int i = 0, length = anims.Length; i < length; i++)
                    {
                        Animator anim = anims[i];
                        if (anim.runtimeAnimatorController == null)
                        {
                            isNull = true;
                            break;
                        }
                    }
                    if (isNull)
                        Debugger.Log(assetsPath);
                }
            }
        }
    }
}