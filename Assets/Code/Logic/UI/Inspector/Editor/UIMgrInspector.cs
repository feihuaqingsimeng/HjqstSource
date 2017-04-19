using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
namespace Logic.UI.Inspecter.Editor
{
    [CustomEditor(typeof(UIMgr))]
    public class UIMgrInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying)
            {
//                UIMgr uiMgr = target as UIMgr;
//                List<KeyValuePair<GameObject, bool>> uiStatus = uiMgr.uiStatus;
//                foreach (var item in uiStatus)
//                {
//                    EditorGUILayout.LabelField(string.Format("ui view Name:{0},parent status:{1}", item.Key.name, item.Value.ToString()));
//                }
            }
        }
    }
}