using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using Logic.Shaders;
using System.Collections.Generic;
namespace Common.PreImprot.Editor
{
    public class PreImportFBX : AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            string path = assetPath;
            ModelImporter modelImporter = (ModelImporter)assetImporter;
            #region character import
            if (path.Contains("hero") || path.Contains("player") || path.Contains("pet") || path.Contains("Model/ui") || path.Contains("Resources/ui"))
            {
                modelImporter.animationType = ModelImporterAnimationType.Generic;
                //modelImporter.globalScale = 0.01f;
                if (!assetPath.Contains("@"))
                {
                    modelImporter.optimizeGameObjects = false;
                    modelImporter.optimizeMesh = true;
                    modelImporter.importAnimation = false;
                    modelImporter.importBlendShapes = false;
                    modelImporter.isReadable = false;
                    modelImporter.meshCompression = ModelImporterMeshCompression.High;
                    modelImporter.tangentImportMode = ModelImporterTangentSpaceMode.None;
                    /* 暂不开启
                    //create rim lighting mat
                    FileInfo fileInfo = new FileInfo(path);
                    string fileName = fileInfo.Name.Replace(".FBX", string.Empty);
                    string dir = fileInfo.DirectoryName;
                    string unityDir = dir.Substring(dir.IndexOf(@"Assets"));
                    //Debugger.Log(unityDir);
                    string matPath = unityDir + "\\Materials\\" + fileName + "_Rim_Lighting.mat";
                    //Debugger.Log(matPath);

                    Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                    Shader shader = Shader.Find(ShadersUtil.Custom_Rim_Lighting);
                    if (mat)
                    {
                        mat.shader = shader;
                        mat.name = fileName + "_Rim_Lighting";
                    }
                    else
                    {
                        mat = new Material(shader);
                        mat.name = fileName + "_Rim_Lighting";
                        AssetDatabase.CreateAsset(mat, matPath);
                    }
                    if (mat)
                    {
                        string infoTexturePath = unityDir + "\\" + fileName + "_info";
                        //Debugger.Log(infoTexturePath);
                        Texture2D infoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(infoTexturePath + ".tga");
                        if (!infoTexture)
                            infoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(infoTexturePath + ".png");
                        //Debugger.Log((infoTexture == null).ToString());
                        if (infoTexture)
                        {
                            mat.SetTexture("_InfoTex", infoTexture);
                        }
                    }*/
                }
                else
                {
                    modelImporter.isReadable = false;
                    modelImporter.importBlendShapes = false;
                    modelImporter.importMaterials = false;
                    modelImporter.importAnimation = true;
					if(path.Contains("hero") || path.Contains("player") || path.Contains("pet"))
					{						
						if (path.Contains("Idle") || path.Contains("Victory"))
						{
							modelImporter.animationCompression = ModelImporterAnimationCompression.Optimal;
							modelImporter.animationRotationError = 0.2f;
							modelImporter.animationPositionError = 0.2f;
							modelImporter.animationScaleError = 0.5f;
						}
					}
					FileInfo fileInfo = new FileInfo(path);
                    string fileName = fileInfo.Name.Replace("_t@", "@");
                    fileName = fileName.Substring(0, fileName.IndexOf(@"@"));
                    //Debugger.Log(fileName);
                    string dir = fileInfo.DirectoryName;
                    dir = dir.Substring(dir.IndexOf(@"Assets"));
                    string tposeFileName = string.Empty;
                    foreach (var file in fileInfo.Directory.GetFiles("*.FBX"))
                    {
                        if (!file.Name.Contains("@") && fileInfo.Name.Contains(fileName))
                        {
                            tposeFileName = file.Name;
                            break;
                        }
                    }
                    Debugger.Log(tposeFileName);
                    if (string.IsNullOrEmpty(tposeFileName))
                        return;
                    string fullName = dir + "\\" + tposeFileName;
                    fullName = fullName.Replace(Path.DirectorySeparatorChar, '/');
                    Debugger.Log(fullName);

                    GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(fullName);
                    if (model)
                    {
                        Animator animator = model.GetComponent<Animator>();
                        if (animator == null)
                        {
                            Debugger.LogError(string.Format("can not find animator from {0}", fullName));
                        }
                        else
                        {
                            Avatar avatar = animator.avatar;
                            //Debugger.Log(avatar == null);
                            modelImporter.sourceAvatar = avatar;
                        }
                    }
                    else
                        Debugger.LogError(string.Format("can not find model from {0}", fullName));


                    //if (path.Contains("@Idle") || path.Contains("@Run"))
                    //{
                    //    Debugger.Log(path + "  " + modelImporter.clipAnimations.Length + "   " + modelImporter.defaultClipAnimations.Length);
                    //    foreach (var c in modelImporter.clipAnimations)
                    //    {
                    //        //Debugger.Log(c.loopTime + "   " + c.loop);
                    //        c.wrapMode = WrapMode.Loop;                           
                    //        c.loopTime = true;
                    //        c.loop = true;
                    //        //Debugger.Log(c.loopTime + "   " + c.loop);
                    //    }
                    //}
                    //modelImporter.animationWrapMode = WrapMode.Loop;
                }
                Debugger.Log("import model:" + assetPath);
            }
            else if (path.Contains("Res/Resources/effects"))
            {
                modelImporter.animationType = ModelImporterAnimationType.None;
                //modelImporter.globalScale = 0.01f;
                if (!assetPath.Contains("@"))
                {
                    modelImporter.optimizeGameObjects = false;
                    modelImporter.optimizeMesh = true;
                    modelImporter.importAnimation = false;
                    modelImporter.importBlendShapes = false;
                    modelImporter.isReadable = false;
                    modelImporter.importMaterials = false;
                    modelImporter.tangentImportMode = ModelImporterTangentSpaceMode.None;
                    modelImporter.meshCompression = ModelImporterMeshCompression.High;
                }
            }
            #endregion
            //#region shadow
            //if (path.Contains("shadow"))
            //{
            //    modelImporter.animationType = ModelImporterAnimationType.None;
            //    modelImporter.optimizeGameObjects = false;
            //    modelImporter.optimizeMesh = true;
            //    modelImporter.importAnimation = false;
            //    modelImporter.importMaterials = true;
            //}
            //#endregion
        }

        void OnPostprocessModel(GameObject go)
        {
            string path = assetPath;
            if (path.Contains("hero") || path.Contains("player") || path.Contains("pet") || path.Contains("Model/ui") || path.Contains("Resources/ui"))
            {
                if (go.name.Contains("shadow"))
                {
                    Shader shader = Shader.Find(ShadersUtil.Unlit_Transparent);
                    foreach (var r in go.GetComponentsInChildren<Renderer>())
                    {
                        foreach (var material in r.sharedMaterials)
                        {
                            material.shader = shader;
                        }
                    }
                }
                else if (!go.name.Contains("@"))
                {
                    if (go.name.Contains("btn_book")) return;
                    Shader shader = Shader.Find(ShadersUtil.Custom_Rim_Lighting_Surf);
                    foreach (var r in go.GetComponentsInChildren<Renderer>())
                    {
                        foreach (var material in r.sharedMaterials)
                        {
                            material.shader = shader;
                            material.color = ShadersUtil.RIM_MAIN_COLOR;
                            material.SetColor("_RimColor", ShadersUtil.RIM_COLOR);
                            material.SetFloat("_RimPower", 3f);
                            material.SetVector("_LightPos", new Vector4(-1, 1, 1, 1));
                            if (material.name.Contains("_body"))
                            {
                                material.SetInt("_IsBody", 1);
                                material.EnableKeyword(ShadersUtil.BODY_ON);
                                material.DisableKeyword(ShadersUtil.BODY_OFF);
                            }
                            else
                            {
                                material.SetInt("_IsBody", 0);
                                material.EnableKeyword(ShadersUtil.BODY_OFF);
                                material.DisableKeyword(ShadersUtil.BODY_ON);
                            }
                            material.SetInt("_ClipPosition", 0);
                            material.EnableKeyword(ShadersUtil.CLIP_POSITION_OFF);
                            material.DisableKeyword(ShadersUtil.CLIP_POSITION_ON);
                            //material.SetInt("_Gloss", 0);
                            //material.EnableKeyword(ShadersUtil.GLOSS_OFF);
                            //material.DisableKeyword(ShadersUtil.GLOSS_ON);
                            //Texture reflTexture = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Res/Model/reflection.png");
                            //Debugger.Log(reflTexture.name);
                            //if (reflTexture)
                            //    material.SetTexture("_GlossTex", reflTexture);
                        }
                    }
                }
                //if (go.name.Contains("@Idle") || go.name.Contains("@Run"))
                //{
                //    Animation anim = go.GetComponent<Animation>();
                //    AnimationClip[] clips = AnimationUtility.GetAnimationClips(anim);
                //    //AnimationClip[] clips = AnimationUtility.GetAnimationClips(go);
                //    Debugger.Log(go.name + "  clips length:" + clips.Length.ToString());
                //    foreach (var c in clips)
                //    {
                //        c.wrapMode = WrapMode.Loop;
                //    }
                //}

                if (go.name.Contains("@"))
                {
                    Animation anim = go.GetComponent<Animation>();
                    AnimationClip clip = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;
                    string filename = go.name + ".FBX";
                    path = path.Substring(0, path.LastIndexOf("/") + 1);
                    //path = path.Replace(filename, "Animations");
                    //path += "/";
                    Debugger.Log(path + "   " + clip.name);
                    //DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    //if (!directoryInfo.Exists)
                    //    directoryInfo.Create();
                    DuplicateAnimationClip(path, clip, go.name);
                }
            }
        }

        void DuplicateAnimationClip(string path, AnimationClip sourceClip, string animName)
        {
            if (sourceClip != null)
            {
                AnimationClip tempClip = new AnimationClip();
                EditorUtility.CopySerialized(sourceClip, tempClip);

                string newPath = path + animName + ".anim";
				if ((animName.Contains("Idle") || animName.Contains("Victory") || animName.Contains("FloatGetHit") || animName.Contains("TumbleGetHit"))&&!path.Contains("Resources/ui"))
                {
                    EditorCurveBinding[] editorCurveBindings = AnimationUtility.GetCurveBindings(tempClip);
                    AnimationClipCurveData[] animationClipCurveDatas = AnimationUtility.GetAllCurves(tempClip);
                    AnimationClip newClip = new AnimationClip();
                    for (int i = 0, length = editorCurveBindings.Length; i < length; i++)
                    {
                        AnimationClipCurveData animationClipCurveData = animationClipCurveDatas[i];
                        EditorCurveBinding editorCurveBinding = editorCurveBindings[i];
                        //Debugger.Log(editorCurveBinding.path);
                        AnimationClipCurveData accd = new AnimationClipCurveData(editorCurveBinding);
                        AnimationCurve animationCurve = animationClipCurveData.curve;
                        AnimationCurve newCurve = new AnimationCurve();
                        for (int j = 0, jLength = animationCurve.keys.Length; j < jLength; j++)
                        {
                            if (j % 2 != 0) continue;
                            Keyframe key = animationCurve.keys[j];
                            newCurve.AddKey(key);
                        }
                        AnimationUtility.SetEditorCurve(newClip, editorCurveBinding, newCurve);
                    }
                    if (animName.Contains("Idle"))
                    {
                        AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(newClip);
                        animationClipSettings.loopTime = true;
                        AnimationUtility.SetAnimationClipSettings(newClip, animationClipSettings);
                    }
                    AssetDatabase.CreateAsset(newClip, newPath);
                }
                else
                {
                    if (animName.Contains("Run") || animName.Contains("Stun"))
                    {
                        AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(tempClip);
                        animationClipSettings.loopTime = true;
                        AnimationUtility.SetAnimationClipSettings(tempClip, animationClipSettings);
                    }
                    AssetDatabase.CreateAsset(tempClip, newPath);
                }
                Debugger.Log("import clip:" + newPath);
            }
        }
    }
}