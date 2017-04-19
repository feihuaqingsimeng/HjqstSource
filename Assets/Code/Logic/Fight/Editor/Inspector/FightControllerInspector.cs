using UnityEngine;
using System.Collections;
using UnityEditor;
using Logic.Fight.Controller;
namespace Logic.Fight.Editor.Inspector
{
    [CustomEditor(typeof(FightController))]
    public class FightControllerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            FightController fightController = target as FightController;
            EditorGUILayout.LabelField("FightType:",fightController.fightType.ToString());
            EditorGUILayout.LabelField("fightStatus:", fightController.fightStatus.ToString());
            EditorGUILayout.LabelField("fightCostTime(s):", fightController.fightCostTime.ToString());
            EditorGUILayout.LabelField("deadCount:", fightController.deadCount.ToString());
            EditorGUILayout.LabelField("remainderHPAverageRate:", fightController.remainderHPAverageRate.ToString());
            EditorGUILayout.LabelField("totalDamage:", fightController.totalDamage.ToString());
            EditorGUILayout.LabelField("comboCount:", fightController.comboCount.ToString());
        }
    }
}