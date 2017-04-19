using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Logic.Effect.Inspector.Editor
{
    [CustomEditor(typeof(Logic.Effect.Controller.EffectController))]
    public class EffectControllerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Logic.Effect.Controller.EffectController effectController = target as Logic.Effect.Controller.EffectController;
            EditorGUILayout.LabelField("特效数量:", effectController.effectDic.Count.ToString());
            foreach (var kvp in effectController.effectDic)
            {
                EditorGUILayout.LabelField("特效名称:", kvp.Key.ToString());
            }
            EditorGUILayout.LabelField("忽略删除特效:", effectController.ignoreDelEffectDic.Count.ToString());
            foreach (var kvp in effectController.ignoreDelEffectDic)
            {
                EditorGUILayout.LabelField("特效名称:", kvp.Key.ToString());
            }
            EditorGUILayout.LabelField("combo make 特效数量:", effectController.comboMakeEffects.Count.ToString());
            foreach (var effect in effectController.comboMakeEffects)
            {
                EditorGUILayout.LabelField("特效名称:", effect.name);
            }

            EditorGUILayout.LabelField("脚底受击提示特效数量:", effectController.attackRootTips.Count.ToString());
            foreach (var effect in effectController.attackRootTips)
            {
                EditorGUILayout.LabelField("特效名称:", effect.name);
            }

            EditorGUILayout.LabelField("范围技能提示特效数量:", effectController.targetRangeTips.Count.ToString());
            foreach (var effect in effectController.targetRangeTips)
            {
                EditorGUILayout.LabelField("特效名称:", effect.name);
            }
        }
    }
}