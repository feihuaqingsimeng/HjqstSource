using UnityEngine;
using System.Collections;
using UnityEditor;
using Logic.Shaders;

public class ParticleMaterialEditor : MaterialEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Material mat = target as Material;
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
    }
}
