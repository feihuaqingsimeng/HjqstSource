using UnityEditor;
namespace Common.ResMgr.Editor.Inspector
{
    [CustomEditor(typeof(Common.ResMgr.ResLoader))]
    public class ResLoaderInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Common.ResMgr.ResLoader resLoader = target as Common.ResMgr.ResLoader;
            EditorGUILayout.LabelField("ResObj", resLoader.ResObj.ToString());
            EditorGUILayout.Toggle("FormLocal", resLoader.FormLocal);
            EditorGUILayout.LabelField("ReLoadNum", resLoader.ReLoadNum.ToString());
            EditorGUILayout.LabelField("State", resLoader.State.ToString());
            EditorGUILayout.LabelField("CurrentProgress", resLoader.CurrentProgress.ToString());
        }
    }

}
