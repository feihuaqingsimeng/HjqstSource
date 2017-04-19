using System;
using UnityEditor;
using UnityEngine;

namespace Common.ResMgr.Editor.Inspector
{
    public sealed class ManifestInfoInspector : UnityEditor.EditorWindow
    {
        //[MenuItem("ResMgr/ShowLocal manifest.bin", false, 1)]
        //public static void ShowLocalManifestInfo()
        //{
        //    ManifestInfoInspector manifestInfoInspector = EditorWindow.GetWindow<ManifestInfoInspector>();
        //    manifestInfoInspector._manifestInfo = SAUtil.GetManifestInfo();
        //    manifestInfoInspector._manifestMd5Str = SAUtil.EncryptMD5(System.IO.File.ReadAllBytes(SAConf.MANIFEST_PATH));
        //}

        //private DateTime _baseDateTime = new DateTime();
        //private ManifestInfo _manifestInfo;
        //private string _manifestMd5Str;
        //private Vector2 _scrollPosition = new Vector2(100, 100);
        //private GUILayoutOption[] _titleOptions;

        //private void OnGUI()
        //{
        //    Color c;
        //    if (_manifestInfo == null)
        //    {
        //        c = GUI.color;
        //        GUI.color = Color.red;
        //        EditorGUILayout.LabelField("找不到：" + SAConf.MANIFEST_PATH);
        //        GUI.color = c;
        //        return;
        //    }
        //    EditorGUILayout.LabelField("_manifestMd5Str:" + _manifestMd5Str);
        //    EditorGUILayout.LabelField("ManifestInfo.version:" + _manifestInfo.version.ToString());
        //    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        //    EditorGUILayout.BeginHorizontal();
        //    EditorGUILayout.LabelField("SubPath");
        //    EditorGUILayout.LabelField("Length/byte");
        //    EditorGUILayout.LabelField("CreateDate");
        //    EditorGUILayout.EndHorizontal();
        //    foreach (var kv in _manifestInfo.assetDic)
        //    {
        //        EditorGUILayout.BeginHorizontal();
        //        EditorGUILayout.LabelField(kv.Value.SubPath);
        //        EditorGUILayout.LabelField(kv.Value.Length.ToString());
        //        EditorGUILayout.LabelField(_baseDateTime.AddTicks(kv.Value.CreateDate).ToString("yyyy年MM月dd日 hh点mm分ss秒fffffff毫秒"));
        //        EditorGUILayout.EndHorizontal();

        //        if (NGUIEditorTools.DrawHeader("FileNameList"))
        //        {
        //            for (int i = 0, count = kv.Value.FilePathList.Count; i < count; i++)
        //            {
        //                EditorGUILayout.LabelField("           " + kv.Value.FilePathList[i]);
        //            }
        //        }
        //    }
        //    EditorGUILayout.EndScrollView();

        //}


    }
}
