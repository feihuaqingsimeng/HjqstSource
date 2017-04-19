using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Logic.Character.Inspector.Editor
{
    [CustomEditor(typeof(Logic.Character.Controller.EnemyController))]
    public class EnemyControllerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Logic.Character.Controller.EnemyController enemyController = target as Logic.Character.Controller.EnemyController;
            EditorGUILayout.LabelField("敌方单位列表:");
            foreach (var kvp in enemyController.enemyDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("EnemyId:", kvp.Key.ToString());
                EditorGUILayout.LabelField("EnemyName:", kvp.Value.characterName);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("连击敌人列表:");
            foreach (var kvp in enemyController.comboEnemyDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("heroId:", kvp.Key.ToString());
                EditorGUILayout.LabelField("heroName:", kvp.Value.characterName);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("死亡敌方单位列表:");
            foreach (var kvp in enemyController.deadEnemyDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("heroId:", kvp.Key.ToString());
                EditorGUILayout.LabelField("heroName:", kvp.Value.characterName);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}