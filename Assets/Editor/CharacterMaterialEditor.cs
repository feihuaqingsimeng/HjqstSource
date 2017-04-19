using UnityEngine;
using System.Collections;
using UnityEditor;
using Logic.Shaders;

public class CharacterMaterialEditor : MaterialEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Material mat = target as Material;
        if (EditorGUILayout.Toggle("Is Body", mat.IsKeywordEnabled(ShadersUtil.BODY_ON) ? true : false))
        {
            mat.SetInt("_IsBody", 1);
            mat.EnableKeyword(ShadersUtil.BODY_ON);
            mat.DisableKeyword(ShadersUtil.BODY_OFF);
        }
        else
        {
            mat.SetInt("_IsBody", 0);
            mat.EnableKeyword(ShadersUtil.BODY_OFF);
            mat.DisableKeyword(ShadersUtil.BODY_ON);
        }
        //if (EditorGUILayout.Toggle("Open Gloss", mat.GetInt("_Gloss") == 0 && !mat.IsKeywordEnabled(ShadersUtil.GLOSS_ON) ? false : true))
        //{
        //    mat.SetInt("_Gloss", 1);
        //    mat.EnableKeyword(ShadersUtil.GLOSS_ON);
        //    mat.DisableKeyword(ShadersUtil.GLOSS_OFF);
        //}
        //else
        //{
        //    mat.SetInt("_Gloss", 0);
        //    mat.EnableKeyword(ShadersUtil.GLOSS_OFF);
        //    mat.DisableKeyword(ShadersUtil.GLOSS_ON);
        //}
        if (EditorGUILayout.Toggle("Open Clip Position", mat.IsKeywordEnabled(ShadersUtil.CLIP_POSITION_ON) ? true : false))
        {
            mat.SetInt("_ClipPosition", 1);
            mat.EnableKeyword(ShadersUtil.CLIP_POSITION_ON);
            mat.DisableKeyword(ShadersUtil.CLIP_POSITION_OFF);
        }
        else
        {
            mat.SetInt("_ClipPosition", 0);
            mat.EnableKeyword(ShadersUtil.CLIP_POSITION_OFF);
            mat.DisableKeyword(ShadersUtil.CLIP_POSITION_ON);
        }
        if (EditorGUILayout.Toggle("Open Rim", mat.IsKeywordEnabled(ShadersUtil.RIMLIGHT_ON) ? true : false))
        {
            mat.SetInt("_RimLight", 1);
            mat.EnableKeyword(ShadersUtil.RIMLIGHT_ON);
            mat.DisableKeyword(ShadersUtil.RIMLIGHT_OFF);
        }
        else
        {
            mat.SetInt("_RimLight", 0);
            mat.EnableKeyword(ShadersUtil.RIMLIGHT_OFF);
            mat.DisableKeyword(ShadersUtil.RIMLIGHT_ON);
        }
    }
}
