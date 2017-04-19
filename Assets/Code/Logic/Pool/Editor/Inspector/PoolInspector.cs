using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Logic.Pool.Inspector.Editor
{
    [CustomEditor(typeof(Logic.Pool.Controller.PoolController))]
    public class PoolInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Logic.Pool.Controller.PoolController poolController = target as Logic.Pool.Controller.PoolController;
            EditorGUILayout.LabelField("内存池资源路径:");
            foreach (var kvp in poolController.pathDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("名称:" + kvp.Key);
                EditorGUILayout.LabelField("路径:" + kvp.Value);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("常驻内存池:");
            foreach (var kvp in poolController.spawnPoolDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("名称:" + kvp.Key);
                EditorGUILayout.LabelField("是否常驻内存:" + kvp.Value.ToString());
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
