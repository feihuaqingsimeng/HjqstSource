using UnityEngine;
using System.Collections;
namespace Logic.Shaders
{
    public class ShadersUtil
    {
        public const string CLIP_POSITION_OFF = "CLIP_POSITION_OFF";
        public const string CLIP_POSITION_ON = "CLIP_POSITION_ON";
        public const string RIMLIGHT_ON = "RIMLIGHT_ON";
        public const string RIMLIGHT_OFF = "RIMLIGHT_OFF";
        public const string BODY_ON = "BODY_ON";
        public const string BODY_OFF = "BODY_OFF";
        //public const string GLOSS_ON = "GLOSS_ON";
        //public const string GLOSS_OFF = "GLOSS_OFF";
        //public const string Custom_Reflective_Glossy = "Custom/Reflective/Glossy";
        public const string Unlit_Transparent = "Unlit/Transparent";
        //public const string Custom_Rim_Lighting = "Custom/RimLight";
        public const string Custom_Rim_Lighting_Surf = "Custom/RimLightSurf";
        //private const string RIM_COLOR_NAME = "_RimColor";
        //private const string RIM_POWER = "_RimPower";
        public static int RIM_COLOR_ID;
        public static int RIM_POWER_ID;
        public static Color MAIN_COLOR = new Color(142 / 255f, 142 / 255f, 142 / 255f, 1f);
        public static Color RIM_COLOR = new Color(70 / 255f, 70 / 255f, 175 / 255f, 1f);
        public static Color RIM_MAIN_COLOR = new Color(240 / 255f, 240 / 255f, 240 / 255f, 1f);

        static ShadersUtil()
        {
            RIM_COLOR_ID = Shader.PropertyToID("_RimColor");
            RIM_POWER_ID = Shader.PropertyToID("_RimPower");
        }

        public static Logic.Character.CharacterEntity SetShader(Logic.Character.CharacterEntity characterEntity)
        {
#if UNITY_EDITOR
            Renderer[] renderers = characterEntity.gameObject.GetComponentsInChildren<Renderer>();
            Renderer renderer = null;
            for (int i = 0, count = renderers.Length; i < count; i++)
            {
                renderer = renderers[i];
                Material[] materials = renderer.materials;
                for (int j = 0, jCount = materials.Length; j < jCount; j++)
                {
                    materials[j].shader = UnityEngine.Shader.Find(Custom_Rim_Lighting_Surf);
                }
            }
            return characterEntity;
#else
            return characterEntity;
#endif
        }

        public static Logic.Character.CharacterEntity SetShaderKeyword(Logic.Character.CharacterEntity characterEntity, string enableKeyword, string disableKeyword)
        {
            Renderer[] renderers = characterEntity.gameObject.GetComponentsInChildren<Renderer>();
            Renderer renderer = null;
            for (int i = 0, count = renderers.Length; i < count; i++)
            {
                renderer = renderers[i];
#if UNITY_EDITOR
                Material[] materials = renderer.materials;
#else
                Material[] materials = renderer.sharedMaterials;
#endif
                for (int j = 0, jCount = materials.Length; j < jCount; j++)
                {
                    materials[j].EnableKeyword(enableKeyword);
                    materials[j].DisableKeyword(disableKeyword);
                }
            }
            return characterEntity;
        }

        public static Logic.Character.CharacterEntity SetMainColor(Logic.Character.CharacterEntity characterEntity, Color color)
        {
            Renderer[] renderers = characterEntity.gameObject.GetComponentsInChildren<Renderer>();
            Renderer renderer = null;
            for (int i = 0, count = renderers.Length; i < count; i++)
            {
                renderer = renderers[i];
#if UNITY_EDITOR
                Material[] materials = renderer.materials;
#else
                Material[] materials = renderer.sharedMaterials;
#endif
                for (int j = 0, jCount = materials.Length; j < jCount; j++)
                {
                    materials[j].color = color;
                }
            }
            return characterEntity;
        }

        public static Logic.Character.CharacterEntity SetColor(Logic.Character.CharacterEntity characterEntity, int colorId, Color color)
        {
            Renderer[] renderers = characterEntity.gameObject.GetComponentsInChildren<Renderer>();
            Renderer renderer = null;
            for (int i = 0, count = renderers.Length; i < count; i++)
            {
                renderer = renderers[i];
#if UNITY_EDITOR
                Material[] materials = renderer.materials;
#else
                Material[] materials = renderer.sharedMaterials;
#endif
                for (int j = 0, jCount = materials.Length; j < jCount; j++)
                {
                    materials[j].SetColor(colorId, color);
                }
            }
            return characterEntity;
        }

        public static Logic.Character.CharacterEntity SetRimPow(Logic.Character.CharacterEntity characterEntity, float rimPow)
        {
            Renderer[] renderers = characterEntity.gameObject.GetComponentsInChildren<Renderer>();
            Renderer renderer = null;
            for (int i = 0, count = renderers.Length; i < count; i++)
            {
                renderer = renderers[i];
#if UNITY_EDITOR
                Material[] materials = renderer.materials;
#else
                Material[] materials = renderer.sharedMaterials;
#endif
                for (int j = 0, jCount = materials.Length; j < jCount; j++)
                {
                    materials[j].SetFloat(RIM_POWER_ID, rimPow);
                }
            }
            return characterEntity;
        }
    }
}