using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using Common.ResMgr;
using UnityEngine.UI;
using Common.UI.Components;


namespace Common.Tools.Editor
{
    public static class MakeAtlas
    {

        /*[MenuItem("Tools/Make Selected Atlas", false, 300)]
        public static void ImportSelectedAtlas()
        {
            UnityEngine.Object[] gos = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);

            List<string> resPaths = new List<string>();

            for (int i = 0, count = gos.Length; i < count; i++)
            {
                string path = AssetDatabase.GetAssetPath(gos[i]);
                if (path.Contains("Res/Atlas"))
                    resPaths.Add(path);
            }

            if (resPaths.Count == 0)
            {
                Debugger.Log("please select a texture or a contain texture of folder !");
                return;
            }
            string spriteDir = Application.dataPath + "/Res/Resources/sprite";

            int total = resPaths.Count;
            int current = 0;
            foreach (string assetPath in resPaths)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                GameObject go = new GameObject(sprite.name);
                go.AddComponent<SpriteRenderer>().sprite = sprite;
                string dirName = spriteDir + "/" + assetPath.Replace(@"Assets/Res/Atlas/", string.Empty);
                dirName = dirName.Replace(sprite.name + ".png", string.Empty);
                dirName = dirName.Replace(sprite.name + ".tga", string.Empty);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                string allPath = dirName + sprite.name + ".prefab";
                string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));

                PrefabUtility.CreatePrefab(prefabPath, go, ReplacePrefabOptions.ReplaceNameBased);
                GameObject.DestroyImmediate(go);
                current++;
                EditorUtility.DisplayProgressBar("打包图集", "当前进度", current / (float)total);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/AtlasMaker", false, 300)]
        static private void Make()
        {
            string spriteDir = Application.dataPath + "/Res/Resources/sprite";

            if (Directory.Exists(spriteDir))
            {
                Directory.Delete(spriteDir, true);
                Directory.CreateDirectory(spriteDir);
            }

            DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Res/Atlas");
            int total = rootDirInfo.GetFiles("*.png", SearchOption.AllDirectories).Length;
            total += rootDirInfo.GetFiles("*.tga", SearchOption.AllDirectories).Length;
            int current = 0;
            foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
            {
                foreach (FileInfo pngFile in dirInfo.GetFiles("*.png", SearchOption.AllDirectories))
                {
                    string allPath = pngFile.FullName;
                    string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    GameObject go = new GameObject(sprite.name);
                    go.AddComponent<SpriteRenderer>().sprite = sprite;
                    string dirName = spriteDir + "/" + dirInfo.Name;
                    if (!Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }
                    allPath = dirName + "/" + sprite.name + ".prefab";
                    string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));

                    PrefabUtility.CreatePrefab(prefabPath, go, ReplacePrefabOptions.ReplaceNameBased);
                    GameObject.DestroyImmediate(go);
                    current++;
                    EditorUtility.DisplayProgressBar("打包图集", "当前进度", current / (float)total);
                }

                foreach (FileInfo tgaFile in dirInfo.GetFiles("*.tga", SearchOption.AllDirectories))
                {
                    string allPath = tgaFile.FullName;
                    string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    GameObject go = new GameObject(sprite.name);
                    go.AddComponent<SpriteRenderer>().sprite = sprite;
                    string dirName = spriteDir + "/" + dirInfo.Name;
                    if (!Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }
                    allPath = dirName + "/" + sprite.name + ".prefab";
                    string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));

                    PrefabUtility.CreatePrefab(prefabPath, go, ReplacePrefabOptions.ReplaceNameBased);
                    GameObject.DestroyImmediate(go);
                    current++;
                    EditorUtility.DisplayProgressBar("打包图集", "当前进度", current / (float)total);
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/CheckAtlas", false, 300)]
        public static void CheckAtlas()
        {
            DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Res/Resources/sprite");
            bool isMiss = false;
            int count = 0;
            foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
            {
                foreach (FileInfo prefabFile in dirInfo.GetFiles("*.prefab", SearchOption.AllDirectories))
                {
                    string allPath = prefabFile.FullName;
                    string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                    //assetPath = assetPath.Replace(".prefab",string.Empty);
                    //Debugger.Log(assetPath);
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    SpriteRenderer sprite = go.GetComponent<SpriteRenderer>();
                    if (!sprite)
                    {
                        count++;
                        isMiss = true;
                        Debugger.LogError(assetPath + " is miss sprite !");
                    }
                }
            }

            if (isMiss)
                Debugger.LogError("atlas make fail! miss count:" + count);
            else
                Debugger.Log("atlas make success !");
        }*/

