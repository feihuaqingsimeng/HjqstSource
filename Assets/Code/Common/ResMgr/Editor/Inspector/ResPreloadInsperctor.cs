using UnityEditor;

namespace Common.ResMgr.Editor.Inspector
{
    [CustomEditor(typeof(Common.ResMgr.ResPreload))]
    public class ResPreloadInsperctor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Common.ResMgr.ResPreload resPreload = target as Common.ResMgr.ResPreload;
            EditorGUILayout.LabelField("TotalLength", resPreload.TotalLength.ToString());
        }
    }
}
