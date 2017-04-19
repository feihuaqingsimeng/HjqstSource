using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Logic.Character;
using Common.Util;
using Logic.Enums;
using Logic.Shaders;
namespace Common.Tools.Editor
{
    public static class CharacterImporter
    {
        static string[] suffixes = new string[] { "_t", "_t1", "_t2" };

        [MenuItem("Assets/Import Models", false, 0)]
        [MenuItem("Tools/Import Models", false, 300)]
        public static void ImportModels()
        {
            DirectoryInfo heroDI = new DirectoryInfo("Assets/Res/Model/hero");
            DirectoryInfo roleDI = new DirectoryInfo("Assets/Res/Model/player");
            ImportModels(heroDI, true);
            ImportModels(roleDI, true);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Import Selected Models", false, 0)]
        [MenuItem("Tools/Import Selected Models %e", false, 300)]
        public static void ImportSelectedModels()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                Debugger.Log("please select a model");
                return;
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            DirectoryInfo dir = new DirectoryInfo(path);
            ImportModels(dir);

            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Import Motions", false, 0)]
        [MenuItem("Tools/Import Motions", false, 300)]
        public static void ImportMotions()
        {
            DirectoryInfo heroDI = new DirectoryInfo("Assets/Res/Model/hero");
            DirectoryInfo roleDI = new DirectoryInfo("Assets/Res/Model/player");
            DirectoryInfo petDI = new DirectoryInfo("Assets/Res/Model/pet");
            ImportMotions(heroDI);
            ImportMotions(roleDI);
            ImportMotions(petDI);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Import Selected Motions", false, 0)]
        [MenuItem("Tools/Import Selected Motions", false, 300)]
        public static void ImportSelectedMotions()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                Debugger.Log("please select a model");
                return;
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            DirectoryInfo dir = new DirectoryInfo(path);
            ImportMotions(dir);

            AssetDatabase.SaveAssets();
        }

        public static void ImportModels(DirectoryInfo di, bool deleteAll = false)
        {
            foreach (FileInfo fi in di.GetFiles("*.FBX"))
            {
                if (!fi.Name.Contains("@") && !fi.Name.Contains(@"_wp_"))
                {
                    string fullName = fi.FullName.Replace(Path.DirectorySeparatorChar, '/');
                    Object selectObj = AssetDatabase.LoadAssetAtPath("Assets" + fullName.Substring(Application.dataPath.Length), typeof(Object));
                    string fiName = fi.Name.Replace(".FBX", "");
                    Debugger.Log("import name:" + di.FullName);
                    if (di.FullName.Contains(@"Model\player"))
                    {
                        string path = Application.dataPath + "/Res/Resources/character/player";
                        if (Directory.Exists(path) && deleteAll)
                            Directory.Delete(path, true);
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        string prefabPath = AssetDatabase.GetAssetPath(selectObj);
                        foreach (var suffix in suffixes)
                        {
                            bool existTransform = ExistTransform(selectObj, prefabPath, suffix);
                            if (existTransform)
                            {
                                CreateCharacter(selectObj, "Assets/Res/Resources/character/player/" + fiName + suffix, suffix, ImportCharacterType.Player);
                            }
                        }
                        CreateCharacter(selectObj, "Assets/Res/Resources/character/player/" + fiName, string.Empty, ImportCharacterType.Player);
                    }
                    else if (di.FullName.Contains(@"Model\hero"))
                    {
                        string path = Application.dataPath + "/Res/Resources/character/hero";
                        if (Directory.Exists(path) && deleteAll)
                            Directory.Delete(path, true);
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        ImportCharacterType importCharacterType;
                        if (fiName.StartsWith("m_"))
                            importCharacterType = ImportCharacterType.Monster;
                        else
                            importCharacterType = ImportCharacterType.Hero;
                        string prefabPath = AssetDatabase.GetAssetPath(selectObj);
                        foreach (var suffix in suffixes)
                        {
                            bool existTransform = ExistTransform(selectObj, prefabPath, suffix);
                            if (existTransform)
                            {
                                CreateCharacter(selectObj, "Assets/Res/Resources/character/hero/" + fiName + suffix, suffix, importCharacterType);
                            }
                        }
                        CreateCharacter(selectObj, "Assets/Res/Resources/character/hero/" + fiName, string.Empty, importCharacterType);
                    }
                    else if (fi.FullName.Contains(@"Model\pet"))
                    {
                        string path = Application.dataPath + "/Res/Resources/character/pet";
                        if (Directory.Exists(path) && deleteAll)
                            Directory.Delete(path, true);
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        CreatePet(selectObj, "Assets/Res/Resources/character/pet/" + fiName);
                    }
                }
            }

            foreach (DirectoryInfo child in di.GetDirectories())
                ImportModels(child);
        }

        public static void ImportMotions(DirectoryInfo di)
        {
            foreach (FileInfo fi in di.GetFiles("*.FBX"))
            {
                if (!fi.Name.Contains("@") && !fi.Name.Contains(@"_wp_"))
                {
                    string fullName = fi.FullName.Replace(Path.DirectorySeparatorChar, '/');
                    Object selectObj = AssetDatabase.LoadAssetAtPath("Assets" + fullName.Substring(Application.dataPath.Length), typeof(Object));
                    string fiName = fi.Name.Replace(".FBX", "");
                    Debugger.Log("import name:" + di.FullName);
                    Debugger.Log(fiName);
                    string racPath = string.Empty;
                    if (di.FullName.Contains(@"Model\player"))
                    {
                        //string path = Application.dataPath + "/Res/Resources/character/player";
                        racPath = "Assets/Res/Resources/animcontroller/player/" + fiName;
                        //if (Directory.Exists(path))
                        //    Directory.Delete(path, true);
                        //if (!Directory.Exists(path))
                        //    Directory.CreateDirectory(path);
                        //CreateCharacter(selectObj, "Assets/Res/Resources/character/player/" + fiName, ImportCharacterType.Player);
                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Res/Resources/character/player/" + fiName + ".prefab");
                        //SetAnimator(go, selectObj, racPath, ImportCharacterType.Transform);
                        SetAnimator(go, selectObj, racPath, ImportCharacterType.Player);
                        string prefabPath = AssetDatabase.GetAssetPath(selectObj);
                        foreach (var suffix in suffixes)
                        {
                            bool existTransform = ExistTransform(selectObj, prefabPath, suffix);
                            if (existTransform)
                            {
                                racPath = "Assets/Res/Resources/animcontroller/player/" + fiName + suffix;
                                GameObject go_t = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Res/Resources/character/player/" + fiName + suffix + ".prefab");
                                SetAnimator(go_t, selectObj, racPath, ImportCharacterType.Player);
                            }
                        }
                    }
                    else if (di.FullName.Contains(@"Model\hero"))
                    {
                        //string path = Application.dataPath + "/Res/Resources/character/hero";
                        racPath = "Assets/Res/Resources/animcontroller/hero/" + fiName;
                        //if (Directory.Exists(path))
                        //    Directory.Delete(path, true);
                        //if (!Directory.Exists(path))
                        //    Directory.CreateDirectory(path);
                        ImportCharacterType importCharacterType;
                        if (fiName.StartsWith("m_"))
                            importCharacterType = ImportCharacterType.Monster;
                        else
                            importCharacterType = ImportCharacterType.Hero;
                        //CreateCharacter(selectObj, "Assets/Res/Resources/character/hero/" + fiName, importCharacterType);
                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Res/Resources/character/hero/" + fiName + ".prefab");
                        Debugger.Log("Assets/Res/Resources/character/hero/" + fiName);
                        Debugger.Log(go.name);
                        //SetAnimator(go, selectObj, racPath, ImportCharacterType.Transform);
                        SetAnimator(go, selectObj, racPath, importCharacterType);

                        string prefabPath = AssetDatabase.GetAssetPath(selectObj);
                        foreach (var suffix in suffixes)
                        {
                            bool existTransform = ExistTransform(selectObj, prefabPath, suffix);
                            if (existTransform)
                            {
                                racPath = "Assets/Res/Resources/animcontroller/hero/" + fiName + suffix;
                                GameObject go_t = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Res/Resources/character/hero/" + fiName + suffix + ".prefab");
                                SetAnimator(go_t, selectObj, racPath, importCharacterType);
                            }
                        }
                    }
                }
            }

            foreach (DirectoryInfo child in di.GetDirectories())
                ImportMotions(child);
        }

        public static GameObject CreateWeapon(Object selectObj, string outputFile)
        {
            GameObject parent = new GameObject();
            GameObject go = GameObject.Instantiate(selectObj) as GameObject;
            string name = go.name.Substring(0, go.name.IndexOf('('));//去掉(clone)
            parent.name = name;
            Transform hairBone = TransformUtil.Find("Bip001 Prop1_Hand", go.transform);
            hairBone.localPosition = Vector3.zero;
            hairBone.localRotation = Quaternion.identity;
            hairBone.localScale = Vector3.one;
            //SetAnimator(go, selectObj, outputFile);
            go.transform.SetParent(parent.transform, false);

            Animation animation = go.GetComponent<Animation>();
            if (animation)
                UnityEngine.Object.DestroyImmediate(animation);

            go.name = parent.name;

            foreach (var r in parent.GetComponentsInChildren<Renderer>(true))
            {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                r.receiveShadows = false;
                r.useLightProbes = false;
                r.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            }

            TransformUtil.SwitchLayer(parent.transform, (int)LayerType.Fight);
            GameObject prefab = CreateOrReplacePrefab(parent, outputFile) as GameObject;
            Object.DestroyImmediate(parent);
            Object.DestroyImmediate(go);
            return prefab;
        }

        public static GameObject CreateHair(Object selectObj, string outputFile)
        {
            GameObject parent = new GameObject();
            GameObject go = GameObject.Instantiate(selectObj) as GameObject;
            string name = go.name.Substring(0, go.name.IndexOf('('));//去掉(clone)
            parent.name = name;
            Transform hairBone = TransformUtil.Find("Bip001 Hair", go.transform);
            if (hairBone == null)
            {
                Object.DestroyImmediate(parent);
                Object.DestroyImmediate(go);
                return null;
                //hairBone = TransformUtil.Find("Bip001 Head", go.transform);//临时使用
            }
            hairBone.localPosition = Vector3.zero;
            hairBone.localRotation = Quaternion.identity;
            hairBone.localScale = Vector3.one;
            //SetAnimator(go, selectObj, outputFile);
            go.transform.SetParent(parent.transform, false);

            string racPath = "Assets/Res/Resources/animcontroller/hair/" + name + ".controller";
            Animator animator = go.GetComponent<Animator>();
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.updateMode = AnimatorUpdateMode.Normal;
            Debugger.Log(racPath);
            if (!string.IsNullOrEmpty(racPath))
            {
                RuntimeAnimatorController rac = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(racPath);
                animator.runtimeAnimatorController = rac;
            }
            go.name = parent.name;

            foreach (var r in parent.GetComponentsInChildren<Renderer>(true))
            {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                r.receiveShadows = false;
                r.useLightProbes = false;
                r.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            }

            TransformUtil.SwitchLayer(parent.transform, (int)LayerType.Fight);
            GameObject prefab = CreateOrReplacePrefab(parent, outputFile) as GameObject;
            Object.DestroyImmediate(parent);
            Object.DestroyImmediate(go);
            return prefab;
        }

        public static GameObject CreatePet(Object selectObj, string outputFile)
        {
            GameObject parent = new GameObject();
            GameObject go = GameObject.Instantiate(selectObj) as GameObject;
            string name = go.name.Substring(0, go.name.IndexOf('('));//去掉(clone)
            parent.name = name;
            Transform rootBone = TransformUtil.Find("Bip001", go.transform);
            if (rootBone == null)
            {
                Object.DestroyImmediate(parent);
                Object.DestroyImmediate(go);
                return null;
                //hairBone = TransformUtil.Find("Bip001 Head", go.transform);//临时使用
            }
            rootBone.localPosition = Vector3.zero;
            rootBone.localRotation = Quaternion.identity;
            rootBone.localScale = Vector3.one;
            //SetAnimator(go, selectObj, outputFile);
            go.transform.SetParent(parent.transform, false);

            PetEntity petEntity = parent.AddComponent<PetEntity>();
            string racPath = "Assets/Res/Resources/animcontroller/pet/" + name + ".controller";
            Animator animator = go.GetComponent<Animator>();
            petEntity.anim = animator;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.updateMode = AnimatorUpdateMode.Normal;
            Debugger.Log(racPath);
            //if (File.Exists(racPath))
            //    File.Delete(racPath);
            //UnityEditor.Animations.AnimatorController animatorController=new UnityEditor.Animations.AnimatorController();
            //animatorController.name=name;
            //UnityEditor.Animations.AnimatorStateMachine asm = animatorController.layers[0].stateMachine;

            //AssetDatabase.CreateAsset(animatorController, racPath);
            if (!string.IsNullOrEmpty(racPath))
            {
                Object obj = AssetDatabase.LoadAssetAtPath(racPath, typeof(Object));
                UnityEditor.Animations.AnimatorController ac = obj as UnityEditor.Animations.AnimatorController;
                animator.runtimeAnimatorController = ac;
            }
            go.name = parent.name;

            foreach (var r in parent.GetComponentsInChildren<Renderer>(true))
            {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                r.receiveShadows = false;
                r.useLightProbes = false;
                r.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            }

            TransformUtil.SwitchLayer(parent.transform, (int)LayerType.Fight);
            GameObject prefab = CreateOrReplacePrefab(parent, outputFile) as GameObject;
            Object.DestroyImmediate(parent);
            Object.DestroyImmediate(go);
            return prefab;
        }

        public static GameObject CreateCharacter(Object selectObj, string outputFile, string suffix, ImportCharacterType importCharacterType)
        {
            GameObject parent = new GameObject();
            GameObject go = GameObject.Instantiate(selectObj) as GameObject;
            string name = go.name.Substring(0, go.name.IndexOf('('));//去掉(clone)
            parent.name = name + suffix;

            //SetAnimator(go, selectObj, outputFile);
            go.transform.SetParent(parent.transform, false);
            CharacterEntity character = null;
            string racPath = string.Empty;
            switch (importCharacterType)
            {
                case ImportCharacterType.Player:
                    character = parent.AddComponent<PlayerEntity>();
                    racPath = "Assets/Res/Resources/animcontroller/player/" + name + suffix;
                    break;
                case ImportCharacterType.Hero:
                    character = parent.AddComponent<HeroEntity>();
                    racPath = "Assets/Res/Resources/animcontroller/hero/" + name + suffix;
                    break;
                case ImportCharacterType.Monster:
                    character = parent.AddComponent<EnemyEntity>();
                    racPath = "Assets/Res/Resources/animcontroller/hero/" + name + suffix;
                    break;
            }

            Animator animator = go.GetComponent<Animator>();
            animator.applyRootMotion = true;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.updateMode = AnimatorUpdateMode.Normal;
            character.anim = animator;
            if (!string.IsNullOrEmpty(racPath))
            {
                //SetAnimator(go, selectObj, racPath, ImportCharacterType.Transform);
                switch (importCharacterType)
                {
                    case ImportCharacterType.Player:
                        SetAnimator(go, selectObj, racPath, ImportCharacterType.Player);
                        break;
                    case ImportCharacterType.Hero:
                    case ImportCharacterType.Monster:
                        SetAnimator(go, selectObj, racPath, importCharacterType);
                        break;
                }
            }
            Transform rootTrans = TransformUtil.Find("Bip001", go.transform, true);
            if (rootTrans)
                character.rootNode = rootTrans.gameObject;
            else
                Debugger.LogError("can not find Bip001 from Model {0}", name);

            go.name = parent.name;
            //Debugger.LogError(go.name+"   "+parent.name);

            foreach (var r in parent.GetComponentsInChildren<Renderer>(true))
            {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                r.receiveShadows = false;
                r.useLightProbes = false;
                r.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                //add rim light  暂不开启
                //if (r.gameObject.name == go.name)
                //{
                //    List<Material> mats = r.sharedMaterials.ToList();
                //    string goDir = AssetDatabase.GetAssetPath(selectObj);
                //    goDir = goDir.Replace(go.name + ".FBX", string.Empty);
                //    Debugger.Log(goDir);
                //    string matPath = goDir + "materials/" + go.name + "_Rim_Lighting.mat";
                //    Debugger.Log(matPath);
                //    Material rimMat = GetRimLightMat(goDir, go.name, matPath);
                //    if (rimMat)
                //    {
                //        mats.Add(rimMat);
                //        r.sharedMaterials = mats.ToArray();
                //    }
                //}
            }

            TransformUtil.SwitchLayer(parent.transform, (int)LayerType.Fight);
            GameObject prefab = null;
            //if (string.IsNullOrEmpty(suffix))
            prefab = CreateOrReplacePrefab(parent, outputFile) as GameObject;
            Object.DestroyImmediate(parent);
            Object.DestroyImmediate(go);
            return prefab;
        }

        private static Material GetRimLightMat(string goDir, string modelName, string matPath)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            Shader shader = Shader.Find(ShadersUtil.Custom_Rim_Lighting_Surf);
            if (mat)
            {
                mat.shader = shader;
                mat.name = modelName + "_Rim_Lighting";
            }
            else
            {
                mat = new Material(shader);
                mat.name = modelName + "_Rim_Lighting";
                AssetDatabase.CreateAsset(mat, matPath);
            }
            if (mat)
            {
                string infoTexturePath = goDir + modelName + "_info";
                Debugger.Log(infoTexturePath);
                Texture2D infoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(infoTexturePath + ".tga");
                if (!infoTexture)
                    infoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(infoTexturePath + ".png");
                Debugger.Log((infoTexture == null).ToString());
                if (infoTexture)
                {
                    mat.SetTexture("_InfoTex", infoTexture);
                }
            }
            return mat;
        }