        private static void ResPacker(UnityEngine.Object[] assets, string assetName)
        {
            if (assets == null)
            {
                return;
            }

            string path = "Assets/Res/Resources/sprite/" + assetName + "_asset.asset";
            UnityEngine.Object assetPackerObj = AssetDatabase.LoadMainAssetAtPath(path);
            AssetPacker assetPacker = null;
            if (assetPackerObj == null)
            {
                assetPacker = ScriptableObject.CreateInstance<AssetPacker>();
                assetPacker.name = assetName;
                AssetDatabase.CreateAsset(assetPacker, path);
            }
            else
            {
                assetPacker = assetPackerObj as AssetPacker;
            }

            assetPacker.mAssets = assets;
            assetPacker.hideFlags = HideFlags.NotEditable;
            EditorUtility.SetDirty(assetPacker);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/AtlasPacker", false, 0)]
        [MenuItem("Tools/AtlasPacker", false, 300)]
        public static void AtlasPacker()
        {
            string resName = string.Empty;
            UnityEngine.Object[] gos = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
            List<string> resPaths = new List<string>();

            for (int i = 0, count = gos.Length; i < count; i++)
            {
                string path = AssetDatabase.GetAssetPath(gos[i]);
                if (path.Contains("Res/Atlas") && path.EndsWith(".png") && !path.Contains("_a.png"))
                {
                    resPaths.Add(path);
                }
            }

            if (resPaths.Count == 0)
            {
                Debugger.Log("please select a texture or a contain texture of folder !");
                return;
            }
            if (resPaths.Count > 1)
            {
                Debugger.Log("can only select one texture once !");
                //return;
            }

            List<UnityEngine.Object> assets = new List<UnityEngine.Object>();
            foreach (string assetPath in resPaths)
            {
                Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                //AssetImporter asIpter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture2D));
                //if (!string.IsNullOrEmpty(asIpter.assetBundleName))
                //{
                //    resName = asIpter.assetBundleName.Substring(asIpter.assetBundleName.LastIndexOf("/") + 1);
                UnityEngine.Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(texture2D));
                assets.AddRange(sprites);
                //}
                Debugger.Log("assetPath:" + assetPath);
                //创建材质球
                int startIndex = assetPath.LastIndexOf("/") + 1;
                int endIndex = assetPath.LastIndexOf(".");
                resName = assetPath.Substring(startIndex, endIndex - startIndex);
                Debugger.Log("resName:" + resName);
                int atlasIndex = assetPath.IndexOf("Atlas/") + 6;
                string matPath = "Assets/Res/Resources/sprite/" + assetPath.Substring(atlasIndex, startIndex - atlasIndex) + resName + ".mat";
                Debugger.Log("matPath:" + matPath);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                if (mat == null)
                {
                    mat = new Material(Shader.Find("Custom/UI/Default_Alpha"));
                    AssetDatabase.CreateAsset(mat, matPath);
                }
                mat.mainTexture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
                Texture alphaTexture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath.Replace(".png", "_a.png"));
                if (alphaTexture)
                    mat.SetTexture("_AlphaTex", alphaTexture);
            }
            Debugger.Log("assets count:" + assets.Count.ToString());
            if (assets.Count <= 0)
            {
                return;
            }

            ResPacker(assets.ToArray(), resName);
            Common.Tools.Editor.MakeAtlas.SynBorderInfo();
        }

