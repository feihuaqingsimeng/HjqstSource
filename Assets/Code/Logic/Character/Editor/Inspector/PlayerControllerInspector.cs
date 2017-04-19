using UnityEngine;
using System.Collections;

using UnityEditor;
namespace Logic.Character.Inspector.Editor
{
    [CustomEditor(typeof(Logic.Character.Controller.PlayerController))]
    public class PlayerControllerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Logic.Character.Controller.PlayerController playerController = target as Logic.Character.Controller.PlayerController;
            EditorGUILayout.LabelField("玩家英雄列表:");
            foreach (var kvp in playerController.heroDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("heroId:", kvp.Key.ToString());
                EditorGUILayout.LabelField("heroName:", kvp.Value.characterName);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("连击英雄列表:");
            foreach (var kvp in playerController.comboHeroDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("heroId:", kvp.Key.ToString());
                EditorGUILayout.LabelField("heroName:", kvp.Value.characterName);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("死亡英雄列表:");
            foreach (var kvp in playerController.deadHeroDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("heroId:", kvp.Key.ToString());
                EditorGUILayout.LabelField("heroName:", kvp.Value.characterName);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.LabelField("死亡英雄墓碑列表:");
            foreach (var t in playerController.deadHeroTombStones)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("tombStone Name:" + t);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}