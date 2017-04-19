using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Common.PreImprot.Editor
{
    class PreImportAssets : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            return;
            foreach (var str in importedAssets)
            {
                if (str.EndsWith(".cs") || str.EndsWith(".js") || str.EndsWith(".asset"))
                    continue;

                //Debugger.Log("Reimported Asset: " + str);
                if (/*str.Contains(@"Res/Resources/character") || str.Contains(@"Res/Resources/animcontroller")
                    ||*/ str.Contains(@"Res/Resources/texture") || str.Contains(@"Res/Resources/shader") /*|| str.Contains(@"Res/Model/hero") || str.Contains(@"Res/Model/pet") || str.Contains(@"Res/Model/player")*/
                    || str.Contains(@"Res/Atlas") || str.Contains(@"Res/Resources/ui") || str.Contains(@"Res/Resources/map") || str.Contains(@"Res/Resources/effects")
                    || str.Contains(@"Res/Resources/fonts") || str.Contains(@"Res/Resources/ui_texture") || str.Contains(@"Res/Resources/material")
                    || str.Contains(@"Res/Resources/audio"))
                {
                    if (str.Contains(@"Res/Resources/ui") && str.EndsWith(".prefab") && !str.Contains("Res/Resources/ui/load_game") && !str.Contains("Res/Resources/ui/launch"))
                    {
                        AssetImporter assetImport = AssetImporter.GetAtPath(str);
                        string extension = str.Substring(str.LastIndexOf("."));
                        //Debugger.LogError(extension);
                        string assetBundleName = str.Replace(@"Assets/Res/", string.Empty);
                        assetBundleName = assetBundleName.Replace(extension, string.Empty);
                        assetBundleName = assetBundleName.Replace(@"Resources/", string.Empty);
                        assetImport.assetBundleName = assetBundleName;
                        assetImport.assetBundleVariant = ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                    }
                    else if ((str.Contains(@"Res/Resources/shader/uishader") || str.Contains(@"Res/Resources/shader/character") || str.Contains(@"Res/Resources/shader/effect")) && str.EndsWith(".shader"))
                    {
                        AssetImporter assetImport = AssetImporter.GetAtPath(str);
                        string extension = str.Substring(str.LastIndexOf("."));
                        //Debugger.LogError(extension);
                        string assetBundleName = str.Replace(@"Assets/Res/", string.Empty);
                        assetBundleName = assetBundleName.Replace(extension, string.Empty);
                        assetBundleName = assetBundleName.Replace(@"Resources/", string.Empty);
                        assetImport.assetBundleName = assetBundleName;
                        assetImport.assetBundleVariant = ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                    }

                    else if (str.Contains(@"Res/Resources/character") || str.Contains(@"Res/Resources/animcontroller")
                        /*|| str.Contains(@"Res/Resources/map")*/)
                    {
                        if (str.EndsWith(".prefab") /*|| str.EndsWith(".mat") */ ||
                            /* str.EndsWith(".tga") || str.EndsWith(".png") ||*/ str.EndsWith(".controller")/* || str.EndsWith(".anim")*/)
                        {
                            AssetImporter assetImport = AssetImporter.GetAtPath(str);
                            string extension = str.Substring(str.LastIndexOf("."));
                            //Debugger.LogError(extension);
                            string assetBundleName = str.Replace(@"Assets/Res/", string.Empty);
                            assetBundleName = assetBundleName.Replace(extension, string.Empty);
                            assetBundleName = assetBundleName.Replace(@"Resources/", string.Empty);
                            assetImport.assetBundleName = assetBundleName;
                            assetImport.assetBundleVariant = ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                        }
                        else
                        {
                            AssetImporter assetImport = AssetImporter.GetAtPath(str);
                            assetImport.assetBundleName = string.Empty;
                        }
                    }
                    else if (str.Contains(@"Res/Resources/texture") && (str.EndsWith(".tga") || str.EndsWith(".png")))
                    {
                        AssetImporter assetImport = AssetImporter.GetAtPath(str);
                        string extension = str.Substring(str.LastIndexOf("."));
                        //Debugger.LogError(extension);
                        string assetBundleName = str.Replace(@"Assets/Res/", string.Empty);
                        assetBundleName = assetBundleName.Replace(extension, string.Empty);
                        assetBundleName = assetBundleName.Replace(@"Resources/", string.Empty);
                        assetImport.assetBundleName = assetBundleName;
                        assetImport.assetBundleVariant = ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                    }
                    else if (str.Contains(@"Res/Atlas") && (str.EndsWith(".tga") || str.EndsWith(".png")) && !str.Contains("always"))
                    {
                        AssetImporter assetImport = AssetImporter.GetAtPath(str);
                        string extension = str.Substring(str.LastIndexOf("."));
                        //Debugger.LogError(extension);
                        //string assetName = str.Substring(str.LastIndexOf(@"/"));
                        //Debugger.LogError(assetName);
                        string assetBundleName = str.Replace(@"Assets/Res/", string.Empty);
                        //assetBundleName = assetBundleName.Replace(assetName, string.Empty);
                        assetBundleName = assetBundleName.Replace(extension, string.Empty);
                        assetBundleName = assetBundleName.Replace(@"Atlas", @"sprite");
                        string[] splits = assetBundleName.ToArray('/');
                        if (splits.Length <= 2)
                        {
                            assetImport.assetBundleName = assetBundleName;
                            assetImport.assetBundleVariant = ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                        }
                        else
                            assetImport.assetBundleName = string.Empty;
                    }
                    else if (str.Contains(@"Res/Resources/effects") && (/*str.EndsWith(".FBX") || str.EndsWith(".fbx") || */str.EndsWith(".prefab") /*|| str.EndsWith(".mat")
                        || str.EndsWith(".png") || str.EndsWith(".tga") || str.EndsWith(".anim") || str.EndsWith(".controller")*/))
                    {
                        AssetImporter assetImport = AssetImporter.GetAtPath(str);
                        //Debugger.Log(str);
                        string extension = str.Substring(str.LastIndexOf("."));
                        //Debugger.LogError(extension);
                        string assetBundleName = str.Replace(@"Assets/Res/", string.Empty);
                        assetBundleName = assetBundleName.Replace(extension, string.Empty);
                        assetBundleName = assetBundleName.Replace(@"Resources/", string.Empty);
                        //Debugger.Log(assetBundleName);
                        assetImport.assetBundleName = assetBundleName;
                        assetImport.assetBundleVariant = ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                        //assetImport.assetBundleName = string.Empty;
                    }
                    else if (str.Contains(@"Res/Resources/fonts") && (str.EndsWith(".fontsettings") || str.EndsWith(".ttf") || str.EndsWith(".TTF")))
                    {
                        AssetImporter assetImport = AssetImporter.GetAtPath(str);
                        string extension = str.Substring(str.LastIndexOf("."));
                        //Debugger.LogError(extension);
                        string assetBundleName = str.Replace(@"Assets/Res/", string.Empty);
                        assetBundleName = assetBundleName.Replace(extension, string.Empty);
                        assetBundleName = assetBundleName.Replace(@"Resources/", string.Empty);
                        //Debugger.Log(assetBundleName);
                        assetImport.assetBundleName = assetBundleName;
                        assetImport.assetBundleVariant = ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                    }
                    else if (str.Contains(@"Res/Resources/ui_texture") && (str.EndsWith(".png") || str.EndsWith("jpg")) && !str.Contains(@"ui_textures/loading_textures"))
                    {
                        AssetImporter assetImport = AssetImporter.GetAtPath(str);
                        string extension = str.Substring(str.LastIndexOf("."));
                        //Debugger.LogError(extension);
                        string assetBundleName = str.Replace(@"Assets/Res/", string.Empty);
                        assetBundleName = assetBundleName.Replace(extension, string.Empty);
                        assetBundleName = assetBundleName.Replace(@"Resources/", string.Empty);
                        //Debugger.Log(assetBundleName);
                        assetImport.assetBundleName = assetBundleName;
                        assetImport.assetBundleVariant = ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                    }
                    else if (str.Contains(@"Res/Resources/material") && str.EndsWith(".mat"))
                    {
                        AssetImporter assetImport = AssetImporter.GetAtPath(str);
                        string extension = str.Substring(str.LastIndexOf("."));
                        //Debugger.LogError(extension);
                        string assetBundleName = str.Replace(@"Assets/Res/", string.Empty);
                        assetBundleName = assetBundleName.Replace(extension, string.Empty);
                        assetBundleName = assetBundleName.Replace(@"Resources/", string.Empty);
                        //Debugger.Log(assetBundleName);
                        assetImport.assetBundleName = assetBundleName;
                        assetImport.assetBundleVariant = ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                    }
                    else if (str.Contains(@"Res/Resources/map") && str.EndsWith(".prefab"))
                    {
                        AssetImporter assetImport = AssetImporter.GetAtPath(str);
                        string extension = str.Substring(str.LastIndexOf("."));
                        //Debugger.LogError(extension);
                        string assetBundleName = str.Replace(@"Assets/Res/", string.Empty);
                        assetBundleName = assetBundleName.Replace(extension, string.Empty);
                        assetBundleName = assetBundleName.Replace(@"Resources/", string.Empty);
                        //Debugger.Log(assetBundleName);
                        assetImport.assetBundleName = assetBundleName;
                        assetImport.assetBundleVariant = ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                    }
                    else if (str.Contains(@"Res/Resources/audio") && (str.EndsWith(".ogg") || str.EndsWith(".wav") || str.EndsWith(".mp3")))
                    {
                        AssetImporter assetImport = AssetImporter.GetAtPath(str);
                        string extension = str.Substring(str.LastIndexOf("."));
                        //Debugger.LogError(extension);
                        string assetBundleName = str.Replace(@"Assets/Res/", string.Empty);
                        assetBundleName = assetBundleName.Replace(extension, string.Empty);
                        assetBundleName = assetBundleName.Replace(@"Resources/", string.Empty);
                        //Debugger.Log(assetBundleName);
                        assetImport.assetBundleName = assetBundleName;
                        assetImport.assetBundleVariant = ResMgr.ResUtil.ASSET_BUNDLE_SUFFIX.Replace(".", string.Empty);
                    }
                    else
                    {
                        AssetImporter assetImport = AssetImporter.GetAtPath(str);
                        assetImport.assetBundleName = string.Empty;
                    }
                }
                else
                {
                    AssetImporter assetImport = AssetImporter.GetAtPath(str);
                    if (assetImport)
                        assetImport.assetBundleName = string.Empty;
                }
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            //foreach (var str in deletedAssets)
            //{
            //    Debug.Log("Deleted Asset: " + str);
            //}

            //for (var i = 0; i < movedAssets.Length; i++)
            //{
            //    Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            //}
        }
    }
}