        [MenuItem("Tools/SaveBorderInfo", false, 300)]
        public static void SaveBorderInfo()
        {
            string path = "Assets/Res/Atlas";
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (var f in dir.GetFiles("*.png", SearchOption.TopDirectoryOnly))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                //Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "/" + f.Name);
                TextureImporter textureImporter = (AssetImporter.GetAtPath(path + "/" + f.Name) as TextureImporter);
                SerializedObject so = new SerializedObject(textureImporter);
                SerializedProperty sp = so.FindProperty("m_SpriteSheet.m_Sprites");
                for (int i = 0, length = sp.arraySize; i < length; i++)
                {
                    SerializedProperty sp1 = sp.GetArrayElementAtIndex(i);
                    string spriteName = sp1.FindPropertyRelative("m_Name").stringValue;
                    Vector4 border = sp1.FindPropertyRelative("m_Border").vector4Value;
                    //if (texture2D.name == "main_ui")
                    //    Debugger.Log(spriteName + ";" + border.x + "," + border.y + "," + border.z + "," + border.w);
                    sb.Append(spriteName + ";" + border.x + "," + border.y + "," + border.z + "," + border.w + "\n");
                }
                string infoFile = Application.dataPath + "/Res/Atlas/" + f.Name.Replace(".png", ".txt");
                //Debugger.Log(infoFile);
                if (File.Exists(infoFile))
                    File.Delete(infoFile);
                File.AppendAllText(infoFile, sb.ToString());
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debugger.Log("save border info success !");
        }

