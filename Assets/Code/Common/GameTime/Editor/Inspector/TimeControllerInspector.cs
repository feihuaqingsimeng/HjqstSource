using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Common.GameTime.Editor.Inspector
{
    [CustomEditor(typeof(Common.GameTime.Controller.TimeController))]
    public class TimeControllerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Common.GameTime.Controller.TimeController timeController = target as Common.GameTime.Controller.TimeController;
            EditorGUILayout.LabelField("server date:", timeController.ServerTime.ToLongDateString() + " " + timeController.ServerTime.ToLongTimeString());
            EditorGUILayout.LabelField("server time second:", timeController.ServerTimeTicksSecond.ToString());
            EditorGUILayout.LabelField("server time millisecond:", timeController.ServerTimeTicksMillisecond.ToString());
            EditorGUILayout.LabelField("Time.time:", Time.time.ToString());
            EditorGUILayout.LabelField("fihgt skill time:", timeController.fightSkillTime.ToString());
        }
    }
}