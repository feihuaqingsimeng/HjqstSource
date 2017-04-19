using UnityEngine;
using System.Collections;
using Common.ResMgr;
using Common.Util;
using Logic.Avatar.Model;
using Logic.Character;
using Logic.Player.Model;
using Logic.Enums;

namespace Logic.Player
{
    public sealed class PlayerEntityUtil
    {
        public static string[] HEAD_NODE_NAMES = new string[] { "p_hair_01", "p_hair_02", "p_hair_03", "p_hair_04" };
        public const string SKIN_NODE_NAME = "body";
        public const string FACE_NODE_NAME = "face";
        public const string WP_NODE_NAME = "wp";

        public static void ChangeHair(uint playerID, uint hairCutIndex, CharacterEntity characterEntity, int layer = (int)LayerType.UI)
        {
            if (characterEntity == null)
            {
                return;
            }

            for (int i = 0, length = HEAD_NODE_NAMES.Length; i < length; i++)
            {
                string hairName = HEAD_NODE_NAMES[i];
                Transform t = TransformUtil.Find(hairName, characterEntity.transform, true);
                if (t)
                {
                    if (i == hairCutIndex)
                        t.gameObject.SetActive(true);
                    else
                        t.gameObject.SetActive(false);
                }
            }
        }

        public static void ChangeHairColor(uint playerID, uint hairCutIndex, uint hairColorIndex, CharacterEntity characterEntity)
        {
            if (characterEntity == null)
            {
                return;
            }

            PlayerData playerData = PlayerData.GetPlayerData(playerID);
            AvatarData avatarData = AvatarData.GetAvatarData(playerData.avatarID);
            string hairColorPath = avatarData.GetHairColorPathByIndex(hairColorIndex);
            Texture hairTexture = ResMgr.instance.Load<Texture>(hairColorPath);
            if (!hairTexture) return;
            Texture hairInfoTexture = ResMgr.instance.Load<Texture>(hairColorPath + "_info");
            string hairName = HEAD_NODE_NAMES[hairCutIndex];
            Transform t = TransformUtil.Find(hairName, characterEntity.transform, true);
            if (t)
            {
                Renderer renderer = t.GetComponentInChildren<Renderer>();
                if (renderer)
                {
                    //#if UNITY_EDITOR
                    renderer.material.mainTexture = hairTexture;
                    if (hairInfoTexture)
                        renderer.material.SetTexture("_InfoTex", hairInfoTexture);
                    //#else
                    //                            renderer.material.mainTexture = hairTexture;
                    //                            renderer.material.SetTexture("_InfoTex", hairInfoTexture);
                    //#endif
                }
            }
        }

        public static void ChangeFace(uint playerID, uint faceIndex, CharacterEntity characterEntity)
        {
            if (characterEntity != null)
            {
                PlayerData playerData = PlayerData.GetPlayerData(playerID);
                AvatarData avatarData = AvatarData.GetAvatarData(playerData.avatarID);
                string facePath = avatarData.GetFacePathByIndex(faceIndex);
                Texture faceTexture = ResMgr.instance.Load<Texture>(facePath);
                if (!faceTexture) return;
                Renderer[] renderers = characterEntity.gameObject.GetComponentsInChildren<Renderer>();
                Renderer renderer = null;
                int rendererLength = renderers.Length;
                bool alreadyFind = false;
                for (int i = 0; i < rendererLength; i++)
                {
                    renderer = renderers[i];
                    //#if UNITY_EDITOR
                    Material[] materials = renderer.materials;
                    //#else
                    //                        Material[] materials = renderer.sharedMaterials;               
                    //#endif
                    Material material = null;
                    int materialsLength = materials.Length;
                    for (int materialIndex = 0; materialIndex < materialsLength; materialIndex++)
                    {
                        material = materials[materialIndex];
                        if (material.name.Contains(FACE_NODE_NAME))
                        {
                            material.mainTexture = faceTexture;
                            alreadyFind = true;
                            break;
                        }
                    }
                    if (alreadyFind)
                        break;
                }

            }
        }

        public static void ChangeSkin(uint playerID, int skinIndex, CharacterEntity characterEntity)
        {
            if (characterEntity != null)
            {
                PlayerData playerData = PlayerData.GetPlayerData(playerID);
                AvatarData avatarData = AvatarData.GetAvatarData(playerData.avatarID);
                string skinPath = avatarData.GetSkinPathByIndex(skinIndex);
                string wpPath = avatarData.GetWPPathByIndex(skinIndex);
                Texture skinTexture = ResMgr.instance.Load<Texture>(skinPath);
                Texture wpTexture = ResMgr.instance.Load<Texture>(wpPath);
                if (!skinTexture || !wpTexture) return;
                Renderer[] renderers = characterEntity.gameObject.GetComponentsInChildren<Renderer>();
                Renderer renderer = null;
                int rendererLength = renderers.Length;
                for (int i = 0; i < rendererLength; i++)
                {
                    renderer = renderers[i];
                    //#if UNITY_EDITOR
                    Material[] materials = renderer.materials;
                    //#else
                    //                        Material[] materials = renderer.sharedMaterials;               
                    //#endif
                    Material material = null;
                    int materialsLength = materials.Length;
                    for (int materialIndex = 0; materialIndex < materialsLength; materialIndex++)
                    {
                        material = materials[materialIndex];
                        if (material.name.Contains(SKIN_NODE_NAME))
                        {
                            material.mainTexture = skinTexture;
                            string skinInfoPath = skinPath + "_info";
                            Texture skinInfoTexture = ResMgr.instance.Load<Texture>(skinInfoPath);
                            if (skinInfoTexture)
                                material.SetTexture("_InfoTex", skinInfoTexture);
                        }
                        if (material.name.Contains(WP_NODE_NAME))
                        {
                            material.mainTexture = wpTexture;
                        }
                    }
                }

            }
        }
    }
}