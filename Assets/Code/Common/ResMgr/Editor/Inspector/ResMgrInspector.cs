using UnityEditor;

namespace Common.ResMgr.Editor.Inspector
{
    [CustomEditor(typeof(Common.ResMgr.ResMgr))]
    public class ResMgrInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Common.ResMgr.ResMgr resMgr = target as Common.ResMgr.ResMgr;


            if (resMgr.loadedRes != null)
            {
                EditorGUILayout.LabelField("LoadedResDicCount", resMgr.loadedRes.Count.ToString());
                int i = 0;
                foreach (var kv in resMgr.loadedRes)
                {
                    EditorGUILayout.LabelField((++i).ToString(), kv);
                }
            }
            if (resMgr.resDic != null)
            {
                EditorGUILayout.LabelField("resDicCount", resMgr.resDic.Count.ToString());
                int i = 0;
                foreach (var kv in resMgr.resDic)
                {
                    EditorGUILayout.LabelField((++i).ToString(), kv.Key);
                }
            }

            EditorGUILayout.LabelField("load fail ab count", resMgr.loadFailCount.ToString());

            if (resMgr.assetBundleDic != null)
            {
                EditorGUILayout.LabelField("assetBundleDic", resMgr.assetBundleDic.Count.ToString());
                int i = 0;
                foreach (var kv in resMgr.assetBundleDic)
                {
                    EditorGUILayout.LabelField((++i).ToString(), kv.Key);
                }
            }

            if (resMgr.localManifest != null)
            {
                EditorGUILayout.LabelField("Ver:", resMgr.localManifest.version.ToString());
                int i = 0;
                foreach (var kv in resMgr.localManifest.assetDic)
                {
                    EditorGUILayout.LabelField((++i).ToString(), kv.Value.ToString());
                }
            }

            if (resMgr.loadingList != null)
            {
                EditorGUILayout.LabelField("loadingList:", resMgr.loadingList.Count.ToString());
                int i = 0;
                foreach (var kv in resMgr.loadingList)
                {
                    EditorGUILayout.LabelField((++i).ToString(), kv.ResObj.SubPath.ToString());
                }
            }

            if (resMgr.waitingList != null)
            {
                EditorGUILayout.LabelField("waitingList:", resMgr.waitingList.Count.ToString());
                int i = 0;
                foreach (var kv in resMgr.waitingList)
                {
                    EditorGUILayout.LabelField((++i).ToString(), kv.ResObj.SubPath.ToString());
                }
            }

            if (resMgr.waitingABs != null)
            {
                EditorGUILayout.LabelField("waitingABs:", resMgr.waitingABs.Count.ToString());
                int i = 0;
                foreach (var kv in resMgr.waitingABs)
                {
                    EditorGUILayout.LabelField((++i).ToString(), kv.assetbundlePath);
                }
            }

            if (resMgr.loadingABs != null)
            {
                EditorGUILayout.LabelField("loadingABs:", resMgr.loadingABs.Count.ToString());
                int i = 0;
                foreach (var kv in resMgr.loadingABs)
                {
                    EditorGUILayout.LabelField((++i).ToString(), kv.assetbundlePath);
                }
            }
        }
    }
}
