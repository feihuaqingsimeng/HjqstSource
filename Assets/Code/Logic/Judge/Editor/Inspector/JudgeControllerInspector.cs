using UnityEngine;
using System.Collections;
using UnityEditor;
using Logic.Judge.Controller;
namespace Logic.Judge.Editor.Inspecter
{
    [CustomEditor(typeof(JudgeController))]
    public class JudgeControllerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            JudgeController judgeController = target as JudgeController;
            EditorGUILayout.LabelField("combo count ：", judgeController.comboCount.ToString());
            EditorGUILayout.LabelField("total damage :", judgeController.totalDamage.ToString());
        }
    }
}