        /// <summary>
        /// 保存prefab
        /// </summary>
        /// <param name="exampleObj"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static Object CreateOrReplacePrefab(GameObject exampleObj, string outputFile)
        {
            string prefabFile = outputFile + ".prefab";
            var prefab = AssetDatabase.LoadAssetAtPath(prefabFile, typeof(GameObject));
            if (prefab)
                prefab = PrefabUtility.ReplacePrefab(exampleObj, prefab, ReplacePrefabOptions.ReplaceNameBased);
            else
                prefab = PrefabUtility.CreatePrefab(prefabFile, exampleObj);
            return prefab;
        }

        public static void SetAnimator(GameObject go, Object selectObj, string outputFile, ImportCharacterType importCharacterType)
        {
            Debugger.Log("outputFile:" + outputFile);
            string prefabPath = AssetDatabase.GetAssetPath(selectObj);
            string tempPath = "Assets/Template/TemplateController.controller";
            string fullPath = string.Empty;
            string suffix = string.Empty;
            string goName = selectObj.name;
            if (goName.EndsWith("_a"))
                goName = goName.Substring(0, goName.LastIndexOf("_a"));
            if (goName.EndsWith("_b"))
                goName = goName.Substring(0, goName.LastIndexOf("_b"));
            if (goName.EndsWith("_c"))
                goName = goName.Substring(0, goName.LastIndexOf("_c"));
            if (goName.EndsWith("_d"))
                goName = goName.Substring(0, goName.LastIndexOf("_d"));
            if (goName.EndsWith("_e"))
                goName = goName.Substring(0, goName.LastIndexOf("_e"));
            if (goName.EndsWith("_f"))
                goName = goName.Substring(0, goName.LastIndexOf("_f"));
            if (goName.EndsWith("_z"))
                goName = goName.Substring(0, goName.LastIndexOf("_z"));
            switch (importCharacterType)
            {
                case ImportCharacterType.Hero:
                case ImportCharacterType.Monster:
                    //fullPath = outputFile;
                    //fullPath = fullPath.Replace("_a.", ".");
                    //fullPath = fullPath.Replace("_b.", ".");
                    //fullPath = fullPath.Replace("_c.", ".");
                    //fullPath = fullPath.Replace("_d.", ".");
                    //fullPath = fullPath.Replace("_e.", ".");
                    //fullPath = fullPath.Replace("_f.", ".");
                    fullPath = outputFile + ".controller";
                    break;
                case ImportCharacterType.Player:
                    fullPath = outputFile + ".controller";
                    break;
                //case ImportCharacterType.Transform:
                //    suffix = "_t";
                //    fullPath = outputFile + suffix + ".controller";
                //    bool existTransform = ExistTransform(selectObj, prefabPath, suffix);
                //    if (!existTransform) return;
                //    break;
            }
            foreach (var s in suffixes)
            {
                if (fullPath.Contains(s + ".controller"))
                {
                    suffix = s;
                    break;
                }
            }
            switch (importCharacterType)
            {
                case ImportCharacterType.Hero:
                case ImportCharacterType.Monster:
                    fullPath = outputFile + ".controller";
                    fullPath = fullPath.Replace("_a" + suffix + ".", suffix + ".");
                    fullPath = fullPath.Replace("_b" + suffix + ".", suffix + ".");
                    fullPath = fullPath.Replace("_c" + suffix + ".", suffix + ".");
                    fullPath = fullPath.Replace("_d" + suffix + ".", suffix + ".");
                    fullPath = fullPath.Replace("_e" + suffix + ".", suffix + ".");
                    fullPath = fullPath.Replace("_f" + suffix + ".", suffix + ".");
                    fullPath = fullPath.Replace("_z" + suffix + ".", suffix + ".");
                    break;
            }
            Debugger.Log(fullPath);
            if (!File.Exists(fullPath))
                File.Copy(tempPath, fullPath, true);
            AssetDatabase.Refresh();
            Object obj = AssetDatabase.LoadAssetAtPath(fullPath, typeof(Object));
            UnityEditor.Animations.AnimatorController ac = obj as UnityEditor.Animations.AnimatorController;
            //Debugger.Log(go.name);
            if (go)
            {
                CharacterEntity character = go.GetComponent<CharacterEntity>();
                if (character)
                    character.anim.runtimeAnimatorController = ac;
                else
                    go.GetComponent<Animator>().runtimeAnimatorController = ac;
            }
            UnityEditor.Animations.AnimatorStateMachine asm = ac.layers[0].stateMachine;
            foreach (var kvp in asm.states)
            {
                UnityEditor.Animations.AnimatorState s = kvp.state;
                //Debugger.Log(kvp.state.name);
                bool boolFoundMotion = false;
                //先在模型所在目录搜索含有@motionName名称的fbx文件
                DirectoryInfo dir = new DirectoryInfo(prefabPath).Parent;
                //FileInfo[] files = dir.GetFiles("*.FBX");
                FileInfo[] files = dir.GetFiles("*.anim");
                foreach (FileInfo f in files)
                {
                    //if (f.Name.ToUpper().Contains(suffix.ToUpper() + "@" + kvp.state.name.ToUpper() + ".FBX"))
                    if (f.Name.ToUpper().Contains(goName.ToUpper() + suffix.ToUpper() + "@" + kvp.state.name.ToUpper() + ".ANIM"))
                    {
                        string path = f.FullName.Substring(f.FullName.LastIndexOf(@"Assets"));
                        Debugger.Log("path:" + path);
                        AnimationClip m = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;
                        s.motion = m;
                        Debugger.Log("use custom motion:" + f.Name);
                        boolFoundMotion = true;
                        break;
                    }
                }
                if (!boolFoundMotion)
                {
                    if (string.IsNullOrEmpty(suffix))
                        Debugger.LogWarning("找不到动作文件:" + goName + suffix + ":" + kvp.state.name);
                    else
                    {
                        #region 如果没有高阶动画，使用基本动画代替
                        foreach (FileInfo f in files)
                        {
                            if (f.Name.ToUpper().Contains(goName.ToUpper() + "@" + kvp.state.name.ToUpper() + ".ANIM"))
                            {
                                string path = f.FullName.Substring(f.FullName.LastIndexOf(@"Assets"));
                                Debugger.Log("path:" + path);
                                AnimationClip m = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;
                                s.motion = m;
                                Debugger.Log("use custom motion:" + f.Name);
                                boolFoundMotion = true;
                                break;
                            }
                        }
                        if (!boolFoundMotion)
                        {
                            Debugger.LogWarning("找不到动作文件:" + goName + suffix + ":" + kvp.state.name);
                        }
                        #endregion
                    }
                }
            }
            AssetDatabase.SaveAssets();
        }

