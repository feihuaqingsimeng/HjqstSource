using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
namespace Common.PreImprot.Editor
{
    public class PreImportTexture : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (assetPath.Contains("Res/Atlas"))
            {
                string atlasName = new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name;
                TextureImporter textureImporter = assetImporter as TextureImporter;
                //string assetName = assetPath.Substring(assetPath.LastIndexOf("/") + 1);
                //assetName = assetName.Replace(".png", string.Empty);
                if (atlasName == "Atlas")
                {
                    //if (assetPath.Contains("always") || assetPath.EndsWith("_a.png"))
                    //{
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.spritePackingTag = string.Empty;
                    textureImporter.mipmapEnabled = false;
                    textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                    textureImporter.filterMode = FilterMode.Bilinear;
                    textureImporter.maxTextureSize = 2048;
                    textureImporter.isReadable = false;
                    if (assetPath.Contains("main_ui")
					    || assetPath.Contains("introduction_1")
					    || assetPath.Contains("introduction_2")
					    || assetPath.Contains("introduction_3")
					    || assetPath.Contains("introduction_4")
					    || assetPath.Contains("introduction_5"))
                    {
                        textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                        textureImporter.ClearPlatformTextureSettings("iPhone");
                        textureImporter.ClearPlatformTextureSettings("Android");
                        textureImporter.ClearPlatformTextureSettings("Standalone");
                        textureImporter.SetPlatformTextureSettings("Android", 2048, TextureImporterFormat.RGBA32, 100, false);
                        textureImporter.SetPlatformTextureSettings("iPhone", 2048, TextureImporterFormat.AutomaticTruecolor, 100, false);
                    }
                    else
                    {
                        textureImporter.ClearPlatformTextureSettings("iPhone");
                        textureImporter.ClearPlatformTextureSettings("Android");
                        textureImporter.ClearPlatformTextureSettings("Standalone");
                        textureImporter.SetPlatformTextureSettings("Android", 2048, TextureImporterFormat.ETC_RGB4, 100, false);
                        textureImporter.SetPlatformTextureSettings("iPhone", 2048, TextureImporterFormat.PVRTC_RGB4, 100, false);
                    }
                    //}
                    //else
                    //{
                    //    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    //    Color newColor = new Color(0f, 0f, 0f, 1f);
                    //    bool hasAlpha = false;
                    //    for (int i = 0; i < texture.width; ++i)
                    //    {
                    //        for (int j = 0; j < texture.height; ++j)
                    //        {
                    //            newColor = texture.GetPixel(i, j);
                    //            if (newColor.a <= 0)
                    //            {
                    //                Debugger.Log(newColor.a.ToString());
                    //                hasAlpha = true;
                    //                break;
                    //            }
                    //        }
                    //        if (hasAlpha)
                    //            break;
                    //    }
                    //    if (hasAlpha)
                    //        return;
                    //    textureImporter.textureType = TextureImporterType.Sprite;
                    //    textureImporter.spritePackingTag = string.Empty;
                    //    textureImporter.mipmapEnabled = false;
                    //    textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                    //    textureImporter.filterMode = FilterMode.Bilinear;
                    //    textureImporter.maxTextureSize = 2048;
                    //    textureImporter.isReadable = false;
                    //    //textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;

                    //    textureImporter.ClearPlatformTextureSettings("iPhone");
                    //    textureImporter.ClearPlatformTextureSettings("Android");
                    //    textureImporter.ClearPlatformTextureSettings("Standalone");
                    //    textureImporter.SetPlatformTextureSettings("Android", 2048, TextureImporterFormat.ETC_RGB4, 100, false);
                    //    textureImporter.SetPlatformTextureSettings("iPhone", 2048, TextureImporterFormat.PVRTC_RGB4, 100, false);
                    //}
                }
                else
                {
                    if (assetPath.Contains("Res/Atlas/map"))
                    {
                        /*if (assetPath.ToArray('_').Length > 2)
                        {
                            textureImporter.spritePackingTag = atlasName;
                            textureImporter.spriteImportMode = SpriteImportMode.Single;
                        }
                        else
                        {*/
                        textureImporter.spritePackingTag = string.Empty;
                        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                        //}
                        textureImporter.textureType = TextureImporterType.Sprite;
                        textureImporter.mipmapEnabled = false;
                        textureImporter.filterMode = FilterMode.Bilinear;
                        textureImporter.maxTextureSize = 2048;
                        //textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                        textureImporter.isReadable = false;
                        textureImporter.ClearPlatformTextureSettings("iPhone");
                        textureImporter.ClearPlatformTextureSettings("Android");
                        textureImporter.ClearPlatformTextureSettings("Standalone");
                        textureImporter.SetPlatformTextureSettings("Android", 2048, TextureImporterFormat.ETC_RGB4, 100, false);
                        textureImporter.SetPlatformTextureSettings("iPhone", 2048, TextureImporterFormat.PVRTC_RGB4, 100, false);
                    }
                    else
                    {
                        //Debugger.Log(assetPath);
                        //string AtlasName = new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name;
                        //TextureImporter textureImporter = (TextureImporter)assetImporter;
                        textureImporter.textureType = TextureImporterType.Sprite;
                        textureImporter.spritePackingTag = atlasName;
                        textureImporter.mipmapEnabled = false;
                        textureImporter.spriteImportMode = SpriteImportMode.Single;
                        textureImporter.filterMode = FilterMode.Bilinear;
                        textureImporter.maxTextureSize = 2048;
                        textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                    }
                }
            }
            else if (assetPath.Contains("Res/Model") || assetPath.Contains("Res/Resources/texture"))
            {
                //if (assetPath.Contains("shadow"))
                //    return;
                //Debugger.Log(assetPath);
                if (Application.isPlaying) return;
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                textureImporter.textureType = TextureImporterType.Image;
                textureImporter.grayscaleToAlpha = false;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                textureImporter.filterMode = FilterMode.Bilinear;
                textureImporter.mipmapEnabled = false;
                textureImporter.anisoLevel = 0;

                textureImporter.ClearPlatformTextureSettings("iPhone");
                textureImporter.ClearPlatformTextureSettings("Android");
                textureImporter.ClearPlatformTextureSettings("Standalone");
                //Debugger.Log(assetPath);
                if (assetPath.EndsWith("shadow.tga") || assetPath.EndsWith("shadow.png"))
                {
                    textureImporter.SetPlatformTextureSettings("Android", 32, TextureImporterFormat.RGBA16);
                    textureImporter.SetPlatformTextureSettings("iPhone", 32, TextureImporterFormat.PVRTC_RGBA4, 100, false);
                }
                //else if (assetPath.EndsWith("_alpha.tga") || assetPath.EndsWith("_alpha.png"))
                //{
                //    textureImporter.SetPlatformTextureSettings("Android", 256, TextureImporterFormat.RGBA32);
                //    textureImporter.SetPlatformTextureSettings("iPhone", 256, TextureImporterFormat.PVRTC_RGBA4, 100, false);
                //}
                //else if (assetPath.EndsWith("_alpha_512.tga") || assetPath.EndsWith("_alpha_512.png"))
                //{
                //    textureImporter.SetPlatformTextureSettings("Android", 512, TextureImporterFormat.RGBA32);
                //    textureImporter.SetPlatformTextureSettings("iPhone", 512, TextureImporterFormat.PVRTC_RGBA4, 100, false);
                //}
                else if (assetPath.EndsWith("_512.tga") || assetPath.EndsWith("_512.png"))
                {
                    textureImporter.SetPlatformTextureSettings("Android", 512, TextureImporterFormat.ETC_RGB4, 100, false);
                    textureImporter.SetPlatformTextureSettings("iPhone", 512, TextureImporterFormat.PVRTC_RGB4, 100, false);
                }
                //else if (assetPath.EndsWith("_refl.tga"))
                //{
                //    textureImporter.SetPlatformTextureSettings("Android", 256, TextureImporterFormat.RGBA32, 100, false);
                //    textureImporter.SetPlatformTextureSettings("iPhone", 256, TextureImporterFormat.PVRTC_RGBA4, 100, false);
                //}
                //else if (assetPath.EndsWith("reflection.tga") || assetPath.EndsWith("reflection.png"))
                //{
                //    textureImporter.SetPlatformTextureSettings("Android", 256, TextureImporterFormat.ETC_RGB4, 100, false);
                //    textureImporter.SetPlatformTextureSettings("iPhone", 256, TextureImporterFormat.PVRTC_RGB4, 100, false);
                //}
                else if (assetPath.EndsWith("_info.tga") || assetPath.EndsWith("_info.png"))
                {
                    textureImporter.SetPlatformTextureSettings("Android", 128, TextureImporterFormat.RGB16);
                    textureImporter.SetPlatformTextureSettings("iPhone", 128, TextureImporterFormat.RGB16);
                }
                else
                {
                    textureImporter.SetPlatformTextureSettings("Android", 256, TextureImporterFormat.ETC_RGB4, 100, false);
                    textureImporter.SetPlatformTextureSettings("iPhone", 256, TextureImporterFormat.PVRTC_RGB4, 100, false);
                }
            }
            else if (assetPath.Contains("Res/Resources/effects"))
            {
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                string textureName = assetPath.Replace(".tga", string.Empty);
                textureName = textureName.Replace(".png", string.Empty);
                bool isAphla = false, turecolor = false;
                if (textureName.EndsWith("_a"))
                {
                    isAphla = true;
                    textureName = assetPath.Replace("_a.tga", string.Empty);
                    textureName = textureName.Replace("_a.png", string.Empty);
                }
                else if (textureName.EndsWith("_t"))
                {
                    turecolor = true;
                    textureName = assetPath.Replace("_t.tga", string.Empty);
                    textureName = textureName.Replace("_t.png", string.Empty);
                }
                string sizeStr = textureName.Substring(textureName.LastIndexOf('_') + 1);
                //Debugger.Log(sizeStr.ToString());
                int size = 0;
                int.TryParse(sizeStr, out size);
                //Debugger.Log(size.ToString());

                bool isPowerOf2 = false;
                switch (size)
                {
                    case 16:
                    case 32:
                    case 64:
                    case 128:
                    case 256:
                    case 512:
                        isPowerOf2 = true;
                        break;
                    default:
                        isPowerOf2 = false;
                        break;
                }
                if (isPowerOf2)
                {
                    if (turecolor)
                    {
                        textureImporter.SetPlatformTextureSettings("Android", size, TextureImporterFormat.RGBA32, 100, false);
                        textureImporter.SetPlatformTextureSettings("iPhone", size, TextureImporterFormat.PVRTC_RGBA4, 100, false);
                    }
                    else if (isAphla)
                    {
                        textureImporter.SetPlatformTextureSettings("Android", size, TextureImporterFormat.RGB16);
                        textureImporter.SetPlatformTextureSettings("iPhone", size, TextureImporterFormat.RGB16);
                    }
                    else
                    {
                        textureImporter.SetPlatformTextureSettings("Android", size, TextureImporterFormat.ETC_RGB4, 100, false);
                        textureImporter.SetPlatformTextureSettings("iPhone", size, TextureImporterFormat.PVRTC_RGB4, 100, false);
                    }
                }
                else
                {
                    Debugger.LogError("{0}'s size is not power of 2 .", assetPath);
                    //if (isAphla)
                    //{
                    //    textureImporter.SetPlatformTextureSettings("Android", size, TextureImporterFormat.RGB16);
                    //    textureImporter.SetPlatformTextureSettings("iPhone", size, TextureImporterFormat.RGB16);
                    //}
                    //else
                    //{
                    //    textureImporter.SetPlatformTextureSettings("Android", 128, TextureImporterFormat.ETC_RGB4, 100, false);
                    //    textureImporter.SetPlatformTextureSettings("iPhone", 128, TextureImporterFormat.PVRTC_RGB4, 100, false);
                    //}
                }
            }
            else if (assetPath.Contains("Res/Resources/launch"))
            {
                if (assetPath.Contains("duwan") || assetPath.Contains("shunwang")) return;

                TextureImporter textureImporter = (TextureImporter)assetImporter;
                textureImporter.textureType = TextureImporterType.Image;
                textureImporter.grayscaleToAlpha = false;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                textureImporter.filterMode = FilterMode.Trilinear;
                textureImporter.mipmapEnabled = false;
                textureImporter.anisoLevel = 0;

                textureImporter.ClearPlatformTextureSettings("iPhone");
                textureImporter.ClearPlatformTextureSettings("Android");
                textureImporter.ClearPlatformTextureSettings("Standalone");

                textureImporter.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.ETC_RGB4, 100, false);
                textureImporter.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.PVRTC_RGBA4, 100, false);
            }
            else if (assetPath.Contains("Res/Resources/ui_textures"))
            {
                //if (assetPath.Contains("loading_07")) return;
                return;
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                textureImporter.textureType = TextureImporterType.Image;
                textureImporter.grayscaleToAlpha = false;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                textureImporter.filterMode = FilterMode.Trilinear;
                textureImporter.mipmapEnabled = false;
                textureImporter.anisoLevel = 0;

                textureImporter.ClearPlatformTextureSettings("iPhone");
                textureImporter.ClearPlatformTextureSettings("Android");
                textureImporter.ClearPlatformTextureSettings("Standalone");

                textureImporter.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.ETC_RGB4, 100, false);
                textureImporter.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.PVRTC_RGBA4, 100, false);
            }
        }
    }
}