        [MenuItem("Tools/SynBorderInfo", false, 300)]
        public static void SynBorderInfo()
        {
            return;//do not loss border info of sprite when atlas was modified
            string path = "Assets/Res/Atlas";
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (var f in dir.GetFiles("*.png", SearchOption.TopDirectoryOnly))
            {
                string infoFile = Application.dataPath + "/Res/Atlas/" + f.Name.Replace(".png", ".txt");
                //Debugger.Log(infoFile);
                if (!File.Exists(infoFile))
                {
                    Debugger.LogError("not exist info file {0}", infoFile);
                    return;
                }
                string[] lines = File.ReadAllLines(infoFile);
                Dictionary<string, Vector4> dic = new Dictionary<string, Vector4>();
                foreach (var l in lines)
                {
                    string[] strs = l.Split(';');
                    if (strs.Length == 2)
                    {
                        float[] vs = strs[1].ToArray<float>();
                        Vector4 border = new Vector4(vs[0], vs[1], vs[2], vs[3]);
                        dic.Add(strs[0], border);
                    }
                    else
                    {
                        throw new System.Exception(string.Format("format error {0} 2 array", l));
                    }
                }
                Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "/" + f.Name);
                TextureImporter textureImporter = (AssetImporter.GetAtPath(path + "/" + f.Name) as TextureImporter);
                SerializedObject so = new SerializedObject(textureImporter);
                SerializedProperty sp = so.FindProperty("m_SpriteSheet.m_Sprites");
                for (int i = 0, length = sp.arraySize; i < length; i++)
                {
                    SerializedProperty sp1 = sp.GetArrayElementAtIndex(i);
                    string spriteName = sp1.FindPropertyRelative("m_Name").stringValue;
                    if (dic.ContainsKey(spriteName))
                    {
                        sp1.FindPropertyRelative("m_Border").vector4Value = dic[spriteName];
                        so.ApplyModifiedPropertiesWithoutUndo();
                    }
                    else
                        Debugger.LogError("not exist sprite name:{0}", spriteName);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debugger.Log("syn border info success !");
        }

        /*[MenuItem("Tools/Copy Border of sprite", false, 300)]
        public static void CopyBorderOfSprite()
        {
            UnityEngine.Object[] gos = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);

            List<string> resPaths = new List<string>();

            for (int i = 0, count = gos.Length; i < count; i++)
            {
                string path = AssetDatabase.GetAssetPath(gos[i]);
                if (path.Contains("Res/Atlas"))
                    resPaths.Add(path);
            }

            if (resPaths.Count == 0)
            {
                Debugger.Log("please select a texture or a contain texture of folder !");
                return;
            }
            foreach (string assetPath in resPaths)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                string path = assetPath.Substring(0, assetPath.LastIndexOf("/"));
                string atlasName = path.Substring(6);
                string borderFile = Application.dataPath + "/" + atlasName + ".txt";
                //Debugger.Log(borderFile);
                TextureImporter textureImporter = (AssetImporter.GetAtPath(path + ".png") as TextureImporter);
                SerializedObject so = new SerializedObject(textureImporter);
                SerializedProperty sp = so.FindProperty("m_SpriteSheet.m_Sprites");
                for (int i = 0, length = sp.arraySize; i < length; i++)
                {
                    SerializedProperty sp1 = sp.GetArrayElementAtIndex(i);
                    string spriteName = sp1.FindPropertyRelative("m_Name").stringValue;
                    if (spriteName == sprite.name)
                    {
                        sp1.FindPropertyRelative("m_Border").vector4Value = sprite.border;
                        so.ApplyModifiedPropertiesWithoutUndo();
                        File.AppendAllText(borderFile, spriteName + ";" + sprite.border.x + "," + sprite.border.y + "," + sprite.border.z + "," + sprite.border.w + "\n");
                        Debugger.Log("copy {0}'s border 2 {1}", sprite.name, spriteName);
                        AssetDatabase.SaveAssets();
                        break;
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        private static void DoTextureReimport(string path)
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                AssetDatabase.ImportAsset(path);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        [MenuItem("Tools/Replace All of sprites in ui prefab", false, 300)]
        public static void ReplaceAllSprites()
        {
            string uiPath = "Assets/Res/Resources/ui";
            UnityEngine.Object[] gos = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

            List<string> resPaths = new List<string>();

            for (int i = 0, count = gos.Length; i < count; i++)
            {
                string path = AssetDatabase.GetAssetPath(gos[i]);
                if (path.Contains("Res/Resources/ui"))
                    resPaths.Add(path);
            }

            if (resPaths.Count == 0)
            {
                Debugger.Log("please select a ui prefab or a contain ui prefab of folder !");
                return;
            }
            foreach (string assetPath in resPaths)
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath) as GameObject;
                if (!go) continue;
                //Debugger.Log(go.name);
                //Transform core = go.transform.FindChild("core");
                //if (!core) continue;
                //Debugger.Log(core.name);
                Image[] imgs = go.transform.GetComponentsInChildren<Image>(true);
                //RawImage[] rawImgs = core.GetComponentsInChildren<RawImage>(true);
                //Debugger.Log(imgs.Length.ToString());
                //Debugger.Log(rawImgs.Length.ToString());
                foreach (var img in imgs)
                {
                    if (img.sprite == null)
                        continue;
                    String path = AssetDatabase.GetAssetPath(img.sprite.texture);
                    string spriteName = img.sprite.name;
                    if (path == "Resources/unity_builtin_extra")
                        continue;
                    if (path.Contains("Assets/Res/Atlas/map/"))
                        continue;
                    Debugger.Log(go.name + "   " + img.gameObject.name + "   " + spriteName + "   " + path);
                    TextureImporter textureImporter = TextureImporter.GetAtPath(path) as TextureImporter;
                    string atlas = textureImporter.spritePackingTag;
                    if (string.IsNullOrEmpty(atlas)) continue;
                    string asset = "sprite/" + atlas;
                    Debugger.Log(asset);
                    AssetPacker assetPacker = Resources.Load<AssetPacker>(asset);
                    Sprite sprite = assetPacker.GetSprite(spriteName);
                    img.sprite = sprite;
                }
                EditorUtility.SetDirty(go);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Tools/Replace All of sprites in ui map", false, 300)]
        public static void ReplaceAllSpritesInMap()
        {
            string uiPath = "Assets/Res/Resources/map";
            UnityEngine.Object[] gos = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

            List<string> resPaths = new List<string>();

            for (int i = 0, count = gos.Length; i < count; i++)
            {
                string path = AssetDatabase.GetAssetPath(gos[i]);
                if (path.Contains("Res/Resources/map"))
                    resPaths.Add(path);
            }

            if (resPaths.Count == 0)
            {
                Debugger.Log("please select a map prefab or a contain map prefab of folder !");
                return;
            }
            foreach (string assetPath in resPaths)
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath) as GameObject;
                if (!go) continue;
                SpriteRenderer[] spriteRenderers = go.transform.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (var r in spriteRenderers)
                {
                    if (r.sprite == null)
                        continue;
                    String path = AssetDatabase.GetAssetPath(r.sprite.texture);
                    string spriteName = r.sprite.name;

                    Debugger.Log(go.name + "   " + r.gameObject.name + "   " + spriteName + "   " + path);
                    TextureImporter textureImporter = TextureImporter.GetAtPath(path) as TextureImporter;
                    string atlas = textureImporter.spritePackingTag;
                    if (string.IsNullOrEmpty(atlas)) continue;
                    string asset = "Assets/Res/Atlas/map/" + atlas + ".png";
                    Debugger.Log(asset);
                    UnityEngine.Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(asset);
                    foreach (var s in sprites)
                    {
                        Sprite sprite = s as Sprite;
                        if (sprite)
                        {
                            if (spriteName == sprite.name)
                            {
                                r.sprite = sprite;
                                break;
                            }
                        }
                    }
                }
                EditorUtility.SetDirty(go);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }*/
		[MenuItem("Assets/Add ButtonSound To Button In select Prefabs ", false, 300)]
		public static void AddButtonSoundToButtonInPrefab()
		{
			UnityEngine.Object[] gos = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
			string resPath = "";
			GameObject go = null;
			for (int i = 0, count = gos.Length; i < count; i++)
			{
				go = gos[i] as GameObject;
				resPath = AssetDatabase.GetAssetPath(go);
				if (resPath.EndsWith(".prefab"))
				{
					Button[] btns = go.transform.GetComponentsInChildren<Button>(true);
					foreach (var r in btns)
					{
						if(r.GetComponent<ButtonSound>() == null)
						{
							ButtonSound bs =  r.gameObject.AddComponent<ButtonSound>();
							bs.type = Logic.Enums.ButtonSoundType.NormalClick;
						}

					}
					EditorUtility.SetDirty(go);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
				
			}
		}
        [MenuItem("Assets/Add Material To Sprite In Prefab ", false, 300)]
        public static void AddMaterialToSpriteInAllPrefab()
        {
            UnityEngine.Object[] gos = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            string resPath = "";
            GameObject go = null;
            for (int i = 0, count = gos.Length; i < count; i++)
            {
                go = gos[i] as GameObject;
                resPath = AssetDatabase.GetAssetPath(go);
                if (resPath.EndsWith(".prefab"))
                {
                    Image[] ImageRenderers = go.transform.GetComponentsInChildren<Image>(true);
                    foreach (var r in ImageRenderers)
                    {
                        SetImageMaterialDefault(r);
                        //						if (r.sprite != null && r.sprite.name.Equals("mask2"))
                        //						{
                        //							Debugger.Log("find sprite:" + go.name +"," +r.sprite.name+","+ r.sprite.texture.name);
                        //
                        //						}
                    }
                    //					RawImage[] rawRenderers = go.transform.GetComponentsInChildren<RawImage>(true);
                    //					foreach (var r in rawRenderers)
                    //					{
                    //						SetImageMaterialDefault(r);
                    //						if (r.texture != null &&r.texture.name.Equals("mask2"))
                    //						{
                    //							Debugger.Log("find texture:"+go.name + ","+r.texture.name);
                    //
                    //                        }
                    //                    }
                    SpriteRenderer[] spriteRenderers = go.transform.GetComponentsInChildren<SpriteRenderer>(true);
                    foreach (var r in spriteRenderers)
                    {
                        SetSpriteRenderMaterialDefault(r);
                    }
                    EditorUtility.SetDirty(go);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

            }

        }
        public static void SetImageMaterialDefault(Image r)
        {
            if (r.sprite == null)
                return;
            String texturePath = AssetDatabase.GetAssetPath(r.sprite.texture);
            string spriteName = r.sprite.name;

            if (texturePath.Contains("Assets/Res/Atlas"))
            {
                int startIndex = texturePath.LastIndexOf("/") + 1;
                int endIndex = texturePath.LastIndexOf(".");
                string resName = texturePath.Substring(startIndex, endIndex - startIndex);
                int atlasIndex = texturePath.IndexOf("Atlas/") + 6;
                string matPath = "Assets/Res/Resources/sprite/" + texturePath.Substring(atlasIndex, startIndex - atlasIndex) + resName + ".mat";
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                r.material = mat;
            }

        }
        public static void SetSpriteRenderMaterialDefault(SpriteRenderer r)
        {
            if (r.sprite == null)
                return;
            String texturePath = AssetDatabase.GetAssetPath(r.sprite.texture);
            string spriteName = r.sprite.name;

            if (texturePath.Contains("Assets/Res/Atlas"))
            {
                int startIndex = texturePath.LastIndexOf("/") + 1;
                int endIndex = texturePath.LastIndexOf(".");
                string resName = texturePath.Substring(startIndex, endIndex - startIndex);
                int atlasIndex = texturePath.IndexOf("Atlas/") + 6;
                string matPath = "Assets/Res/Resources/sprite/" + texturePath.Substring(atlasIndex, startIndex - atlasIndex) + resName + ".mat";
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                r.material = mat;
            }
        }

    }
}