        private static bool ExistTransform(Object selectObj, string prefabPath, string suffix)
        {
            DirectoryInfo d = new DirectoryInfo(prefabPath).Parent;
            FileInfo[] fs = d.GetFiles("*.FBX");
            bool existTransform = false;
            string goName = selectObj.name;
            if (goName.EndsWith("_a"))
                goName = goName.Substring(0, goName.LastIndexOf("_a"));
            if (goName.EndsWith("_b"))
                goName = goName.Substring(0, goName.LastIndexOf("_b"));
            if (goName.EndsWith("_c"))
                goName = goName.Substring(0, goName.LastIndexOf("_c"));
            if (goName.EndsWith("_d"))
                goName = goName.Substring(0, goName.LastIndexOf("_d"));
            if (goName.EndsWith("_e"))
                goName = goName.Substring(0, goName.LastIndexOf("_e"));
            if (goName.EndsWith("_f"))
                goName = goName.Substring(0, goName.LastIndexOf("_f"));
            if (goName.EndsWith("_z"))
                goName = goName.Substring(0, goName.LastIndexOf("_z"));
            foreach (var f in fs)
            {
                if (f.Name.Contains(goName + suffix + "@"))
                {
                    existTransform = true;
                    break;
                }
            }
            return existTransform;
        }

        [MenuItem("Tools/ReImport FBX", false, 300)]
        public static void ReImportFBX()
        {
            DirectoryInfo di = new DirectoryInfo("Assets/Res/Model/");
            int total = di.GetFiles("*.FBX", SearchOption.AllDirectories).Length;
            int progress = 0;
            foreach (FileInfo fi in di.GetFiles("*.FBX", SearchOption.AllDirectories))
            {
                progress++;
                string modelPath = fi.FullName.Substring(fi.FullName.IndexOf(@"Assets"));
                //Debugger.Log(modelPath);
                ModelImporter modelImporter = ModelImporter.GetAtPath(modelPath) as ModelImporter;
                modelImporter.SaveAndReimport();
                EditorUtility.DisplayProgressBar("导入模型", "当前进度", progress / (float)total);
            }
            EditorUtility.ClearProgressBar();
        }
    }

    public enum ImportCharacterType
    {
        None = 0,
        Player = 1,
        Hero = 2,
        Monster = 3,//特殊情况
        Transform = 4,//第二套动作
    }
}